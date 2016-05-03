using System.Collections.Generic;
using System.Linq;
using Othello;

namespace OthelloAdapter
{
    public class OthelloAdapter : Othello
    {
        private OthelloGame _oGame;

        public override void GameCreateNewHumanVSHuman(string playerWhiteName, string playerBlackName, OthelloPlayerKind firstPlayerKind, bool IsAlternate = false)
        {
            OthelloPlayer oPlayerA = new OthelloPlayer(OthelloPlayerKind.White, playerWhiteName);
            OthelloPlayer oPlayerB = new OthelloPlayer(OthelloPlayerKind.Black, playerBlackName);

            OthelloPlayer oPlayerFirst = oPlayerA;

            if (firstPlayerKind == OthelloPlayerKind.Black)
                oPlayerFirst = oPlayerB;

            //create a new game by default
            _oGame = new OthelloGame(oPlayerA, oPlayerB, oPlayerFirst, false);

            _oGame.GameEnableLogging();
        }

        public override void GameCreateNewHumanVSAI(string playerWhiteName, string playerBlackName, OthelloPlayerKind firstPlayerKind, bool IsHumanWhite= true, bool IsAlternate = false, GameDifficultyMode DifficultyMode = GameDifficultyMode.Easy)
        {
            if (IsHumanWhite)
            {
                OthelloPlayer oPlayerA = new OthelloPlayer(OthelloPlayerKind.White, "Human");
                OthelloPlayer oPlayerB = new OthelloPlayer(OthelloPlayerKind.Black, "Computer");

                _oGame = new OthelloGame(oPlayerA, oPlayerB, oPlayerA, false, true, true, DifficultyMode);
            }
            else
            {
                OthelloPlayer oPlayerA = new OthelloPlayer(OthelloPlayerKind.White, "Computer");
                OthelloPlayer oPlayerB = new OthelloPlayer(OthelloPlayerKind.Black, "Human");

                _oGame = new OthelloGame(oPlayerA, oPlayerB, oPlayerB, false, true, false, DifficultyMode);
            }

            _oGame.GameEnableLogging();
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

        public override List<OthelloToken> GameMakeMove(int GameX, int GameY, OthelloPlayer oCurrentPlayer, out bool IsValidMove)
        {
            List<OthelloToken> oFlipList = _oGame.GameMakeMove(GameX, GameY, oCurrentPlayer);
            IsValidMove = oFlipList.Count() < 1 ? true : false;
            return oFlipList;
        }

        public override OthelloPlayer GameUpdatePlayer()
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

        public override OthelloGameAi GameGetAiPlayer()
        {
            return _oGame.AIPlayer;
        }

        public override OthelloPlayer GameGetPlayerWhite()
        {
            return _oGame.PlayerWhite;
        }

        public override OthelloPlayer GameGetPlayerBlack()
        {
            return _oGame.PlayerBlack;
        }

        public override int GameGetScore(OthelloPlayer player)
        {
            if (player.PlayerKind == OthelloPlayerKind.Black)
                return _oGame.GameGetScoreBlack();
            
            return _oGame.GameGetScoreWhite();
        }

    }
}
