using System;
using System.Collections.Generic;
using System.Linq;
using Othello;

namespace OthelloAdapters
{
    /// <summary>
    /// OthelloAdapter is a brige between the Othello Library and the Clients of it, such as the Console version of Othello, or the WPF version
    /// </summary>
    public class OthelloAdapter : OthelloAdapterBase
    {
        private OthelloGame _oGame;

        public OthelloAdapter(bool IsEnabledLogging = false): base()
        {
            _oGame = new OthelloGame();

            if (IsEnabledLogging)
            {
                OthelloGame.GameEnableLogging();
            }
            else
            {
                OthelloGame.GameDisableLog();
            }
        }

        public override void GameCreateNewHumanVSHuman(string playerWhiteName, string playerBlackName, OthelloPlayerKind firstPlayerKind, bool IsAlternate = false)
        {
            OthelloGamePlayer oPlayerA = new OthelloGamePlayer(OthelloPlayerKind.White, playerWhiteName);
            OthelloGamePlayer oPlayerB = new OthelloGamePlayer(OthelloPlayerKind.Black, playerBlackName);

            OthelloGamePlayer oPlayerFirst = oPlayerA;

            if (firstPlayerKind == OthelloPlayerKind.Black)
                oPlayerFirst = oPlayerB;

            //create a new game by default
            _oGame = new OthelloGame(oPlayerA, oPlayerB, oPlayerFirst, false);
        }

        public override void GameCreateNewHumanVSAI(string playerWhiteName, string playerBlackName, bool IsHumanWhite= true, bool IsAlternate = false, GameDifficultyMode DifficultyMode = GameDifficultyMode.Easy)
        {
            if (IsHumanWhite)
            {
                OthelloGamePlayer oPlayerA = new OthelloGamePlayer(OthelloPlayerKind.White, "Human");
                OthelloGamePlayer oPlayerB = new OthelloGamePlayer(OthelloPlayerKind.Black, "Computer");

                _oGame = new OthelloGame(oPlayerA, oPlayerB, oPlayerA, false, true, true, DifficultyMode);
            }
            else
            {
                OthelloGamePlayer oPlayerA = new OthelloGamePlayer(OthelloPlayerKind.White, "Computer");
                OthelloGamePlayer oPlayerB = new OthelloGamePlayer(OthelloPlayerKind.Black, "Human");

                _oGame = new OthelloGame(oPlayerA, oPlayerB, oPlayerB, false, true, false, DifficultyMode);
            }
        }

        public override void GameLoad()
        {
            _oGame.GameLoad();
        }

        public override void GameSave()
        {
            _oGame.GameSave();
        }

        public override void GameUndo()
        {
            _oGame.GameUndo();
        }

        public override void GameRedo()
        {
            _oGame.GameRedo();
        }

        public override OthelloToken[,] GameGetBoardData()
        {
            return (OthelloToken[,])_oGame.GameGetBoardData(OthelloBoardType.TokenMatrix);
        }

        public override int GameGetBoardSize()
        {
            return OthelloBoard.BoardSize;
        }

        public override List<OthelloToken> GameMakeMove(int GameX, int GameY, OthelloGamePlayer oCurrentPlayer, out bool IsInvalidMove)
        {
            List<OthelloToken> oFlipList = _oGame.GameMakeMove(GameX, GameY, oCurrentPlayer);
            IsInvalidMove = !oFlipList.Any() ? true : false;
            return oFlipList;
        }

        public override OthelloGamePlayer GameUpdatePlayer()
        {
            return _oGame.GameUpdatePlayer();
        }

        public override int GameUpdateTurn()
        {
            return _oGame.GameUpdateTurn();
        }

        public override bool GameIsEndGame()
        {
            return _oGame.GameIsEndGame();
        }

        public override GameMode GameGetMode()
        {
            return _oGame.GameMode;
        }

        public override GameDifficultyMode GameGetDifficultyMode()
        {
            return _oGame.GameDifficultyMode;
        }

        public override List<OthelloToken> GameAIMakeMove()
        {
            List<OthelloToken> oFlipList = _oGame.GameAIMakeMove();
            return oFlipList;
        }

        public override void GameSetDifficultyMode(GameDifficultyMode mode)
        {
            _oGame.GameDifficultyMode = mode;
        }

        public override OthelloGameAiSystem GameGetAiPlayer()
        {
            return _oGame.AIPlayer;
        }

        public override OthelloGamePlayer GameGetPlayerWhite()
        {
            return _oGame.PlayerWhite;
        }

        public override OthelloGamePlayer GameGetPlayerBlack()
        {
            return _oGame.PlayerBlack;
        }

        public override int GameGetScore(OthelloGamePlayer player)
        {
            OthelloExceptions.ThrowExceptionIfNull(player);

            if (player!= null && player.PlayerKind == OthelloPlayerKind.Black)
                return _oGame.GameGetScoreBlack();
            
            return _oGame.GameGetScoreWhite();
        }

        public override string GetGameJSON()
        {
            return _oGame.GameGetJSON();
        }

        public override void GetGameFromJSON(string json)
        {
            _oGame.GameGetGameFromJSON(json);
        }

        public string GameDebugGetBoardInString()
        {
            return _oGame.DebugGameBoard(true);
        }
    }
}
