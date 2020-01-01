using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Globalization;

namespace Othello
{
    /// <summary>
    /// Class to encapsulate the entire game management required for an external client application to initiate, execute and get information about an othello game.
    /// An Othello game is typically:
    ///     1. initiated.
    ///     2. get and update the player to take the turn to place a token
    ///     3. move is made by player
    ///     4. check that the game is at an end
    ///     5. update the rendered representation of a othello board given an internal representation
    ///     6. repeat from 2 if game is not at an end
    /// 
    /// integrated AI as a member field in the OthelloGame Class. Now Game clients do not have to implement the intrinsic A.I and non A.I logic.
    /// 
    /// TODO : support multiple AI players..
    /// TODO : support logging via Singleton design pattern
    /// </summary>
    [Serializable]
    public sealed class OthelloGame : OthelloGameAiSystem.IOthelloGameAiAccessor
    {
        #region FIELDS
        private const string defaultSaveDirectoryName = "OthelloSaves";
        private const string defaultFileName = "Save" + "Game" + ".dat";
        private const string defaultFileNameAIConfig = "AIConfig.txt";
        private const int defaultMilliSecondsTimeLimit = 10000;
        private const string othelloEmptyChar = "x";
        private const string othelloBlackChar = "b";
        private const string othelloWhiteChar = "w";

        public string SaveFileName {get;set;}
        public string DefaultSaveDir { get; set; }

        private IEnumerable<OthelloGameAIConfig> AIConfigs = null;

        private OthelloState CurrentOthelloState;
        private Stack<OthelloState> statesUndoCollection;
        private Stack<OthelloState> statesRedoCollection;

        private OthelloGameAIFactory Factory;
        public OthelloGamePlayer PlayerWhite { get; set; }
        public OthelloGamePlayer PlayerBlack { get; set; }
        public OthelloGameAiSystem AIPlayer { get; set; }

        //AI best move tracked here after making an AI Move
        public OthelloToken AIMove { get;set; }

        public int CurrentTurn { get { return CurrentOthelloState.Turn; } }

        public GameMode GameMode { get; set; }
        public GameDifficultyMode GameDifficultyMode { get; set; }

        //store list of objects for serialization
        private ArrayList gameObjectsToSerialized;

        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Default
        /// </summary>
        public OthelloGame()
        {
            InitializeOthelloGameVariables();
            InitializeOthelloGameWithAI();
        }

        /// <summary>
        /// Constructor that uses given values to initiate a game
        /// </summary>
        /// <param name="oPlayerWhite"></param>
        /// <param name="oPlayerBlack"></param>
        /// <param name="firstPlayer"></param>
        /// <param name="IsAlternate"></param>
        /// //TODO: shift configuration loader to per A.I in OthelloAI, the concrete implementation of OthelloProduct, as the new factory design pattern allows multiple A.I players
        public OthelloGame(OthelloGamePlayer oPlayerWhite, OthelloGamePlayer oPlayerBlack, OthelloGamePlayer firstPlayer, bool IsAlternate = false, bool IsAIMode = false, bool IsPlayerBlackAI = true, GameDifficultyMode difficulty = GameDifficultyMode.Default)
            : this()
        {
            GameInitializeWithAI(oPlayerWhite, oPlayerBlack, firstPlayer, IsAlternate, IsPlayerBlackAI, difficulty);
            GameMode = IsAIMode ? GameMode.HumanVSComputer : GameMode.HumanVSHuman;
        }
        #endregion

        #region GAME MANAGERs ,STATE MANAGERS, VALIDATORS
        /// <summary>
        /// Create a new Game. 
        /// </summary>
        /// <param name="oPlayerWhite"></param>
        /// <param name="oPlayerBlack"></param>
        /// <param name="firstPlayer"></param>
        /// <param name="IsAlternate"></param>
        public void GameCreateNew(OthelloGamePlayer oPlayerWhite, OthelloGamePlayer oPlayerBlack, OthelloGamePlayer firstPlayer, bool IsAlternate = false)
        {
            OthelloExceptions.ThrowExceptionIfNull(oPlayerWhite);
            OthelloExceptions.ThrowExceptionIfNull(oPlayerBlack);
            OthelloExceptions.ThrowExceptionIfNull(firstPlayer);

            if (oPlayerWhite.PlayerKind == oPlayerBlack.PlayerKind)
                throw new Exception(string.Format(CultureInfo.InvariantCulture, "cannot have both player of same type."));
            

            //check for exceptions and set up players
            if(oPlayerWhite.PlayerKind != OthelloPlayerKind.White)
                throw new Exception(string.Format(CultureInfo.InvariantCulture, "GameLoad [Wrong Player Kind] Black Player specfied when expected was White"));
            

            if(oPlayerBlack.PlayerKind != OthelloPlayerKind.Black)
                throw new Exception(string.Format(CultureInfo.InvariantCulture, "GameLoad [Wrong Player Kind] White Player specfied when expected was Black"));
            
            PlayerWhite = oPlayerWhite;
            PlayerBlack = oPlayerBlack;

            //setup state : player = first player and turn = 1
            CurrentOthelloState = new OthelloState(firstPlayer, 1, IsAlternate);

        }

        /// <summary>
        /// make moves after validation
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public List<OthelloToken> GameMakeMove(int x, int y, OthelloGamePlayer player)
        {
            List<OthelloToken> flipTokens = new List<OthelloToken>();

            //Note: Past comments indicate CreateNextState may included repetition with IsValidMove.
            if (!CurrentOthelloState.IsValidMove(x, y, player))
            {
#if TRACE
                Trace.WriteLine("MakeMove: Move is invalid");
#endif
                return flipTokens;
            }

            OthelloState oNextState = CreateNextState(x, y, player, ref flipTokens);

            Update(oNextState);

#if TRACE
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "MakeMove: Turn:{0} [{1}({2},{3})] Player={4}", 
                CurrentOthelloState.Turn,
                CurrentOthelloState.GetBoardData().GetCell(x, y).Token, 
                x, 
                y, 
                player.PlayerName));
#endif
            return flipTokens;
        }

        /// <summary>
        /// make an AI move given an AI player mode
        /// </summary>
        /// <returns></returns>
        public List<OthelloToken> GameAIMakeMove(int MilliSecondsTimeLimit = defaultMilliSecondsTimeLimit)
        {
            int? depth = null;
            float? alpha = null;
            float? beta = null;

            List<OthelloToken> FlipList = new List<OthelloToken>();

            AIPlayer.Initialize(this, this.AIPlayer.AiPlayer,this.GetOpposingCurrentPlayer(), MilliSecondsTimeLimit);

            this.GameUpdateTurn();
            this.ObtainTurnAIVariablesFromConfig(this.GameDifficultyMode, this.CurrentTurn,ref depth, ref alpha, ref beta);

            AIMove = AIPlayer.GetBestMove(AIPlayer.AiPlayer, (int)depth, (float)alpha, (float)beta);

            if (AIMove != null)
                FlipList = this.GameMakeMove(AIMove.X, AIMove.Y, AIPlayer.AiPlayer);           

            return FlipList;
        }


        /// <summary>
        /// Undo a game turn
        /// </summary>
        public void GameUndo()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture,"GameUndo: Undoable Count =  {0}", statesUndoCollection.Count));
            if (statesUndoCollection.Count > 0)
            {
                OthelloState state = statesUndoCollection.Pop();
                statesRedoCollection.Push(CurrentOthelloState);
                CurrentOthelloState = state;            
            }

            if(statesUndoCollection.Count > 0 && 
                this.GameMode == GameMode.HumanVSComputer)
            {
                OthelloState state = statesUndoCollection.Pop();
                statesRedoCollection.Push(CurrentOthelloState);
                CurrentOthelloState = state; 
            }
        }

        /// <summary>
        /// Redo a past undo turn
        /// </summary>
        public void GameRedo()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "GameUndo: Reddoable Count =  {0}", statesRedoCollection.Count));
            if (statesRedoCollection.Count > 0)
            {
                OthelloState state = statesRedoCollection.Pop();
                statesUndoCollection.Push(CurrentOthelloState);
                CurrentOthelloState = state;  
            }

            if(statesRedoCollection.Count > 0 && 
                this.GameMode == GameMode.HumanVSComputer)
            {
                OthelloState state = statesRedoCollection.Pop();
                statesUndoCollection.Push(CurrentOthelloState);
                CurrentOthelloState = state; 
            }
        }

        /// <summary>
        /// updates and return the correct player for this game given the state of a game
        /// </summary>
        /// <returns></returns>
        public OthelloGamePlayer GameUpdatePlayer()
        {
            OthelloGamePlayer player = GetCurrentPlayer();

            if (!IsValidPlayer(player))
                player = CurrentOthelloState.CurrentPlayer = GetOpposingCurrentPlayer();
            
            return player;
        }

        /// <summary>
        /// try to set a game player.returne false if not a valid player for turn
        /// </summary>
        /// <param name="oPlayer"></param>
        /// <returns></returns>
        public bool GameSetPlayer(OthelloGamePlayer oPlayer)
        {
            if (IsValidPlayer(oPlayer))
            {
                CurrentOthelloState.CurrentPlayer = oPlayer;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the list of allowable moves by a given player
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public List<OthelloToken> GameGetPlayerAllowedMoves(OthelloGamePlayer player)
        {
            return CurrentOthelloState.GetAllowedMoves(player);
        }

        /// <summary>
        /// Updates and return the correct Turn number for a given state of a game
        /// </summary>
        /// <returns></returns>
        public int GameUpdateTurn()
        {
            CurrentOthelloState.Turn = CurrentOthelloState.CalculateTurn();
            return CurrentOthelloState.Turn;
        }

        /// <summary>
        /// check if end game is reached.
        /// </summary>
        /// <returns></returns>
        public bool GameIsEndGame()
        {
            //if game turn is 61.
            if(CurrentOthelloState.Turn == 61)
                return true;
            
            //memo: not all cases end at turn 61. Some rare cases end when both players cannot move

            //when both player cannot move, game also ends
            if(GameGetPlayerAllowedMoves(GetCurrentPlayer()).Count == 0 &&
                GameGetPlayerAllowedMoves(GetOpposingCurrentPlayer()).Count == 0)
                return true;
            

            return false;
        }

        /// <summary>
        /// Get Score for white
        /// </summary>
        /// <returns></returns>
        public int GameGetScoreWhite()
        {
            return CurrentOthelloState.ScoreW;
        }

        /// <summary>
        /// Get Score for Black
        /// </summary>
        /// <returns></returns>
        public int GameGetScoreBlack()
        {
            return CurrentOthelloState.ScoreB;
        }



        /// <summary>
        /// return board data given the type for a board(see OthelloBoardType)
        /// </summary>
        /// <param name="boardType"></param>
        /// <returns></returns>
        public object GameGetBoardData(OthelloBoardType boardType)
        {
            switch (boardType)
            {
                case OthelloBoardType.Bit:
                    return this.CurrentOthelloState.GetBoardData().GetOthelloBoard(OthelloBoardType.Bit);
                case OthelloBoardType.CharMatrix:
                    return this.CurrentOthelloState.GetBoardData().GetOthelloBoard(OthelloBoardType.CharMatrix);
                case OthelloBoardType.TokenMatrix:
                    return this.CurrentOthelloState.GetBoardData().GetOthelloBoard(OthelloBoardType.TokenMatrix);
                case OthelloBoardType.StringSequence:
                    return this.CurrentOthelloState.GetBoardData().GetOthelloBoard(OthelloBoardType.StringSequence);
                default:
                    return null;
            }
        }

        /// <summary>
        /// set a game board any time using a charBoard
        /// </summary>
        /// <param name="charBoard"></param>
        public void GameSetBoardData(char[,] charBoard)
        {
            CurrentOthelloState.SetBoardData(charBoard);
        }

        /// <summary>
        /// set a game board any time using a stringBoard
        /// </summary>
        /// <param name="stringBoard"></param>
        public void GameSetBoardData(string stringBoard)
        {
            CurrentOthelloState.SetBoardData(stringBoard);
        }

        /// <summary>
        /// set a game board any time using a tokenBoard
        /// </summary>
        /// <param name="tokenBoard"></param>
        public void GameSetBoardData(OthelloToken[,] tokenBoard)
        {
            CurrentOthelloState.SetBoardData(tokenBoard);
        }

        /// <summary>
        /// Disable Logging: for performance testing and other uses
        /// </summary>
        public static void GameDisableLog()
        {
            OthelloLogger.Disable();
        }

        public static void GameEnableLogging()
        {
            OthelloLogger.GetInstance();
        }

        #endregion

        #region SAVE_AND_LOAD
        //Save game
        public void GameSave(bool UseDefaultpath = true, string pathDir = @".\")
        {
            AddGameObjectsToObjectsArrayList();

            string strDefaultSaveDir = UseDefaultpath ?
                OthelloIO.CreateDefaultDirectory(Path.Combine(Directory.GetCurrentDirectory(), defaultSaveDirectoryName)) :
                OthelloIO.CreateDefaultDirectory(pathDir);

            try
            {
                OthelloIO.SaveToBinaryFile(this.gameObjectsToSerialized, OthelloIO.GetFileSavePath(strDefaultSaveDir, defaultFileName));
            }
            catch (Exception e)
            {
                throw new Exception(string.Format(CultureInfo.InvariantCulture, "OthelloIO.SaveToBinaryFile [Exception] : Exception Message = {0}", e.Message));
            }
            finally
            {
                gameObjectsToSerialized.Clear();
            }
        }

        //Load game                                                                                                                                                                                                                              
        public void GameLoad(bool UseDefaultpath = true, string pathDir = "./")
        {
            try
            {
                DefaultSaveDir = UseDefaultpath ? this.DefaultSaveDir : OthelloIO.CreateDefaultDirectory(pathDir);
                string fullpath = OthelloIO.GetFileSavePath(DefaultSaveDir, defaultFileName);

                this.gameObjectsToSerialized = (ArrayList)OthelloIO.LoadFromBinaryFile(fullpath);

                GetGameObjectsFromGameObjectArrayList();
            }
            catch (Exception e)
            {
                throw new Exception(string.Format(CultureInfo.InvariantCulture, "OthelloIO.LoadFromBinaryFile : {0}", e.ToString()));
            }
            finally
            {
                gameObjectsToSerialized.Clear();
            }
        }

        #endregion

        #region JSON SERIALIZATION
        public string GameGetJSON()
        {
            AddGameObjectsToObjectsArrayList();

            try
            {
                var gamebase64Str = OthelloIO.GetBase64String(gameObjectsToSerialized);
                return OthelloIO.ConvertBase64StringToJSON(gamebase64Str);
            }
            catch (Exception e)
            {
                throw new Exception(string.Format(CultureInfo.InvariantCulture, "OthelloGame.GameGetJSON : something went wrong: {0}", e.ToString()));
            }
            finally
            {
                gameObjectsToSerialized.Clear();
            }
        }

        public OthelloGame GameGetGameFromJSON(string json)
        {
            try
            {
                var b64str = OthelloIO.ConvertJSONToBase64String(json);
                this.gameObjectsToSerialized = (ArrayList)OthelloIO.GetObjectFromBase64String(b64str);

                GetGameObjectsFromGameObjectArrayList();

                return this;
            }
            catch (Exception e)
            {
                throw new Exception(string.Format(CultureInfo.InvariantCulture, "OthelloGame.GameGetGameFromJSON : something went wrong: {0}", e.ToString()));
            }
            finally
            {
                gameObjectsToSerialized.Clear();
            }

        }

        #endregion

        #region HELPER FUNCTIONS
        /// <summary>
        /// get next state according to move
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="player"></param>
        /// <param name="fliplist"></param>
        /// <returns></returns>
        private OthelloState CreateNextState(int x, int y, OthelloGamePlayer player, ref List<OthelloToken> fliplist)
        {
            //create deep copy of current state
            OthelloState oNextState = (OthelloState)CurrentOthelloState.Clone();

            //get a list of tokens to flip given a move at x,y
            fliplist = oNextState.GetAllFlipsTokens(x, y, player);

            //get the OthelloBitType
            OthelloBitType bt = player.GetPlayerOthelloToken();

            //Set the cell where player placed it
            oNextState.GetBoardData().SetCell(bt, x, y);

            //Set the cells for all cell tokens that need flipping
            foreach (OthelloToken token in fliplist)
                oNextState.GetBoardData().SetCell(bt, token.X, token.Y);

            //go to next turn
            oNextState.Turn++;

            //validate and switch players
            oNextState.CurrentPlayer = GetOpposingCurrentPlayer();

            //update score
            oNextState.ScoreB = oNextState.GetBoardBlackCount();
            oNextState.ScoreW = oNextState.GetBoardWhiteCount();

            return oNextState;
        }

        /// <summary>
        /// Update Current Othello State
        /// </summary>
        /// <param name="updatedState"></param>
        private void Update(OthelloState updatedState)
        {
            //add curretn state to past state collection
            statesUndoCollection.Push(CurrentOthelloState);

            //remove states from the redo collection (by design. Do not support redo after a move)
            statesRedoCollection.Clear();

            //update state to be next state
            CurrentOthelloState = updatedState;
        }

        /// <summary>
        /// evaluate valid player move. Checks whether playable
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private bool IsValidPlayer(OthelloGamePlayer player)
        {
            return CurrentOthelloState.IsValidPlayer(player);
        }

        /// <summary>
        /// get current player 
        /// </summary>
        /// <returns></returns>
        private OthelloGamePlayer GetCurrentPlayer()
        {
            return CurrentOthelloState.CurrentPlayer;
        }

        /// <summary>
        /// Get opposing player of current player
        /// </summary>
        /// <returns></returns>
        private OthelloGamePlayer GetOpposingCurrentPlayer()
        {
            return (GetCurrentPlayer().PlayerKind == OthelloPlayerKind.White) ? PlayerBlack : PlayerWhite;
        }

        /// <summary>
        /// private member accessible from another class (friend)
        /// </summary>
        /// <returns></returns>
        OthelloState OthelloGameAiSystem.IOthelloGameAiAccessor.GetCurrentState()
        {
            return CurrentOthelloState;
        }

        /// <summary>
        /// Used in Game Saving and Game to JSON serialization
        /// </summary>
        private void AddGameObjectsToObjectsArrayList()
        {
            gameObjectsToSerialized.Add(this.PlayerWhite);
            gameObjectsToSerialized.Add(this.PlayerBlack);
            gameObjectsToSerialized.Add(this.CurrentOthelloState);
            gameObjectsToSerialized.Add(this.statesUndoCollection);
            gameObjectsToSerialized.Add(this.statesRedoCollection);
            gameObjectsToSerialized.Add(this.GameMode);
            gameObjectsToSerialized.Add(this.AIPlayer);
            gameObjectsToSerialized.Add(this.GameDifficultyMode);
        }

        /// <summary>
        /// Inverse of AddGameObjectsToObjectsArrayList
        /// </summary>
        private void GetGameObjectsFromGameObjectArrayList()
        {
            int i = 0;
            this.PlayerWhite = (OthelloGamePlayer)this.gameObjectsToSerialized[i++];
            this.PlayerBlack = (OthelloGamePlayer)this.gameObjectsToSerialized[i++];
            this.CurrentOthelloState = (OthelloState)this.gameObjectsToSerialized[i++];
            this.statesUndoCollection = (Stack<OthelloState>)this.gameObjectsToSerialized[i++];
            this.statesRedoCollection = (Stack<OthelloState>)this.gameObjectsToSerialized[i++];
            this.GameMode = (GameMode)this.gameObjectsToSerialized[i++];
            this.AIPlayer = (OthelloGameAiSystem)this.gameObjectsToSerialized[i++];
            this.GameDifficultyMode = (GameDifficultyMode)this.gameObjectsToSerialized[i++];
        }

        /// <summary>
        /// Intializes game variables at the highest level
        /// </summary>
        private void InitializeOthelloGameVariables()
        {
            statesUndoCollection = new Stack<OthelloState>();
            statesRedoCollection = new Stack<OthelloState>();

            Factory = new OthelloGameAIFactory();

            GameMode = GameMode.HumanVSHuman;

            gameObjectsToSerialized = new ArrayList();

            DefaultSaveDir = Path.Combine(Directory.GetCurrentDirectory(), defaultSaveDirectoryName);

            SaveFileName = defaultFileName;
        }

        /// <summary>
        /// Initialize Othello Core Game with AI at the highest level
        /// </summary>
        private void InitializeOthelloGameWithAI()
        {
            OthelloGamePlayer playerWhite = new OthelloGamePlayer(OthelloPlayerKind.White, "default");
            OthelloGamePlayer playerBlack = new OthelloGamePlayer(OthelloPlayerKind.Black, "default");

            GameInitializeWithAI(playerWhite, playerBlack, playerWhite, false, true, GameDifficultyMode.Default);
        }

        /// <summary>
        /// Initialize Game with AI at the highest level
        /// </summary>
        /// <param name="oPlayerWhite"></param>
        /// <param name="oPlayerBlack"></param>
        /// <param name="firstPlayer"></param>
        /// <param name="IsAlternate"></param>
        /// <param name="IsAIMode"></param>
        /// <param name="IsPlayerBlackAI"></param>
        /// <param name="difficulty"></param>
        private void GameInitializeWithAI(OthelloGamePlayer oPlayerWhite, OthelloGamePlayer oPlayerBlack, OthelloGamePlayer firstPlayer, bool IsAlternate, bool IsPlayerBlackAI, GameDifficultyMode difficulty)
        {
            GameCreateNew(oPlayerWhite, oPlayerBlack, firstPlayer, IsAlternate);
            GameSetupAI(oPlayerWhite, oPlayerBlack, IsPlayerBlackAI, difficulty);
        }
        /// <summary>
        /// Initialize all AI aspects
        /// <param name="oPlayerWhite"></param>
        /// <param name="oPlayerBlack"></param>
        /// <param name="IsAIMode"></param>
        /// <param name="IsPlayerBlackAI"></param>
        /// <param name="difficulty"></param>
        private void GameSetupAI(OthelloGamePlayer oPlayerWhite, OthelloGamePlayer oPlayerBlack, bool IsPlayerBlackAI, GameDifficultyMode difficulty)
        {
            //set up AI player type
            AIPlayer = (OthelloGameAiSystem)Factory.Create(this,
                    IsPlayerBlackAI ? oPlayerBlack : oPlayerWhite,
                    IsPlayerBlackAI ? oPlayerWhite : oPlayerBlack);

            GameDifficultyMode = difficulty;

            LoadAiConfig();
        }

        /// <summary>
        /// Loads a configuration written in TSV for AI tuning into memory
        /// </summary>
        private void LoadAiConfig()
        {
            try
            {
                string[] AIConfigLines = File.ReadAllLines(defaultFileNameAIConfig, Encoding.UTF8);

                this.AIConfigs = from data in AIConfigLines.Skip(1)
                            select new OthelloGameAIConfig
                            {
                                depth = int.Parse(data.Split('\t')[0], CultureInfo.InvariantCulture),
                                alpha = float.Parse(data.Split('\t')[1], CultureInfo.InvariantCulture),
                                beta = float.Parse(data.Split('\t')[2], CultureInfo.InvariantCulture),
                                turnrange = data.Split('\t')[3],
                                difficulty = int.Parse(data.Split('\t')[4], CultureInfo.InvariantCulture)
                            };

                //validate config. Any problems with loading these values will throw an exception
                foreach(GameDifficultyMode mode in Enum.GetValues(typeof(GameDifficultyMode))) 
                    for(int turn = 1; turn <= OthelloBoard.BoardSize*OthelloBoard.BoardSize; turn++)
                    {
                        int? depth =null;
                        float? alpha = null;
                        float? beta= null;

                        ObtainTurnAIVariablesFromConfig(mode,turn,ref depth, ref alpha, ref beta);
                    }

            }
            catch (Exception e)
            {
                throw new Exception(string.Format(CultureInfo.InvariantCulture, "Error with config file!"), e);
            }
        }

        /// <summary>
        /// Gets and Validates the config, applying to whoever that uses the references to the config parameters
        /// </summary>
        /// <param name="depth"></param>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        private void ObtainTurnAIVariablesFromConfig(GameDifficultyMode mode, int turn, ref int? depth, ref float? alpha, ref float? beta)
        {
            GetTurnConfig(mode, turn, AIConfigs, out depth, out alpha, out beta);

            if (depth == null || alpha == null || beta == null)
                throw new Exception("failed to load/parse AI config for this turn." + string.Format(CultureInfo.InvariantCulture, "mode={0},turn={1}",mode, turn) );
        }

        /// <summary>
        /// Parses and outputs the corresponding AI parameters given the difficulty mode and the game turn
        /// </summary>
        /// <param name="difficultymode"></param>
        /// <param name="turn"></param>
        /// <param name="configs"></param>
        /// <param name="depth"></param>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        private static void GetTurnConfig(GameDifficultyMode difficultymode, int turn, IEnumerable<OthelloGameAIConfig> configs, out int? depth, out float? alpha, out float? beta)
        {
            foreach (OthelloGameAIConfig c in configs)
            {
                if (c.IsInRange(turn, difficultymode))
                {
                    depth = c.depth;
                    alpha = c.alpha;
                    beta = c.beta;
                    return;
                }
            }
            depth = null;
            alpha = null;
            beta = null;
        }

        #endregion

        #region DEBUGGING

        public IEnumerable<OthelloGameAIConfig> DebugAIConfig()
        {
            if(AIConfigs == null)
                LoadAiConfig();
            return AIConfigs;
        }

        public static void DebugGetTurnConfig(GameDifficultyMode difficultymode, int turn, IEnumerable<OthelloGameAIConfig> configs, out int? depth, out float? alpha, out float? beta)
        {
            OthelloExceptions.ThrowExceptionIfNull(configs);

            GetTurnConfig(difficultymode, turn, configs, out depth, out alpha, out beta);
        }

        public void DebugGameState()
        {
            this.CurrentOthelloState.DebugState();
        }
        public string DebugGameBoard(bool IsCreateAxis = true)
        {
            StringBuilder sb = new StringBuilder();
            OthelloToken[,] oBoard = (OthelloToken[,])this.GameGetBoardData(OthelloBoardType.TokenMatrix);

            //create x-axis
            if (IsCreateAxis)
            {
                sb.Append("\t ");
                for (int i = 0; i < OthelloBoard.BoardSize; i++)
                {
                    sb.Append(Convert.ToString(i, CultureInfo.InvariantCulture));
                }
                sb.Append("\n");
            }

            for (int i = 0; i < OthelloBoard.BoardSize; i++)
            {
                //create y-axis
                if (IsCreateAxis)
                {
                    sb.Append(string.Format(CultureInfo.InvariantCulture,"\t{0}", Convert.ToString(i, CultureInfo.InvariantCulture)));
                }

                for (int j = 0; j < OthelloBoard.BoardSize; j++)
                {
                    //
                    switch ((int)oBoard[j, i].Token)
                    {
                        case ((int)OthelloBitType.Empty):
                            sb.Append(othelloEmptyChar);
                            break;
                        case ((int)OthelloBitType.Black):
                            sb.Append(othelloBlackChar);
                            break;
                        case ((int)OthelloBitType.White):
                            sb.Append(othelloWhiteChar);
                            break;
                        default:
                            break;
                    }
                }
                sb.Append("\n");
            }

            Console.WriteLine(sb.ToString());
            Trace.WriteLine(sb.ToString());
            return sb.ToString();
        }
        public void DebugGameInformation()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture,"[DebugGameInformation]:{0} Allowed Move Count={1}",
                this.GetCurrentPlayer().PlayerName,
                this.CurrentOthelloState.GetAllowedMoves(this.GetCurrentPlayer()).Count));

            Console.WriteLine(string.Format(CultureInfo.CurrentCulture, "{0}({1}) Score={2}, {3}({4}) Score={5}",
               this.PlayerWhite.PlayerName,
               this.PlayerWhite.PlayerKind,
               this.GameGetScoreWhite(),
               this.PlayerBlack.PlayerName,
               this.PlayerBlack.PlayerKind,
               this.GameGetScoreBlack()));

            Console.WriteLine(string.Format(CultureInfo.CurrentCulture, "Turn: {0}, Player: {1}, Color: {2}",
                this.CurrentOthelloState.Turn,
                this.GetCurrentPlayer().PlayerName,
                this.GetCurrentPlayer().PlayerKind));
        }
#endregion
    }
}
