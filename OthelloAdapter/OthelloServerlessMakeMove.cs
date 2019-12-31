using System.Collections.Generic;
using Othello;

namespace OthelloAWSServerless
{
    public class OthelloServerlessMakeMove
    {
        public int GameX { get; set; }
        public int GameY { get; set; }
        public string CurrentPlayer { get; set; }
    }

    public class OthelloServerlessMakeMoveFliplist
    {
        public string Id { get; set; }
        public bool IsValid { get;set;}
        public int Reason { get; set; }
        public OthelloServerlessMakeMove Move { get; set; }
        public List<OthelloToken> Fliplist { get; set; }
    }
}
