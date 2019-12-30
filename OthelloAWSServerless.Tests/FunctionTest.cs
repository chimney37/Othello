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
        public async Task GameTestAsync()
        {
            TestLambdaContext context;
            APIGatewayProxyRequest request;
            APIGatewayProxyResponse response;

            Functions functions = new Functions(this.DDBClient, this.TableName);

            // Add a new game using player parameters
            OthelloGameRepresentation myGame = new OthelloGameRepresentation();
            OthelloServerlessPlayers myPlayers = new OthelloServerlessPlayers();
            myPlayers.PlayerNameWhite = "PlayerA";
            myPlayers.PlayerNameBlack = "PlayerB";
            myPlayers.FirstPlayer = "White";
            var playerkind = (OthelloPlayerKind)Enum.Parse(typeof(OthelloPlayerKind), myPlayers.FirstPlayer);

            OthelloAdapters.OthelloAdapterBase OthelloGameAdapter = new OthelloAdapters.OthelloAdapter();
            OthelloGameAdapter.GameCreateNewHumanVSHuman(myPlayers.PlayerNameWhite, myPlayers.PlayerNameBlack, playerkind, false);
            var currentPlayer = OthelloGameAdapter.GameUpdatePlayer();
            var currentPlayerKind = currentPlayer.PlayerKind.ToString();

            myGame.CreatedTimestamp = DateTime.Now;
            myGame.OthelloGameStrRepresentation = OthelloGameAdapter.GetGameJSON();

            Trace.WriteLine(JsonConvert.SerializeObject(myPlayers));

            request = new APIGatewayProxyRequest
            {
                Body = JsonConvert.SerializeObject(myPlayers)
            };

            context = new TestLambdaContext();
            response = await functions.AddGameAsync(request, context).ConfigureAwait(false);
            Assert.Equal(200, response.StatusCode);
            var gameId = response.Body;


            // Confirm we can get the game back out
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


            //Confirm we can get the game's current player
            request = new APIGatewayProxyRequest
            {
                PathParameters = new Dictionary<string, string> { { Functions.IdQueryStringName, gameId } }
            };
            context = new TestLambdaContext();
            response = await functions.GetGameCurrentPlayerAsync(request, context).ConfigureAwait(false);
            Assert.Equal(200, response.StatusCode);

            OthelloServerlessCurrentPlayer getcurrentPlayer = JsonConvert.DeserializeObject<OthelloServerlessCurrentPlayer>(response.Body);
            Assert.Equal(currentPlayer.ToString(), getcurrentPlayer.CurrentPlayer);

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
