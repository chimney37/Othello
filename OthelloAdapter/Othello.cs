using System.Collections.Generic;
using Othello;

namespace OthelloAdapter
{
    public abstract class Othello
    {
        public abstract void GameCreateNewHumanVSHuman(string playerWhiteName, string playerBlackName, OthelloPlayerKind firstPlayerKind, bool IsAlternate = false);
        public abstract void GameCreateNewHumanVSAI(string playerWhiteName, string playerBlackName, OthelloPlayerKind firstPlayerKind, bool IsHumanWhite = true, bool IsAlternate = false, GameDifficultyMode mode = GameDifficultyMode.Easy);
        public abstract void GameLoad();
        public abstract void GameSave();
        public abstract void GameUndo();
        public abstract void GameRedo();
        public abstract OthelloToken[,] GameGetBoardData();
        public abstract int GameGetBoardSize();
        public abstract List<OthelloToken> GameMakeMove(int GameX, int GameY, OthelloPlayer player, out bool IsValidMove);
        public abstract List<OthelloToken> GameAIMakeMove();
        public abstract OthelloPlayer GameUpdatePlayer();
        public abstract int GameUpdateTurn();
        public abstract bool GameIsEndGame();
        public abstract GameMode GameGetMode();
        public abstract GameDifficultyMode GameGetDifficultyMode();
        public abstract void GameSetDifficultyMode(GameDifficultyMode mode);
        public abstract OthelloGameAi GameGetAiPlayer();
        public abstract OthelloPlayer GameGetPlayerWhite();
        public abstract OthelloPlayer GameGetPlayerBlack();
        public abstract int GameGetScore(OthelloPlayer player);

    }
}
