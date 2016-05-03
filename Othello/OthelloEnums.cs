namespace Othello
{
    //othello bit types can have 4 types of values. value of each is implementation dependent
    //othello player kind is 2 types
    //direction has 8 types starting at horizontal axis going from left to right, angle going anti-clockwise 0, 45 degrees, 90 degrees, etc.
    //board type has 4 types

    public enum OthelloBitType { Empty = 0x0, Black = 0x1, White = 0x3, OOB = 0xffff};
    public enum OthelloPlayerKind { Black, White};
    public enum OthelloDirection { Deg0,Deg45,Deg90,Deg135,Deg180,Deg225,Deg270,Deg315};
    public enum OthelloBoardType { TokenMatrix, CharMatrix, String, Bit};

    //GameState Modes used by the application (game) loop
    public enum GameStateMode { Debug, NewGame, NewAlternateGame, SwitchPlayer, LoadGame, SaveGame, InputMove, AIMove, TestMode, Undo, Redo, DoNothing };

    //game modes determine if human vs human or human vs computer (A.I.)
    public enum GameMode { HumanVSHuman, HumanVSComputer };

    //difficulty settings
    public enum GameDifficultyMode { Default=0, Easy=1, Medium=2, Hard=3}
}
