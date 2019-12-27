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
        public abstract List<OthelloToken> GameMakeMove(int GameX, int GameY, OthelloGamePlayer player, out bool IsValidMove);
        public abstract List<OthelloToken> GameAIMakeMove();
        public abstract OthelloGamePlayer GameUpdatePlayer();
        public abstract int GameUpdateTurn();
        public abstract bool GameIsEndGame();
        public abstract GameMode GameGetMode();
        public abstract GameDifficultyMode GameGetDifficultyMode();
        public abstract void GameSetDifficultyMode(GameDifficultyMode mode);
        public abstract OthelloGameAiSystem GameGetAiPlayer();
        public abstract OthelloGamePlayer GameGetPlayerWhite();
        public abstract OthelloGamePlayer GameGetPlayerBlack();
        public abstract int GameGetScore(OthelloGamePlayer player);
        public abstract string GetGameJSON();
        public abstract void GetGameFromJSON(string json);
    }
}
