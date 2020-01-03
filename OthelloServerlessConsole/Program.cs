using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using RestSharp;
using Newtonsoft.Json;

using Othello;
using OthelloAdapters;
using OthelloAWSServerless;

namespace OthelloServerlessConsole
{
    class Program
    {
        private const string GameIdQueryParameter = "Id";
        static bool gameContinue;
        static GameStateMode gameMode;
        private static OthelloAdapter adapter;
        private static OthelloGamePlayer playerWhite { get; set; }
        private static OthelloGamePlayer playerBlack { get; set; }
        private static OthelloGamePlayer currentPlayer { get; set; }
        private static int turn { get; set; }
        private static bool isGameEnded { get; set; }
        private static int playerWhiteScore { get; set; }
        private static int playerBlackScore { get; set; }

        private static string Id { get; set; }
        private static DateTime UpdatedDatetime { get; set; }

        private static RestClient client;

        private static StringBuilder messageBuffer { get; set; }

        static void Main()
        {
            InitializeGame();

            // game loop
            do
            {
                Update();

                Render();

                ProcessUserInput();

                ProcessGame();

            } while (gameContinue);

            DisposeGame();
        }

        private static void DisposeGame()
        {
            OthelloServerlessRESTDestroyGame();
        }

        private static void InitializeGame()
        {
            gameContinue = true;
            gameMode = 0;

            adapter = new OthelloAdapter();

            client = new RestClient("https://7q06k0lshh.execute-api.ap-northeast-1.amazonaws.com/Prod");

            playerWhite = new OthelloGamePlayer(OthelloPlayerKind.White, "Player White");
            playerBlack = new OthelloGamePlayer(OthelloPlayerKind.Black, "Player Black");

            messageBuffer = new StringBuilder();
        }

        private static void Update()
        {
            if (Id != null)
            {
                OthelloServerlessRESTUpdateGame();
            }

            currentPlayer = adapter.GameUpdatePlayer();
            turn = adapter.GameUpdateTurn();
            isGameEnded = adapter.GameIsEndGame();
            playerWhiteScore = adapter.GameGetScore(playerWhite);
            playerBlackScore = adapter.GameGetScore(playerBlack);
        }

        private static void ProcessUserInput()
        {
            ConsoleKeyInfo c = Console.ReadKey(true);

            switch (c.Key)
            {
                case ConsoleKey.Escape:
                    gameMode = GameStateMode.DestroyGame;
                    gameContinue = false;
                    break;
                case ConsoleKey.D0:
                    gameMode = GameStateMode.Debug;
                    break;
                case ConsoleKey.N:
                    gameMode = GameStateMode.NewServerlessAIGame;
                    break;
                case ConsoleKey.F1:
                    gameMode = GameStateMode.NewHumansGame;
                    break;
                case ConsoleKey.F2:
                    gameMode = GameStateMode.SwitchPlayer;
                    break;
                case ConsoleKey.D4:
                    gameMode = GameStateMode.InputMove;
                    break;
                case ConsoleKey.D5:
                    gameMode = GameStateMode.AIMove;
                    break;
                default:
                    gameMode = GameStateMode.DoNothing;
                    break;
            }
        }

        private static void Render()
        {
            Console.Clear();
            Console.WriteLine(string.Format(CultureInfo.CurrentCulture, "Press <Esc> to Exit"));
            Console.WriteLine(string.Format(CultureInfo.CurrentCulture, "Menu: N:NewAIGame, F1:NewHumanVsHumanGame, L:LoadGame, S: SaveGame, 4: InputMove, 5: A.I., 6: Run Basic Tests\n"));

            adapter.GameDebugGetBoardInString();
            Console.WriteLine(string.Format(CultureInfo.CurrentCulture, "Current player: {0}, {1}", currentPlayer.PlayerName, currentPlayer.PlayerKind));
            Console.WriteLine(string.Format(CultureInfo.CurrentCulture, "Current score: {0}:{1}, {2}:{3}", 
                playerWhite, playerWhiteScore, 
                playerBlack, playerBlackScore));

            Console.WriteLine(string.Format("Turn: {0}", turn));

            if (isGameEnded)
            {
                Console.WriteLine("Game ended.");
            }

            Console.WriteLine(messageBuffer.ToString());
            messageBuffer.Clear();
        }

        static void ProcessGame()
        {
            switch (gameMode)
            {
                case GameStateMode.Debug:
                    break;
                case GameStateMode.NewServerlessAIGame:
                    OthelloServerlessRESTDestroyGame();
                    OthelloServerlessRESTCreateNewGame(CreateNewHumanVsAiPlayers);
                    break;
                case GameStateMode.NewHumansGame:
                    OthelloServerlessRESTCreateNewGame(CreateNewHumansOnlyPlayers);
                    break;
                case GameStateMode.InputMove:
                    ProcessMoveInput();
                    break;
                case GameStateMode.AIMove:
                    OthelloServerlessRESTMakeMoveAI();
                    break;
                case GameStateMode.DestroyGame:
                    OthelloServerlessRESTDestroyGame();
                    break;
            }
        }

        private static void OthelloServerlessRESTMakeMove(int x, int y)
        {
            var request = new RestRequest(string.Format(CultureInfo.InvariantCulture, "move/{0}", Id), Method.PUT);
            OthelloServerlessMakeMove myMoves = new OthelloServerlessMakeMove();
            myMoves.CurrentPlayer = adapter.GameUpdatePlayer();
            myMoves.GameX = x;
            myMoves.GameY = y;
            request.AddJsonBody(myMoves);

            IRestResponse response = client.Execute(request);
            var data = JsonConvert.DeserializeObject<OthelloServerlessMakeMoveFliplist>(response.Content);

            var flipliststr = string.Join(",", data.Fliplist.Select(x => x.ToString()));
            var message = string.Format(CultureInfo.CurrentCulture, "move by AI. http response:{0}, FlipList: {1}",
                response.StatusCode,
                flipliststr);
            messageBuffer.AppendLine(message);
        }

        private static void OthelloServerlessRESTMakeMoveAI()
        {
            if (Id != null && adapter.GameGetMode() == GameMode.HumanVSComputer)
            {
                var request = new RestRequest(string.Format(CultureInfo.InvariantCulture, "movebyai/{0}", Id), Method.PUT);
                OthelloServerlessMakeMove myMoves = new OthelloServerlessMakeMove();
                myMoves.CurrentPlayer = adapter.GameUpdatePlayer();
                request.AddJsonBody(myMoves);

                IRestResponse response = client.Execute(request);
                var data = JsonConvert.DeserializeObject<OthelloServerlessMakeMoveFliplist>(response.Content);

                var flipliststr = string.Join(",", data.Fliplist.Select(x => x.ToString()));
                var message = string.Format(CultureInfo.CurrentCulture, "move by AI. http response:{0}, FlipList: {1}",
                    response.StatusCode,
                    flipliststr);
                messageBuffer.AppendLine(message);
            }
            else
            {
                var message = "A.I game not initialized.";
                messageBuffer.AppendLine(message);
            }
        }

        private static void OthelloServerlessRESTCreateNewGame(Func<OthelloServerlessPlayers> createNewOthelloGame)
        {
            var request = new RestRequest("/", Method.PUT);
            var body = createNewOthelloGame();
            request.AddJsonBody(body);

            IRestResponse response = client.Execute(request);
            var data = JsonConvert.DeserializeObject<OthelloGameRepresentation>(response.Content);
            adapter.GetGameFromJSON(data.OthelloGameStrRepresentation);
            Id = data.Id;
            UpdatedDatetime = data.CreatedTimestamp;

            var message = string.Format(CultureInfo.CurrentCulture,
                "Creating New Game. Http response: {0}, gameId={1}",
                response.StatusCode, data.Id);
            messageBuffer.AppendLine(message);
        }

        private static void OthelloServerlessRESTUpdateGame()
        {
            var request = new RestRequest(Id, Method.GET);
            IRestResponse response = client.Execute(request);
            var data = JsonConvert.DeserializeObject<OthelloGameRepresentation>(response.Content);
            adapter.GetGameFromJSON(data.OthelloGameStrRepresentation);

            var message = string.Format(CultureInfo.CurrentCulture, "Updated Game. http response: {0}",
                response.StatusCode);
            messageBuffer.AppendLine(message);
        }

        private static void OthelloServerlessRESTDestroyGame()
        {
            if (Id != null)
            {
                var request = new RestRequest("/", Method.DELETE);
                request.AddQueryParameter(GameIdQueryParameter, Id);

                IRestResponse response = client.Execute(request);

                var message = string.Format(CultureInfo.CurrentCulture,
                    "Deleted Game. Http response: {0}, gameId={1}",
                    response.StatusCode, Id);
                messageBuffer.AppendLine(message);
            }
        }

        private static void ProcessMoveInput()
        {
            Console.WriteLine(string.Format(CultureInfo.CurrentCulture, "Enter x,y then <Enter>"));
            string input = Console.ReadLine();
            if (Regex.IsMatch(input, @"^\d,\d"))
            {
                MatchCollection mc = Regex.Matches(input, @"\d");

                int x = int.Parse(mc[0].Value, CultureInfo.InvariantCulture);
                int y = int.Parse(mc[1].Value, CultureInfo.InvariantCulture);

                OthelloServerlessRESTMakeMove(x, y);
            }
        }
        private static OthelloServerlessPlayers CreateNewHumanVsAiPlayers()
        {
            //TODO: found a bug that when creating a new human vs. ai game, the name becomes "human" and "computer" as that is hardcoed inside othello adapter.
            var myPlayers = new OthelloServerlessPlayers();
            myPlayers.PlayerNameWhite = playerWhite.PlayerName;
            myPlayers.PlayerNameBlack = playerBlack.PlayerName;
            myPlayers.FirstPlayerKind = playerWhite.PlayerKind;
            myPlayers.UseAI = true;
            myPlayers.IsHumanWhite = true;
            myPlayers.Difficulty = GameDifficultyMode.Default;

            return myPlayers;
        }

        private static OthelloServerlessPlayers CreateNewHumansOnlyPlayers()
        {
            OthelloServerlessPlayers myPlayers = new OthelloServerlessPlayers();
            myPlayers = new OthelloServerlessPlayers();
            myPlayers.PlayerNameWhite = playerWhite.PlayerName;
            myPlayers.PlayerNameBlack = playerBlack.PlayerName;
            myPlayers.FirstPlayerKind = playerWhite.PlayerKind;
            myPlayers.UseAI = false;

            return myPlayers;
        }
    }
}
