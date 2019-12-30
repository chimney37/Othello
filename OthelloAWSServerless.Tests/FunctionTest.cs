using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace OthelloAWSServerless.Tests
{
    public class FunctionTest : IDisposable
    { 
        string TableName { get; }
        IAmazonDynamoDB DDBClient { get; }
        
        public FunctionTest()
        {
            this.TableName = "OthelloAWSServerless-OthelloGame-" + DateTime.Now.Ticks;
            this.DDBClient = new AmazonDynamoDBClient(RegionEndpoint.USWest2);

            SetupTableAsync().Wait();
        }

        [Fact]
        public async Task GameListTestAsync()
        {
            TestLambdaContext context;
            APIGatewayProxyRequest request;
            APIGatewayProxyResponse response;

            Functions functions = new Functions(this.DDBClient, this.TableName);

            var myGame = CreateNewOthelloGame(out var myPlayers, out var currentPlayer);
            request = new APIGatewayProxyRequest
            {
                Body = JsonConvert.SerializeObject(myPlayers)
            };

            context = new TestLambdaContext();
            response = await functions.AddGameAsync(request, context).ConfigureAwait(false);
            Assert.Equal(200, response.StatusCode);

            var gameId = response.Body;

            // List the games
            request = new APIGatewayProxyRequest
            {
            };
            context = new TestLambdaContext();
            response = await functions.GetGamesAsync(request, context).ConfigureAwait(false);
            Assert.Equal(200, response.StatusCode);

            OthelloGameRepresentation[] gamePosts = JsonConvert.DeserializeObject<OthelloGameRepresentation[]>(response.Body);
            Assert.Single(gamePosts);
            Assert.Equal(gameId, gamePosts[0].Id);
            Assert.Equal(myGame.OthelloGameStrRepresentation, gamePosts[0].OthelloGameStrRepresentation);
        }

        [Fact]
        public async Task GameGetGameTestAsync()
        {
            TestLambdaContext context;
            APIGatewayProxyRequest request;
            APIGatewayProxyResponse response;

            Functions functions = new Functions(this.DDBClient, this.TableName);

            // Add a new game using player parameters
            var myGame = CreateNewOthelloGame(out var myPlayers, out var currentPlayer);

            request = new APIGatewayProxyRequest
            {
                Body = JsonConvert.SerializeObject(myPlayers)
            };

            context = new TestLambdaContext();
            response = await functions.AddGameAsync(request, context).ConfigureAwait(false);
            Assert.Equal(200, response.StatusCode);

            var gameId = response.Body;

            // get the game back out
            request = new APIGatewayProxyRequest
            {
                PathParameters = new Dictionary<string, string> { { Functions.IdQueryStringName, gameId } }
            };
            context = new TestLambdaContext();
            response = await functions.GetGameAsync(request, context).ConfigureAwait(false);
            Assert.Equal(200, response.StatusCode);

            OthelloGameRepresentation readGame = JsonConvert.DeserializeObject<OthelloGameRepresentation>(response.Body);
            Assert.Equal(gameId, readGame.Id);
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
            var myGame = CreateNewOthelloGame(out var myPlayers, out var currentPlayer);

            request = new APIGatewayProxyRequest
            {
                Body = JsonConvert.SerializeObject(myPlayers)
            };

            context = new TestLambdaContext();
            response = await functions.AddGameAsync(request, context).ConfigureAwait(false);
            Assert.Equal(200, response.StatusCode);

            var gameId = response.Body;

            // Delete the game
            request = new APIGatewayProxyRequest
            {
                PathParameters = new Dictionary<string, string> { { Functions.IdQueryStringName, gameId } }
            };
            context = new TestLambdaContext();
            response = await functions.RemoveGameAsync(request, context).ConfigureAwait(false);
            Assert.Equal(200, response.StatusCode);

            // Make sure the game was deleted.
            request = new APIGatewayProxyRequest
            {
                PathParameters = new Dictionary<string, string> { { Functions.IdQueryStringName, gameId } }
            };
            context = new TestLambdaContext();
            response = await functions.GetGameAsync(request, context).ConfigureAwait(false);
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
            var myGame = CreateNewOthelloGame(out var myPlayers, out var currentPlayer);

            request = new APIGatewayProxyRequest
            {
                Body = JsonConvert.SerializeObject(myPlayers)
            };

            context = new TestLambdaContext();
            response = await functions.AddGameAsync(request, context).ConfigureAwait(false);
            Assert.Equal(200, response.StatusCode);
            var gameId = response.Body;

            //Test we can get the game's current player
            request = new APIGatewayProxyRequest
            {
                PathParameters = new Dictionary<string, string> { { Functions.IdQueryStringName, gameId } }
            };
            context = new TestLambdaContext();
            response = await functions.GetGameCurrentPlayerAsync(request, context).ConfigureAwait(false);
            Assert.Equal(200, response.StatusCode);

            OthelloServerlessCurrentPlayer getcurrentPlayer = JsonConvert.DeserializeObject<OthelloServerlessCurrentPlayer>(response.Body);
            Assert.Equal(currentPlayer.ToString(), getcurrentPlayer.CurrentPlayer);
        }

        [Fact]
        public async Task GameTestMakeMoveAsync()
        {
            TestLambdaContext context;
            APIGatewayProxyRequest request;
            APIGatewayProxyResponse response;

            Functions functions = new Functions(this.DDBClient, this.TableName);

            // Add a new game using player parameters
            var myGame = CreateNewOthelloGame(out var myPlayers, out var currentPlayer);

            request = new APIGatewayProxyRequest
            {
                Body = JsonConvert.SerializeObject(myPlayers)
            };

            context = new TestLambdaContext();
            response = await functions.AddGameAsync(request, context).ConfigureAwait(false);
            Assert.Equal(200, response.StatusCode);

            var gameId = response.Body;

            // Test we can get an valid move response
            OthelloServerlessMakeMove myMoves = new OthelloServerlessMakeMove();
            myMoves.GameX = 3;
            myMoves.GameY = 2;
            myMoves.CurrentPlayer = "White";

            request = new APIGatewayProxyRequest
            {
                PathParameters = new Dictionary<string, string> { { Functions.IdQueryStringName, gameId } },
                Body = JsonConvert.SerializeObject(myMoves)
            };

            context = new TestLambdaContext();
            response = await functions.MakeGameMoveAsync(request, context).ConfigureAwait(false);
            Assert.Equal(200, response.StatusCode);
        }

        private static OthelloGameRepresentation CreateNewOthelloGame(out OthelloServerlessPlayers myPlayers,
            out OthelloGamePlayer currentPlayer)
        {
            OthelloGameRepresentation myGame = new OthelloGameRepresentation();
            myPlayers = new OthelloServerlessPlayers();
            myPlayers.PlayerNameWhite = "PlayerA";
            myPlayers.PlayerNameBlack = "PlayerB";
            myPlayers.FirstPlayer = "White";
            var playerkind = (OthelloPlayerKind) Enum.Parse(typeof(OthelloPlayerKind), myPlayers.FirstPlayer);

            OthelloAdapters.OthelloAdapterBase OthelloGameAdapter = new OthelloAdapters.OthelloAdapter();
            OthelloGameAdapter.GameCreateNewHumanVSHuman(myPlayers.PlayerNameWhite, myPlayers.PlayerNameBlack, playerkind,
                false);
            currentPlayer = OthelloGameAdapter.GameUpdatePlayer();
            var currentPlayerKind = currentPlayer.PlayerKind.ToString();

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
                response = await this.DDBClient.DescribeTableAsync(describeRequest).ConfigureAwait(false);
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
