using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using Amazon.Lambda.TestUtilities;
using Amazon.Lambda.APIGatewayEvents;

using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Newtonsoft.Json;

using Xunit;

using Othello;
using OthelloAdapters;
using Amazon.Runtime;

namespace OthelloAWSServerless.Tests
{
    public class FunctionTest : IDisposable
    { 
        string TableName { get; }
        IAmazonDynamoDB DDBClient { get; }
        
        public FunctionTest()
        {
            this.TableName = "OthelloAWSServerless-OthelloGame-" + DateTime.Now.Ticks;
            this.DDBClient = new AmazonDynamoDBClient(RegionEndpoint.APNortheast1);

            SetupTableAsync().Wait();
        }

        [Fact]
        public async Task GameListTestAsync()
        {
            TestLambdaContext context;
            APIGatewayProxyRequest request;
            APIGatewayProxyResponse response;

            Functions functions = new Functions(this.DDBClient, this.TableName);

            var myGame = CreateNewOthelloHumanVSHumanGame(out var myPlayers, out var currentPlayer);
            var game = await AddOthelloGameRepresentation(myPlayers, functions).ConfigureAwait(true);

            // List the games
            request = new APIGatewayProxyRequest
            {
            };
            context = new TestLambdaContext();
            response = await functions.GetGamesAsync(request, context).ConfigureAwait(true);
            Assert.Equal(200, response.StatusCode);

            OthelloGameRepresentation[] gamePosts = JsonConvert.DeserializeObject<OthelloGameRepresentation[]>(response.Body);
            Assert.Single(gamePosts);
            Assert.Equal(game.Id, gamePosts[0].Id);
            Assert.Equal(myGame.OthelloGameStrRepresentation, gamePosts[0].OthelloGameStrRepresentation);
        }

        [Fact]
        public async Task GameAddHumanVsHumanAndGetGameTestAsync()
        {
            TestLambdaContext context;
            APIGatewayProxyRequest request;
            APIGatewayProxyResponse response;

            Functions functions = new Functions(this.DDBClient, this.TableName);

            // Test adding a new game using player parameters
            var myGame = CreateNewOthelloHumanVSHumanGame(out var myPlayers, out var currentPlayer);
            var game = await AddOthelloGameRepresentation(myPlayers, functions).ConfigureAwait(true);

            // Test getting the game back out
            request = new APIGatewayProxyRequest
            {
                PathParameters = new Dictionary<string, string> { { Functions.IdQueryStringName, game.Id } }
            };
            context = new TestLambdaContext();
            response = await functions.GetGameAsync(request, context).ConfigureAwait(true);
            Assert.Equal(200, response.StatusCode);

            OthelloGameRepresentation readGame = JsonConvert.DeserializeObject<OthelloGameRepresentation>(response.Body);
            Assert.Equal(game.Id, readGame.Id);
            Assert.Equal(myGame.OthelloGameStrRepresentation, readGame.OthelloGameStrRepresentation);

        }

        [Fact]
        public async Task GameTestDeleteAsync()
        {
            TestLambdaContext context;
            APIGatewayProxyRequest request;
            APIGatewayProxyResponse response;

            Functions functions = new Functions(this.DDBClient, this.TableName);

            // Test adding a new game using player parameters
            var myGame = CreateNewOthelloHumanVSHumanGame(out var myPlayers, out var currentPlayer);
            var game = await AddOthelloGameRepresentation(myPlayers, functions).ConfigureAwait(true);

            // Delete the game
            request = new APIGatewayProxyRequest
            {
                PathParameters = new Dictionary<string, string> { { Functions.IdQueryStringName, game.Id } }
            };
            context = new TestLambdaContext();
            response = await functions.RemoveGameAsync(request, context).ConfigureAwait(true);
            Assert.Equal(200, response.StatusCode);

            // Make sure the game was deleted.
            request = new APIGatewayProxyRequest
            {
                PathParameters = new Dictionary<string, string> { { Functions.IdQueryStringName, game.Id } }
            };
            context = new TestLambdaContext();
            response = await functions.GetGameAsync(request, context).ConfigureAwait(true);
            Assert.Equal((int)HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GameTestGetCurrentPlayerAsync()
        {
            TestLambdaContext context;
            APIGatewayProxyRequest request;
            APIGatewayProxyResponse response;

            Functions functions = new Functions(this.DDBClient, this.TableName);

            // Add a new game using player parameters
            var myGame = CreateNewOthelloHumanVSHumanGame(out var myPlayers, out var currentPlayer);
            var game = await AddOthelloGameRepresentation(myPlayers, functions).ConfigureAwait(true);

            //Test we can get the game's current player
            request = new APIGatewayProxyRequest
            {
                PathParameters = new Dictionary<string, string> { { Functions.IdQueryStringName, game.Id } }
            };
            context = new TestLambdaContext();
            response = await functions.GetGameCurrentPlayerAsync(request, context).ConfigureAwait(true);
            Assert.Equal(200, response.StatusCode);

            OthelloServerlessCurrentPlayer getcurrentPlayer = JsonConvert.DeserializeObject<OthelloServerlessCurrentPlayer>(response.Body);
            Assert.Equal(currentPlayer.ToString(), getcurrentPlayer.CurrentPlayer);
        }

        [Fact]
        public async Task GameTestMakeValidMoveValidPlayerAsync()
        {
            TestLambdaContext context;
            APIGatewayProxyRequest request;
            APIGatewayProxyResponse response;

            Functions functions = new Functions(this.DDBClient, this.TableName);

            // Add a new game using player parameters
            var myGame = CreateNewOthelloHumanVSHumanGame(out var myPlayers, out var currentPlayer);
            var game = await AddOthelloGameRepresentation(myPlayers, functions).ConfigureAwait(true);

            // Test we can get an valid move response
            OthelloServerlessMakeMove myMoves = new OthelloServerlessMakeMove();
            myMoves.GameX = 3;
            myMoves.GameY = 2;
            myMoves.CurrentPlayer = new OthelloGamePlayer(OthelloPlayerKind.White, "default-name-white");

            request = new APIGatewayProxyRequest
            {
                PathParameters = new Dictionary<string, string> { { Functions.IdQueryStringName, game.Id } },
                Body = JsonConvert.SerializeObject(myMoves)
            };

            context = new TestLambdaContext();
            response = await functions.MakeGameMoveAsync(request, context).ConfigureAwait(true);
            var makemoveresponse = JsonConvert.DeserializeObject<OthelloServerlessMakeMoveFliplist>(response?.Body);

            Assert.Equal(200, response.StatusCode);
            Assert.True(makemoveresponse.IsValid);
            Assert.Equal(0,makemoveresponse.Reason);
        }

        [Fact]
        public async Task GameTestMakeInvalidMoveValidPlayerAsync()
        {
            TestLambdaContext context;
            APIGatewayProxyRequest request;
            APIGatewayProxyResponse response;

            Functions functions = new Functions(this.DDBClient, this.TableName);

            // Add a new game using player parameters
            var myGame = CreateNewOthelloHumanVSHumanGame(out var myPlayers, out var currentPlayer);
            var game = await AddOthelloGameRepresentation(myPlayers, functions).ConfigureAwait(true);


            // Test we can get an valid move response
            OthelloServerlessMakeMove myMoves = new OthelloServerlessMakeMove();
            myMoves.GameX = 4;
            myMoves.GameY = 2;
            myMoves.CurrentPlayer = new OthelloGamePlayer(OthelloPlayerKind.White, "");

            request = new APIGatewayProxyRequest
            {
                PathParameters = new Dictionary<string, string> { { Functions.IdQueryStringName, game.Id } },
                Body = JsonConvert.SerializeObject(myMoves)
            };

            context = new TestLambdaContext();
            response = await functions.MakeGameMoveAsync(request, context).ConfigureAwait(true);
            var makemoveresponse = JsonConvert.DeserializeObject<OthelloServerlessMakeMoveFliplist>(response?.Body);

            Assert.Equal(200, response.StatusCode);
            Assert.False(makemoveresponse.IsValid);
            Assert.Equal(0x2, makemoveresponse.Reason);
        }

        [Fact]
        public async Task GameTestMakeValidMoveInvalidPlayerAsync()
        {
            TestLambdaContext context;
            APIGatewayProxyRequest request;
            APIGatewayProxyResponse response;

            Functions functions = new Functions(this.DDBClient, this.TableName);

            // Add a new game using player parameters
            var myGame = CreateNewOthelloHumanVSHumanGame(out var myPlayers, out var currentPlayer);
            var game = await AddOthelloGameRepresentation(myPlayers, functions).ConfigureAwait(true);

            // Test we can get an valid move response
            OthelloServerlessMakeMove myMoves = new OthelloServerlessMakeMove();
            myMoves.GameX = 3;
            myMoves.GameY = 2;
            myMoves.CurrentPlayer = new OthelloGamePlayer(OthelloPlayerKind.Black, "default-name-black");

            request = new APIGatewayProxyRequest
            {
                PathParameters = new Dictionary<string, string> { { Functions.IdQueryStringName, game.Id } },
                Body = JsonConvert.SerializeObject(myMoves)
            };

            context = new TestLambdaContext();
            response = await functions.MakeGameMoveAsync(request, context).ConfigureAwait(true);
            var makemoveresponse = JsonConvert.DeserializeObject<OthelloServerlessMakeMoveFliplist>(response?.Body);

            Assert.Equal(200, response.StatusCode);
            Assert.False(makemoveresponse.IsValid);
            Assert.Equal(0x1, makemoveresponse.Reason);
        }

        [Fact]
        public async Task GameTestMakeInvalidMoveInvalidPlayerAsync()
        {
            TestLambdaContext context;
            APIGatewayProxyRequest request;
            APIGatewayProxyResponse response;

            Functions functions = new Functions(this.DDBClient, this.TableName);

            // Add a new game using player parameters
            var myGame = CreateNewOthelloHumanVSHumanGame(out var myPlayers, out var currentPlayer);
            var game = await AddOthelloGameRepresentation(myPlayers, functions).ConfigureAwait(true);


            // Test we can get an valid move response
            OthelloServerlessMakeMove myMoves = new OthelloServerlessMakeMove();
            myMoves.GameX = 5;
            myMoves.GameY = 2;
            myMoves.CurrentPlayer = new OthelloGamePlayer(OthelloPlayerKind.Black, "default-name-black");

            request = new APIGatewayProxyRequest
            {
                PathParameters = new Dictionary<string, string> { { Functions.IdQueryStringName, game.Id } },
                Body = JsonConvert.SerializeObject(myMoves)
            };

            context = new TestLambdaContext();
            response = await functions.MakeGameMoveAsync(request, context).ConfigureAwait(true);
            var makemoveresponse = JsonConvert.DeserializeObject<OthelloServerlessMakeMoveFliplist>(response?.Body);

            Assert.Equal(200, response.StatusCode);
            Assert.False(makemoveresponse.IsValid);
            Assert.Equal(0x3, makemoveresponse.Reason);
        }

        [Fact]
        public async Task GameAddAIVsHumanAndGetGameAsync()
        {
            TestLambdaContext context;
            APIGatewayProxyRequest request;
            APIGatewayProxyResponse response;

            Functions functions = new Functions(this.DDBClient, this.TableName);

            // Test adding a new game using player parameters
            var myGame = CreateNewOthelloAIVSHumanGame(out var myPlayers, out var currentPlayer);
            var game = await AddOthelloGameRepresentation(myPlayers, functions).ConfigureAwait(true);

            // Test getting the game back out
            request = new APIGatewayProxyRequest
            {
                PathParameters = new Dictionary<string, string> { { Functions.IdQueryStringName, game.Id } }
            };
            context = new TestLambdaContext();
            response = await functions.GetGameAsync(request, context).ConfigureAwait(true);
            Assert.Equal(200, response.StatusCode);

            OthelloGameRepresentation readGame = JsonConvert.DeserializeObject<OthelloGameRepresentation>(response.Body);
            Assert.Equal(game.Id, readGame.Id);
            Assert.Equal(myGame.OthelloGameStrRepresentation, readGame.OthelloGameStrRepresentation);

            OthelloAdapter adapter = new OthelloAdapter();
            adapter.GetGameFromJSON(readGame.OthelloGameStrRepresentation);
            var actualcurrentplayer = adapter.GameUpdatePlayer();

            Assert.Equal(currentPlayer.PlayerKind, actualcurrentplayer.PlayerKind);
        }

        [Fact]
        public async Task GameAddAIVsHumanAndMoveAIAsync()
        {
            TestLambdaContext context;
            APIGatewayProxyRequest request;
            APIGatewayProxyResponse response;

            Functions functions = new Functions(this.DDBClient, this.TableName);

            // Test adding a new game using player parameters
            var myGame = CreateNewOthelloAIVSHumanGame(out var myPlayers, out var currentPlayer);
            var game = await AddOthelloGameRepresentation(myPlayers, functions).ConfigureAwait(true);


            // Test getting the game back out
            request = new APIGatewayProxyRequest
            {
                PathParameters = new Dictionary<string, string> { { Functions.IdQueryStringName, game.Id } }
            };
            context = new TestLambdaContext();
            response = await functions.GetGameAsync(request, context).ConfigureAwait(true);
            Assert.Equal(200, response.StatusCode);

            OthelloGameRepresentation readGame = JsonConvert.DeserializeObject<OthelloGameRepresentation>(response.Body);
            Assert.Equal(game.Id, readGame.Id);
            Assert.Equal(myGame.OthelloGameStrRepresentation, readGame.OthelloGameStrRepresentation);

            // Test AI Move
            // Test we can get an valid move response
            OthelloServerlessMakeMove myMoves = new OthelloServerlessMakeMove();
            myMoves.CurrentPlayer = new OthelloGamePlayer(OthelloPlayerKind.White, "default-name-white");

            request = new APIGatewayProxyRequest
            {
                PathParameters = new Dictionary<string, string> {{Functions.IdQueryStringName, game.Id}},
                Body = JsonConvert.SerializeObject(myMoves)
            };
            context = new TestLambdaContext();
            response = await functions.MakeGameAIMoveAsync(request, context).ConfigureAwait(true);
            var makemoveresponse = JsonConvert.DeserializeObject<OthelloServerlessMakeMoveFliplist>(response?.Body);

            Assert.Equal(200, response.StatusCode);
            Assert.True(makemoveresponse.IsValid);
            Assert.Equal(0, makemoveresponse.Reason);
        }

        [Fact]
        public async Task GameAddAIVsHumanAndMoveAIGetBoardAsync()
        {
            TestLambdaContext context;
            APIGatewayProxyRequest request;
            APIGatewayProxyResponse response;

            Functions functions = new Functions(this.DDBClient, this.TableName);

            // Test adding a new game using player parameters
            var myGame = CreateNewOthelloAIVSHumanGame(out var myPlayers, out var currentPlayer);
            var game = await AddOthelloGameRepresentation(myPlayers, functions).ConfigureAwait(true);


            // Test getting the game back out
            request = new APIGatewayProxyRequest
            {
                PathParameters = new Dictionary<string, string> { { Functions.IdQueryStringName, game.Id } }
            };
            context = new TestLambdaContext();
            response = await functions.GetGameAsync(request, context).ConfigureAwait(true);
            Assert.Equal(200, response.StatusCode);

            OthelloGameRepresentation readGame = JsonConvert.DeserializeObject<OthelloGameRepresentation>(response.Body);
            Assert.Equal(game.Id, readGame.Id);
            Assert.Equal(myGame.OthelloGameStrRepresentation, readGame.OthelloGameStrRepresentation);

            OthelloAdapter adapter = new OthelloAdapter();
            adapter.GetGameFromJSON(readGame.OthelloGameStrRepresentation);
            var initialboard = adapter.GameDebugGetBoardInString();

            // Test AI Move
            // Test we can get an valid move response
            OthelloServerlessMakeMove myMoves = new OthelloServerlessMakeMove();
            myMoves.CurrentPlayer = new OthelloGamePlayer(OthelloPlayerKind.White, "default-name-white");

            request = new APIGatewayProxyRequest
            {
                PathParameters = new Dictionary<string, string> { { Functions.IdQueryStringName, game.Id } },
                Body = JsonConvert.SerializeObject(myMoves)
            };
            context = new TestLambdaContext();
            response = await functions.MakeGameAIMoveAsync(request, context).ConfigureAwait(true);
            var makemoveresponse = JsonConvert.DeserializeObject<OthelloServerlessMakeMoveFliplist>(response?.Body);

            Assert.Equal(200, response.StatusCode);
            Assert.True(makemoveresponse.IsValid);
            Assert.Equal(0, makemoveresponse.Reason);

            //Get Board Data
            request = new APIGatewayProxyRequest
            {
                PathParameters = new Dictionary<string, string> { { Functions.IdQueryStringName, game.Id } },
                QueryStringParameters = new Dictionary<string, string> { { Functions.DebugStringName, "true" } }
            };
            context = new TestLambdaContext();
            response = await functions.GetGameBoardDataAsync(request, context).ConfigureAwait(true);
            var board = JsonConvert.DeserializeObject<string>(response?.Body);
            Assert.Equal(200, response.StatusCode);
            Assert.NotEqual(initialboard, board);
        }


        private static async Task<OthelloGameRepresentation> AddOthelloGameRepresentation(OthelloServerlessPlayers myPlayers,
            Functions functions)
        {
            APIGatewayProxyRequest request;
            TestLambdaContext context;
            APIGatewayProxyResponse response;
            request = new APIGatewayProxyRequest
            {
                Body = JsonConvert.SerializeObject(myPlayers)
            };

            context = new TestLambdaContext();
            response = await functions.AddGameAsync(request, context).ConfigureAwait(false);
            Assert.Equal(200, response.StatusCode);

            var game = JsonConvert.DeserializeObject<OthelloGameRepresentation>(response.Body);
            return game;
        }
        private static OthelloGameRepresentation CreateNewOthelloHumanVSHumanGame(out OthelloServerlessPlayers myPlayers,
            out OthelloGamePlayer currentPlayer)
        {
            OthelloGameRepresentation myGame = new OthelloGameRepresentation();
            myPlayers = new OthelloServerlessPlayers();
            myPlayers.PlayerNameWhite = "PlayerA";
            myPlayers.PlayerNameBlack = "PlayerB";
            myPlayers.FirstPlayerKind = OthelloPlayerKind.White;

            OthelloAdapterBase OthelloGameAdapter = new OthelloAdapters.OthelloAdapter();
            OthelloGameAdapter.GameCreateNewHumanVSHuman(myPlayers.PlayerNameWhite, myPlayers.PlayerNameBlack, myPlayers.FirstPlayerKind,
                false);
            currentPlayer = OthelloGameAdapter.GameUpdatePlayer();

            myGame.CreatedTimestamp = DateTime.Now;
            myGame.OthelloGameStrRepresentation = OthelloGameAdapter.GetGameJSON();

            return myGame;
        }

        private static OthelloGameRepresentation CreateNewOthelloAIVSHumanGame(out OthelloServerlessPlayers myPlayers,
            out OthelloGamePlayer currentPlayer)
        {
            OthelloGameRepresentation myGame = new OthelloGameRepresentation();
            myPlayers = new OthelloServerlessPlayers();
            myPlayers.PlayerNameWhite = "PlayerA";
            myPlayers.PlayerNameBlack = "PlayerB";
            myPlayers.FirstPlayerKind = OthelloPlayerKind.White;
            myPlayers.UseAI = true;
            myPlayers.IsHumanWhite = false;
            myPlayers.Difficulty = GameDifficultyMode.Default;

            OthelloAdapterBase OthelloGameAdapter = new OthelloAdapters.OthelloAdapter();
            OthelloGameAdapter.GameCreateNewHumanVSAI(myPlayers.PlayerNameWhite, myPlayers.PlayerNameBlack,
                false, false, myPlayers.Difficulty);
            currentPlayer = OthelloGameAdapter.GameUpdatePlayer();

            myGame.CreatedTimestamp = DateTime.Now;
            myGame.OthelloGameStrRepresentation = OthelloGameAdapter.GetGameJSON();

            return myGame;
        }

        /// <summary>
        /// Create the DynamoDB table for testing. This table is deleted as part of the object dispose method.
        /// </summary>
        /// <returns></returns>
        private async Task SetupTableAsync()
        {
            
            CreateTableRequest request = new CreateTableRequest
            {
                TableName = this.TableName,
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 2,
                    WriteCapacityUnits = 2
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement
                    {
                        KeyType = KeyType.HASH,
                        AttributeName = Functions.IdQueryStringName
                    }
                },
                AttributeDefinitions = new List<AttributeDefinition>
                {
                    new AttributeDefinition
                    {
                        AttributeName = Functions.IdQueryStringName,
                        AttributeType = ScalarAttributeType.S
                    }
                }
            };

            await this.DDBClient.CreateTableAsync(request).ConfigureAwait(false);

            var describeRequest = new DescribeTableRequest { TableName = this.TableName };
            DescribeTableResponse response = null;
            do
            {
                Thread.Sleep(1000);
                response = await this.DDBClient.DescribeTableAsync(describeRequest).ConfigureAwait(true);
            } while (response.Table.TableStatus != TableStatus.ACTIVE);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.DDBClient.DeleteTableAsync(this.TableName).Wait();
                    this.DDBClient.Dispose();
                }

                disposedValue = true;
            }
        }


        public void Dispose()
        {
            Dispose(true);
            // https://docs.microsoft.com/ja-jp/visualstudio/code-quality/ca1816-call-gc-suppressfinalize-correctly?view=vs-2015
            GC.SuppressFinalize(this);
        }
        #endregion


    }
}
