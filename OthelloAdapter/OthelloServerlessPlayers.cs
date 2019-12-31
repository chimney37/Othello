using NUnit.Framework;
using Othello;


namespace OthelloAWSServerless
{
    public class OthelloServerlessPlayers
    {
        public string PlayerNameWhite { get; set; }
        public string PlayerNameBlack { get; set; }
        public string FirstPlayer { get; set; }
        public bool UseAI { get; set; }
        public bool IsHumanWhite { get; set; }
        public GameDifficultyMode difficulty { get; set; }
    }
}
