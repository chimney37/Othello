using System;
using System.Globalization;
using System.Text.Json.Serialization;
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
        static bool gameContinue;
        static GameStateMode gameMode;
        static bool IsAlternateGame = true;
        static OthelloToken[,] oBoard;
        private static OthelloAdapter adapter;
        private static string Id { get; set; }

        private static RestClient client;

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
        }

        private static void InitializeGame()
        {
            gameContinue = true;
            gameMode = 0;

            adapter = new OthelloAdapter();

            client = new RestClient("https://7q06k0lshh.execute-api.ap-northeast-1.amazonaws.com/Prod");
        }

        private static void Update()
        {
            if (Id != null)
            {
                var request = new RestRequest(Id, Method.GET);
                IRestResponse response = client.Execute(request);
                var data = JsonConvert.DeserializeObject<OthelloGameRepresentation>(response.Content);
                adapter.GetGameFromJSON(data.OthelloGameStrRepresentation);
            }

            //oBoard = (OthelloToken[,])oGame.GameGetBoardData(OthelloBoardType.TokenMatrix);
        }

        private static void ProcessUserInput()
        {
            ConsoleKeyInfo c = Console.ReadKey(true);

            switch (c.Key)
            {
                case ConsoleKey.Escape:
                    gameMode = GameStateMode.DoNothing;
                    gameContinue = false;
                    break;
                case ConsoleKey.D0:
                    gameMode = GameStateMode.Debug;
                    break;
                case ConsoleKey.N:
                    gameMode = GameStateMode.NewGame;
                    break;
                case ConsoleKey.F1:
                    gameMode = GameStateMode.NewAlternateGame;
                    break;
                case ConsoleKey.F2:
                    gameMode = GameStateMode.SwitchPlayer;
                    break;
                case ConsoleKey.L:
                    gameMode = GameStateMode.LoadGame;
                    break;
                case ConsoleKey.S:
                    gameMode = GameStateMode.SaveGame;
                    break;
                case ConsoleKey.D4:
                    gameMode = GameStateMode.InputMove;
                    break;
                case ConsoleKey.U:
                    gameMode = GameStateMode.Undo;
                    break;
                case ConsoleKey.R:
                    gameMode = GameStateMode.Redo;
                    break;
                case ConsoleKey.D5:
                    gameMode = GameStateMode.AIMove;
                    break;
                case ConsoleKey.D6:
                    gameMode = GameStateMode.TestMode;
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
            Console.WriteLine(string.Format(CultureInfo.CurrentCulture, "Menu: N:NewGame, L:LoadGame, S: SaveGame, 4: InputMove, 5: A.I., 6: Run Basic Tests\n"));

            adapter.GameDebugGetBoardInString();
            Console.WriteLine(string.Format(CultureInfo.CurrentCulture, "Current player: {0}",adapter.GameUpdatePlayer().PlayerName));
        }

        static void ProcessGame()
        {
            switch (gameMode)
            {
                case GameStateMode.Debug:
                    break;
                case GameStateMode.NewGame:
                    var request = new RestRequest("/", Method.PUT);
                    var body = CreateNewHumanVsAiPlayers();
                    request.AddJsonBody(body);

                    // execute the request
                    IRestResponse response = client.Execute(request);

                    var data = JsonConvert.DeserializeObject<OthelloGameRepresentation>(response.Content);
                    adapter.GetGameFromJSON(data.OthelloGameStrRepresentation);
                    Id = data.Id;

                    Console.WriteLine(string.Format(CultureInfo.CurrentCulture, "Creating New Game. {0}, gameId={1}", response.StatusCode, data.Id));
                    break;
                case GameStateMode.NewAlternateGame:

                    Console.WriteLine(string.Format(CultureInfo.CurrentCulture, "Creating New Game (Alternate Layout)."));
                    break;
                case GameStateMode.SwitchPlayer:
                    

                    break;
                case GameStateMode.LoadGame:

                    Console.WriteLine(string.Format(CultureInfo.CurrentCulture, "Loading Game."));
                    break;
                case GameStateMode.SaveGame:

                    Console.WriteLine(string.Format(CultureInfo.CurrentCulture, "Saving Game."));
                    break;
                case GameStateMode.InputMove:
                    Console.WriteLine(string.Format(CultureInfo.CurrentCulture, "Menu: Enter x,y then <Enter>"));
                    string input = Console.ReadLine();

                    if (Regex.IsMatch(input, @"^\d,\d"))
                    {
                        MatchCollection mc = Regex.Matches(input, @"\d");

                        int x = int.Parse(mc[0].Value, CultureInfo.InvariantCulture);
                        int y = int.Parse(mc[1].Value, CultureInfo.InvariantCulture);

                        //OthelloGamePlayer currentPlayer = oGame.GameUpdatePlayer();
                        //int move = oGame.GameMakeMove(x, y, currentPlayer).Count;

                        //if (move == 0)
                            //Console.WriteLine(string.Format(CultureInfo.CurrentCulture, "Invalid Move.."));
                    }

                    break;
                case GameStateMode.Undo:

                    Console.WriteLine(string.Format(CultureInfo.CurrentCulture, "Undoing a move."));
                    break;
                case GameStateMode.Redo:

                    Console.WriteLine(string.Format(CultureInfo.CurrentCulture, "Redoing a move."));
                    break;
                case GameStateMode.AIMove:


                    Console.WriteLine(string.Format(CultureInfo.CurrentCulture, "A.I move: ({0},{1}). Press Enter for next turn...", "", ""));
                    break;
                case GameStateMode.TestMode:
                    //quickly test the results of a series of moves.

                    Console.WriteLine(string.Format(CultureInfo.CurrentCulture, "Doing tests"));
                    //******************************write any tests here ****************************************
                    OthelloTest.CheckAIConfigLoaderSanity(GameDifficultyMode.Default);
                    //******************************end of tests*************************************************
                    break;
                default:
                    break;
            }
            Console.WriteLine(string.Format(CultureInfo.CurrentCulture, "Press <Enter> to continue"));
            Console.ReadLine();
        }

        private static OthelloServerlessPlayers CreateNewHumanVsAiPlayers()
        {
            //TODO: found a bug that when creating a new human vs. ai game, the name becomes "human" and "computer" as that is hardcoed inside othello adapter.
            var myPlayers = new OthelloServerlessPlayers();
            myPlayers.PlayerNameWhite = "PlayerA";
            myPlayers.PlayerNameBlack = "PlayerB";
            myPlayers.FirstPlayer = "White";
            myPlayers.UseAI = true;
            myPlayers.Difficulty = GameDifficultyMode.Default;

            return myPlayers;
        }
    }
}
