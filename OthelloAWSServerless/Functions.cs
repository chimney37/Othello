using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;

using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

using Newtonsoft.Json;

using Othello;
using OthelloAdapters;
using System.Globalization;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace OthelloAWSServerless
{
    public class Functions
    {
        // This const is the name of the environment variable that the serverless.template will use to set
        // the name of the DynamoDB table used to store Othello Game
        const string TableNameEnvironmentVariableLookup = "OthelloGameTable";

        public const string IdQueryStringName = "Id";

        IDynamoDBContext DDBContext { get; set; }
        AmazonDynamoDBClient DDBClient { get; set; }

        /// <summary>
        /// Default constructor that Lambda will invoke.
        /// </summary>
        public Functions()
        {
            // Check to see if a table name was passed in through environment variables and if so add the table mapping
            var tableName = System.Environment.GetEnvironmentVariable(TableNameEnvironmentVariableLookup);
            if(!string.IsNullOrEmpty(tableName))
            {
                AWSConfigsDynamoDB.Context.TypeMappings[typeof(OthelloGameRepresentation)] = new Amazon.Util.TypeMapping(typeof(OthelloGameRepresentation), tableName);
            }

            var config = new DynamoDBContextConfig { Conversion = DynamoDBEntryConversion.V2 };
            this.DDBClient = new AmazonDynamoDBClient();
            this.DDBContext = new DynamoDBContext(DDBClient, config);
        }

        /// <summary>
        /// Constructor for Lambda will invoke to set up the DDB context using a preconfigured DynamoDB client.
        /// </summary>
        /// <param name="ddbClient"></param>
        /// <param name="tableName"></param>
        public Functions(IAmazonDynamoDB ddbClient, string tableName)
        {
            if (!string.IsNullOrEmpty(tableName))
            {
                AWSConfigsDynamoDB.Context.TypeMappings[typeof(OthelloGameRepresentation)] = new Amazon.Util.TypeMapping(typeof(OthelloGameRepresentation), tableName);
            }

            var config = new DynamoDBContextConfig { Conversion = DynamoDBEntryConversion.V2 };
            this.DDBContext = new DynamoDBContext(ddbClient, config);
        }

        /// <summary>
        /// A Lambda function that returns back a page worth of Othello Games
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns>The list of games</returns>
        public async Task<APIGatewayProxyResponse> GetGamesAsync(APIGatewayProxyRequest request, ILambdaContext context)
        {
            ThrowExceptionIfNull(context);

            if (request == null)
                context.Logger.LogLine(string.Format(CultureInfo.InvariantCulture, "request is empty."));

            context.Logger.LogLine(string.Format(CultureInfo.InvariantCulture, "Getting games."));
            var search = this.DDBContext.ScanAsync<OthelloGameRepresentation>(null);
            var page = await search.GetNextSetAsync().ConfigureAwait(false);
            context.Logger.LogLine($"Found {page.Count} games");

            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = JsonConvert.SerializeObject(page),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };

            return response;
        }

        /// <summary>
        /// A Lambda function that returns the Othello Game identified by Id
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns>return base64 string version of Othello game object</returns>
        public async Task<APIGatewayProxyResponse> GetGameAsync(APIGatewayProxyRequest request, ILambdaContext context)
        {
            ThrowExceptionIfNull(request);
            ThrowExceptionIfNull(context);

            var gameId = SetGameId(request);

            if (string.IsNullOrEmpty(gameId))
            {
                return ApiGatewayProxyResponseMissingGameId();
            }

            context.Logger.LogLine($"Getting game: {gameId}");
            var game = await DDBContext.LoadAsync<OthelloGameRepresentation>(gameId).ConfigureAwait(false);
            context.Logger.LogLine($"Found game: {game != null}");

            if (game == null)
            {
                return ApiGatewayProxyResponseGameIdNotFound();
            }

            context.Logger.LogLine($"Loading game with representation: {game.OthelloGameStrRepresentation}");

            OthelloAdapter othelloGameAdapter = new OthelloAdapter();
            othelloGameAdapter.GetGameFromJSON(game.OthelloGameStrRepresentation);
            var player = othelloGameAdapter.GameUpdatePlayer();
            var gameMode = othelloGameAdapter.GameGetMode();

            context.Logger.LogLine($"Next player is: {player.PlayerKind} and name={player.PlayerName}");
            context.Logger.LogLine($"board data is: {gameMode.ToString()}");

            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int) HttpStatusCode.OK,
                Body = JsonConvert.SerializeObject(game),
                Headers = new Dictionary<string, string> {{"Content-Type", "application/json"}}
            };
            return response;
        }


        /// <summary>
        /// A Lambda function that adds (creates) a Human vs Human Othello Game.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<APIGatewayProxyResponse> AddGameAsync(APIGatewayProxyRequest request, ILambdaContext context)
        {
            ThrowExceptionIfNull(context);
            ThrowExceptionIfNull(request);

            var playerdata = JsonConvert.DeserializeObject<OthelloServerlessPlayers>(request?.Body);
            OthelloPlayerKind playerkind = OthelloPlayerKind(playerdata.FirstPlayer);

            OthelloAdapter OthelloGameAdapter = new OthelloAdapter();
            OthelloGameAdapter.GameCreateNewHumanVSHuman(playerdata.PlayerNameWhite, playerdata.PlayerNameBlack, playerkind, false);

            var othellogame = new OthelloGameRepresentation();
            othellogame.Id = Guid.NewGuid().ToString();
            othellogame.CreatedTimestamp = DateTime.Now;
            othellogame.OthelloGameStrRepresentation = OthelloGameAdapter.GetGameJSON();

            context.Logger.LogLine($"Saving game with id {othellogame.Id}");
            await DDBContext.SaveAsync<OthelloGameRepresentation>(othellogame).ConfigureAwait(false);

            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = othellogame.Id.ToString(CultureInfo.InvariantCulture),
                Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
            };
            return response;
        }

        /// <summary>
        /// A Lambda function that removes a Othello Game from the DynamoDB table.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<APIGatewayProxyResponse> RemoveGameAsync(APIGatewayProxyRequest request, ILambdaContext context)
        {
            ThrowExceptionIfNull(request);
            ThrowExceptionIfNull(context);

            var gameId = SetGameId(request);

            if (string.IsNullOrEmpty(gameId))
            {
                return ApiGatewayProxyResponseMissingGameId();
            }
            context.Logger.LogLine($"Deleting game with id {gameId}");

            await this.DDBContext.DeleteAsync<OthelloGameRepresentation>(gameId).ConfigureAwait(false);

            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// A lambda function that get the current player identifier for a given game.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns>current player kind</returns>
        public async Task<APIGatewayProxyResponse> GetGameCurrentPlayerAsync(APIGatewayProxyRequest request, ILambdaContext context)
        {
            ThrowExceptionIfNull(request);
            ThrowExceptionIfNull(context);

            var gameId = SetGameId(request);

            if (string.IsNullOrEmpty(gameId))
            {
                return ApiGatewayProxyResponseMissingGameId();
            }
            context.Logger.LogLine($"Getting game: {gameId}");

            var game = await DDBContext.LoadAsync<OthelloGameRepresentation>(gameId).ConfigureAwait(false);
            context.Logger.LogLine($"Found game: {game != null}");

            if (game == null)
            {
                return ApiGatewayProxyResponseGameIdNotFound();
            }

            OthelloAdapter othelloGameAdapter = new OthelloAdapter();
            othelloGameAdapter.GetGameFromJSON(game.OthelloGameStrRepresentation);
            var player = othelloGameAdapter.GameUpdatePlayer();

            var currentplayer = new OthelloServerlessCurrentPlayer();
            currentplayer.CurrentPlayer = player.PlayerKind.ToString();

            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = JsonConvert.SerializeObject(currentplayer),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
            return response;
        }

        /// <summary>
        /// A lambda function that makes a move for a given player specified for a given game. 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<APIGatewayProxyResponse> MakeGameMoveAsync(APIGatewayProxyRequest request, ILambdaContext context)
        {
            ThrowExceptionIfNull(request);
            ThrowExceptionIfNull(context);

            var gameId = SetGameId(request);

            if (string.IsNullOrEmpty(gameId))
            {
                return ApiGatewayProxyResponseMissingGameId();
            }
            context.Logger.LogLine($"Getting game: {gameId}");

            var game = await DDBContext.LoadAsync<OthelloGameRepresentation>(gameId).ConfigureAwait(false);
            context.Logger.LogLine($"Found game: {game != null}");

            if (game == null)
            {
                return ApiGatewayProxyResponseGameIdNotFound();
            }

            OthelloAdapter othelloGameAdapter = new OthelloAdapter();
            othelloGameAdapter.GetGameFromJSON(game.OthelloGameStrRepresentation);
            var exepectedPlayer = othelloGameAdapter.GameUpdatePlayer();

            var movedata = JsonConvert.DeserializeObject<OthelloServerlessMakeMove>(request?.Body);
            var playerkind = OthelloPlayerKind(movedata.CurrentPlayer);
            var fliplist = othelloGameAdapter.GameMakeMove(movedata.GameX, movedata.GameY, exepectedPlayer, out var isInvalidMove);

            //generate final response body
            var moveresponse = new OthelloServerlessMakeMoveFliplist();
            moveresponse.Move = movedata;
            moveresponse.Fliplist = fliplist;
            moveresponse.Reason |= (playerkind == exepectedPlayer.PlayerKind) ? 0 : 0x1;
            moveresponse.Reason |= (!isInvalidMove) ? 0 : 0x2;
            moveresponse.IsValid = moveresponse.Reason == 0;

            //update DDB with new game state if valid
            if (moveresponse.IsValid)
            {
                var othellogame = new OthelloGameRepresentation();
                othellogame.Id = gameId;
                othellogame.CreatedTimestamp = DateTime.Now;
                othellogame.OthelloGameStrRepresentation = othelloGameAdapter.GetGameJSON();

                context.Logger.LogLine($"Saving game with id {gameId}");
                await DDBContext.SaveAsync<OthelloGameRepresentation>(othellogame).ConfigureAwait(false);
            }

            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = JsonConvert.SerializeObject(moveresponse),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
            return response;
        }

        /// <summary>
        /// A lambda function that gets the data of the game board
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<APIGatewayProxyResponse> GetGameBoardDataAsync(APIGatewayProxyRequest request, ILambdaContext context)
        {
            ThrowExceptionIfNull(request);
            ThrowExceptionIfNull(context);

            var gameId = SetGameId(request);

            if (string.IsNullOrEmpty(gameId))
            {
                return ApiGatewayProxyResponseMissingGameId();
            }
            context.Logger.LogLine($"Getting game: {gameId}");

            var game = await DDBContext.LoadAsync<OthelloGameRepresentation>(gameId).ConfigureAwait(false);
            context.Logger.LogLine($"Found game: {game != null}");

            if (game == null)
            {
                return ApiGatewayProxyResponseGameIdNotFound();
            }

            OthelloAdapter othelloGameAdapter = new OthelloAdapter();
            othelloGameAdapter.GetGameFromJSON(game.OthelloGameStrRepresentation);
            var boarddata = othelloGameAdapter.GameGetBoardData();

            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = JsonConvert.SerializeObject(boarddata),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
            return response;
        }
        private static OthelloPlayerKind OthelloPlayerKind(string parsesource)
        {
            OthelloPlayerKind playerkind;
            if (!Enum.TryParse(parsesource, out playerkind))
                throw new Exception(string.Format(CultureInfo.InvariantCulture, "failed enum parse"));
            return playerkind;
        }
        private static APIGatewayProxyResponse ApiGatewayProxyResponseMissingGameId()
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Body = $"Missing/Bad required parameter {IdQueryStringName}"
            };
        }
        private static APIGatewayProxyResponse ApiGatewayProxyResponseGameIdNotFound()
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.NotFound
            };
        }
        private static string SetGameId(APIGatewayProxyRequest request)
        {
            string gameId = null;
            if (request.PathParameters != null && request.PathParameters.ContainsKey(IdQueryStringName))
                gameId = request.PathParameters[IdQueryStringName];
            else if (request.QueryStringParameters != null && request.QueryStringParameters.ContainsKey(IdQueryStringName))
                gameId = request.QueryStringParameters[IdQueryStringName];
            return gameId;
        }
        private static void ThrowExceptionIfNull(Object target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
        }

    }
}
