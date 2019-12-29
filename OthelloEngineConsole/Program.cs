using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Othello;


namespace OthelloEngineConsole
{
    class Program
    {
        static bool gameContinue;
        static GameStateMode gameMode;
        static Othello.OthelloGamePlayer oPlayerA;
        static Othello.OthelloGamePlayer oPlayerB;
        static Othello.OthelloGamePlayer ofirstPlayer;
        static Othello.OthelloGamePlayer oCurrentPlayer;
        static bool IsAlternateGame = true;
        static Othello.OthelloGame oGame;
        static OthelloToken[,] oBoard;

        static void Main()
        {
            //intiialize game
            InitializeGame();
            
            //enter game loop
            do
            {
                Update();

                Render();

                ProcessUserInput();

                ProcessGame();

            } while (gameContinue);
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

            oGame.DebugGameState();
            oGame.DebugGameBoard();
            oGame.DebugGameInformation();

            if (oGame.GameIsEndGame())
            {
                Console.WriteLine(string.Format(CultureInfo.CurrentCulture, "Game has ended. Start NewGame or End game"));
            }
        }

        private static void Update()
        {
            oCurrentPlayer = oGame.GameUpdatePlayer();
            oBoard = (OthelloToken[,])oGame.GameGetBoardData(OthelloBoardType.TokenMatrix);
        }

        private static void InitializeGame()
        {
            gameContinue = true;
            gameMode = 0;

            oPlayerA = new OthelloGamePlayer(OthelloPlayerKind.White, "PlayerA");
            oPlayerB = new OthelloGamePlayer(OthelloPlayerKind.Black, "PlayerB");
            ofirstPlayer = oPlayerA;
            IsAlternateGame = false;

            //create a new game by default
            oGame = new OthelloGame(oPlayerA, oPlayerB, oPlayerA);
        }

        static void ProcessGame()
        {
            switch(gameMode)
            {
                case GameStateMode.Debug:                   
                    break;
                case GameStateMode.NewGame:
                    oGame.GameCreateNew(oPlayerA, oPlayerB, ofirstPlayer, IsAlternateGame = false); 
                    Console.WriteLine(string.Format(CultureInfo.CurrentCulture, "Creating New Game."));
                    break;
                case GameStateMode.NewAlternateGame:
                    oGame.GameCreateNew(oPlayerA, oPlayerB, ofirstPlayer, IsAlternateGame = true); 
                    Console.WriteLine(string.Format(CultureInfo.CurrentCulture, "Creating New Game (Alternate Layout)."));
                    break;
                case GameStateMode.SwitchPlayer:
                    if(oGame.GameUpdateTurn() == 1)
                    {
                        ofirstPlayer = ofirstPlayer.PlayerKind == oPlayerA.PlayerKind ? ofirstPlayer = oPlayerB : oPlayerA;
                        oGame.GameCreateNew(oPlayerA, oPlayerB, ofirstPlayer, IsAlternateGame);
                        Console.WriteLine(string.Format(CultureInfo.CurrentCulture, "Switching Players. PlayerA <=> PlayerB"));
                    }
                    else
                        Console.WriteLine(string.Format(CultureInfo.CurrentCulture, "Cannot Switch Player halfway in game"));                  
                    
                    break;
                case GameStateMode.LoadGame:
                    oGame.GameLoad();
                    Console.WriteLine(string.Format(CultureInfo.CurrentCulture, "Loading Game."));
                    break;
                case GameStateMode.SaveGame:
                    oGame.GameSave();
                    Console.WriteLine(string.Format(CultureInfo.CurrentCulture, "Saving Game."));
                    break;
                case GameStateMode.InputMove:
                    Console.WriteLine(string.Format(CultureInfo.CurrentCulture, "Menu: Enter x,y then <Enter>"));
                    string input = Console.ReadLine();

                    if(Regex.IsMatch(input, @"^\d,\d"))
                    {
                        MatchCollection mc = Regex.Matches(input, @"\d");

                        int x = int.Parse(mc[0].Value, CultureInfo.InvariantCulture);
                        int y = int.Parse(mc[1].Value, CultureInfo.InvariantCulture);

                        OthelloGamePlayer currentPlayer = oGame.GameUpdatePlayer();
                        int move = oGame.GameMakeMove(x, y, currentPlayer).Count;

                        if(move == 0)
                            Console.WriteLine(string.Format(CultureInfo.CurrentCulture, "Invalid Move.."));
                    }

                    break;
                case GameStateMode.Undo:
                    oGame.GameUndo();
                    Console.WriteLine(string.Format(CultureInfo.CurrentCulture, "Undoing a move."));
                    break;
                case GameStateMode.Redo:
                    oGame.GameRedo();
                    Console.WriteLine(string.Format(CultureInfo.CurrentCulture, "Redoing a move."));
                    break;
                case GameStateMode.AIMove:
                    OthelloGameAIFactory factory = new OthelloGameAIFactory();
                    OthelloGameAISystemProduct AI1 = factory.Create(oGame, oCurrentPlayer, oPlayerA);
                    oGame.AIPlayer = (OthelloGameAiSystem)AI1;

                    Console.WriteLine(string.Format(CultureInfo.CurrentCulture, "A.I {0} processing...", AI1.ToString()));

                    oGame.GameAIMakeMove();

                    Console.WriteLine(string.Format(CultureInfo.CurrentCulture, "A.I move: ({0},{1}). Press Enter for next turn...", oGame.AIMove.X, oGame.AIMove.Y));
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
    }

}
