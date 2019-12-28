﻿using System;
using System.IO;
using System.Collections.Generic;
using NUnit.Framework;
using System.Linq;

namespace Othello
{

    /// <summary>
    /// A test class used to test the validty of the Othello Game Engine during its development. 
    /// Contains both functional and performance tests
    /// </summary>
    [TestFixture]
    public class OthelloTest
    {
        public static Tuple<string, int, int>[] tlist;

        public static Tuple<string,int,int,string>[,] multilist;

        /// <summary>
        /// Fix to change the test directory to the location of the assembly. Default is within VS 2017's program directory
        /// </summary>
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var dir = Path.GetDirectoryName(typeof(OthelloTest).Assembly.Location);
            Environment.CurrentDirectory = dir;
        }

        public OthelloTest()
        {
            #region TURN1CASES
            tlist = new Tuple<string, int, int>[10];

            //Turn1 : 3,2
            tlist[1] = new Tuple<string, int, int>(
                "xxxxxxxx" +
                "xxxxxxxx" +
                "xxxwxxxx" +
                "xxxwwxxx" +
                "xxxwbxxx" +
                "xxxxxxxx" +
                "xxxxxxxx" +
                "xxxxxxxx",
                3, 2);

            //Turn1 : 2,3
            tlist[2] = new Tuple<string, int, int>(
                "xxxxxxxx" +
                "xxxxxxxx" +
                "xxxxxxxx" +
                "xxwwwxxx" +
                "xxxwbxxx" +
                "xxxxxxxx" +
                "xxxxxxxx" +
                "xxxxxxxx",
                2, 3);

            //Turn1 : 5,4
            tlist[3] = new Tuple<string, int, int>(
                "xxxxxxxx" +
                "xxxxxxxx" +
                "xxxxxxxx" +
                "xxxbwxxx" +
                "xxxwwwxx" +
                "xxxxxxxx" +
                "xxxxxxxx" +
                "xxxxxxxx",
                5, 4);

            //Turn1 : 5,4
            tlist[4] = new Tuple<string, int, int>(
                "xxxxxxxx" +
                "xxxxxxxx" +
                "xxxxxxxx" +
                "xxxbwxxx" +
                "xxxwwwxx" +
                "xxxxxxxx" +
                "xxxxxxxx" +
                "xxxxxxxx",
                5, 4);

            //Turn1 : 4,5
            tlist[5] = new Tuple<string, int, int>(
                "xxxxxxxx" +
                "xxxxxxxx" +
                "xxxxxxxx" +
                "xxxbwxxx" +
                "xxxwwxxx" +
                "xxxxwxxx" +
                "xxxxxxxx" +
                "xxxxxxxx",
                4, 5);
            #endregion

            #region TURN_MULTI_CASES
            multilist = new Tuple<string,int,int,string>[6,4];

            //Case 1: (3,2)w, (4,2)b
            multilist[0, 0] = new Tuple<string, int, int,string>(
                "xxxxxxxx" +
                "xxxxxxxx" +
                "xxxwxxxx" +
                "xxxwwxxx" +
                "xxxwbxxx" +
                "xxxxxxxx" +
                "xxxxxxxx" +
                "xxxxxxxx",
                3, 2, "w");

            multilist[1, 0] = new Tuple<string, int, int,string>(
                "xxxxxxxx" +
                "xxxxxxxx" +
                "xxxwbxxx" +
                "xxxwbxxx" +
                "xxxwbxxx" +
                "xxxxxxxx" +
                "xxxxxxxx" +
                "xxxxxxxx",
                4, 2, "b");



            //Case 2: (2,3)w, (2,2)b
            multilist[0, 1] = new Tuple<string, int, int, string>(
               "xxxxxxxx" +
               "xxxxxxxx" +
               "xxxxxxxx" +
               "xxwwwxxx" +
               "xxxwbxxx" +
               "xxxxxxxx" +
               "xxxxxxxx" +
               "xxxxxxxx",
               2, 3, "w");

            multilist[1, 1] = new Tuple<string, int, int, string>(
               "xxxxxxxx" +
               "xxxxxxxx" +
               "xxbxxxxx" +
               "xxwbwxxx" +
               "xxxwbxxx" +
               "xxxxxxxx" +
               "xxxxxxxx" +
               "xxxxxxxx",
               2, 2, "b");


            //Case 3: (5,4)w, (5,3)b, (3,2)w
            multilist[0, 2] = new Tuple<string, int, int, string>(
               "xxxxxxxx" +
               "xxxxxxxx" +
               "xxxxxxxx" +
               "xxxbwxxx" +
               "xxxwwwxx" +
               "xxxxxxxx" +
               "xxxxxxxx" +
               "xxxxxxxx",
               5, 4, "w");

            multilist[1, 2] = new Tuple<string, int, int, string>(
               "xxxxxxxx" +
               "xxxxxxxx" +
               "xxxxxxxx" +
               "xxxbbbxx" +
               "xxxwwwxx" +
               "xxxxxxxx" +
               "xxxxxxxx" +
               "xxxxxxxx",
               5, 3, "b");

            multilist[2, 2] = new Tuple<string, int, int, string>(
               "xxxxxxxx" +
               "xxxxxxxx" +
               "xxxwxxxx" +
               "xxxwwbxx" +
               "xxxwwwxx" +
               "xxxxxxxx" +
               "xxxxxxxx" +
               "xxxxxxxx",
               3, 2, "w");

            //Case 4: (4,5)w,(5,5)b,(5,4)w,(5,3)b,(3,2)w,(3,5)b
            multilist[0, 3] = new Tuple<string, int, int, string>(
                "xxxxxxxx" +
                "xxxxxxxx" +
                "xxxxxxxx" +
                "xxxbwxxx" +
                "xxxwwxxx" +
                "xxxxwxxx" +
                "xxxxxxxx" +
                "xxxxxxxx",
               4, 5, "w");

            multilist[1, 3] = new Tuple<string, int, int, string>(
                "xxxxxxxx" +
                "xxxxxxxx" +
                "xxxxxxxx" +
                "xxxbwxxx" +
                "xxxwbxxx" +
                "xxxxwbxx" +
                "xxxxxxxx" +
                "xxxxxxxx",
               5, 5, "b");

            multilist[2, 3] = new Tuple<string, int, int, string>(
                "xxxxxxxx" +
                "xxxxxxxx" +
                "xxxxxxxx" +
                "xxxbwxxx" +
                "xxxwwwxx" +
                "xxxxwbxx" +
                "xxxxxxxx" +
                "xxxxxxxx",
               5, 4, "w");

            multilist[3, 3] = new Tuple<string, int, int, string>(
                "xxxxxxxx"+
                "xxxxxxxx"+
                "xxxxxxxx"+
                "xxxbbbxx"+
                "xxxwwbxx"+
                "xxxxwbxx"+
                "xxxxxxxx"+
                "xxxxxxxx",
               5, 3, "b");

            multilist[4, 3] = new Tuple<string, int, int, string>(
                "xxxxxxxx"+
                "xxxxxxxx"+
                "xxxwxxxx"+
                "xxxwbbxx"+
                "xxxwwbxx"+
                "xxxxwbxx"+
                "xxxxxxxx"+
                "xxxxxxxx",
               3, 2, "w");

            multilist[5, 3] = new Tuple<string, int, int, string>(
                "xxxxxxxx" +
                "xxxxxxxx" +
                "xxxwxxxx" +
                "xxxwbbxx" +
                "xxxwbbxx" +
                "xxxbbbxx" +
                "xxxxxxxx" +
                "xxxxxxxx",
               3, 5, "b");

            #endregion
        }

        #region GAME FEATURE, REQUIREMENT TESTS

        [TestCase(3,2)]
        [TestCase(2,3)]
        public static void CheckSetAndGetOthelloBoardMatrix(int x, int y)
        {
            OthelloBoard ob = new OthelloBoard();
            ob.SetCell(OthelloBitType.White,x, y);
            var target = ob.GetCell(x, y);

            Assert.AreEqual(target.Token, OthelloBitType.White);
        }

        [TestCase(3,2)]
        public static void CheckTokenMatrixCoordinateSystem(int x, int y)
        {
            var oPlayerA = new OthelloGamePlayer(OthelloPlayerKind.White, "PlayerA");
            var oPlayerB = new OthelloGamePlayer(OthelloPlayerKind.Black, "PlayerB");
            var oGame = new OthelloGame(oPlayerA, oPlayerB, oPlayerA);

            oGame.GameMakeMove(x, y, oPlayerA);
            OthelloToken[,] target = (OthelloToken[,])oGame.GameGetBoardData(OthelloBoardType.TokenMatrix);

            Assert.AreEqual(OthelloBitType.White, target[x, y].Token);
        }

        /// <summary>
        /// Check that the iterator is working correctly
        /// <description>heck that the OthelloBoardIterator Enumerator is working correctly</description>
        /// </summary>
        [TestCase]
        public static void CheckBoardIterator()
        {
            OthelloBoard b = new OthelloBoard(false);
            OthelloBoardIterator oIter = b.CreateIterator();
            int count = 0;

            //check first
            OthelloToken first = oIter.Current;
            Assert.AreEqual(0, first.X);
            Assert.AreEqual(0, first.Y);

            count++;

            //check second
            oIter.MoveNext();
            Assert.AreEqual(1, oIter.Current.X);
            Assert.AreEqual(0, oIter.Current.Y);

            count++;

            //check coordinates don't go out of bounds
            while(oIter.MoveNext())
            {
                Assert.False(oIter.Current.X >= OthelloBoard.BoardSize);
                Assert.False(oIter.Current.X < 0);
                Assert.False(oIter.Current.Y >= OthelloBoard.BoardSize);
                Assert.False(oIter.Current.Y < 0);
                    
                count++;

                Console.Write("({0},{1})", oIter.Current.X, oIter.Current.Y);
            }

            //check movenext returns false after hitting end
            bool whatsnext = oIter.MoveNext();
            Assert.AreEqual(false, whatsnext);

            //assert throwing from an anonymous delegate used to check that Current after hitting end throws exception (by IEnumerator specification)
            Assert.Throws<InvalidOperationException>(() => { OthelloToken ot = oIter.Current; });
            
            //check all tokens are counted
            Assert.AreEqual(OthelloBoard.BoardSize * OthelloBoard.BoardSize, count);
        }

        //check Basic single move
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        public static void CheckTurn1MovesCases(int ExpectIndex)
        {
            var oPlayerA = new OthelloGamePlayer(OthelloPlayerKind.White, "PlayerA");
            var oPlayerB = new OthelloGamePlayer(OthelloPlayerKind.Black, "PlayerB");
            var target = new OthelloGame(oPlayerA, oPlayerB, oPlayerA);

            target.GameMakeMove(tlist[ExpectIndex].Item2, tlist[ExpectIndex].Item3, oPlayerA);
            Assert.AreEqual(tlist[ExpectIndex].Item1, target.GameGetBoardData(OthelloBoardType.String));
        }

        //Check Basic multi moves
        [TestCase(0,2)]
        [TestCase(1,2)]
        [TestCase(2,3)]
        [TestCase(3,6)]
        public static void CheckMultiMovesCases(int index, int turncount)
        {
            var oPlayerA = new OthelloGamePlayer(OthelloPlayerKind.White, "PlayerA");
            var oPlayerB = new OthelloGamePlayer(OthelloPlayerKind.Black, "PlayerB");
            var target = new OthelloGame(oPlayerA,oPlayerB, oPlayerA);

            for (int t = 0; t < turncount; t++)
            {
                target.GameMakeMove(multilist[t, index].Item2, multilist[t, index].Item3, multilist[t, index].Item4 == "w" ? oPlayerA : oPlayerB);
                Assert.AreEqual(multilist[t, index].Item1, target.GameGetBoardData(OthelloBoardType.String));
            }
        }

        //helper method for testing flips
        private static void CompareFlips(OthelloToken ob, string[] paths, string[] flips)
        {
            List<OthelloToken> path = new List<OthelloToken>();

            for (int i = 0; i < paths.Length; i++)
            {
                foreach (char c in paths[i])
                {
                    switch (c)
                    {
                        case 'w':
                            path.Add(new OthelloToken(-1, -1, OthelloBitType.White));
                            break;
                        case 'b':
                            path.Add(new OthelloToken(-1, -1, OthelloBitType.Black));
                            break;
                        case 'e':
                            path.Add(new OthelloToken(-1, -1, OthelloBitType.Empty));
                            break;
                        case 'o':
                            path.Add(new OthelloToken(-1, -1, OthelloBitType.OOB));
                            break;
                    }
                }


                List<OthelloToken> fliptokens = OthelloState.GetFlipTokenFromPath(ob, path);

                string strfliptokens = "";
                foreach (OthelloToken ftoken in fliptokens)
                {
                    switch (ftoken.Token)
                    {
                        case OthelloBitType.Black:
                            strfliptokens = strfliptokens + "b";
                            break;
                        case OthelloBitType.White:
                            strfliptokens = strfliptokens + "w";
                            break;
                        case OthelloBitType.Empty:
                            strfliptokens = strfliptokens + "e";
                            break;
                        case OthelloBitType.OOB:
                            strfliptokens = strfliptokens + "o";
                            break;
                    }
                }

                Assert.AreEqual(flips[i], strfliptokens);

                fliptokens.Clear();
                path.Clear();
            }
        }

        [TestCase]
        public static void CheckInvalidMoves()
        {
            var oPlayerA = new OthelloGamePlayer(OthelloPlayerKind.White, "PlayerA");
            var oPlayerB = new OthelloGamePlayer(OthelloPlayerKind.Black, "PlayerB");
            var target = new OthelloGame(oPlayerA, oPlayerB, oPlayerA);

            string initboard =  "bxxxxxxx" +
                                "xwxxxxxx" +
                                "xxwwbxxx" +
                                "xxxwwxxx" +
                                "xxxbbbxx" +
                                "xxxxxwwx" +
                                "xxxxxbwx" +
                                "xxxxxbwb";

            target.GameSetBoardData(initboard);
            target.GameSetPlayer(oPlayerA);

            Assert.AreEqual(0, target.GameMakeMove(7, 7, oPlayerA).Count);
            Assert.AreEqual(0, target.GameMakeMove(1, 1, oPlayerA).Count);
            Assert.AreEqual(0, target.GameMakeMove(2, 3, oPlayerA).Count);
            Assert.AreEqual(0, target.GameMakeMove(7, 6, oPlayerA).Count);

        }

        //given a certain board state, game can make correct move.
        [TestCase]
        public static void CheckMidGameMove()
        {
            var oPlayerA = new OthelloGamePlayer(OthelloPlayerKind.White, "PlayerA");
            var oPlayerB = new OthelloGamePlayer(OthelloPlayerKind.Black, "PlayerB");
            var target = new OthelloGame(oPlayerA, oPlayerB, oPlayerA);

            string initboard =  "bxxxxxxx" +
                                "xxxxxxxx" +
                                "xxwwbxxx" +
                                "xxxwwxxx" +
                                "xxxwwwxx" +
                                "xxxxxwwx" +
                                "xxxxxxwx" +
                                "xxxxxbwx";

            string expect =     "bxxxxxxx" +
                                "xxxxxxxx" +
                                "xxwwbxxx" +
                                "xxxwwxxx" +
                                "xxxwwwxx" +
                                "xxxxxwwx" +
                                "xxxxxxwx" +
                                "xxxxxbbb";


            target.GameSetBoardData(initboard);
            target.GameSetPlayer(oPlayerB);
            target.GameMakeMove(7, 7, oPlayerB);

            Assert.AreEqual(expect, target.GameGetBoardData(OthelloBoardType.String));
        }

        //Othello State checks : check that correct flips list can be obtaine from a path for white move
        [TestCase]
        public static void CheckGetValidFlipsW()
        {
            var oPlayerA = new OthelloGamePlayer(OthelloPlayerKind.White, "PlayerA");

            //test Player A as white
            string[] paths = { "w", "b", "e,", "o", "ooo", "eee", "www", "bbb", "eb", "wbw", "wbbw",  "wbbwb", "wbbbwee" };
            string[] flips = { "",  "",  "",   "",  "",    "",    "",    "",    "",   "b",   "bb",    "bb",    "bbb"};

            OthelloToken ob = new OthelloToken(-1, -1, oPlayerA.GetPlayerOthelloToken());
            CompareFlips(ob, paths, flips);            
        }

        //Othello State checks : check that correct flips list can be obtaine from a path for black move
        [TestCase]
        public static void CheckGetValidFlipsB()
        {
            var oPlayerB = new OthelloGamePlayer(OthelloPlayerKind.Black, "PlayerB");

            //test Player B as white
            string[] paths2 = { "bwb", "bwwb", "bwwbw", "bwwwwweb", "bwwwwbeb"};
            string[] flips2 = { "w", "ww", "ww", "", "wwww"};

            OthelloToken ob = new OthelloToken(-1, -1, oPlayerB.GetPlayerOthelloToken());
            CompareFlips(ob, paths2, flips2);
        }

        //check that flipped tokens return correctly after a move
        [TestCase]
        public static void CheckGetValidFlipsOnMove()
        {
            var oPlayerA = new OthelloGamePlayer(OthelloPlayerKind.White, "PlayerA");
            var oPlayerB = new OthelloGamePlayer(OthelloPlayerKind.Black, "PlayerB");
            var target = new OthelloGame(oPlayerA, oPlayerB, oPlayerA);

            var initboard = "xxxxxxxx" +
                            "xxxxxxxx" +
                            "xxxxxxxx" +
                            "xxxbwxxx" +
                            "xxxwwwxx" +
                            "xxxbwbxx" +
                            "xxxxxxxx" +
                            "xxxxxxxx";

            target.GameSetBoardData(initboard);
            target.GameSetPlayer(oPlayerB);

            OthelloGamePlayer player = target.GameUpdatePlayer();
            List<OthelloToken> fliptokens = target.GameMakeMove(5, 3, player);

            Assert.GreaterOrEqual(fliptokens.FindIndex(a => a.X == 5 && a.Y == 4 && a.Token == OthelloBitType.White),0);
            Assert.GreaterOrEqual(fliptokens.FindIndex(a => a.X == 4 && a.Y == 4 && a.Token == OthelloBitType.White),0);
            Assert.GreaterOrEqual(fliptokens.FindIndex(a => a.X == 4 && a.Y == 3 && a.Token == OthelloBitType.White),0);
        }

        //check a game works after initializing alternatively
        [TestCase]
        public static void CheckAlternateBoardSettingMoves()
        {
            var oPlayerA = new OthelloGamePlayer(OthelloPlayerKind.White, "PlayerA");
            var oPlayerB = new OthelloGamePlayer(OthelloPlayerKind.Black, "PlayerB");
            var target = new OthelloGame(oPlayerA, oPlayerB, oPlayerA, true);

            var Expect =    "xxxxxxxx" +
                            "xxxxxxxx" +
                            "xxxxxxxx" +
                            "xxxwbxxx" +
                            "xxxbwxxx" +
                            "xxxxxxxx" +
                            "xxxxxxxx" +
                            "xxxxxxxx";

            Assert.AreEqual(Expect, target.GameGetBoardData(OthelloBoardType.String));

            var player = target.GameUpdatePlayer();
            target.GameMakeMove(4, 2, player);

            Expect =        "xxxxxxxx" +
                            "xxxxxxxx" +
                            "xxxxwxxx" +
                            "xxxwwxxx" +
                            "xxxbwxxx" +
                            "xxxxxxxx" +
                            "xxxxxxxx" +
                            "xxxxxxxx";

            Assert.AreEqual(Expect, target.GameGetBoardData(OthelloBoardType.String));

        }

        //check swtiching start player works
        [TestCase]
        public static void CheckStartPlayerSwitch()
        {
            var oPlayerA = new OthelloGamePlayer(OthelloPlayerKind.White, "PlayerA");
            var oPlayerB = new OthelloGamePlayer(OthelloPlayerKind.Black, "PlayerB");
            var oGame = new OthelloGame(oPlayerA, oPlayerB, oPlayerB, false);

            var targetPlayer = oGame.GameUpdatePlayer();

            Assert.AreEqual(targetPlayer.PlayerKind, oPlayerB.PlayerKind);

        }

        //checks game can calculate turn properly given state
        [TestCase]
        public static void CheckCalculateTurn()
        {
            var oPlayerA = new OthelloGamePlayer(OthelloPlayerKind.White, "PlayerA");
            var oPlayerB = new OthelloGamePlayer(OthelloPlayerKind.Black, "PlayerB");
            var target = new OthelloGame(oPlayerA, oPlayerB, oPlayerA);
         
            Console.WriteLine(string.Format("Before ExecuteMoves. Turn = {0}", target.GameUpdateTurn()));

            OthelloGamePlayer player = target.GameUpdatePlayer();
            target.GameMakeMove(5, 4, player);

            player = target.GameUpdatePlayer();
            target.GameMakeMove(5, 3, player);

            Console.WriteLine(string.Format("After Execute 2 Moves. Current Turn = {0}", target.GameUpdateTurn()));

            Assert.AreEqual(3, target.GameUpdateTurn());
        }

        //check that game knows it's at an end given a common end game condition
        [TestCase]
        public static void CheckValidateEndGame1()
        {
            var oPlayerA = new OthelloGamePlayer(OthelloPlayerKind.White, "PlayerA");
            var oPlayerB = new OthelloGamePlayer(OthelloPlayerKind.Black, "PlayerB");
            var target = new OthelloGame(oPlayerA, oPlayerB, oPlayerA);

            //initialize a near end game
            string initboard =  "bbbbbbbb" +
                                "wwbbbbbb" +
                                "wbwwbwbb" +
                                "wbbwwwbb" +
                                "wwwwwwbb" +
                                "bbwbbwbb" +
                                "xbbbbbbb" +
                                "xbbbbbww";

            target.GameSetBoardData(initboard);
            target.GameSetPlayer(oPlayerA);
            
            target.GameUpdateTurn();
            OthelloGamePlayer player = target.GameUpdatePlayer();
            target.GameMakeMove(0, 6, player);

            Console.WriteLine(string.Format("Calculated Turn = {0}", target.GameUpdateTurn()));

            player = target.GameUpdatePlayer();
            target.GameMakeMove(0, 7, player);

            Console.WriteLine(string.Format("Calculated Turn = {0}", target.GameUpdateTurn()));

            Assert.AreEqual(true, target.GameIsEndGame());

        }

        //check that game nows it's at an end given a rare game end condition (both players cannot move)
        [TestCase]
        public static void CheckValidateEndGame2()
        {
            var oPlayerA = new OthelloGamePlayer(OthelloPlayerKind.White, "PlayerA");
            var oPlayerB = new OthelloGamePlayer(OthelloPlayerKind.Black, "PlayerB");
            var target = new OthelloGame(oPlayerA, oPlayerB, oPlayerA);

            string initboard = "wwwwwwwx" +
                               "wbbbbwwb" +
                               "wbbwwwwb" +
                               "wwbbwbwb" +
                               "wwbwbwwb" +
                               "wbwbwbwb" +
                               "wwbbbwbb" +
                               "wbbbbbbb";

            OthelloGamePlayer player = target.GameUpdatePlayer();
            target.GameSetBoardData(initboard);

            Assert.AreEqual(true, target.GameIsEndGame());
        }


        [TestCase]
        public static void CheckAllowedPlayerMoveCount()
        {
            var oPlayerA = new OthelloGamePlayer(OthelloPlayerKind.White, "PlayerA");
            var oPlayerB = new OthelloGamePlayer(OthelloPlayerKind.Black, "PlayerB");
            var oGame = new OthelloGame(oPlayerA, oPlayerB, oPlayerA);

            OthelloGamePlayer player = oGame.GameUpdatePlayer();
            int targetcount = oGame.GameGetPlayerAllowedMoves(player).Count;


            Console.WriteLine(string.Format("Validated and Set Player to Move = {0}, allowed moves count = {1}", player, targetcount));

            Assert.AreEqual(4, targetcount);
        }

        //player switches to opposition once a turn is complete.
        [TestCase]
        public static void CheckValidatePlayerSwitch1()
        {
            var oPlayerA = new OthelloGamePlayer(OthelloPlayerKind.White, "PlayerA");
            var oPlayerB = new OthelloGamePlayer(OthelloPlayerKind.Black, "PlayerB");
            var target = new OthelloGame(oPlayerA, oPlayerB, oPlayerA);

            OthelloGamePlayer player = target.GameUpdatePlayer();
            target.GameMakeMove(5, 4, player);

            player = target.GameUpdatePlayer();
            Assert.AreEqual(OthelloPlayerKind.Black, player.PlayerKind);
        }

        // when player switches to opposition once validated when the board has no moves for a player
        [TestCase]
        public static void CheckValidatePlayerSwitch2()
        {
            var oPlayerA = new OthelloGamePlayer(OthelloPlayerKind.White, "PlayerA");
            var oPlayerB = new OthelloGamePlayer(OthelloPlayerKind.Black, "PlayerB");
            var target = new OthelloGame(oPlayerA, oPlayerB, oPlayerA);

            //initialize a near end game
            string initboard =  "bbbbbbbb" +
                                "wwbbbbbb" +
                                "wbwwbwbb" +
                                "wbbwwwbb" +
                                "wwwwwwbb" +
                                "bbwbbwbb" +
                                "xbbbbbbb" +
                                "xbbbbbww";

            target.GameSetBoardData(initboard);

            target.GameSetPlayer(oPlayerB);
            Console.WriteLine(string.Format("Validated and Set Player to Move = {0}, allowed moves count = {1}", oPlayerB.PlayerKind, target.GameGetPlayerAllowedMoves(oPlayerB).Count));

            OthelloGamePlayer targetPlayer = target.GameUpdatePlayer();

            Console.WriteLine(string.Format("Validated and Set Player to Move = {0}, allowed moves count = {1}", targetPlayer.PlayerKind, target.GameGetPlayerAllowedMoves(targetPlayer).Count));

            Assert.AreEqual(OthelloPlayerKind.White, targetPlayer.PlayerKind);
        }

        // Custom Save / Load (saves with custom paths)
        [TestCase]
        public static void CheckCustomSaveLoad()
        {
            var oPlayerA = new OthelloGamePlayer(OthelloPlayerKind.White, "PlayerA");
            var oPlayerB = new OthelloGamePlayer(OthelloPlayerKind.Black, "PlayerB");
            var target = new OthelloGame(oPlayerA, oPlayerB, oPlayerA);

            var customPath = @"..\..\..\OthelloSaveGames";

            OthelloGamePlayer player = target.GameUpdatePlayer();
            target.GameMakeMove(5, 4, player);
            target.GameSave(false, customPath);

            target.GameCreateNew(oPlayerA, oPlayerB, oPlayerB);
            target.GameLoad(false, customPath);

            string Expect = "xxxxxxxx" +
                            "xxxxxxxx" +
                            "xxxxxxxx" +
                            "xxxbwxxx" +
                            "xxxwwwxx" +
                            "xxxxxxxx" +
                            "xxxxxxxx" +
                            "xxxxxxxx";

            Assert.AreEqual(Expect, target.GameGetBoardData(OthelloBoardType.String));
        }

        // Check Saves are idempotent - saving more than once will result in same file size
        [TestCase]
        public static void CheckMultipleSaves()
        {
            var oPlayerA = new OthelloGamePlayer(OthelloPlayerKind.White, "PlayerA");
            var oPlayerB = new OthelloGamePlayer(OthelloPlayerKind.Black, "PlayerB");
            var target = new OthelloGame(oPlayerA, oPlayerB, oPlayerA);
            var target_multiple_saves = new OthelloGame(oPlayerA, oPlayerB, oPlayerA);

            OthelloGamePlayer player = target.GameUpdatePlayer();
            target.GameMakeMove(5, 4, player);
            var customdir = @"./OthelloSaveGamesTest";
            target.GameSave(false, customdir);
            long Expect = new FileInfo(OthelloIO.GetFileSavePath(customdir, target.SaveFileName)).Length;

            player = target_multiple_saves.GameUpdatePlayer();
            target_multiple_saves.GameMakeMove(5, 4, player);

            for (int i = 0; i < 100; i++)
            {
                target_multiple_saves.GameSave();
            }

            long target_size = new FileInfo(OthelloIO.GetFileSavePath(target_multiple_saves.DefaultSaveDir, target_multiple_saves.SaveFileName)).Length;

            Assert.AreEqual(Expect, target_size);

            Directory.Delete(customdir, true);
            Directory.Delete(target_multiple_saves.DefaultSaveDir, true);
        }

        // for Custom path Load Undo
        [TestCase]
        public static void CheckSaveLoadUndo()
        {
            var oPlayerA = new OthelloGamePlayer(OthelloPlayerKind.White, "PlayerA");
            var oPlayerB = new OthelloGamePlayer(OthelloPlayerKind.Black, "PlayerB");
            var target = new OthelloGame(oPlayerA, oPlayerB, oPlayerA);

            var customPath = @"../../../OthelloSaveGames";

            OthelloGamePlayer player = target.GameUpdatePlayer();
            target.GameMakeMove(5, 4, player);

            player = target.GameUpdatePlayer();
            target.GameMakeMove(5, 3, player);

            target.GameSave(false, customPath);

            target.GameCreateNew(oPlayerA, oPlayerB, oPlayerB);
            target.GameLoad(false, customPath);

            target.GameUndo();

            string Expect = "xxxxxxxx" +
                            "xxxxxxxx" +
                            "xxxxxxxx" +
                            "xxxbwxxx" +
                            "xxxwwwxx" +
                            "xxxxxxxx" +
                            "xxxxxxxx" +
                            "xxxxxxxx";

            Assert.AreEqual(Expect, target.GameGetBoardData(OthelloBoardType.String));
        }

        //Custom path Load Redo
        [TestCase]
        public static void CheckSaveLoadRedo()
        {
            var oPlayerA = new OthelloGamePlayer(OthelloPlayerKind.White, "PlayerA");
            var oPlayerB = new OthelloGamePlayer(OthelloPlayerKind.Black, "PlayerB");
            var target = new OthelloGame(oPlayerA, oPlayerB, oPlayerA);

            var customPath = @"../../../OthelloSaveGames";

            OthelloGamePlayer player = target.GameUpdatePlayer();
            target.GameMakeMove(5, 4, player);

            player = target.GameUpdatePlayer();
            target.GameMakeMove(5, 3, player);
            target.GameUndo();

            target.GameSave(false, customPath);

            target.GameCreateNew(oPlayerA, oPlayerB, oPlayerB);
            target.GameLoad(false, customPath);

            target.GameRedo();

            string Expect = "xxxxxxxx" +
                            "xxxxxxxx" +
                            "xxxxxxxx" +
                            "xxxbbbxx" +
                            "xxxwwwxx" +
                            "xxxxxxxx" +
                            "xxxxxxxx" +
                            "xxxxxxxx";

            Assert.AreEqual(Expect, target.GameGetBoardData(OthelloBoardType.String));
        }

        //add a test case for S1 -> M1 -> S2 -> M2 -> S3 -> M3 -> S4 -> U -> S3 -> M4 -> S5 -> R -> S5
        [TestCase]
        public static void CheckNoRedoAfterMove()
        {
            var oPlayerA = new OthelloGamePlayer(OthelloPlayerKind.White, "PlayerA");
            var oPlayerB = new OthelloGamePlayer(OthelloPlayerKind.Black, "PlayerB");
            var target = new OthelloGame(oPlayerA, oPlayerB, oPlayerA);

            OthelloGamePlayer player = target.GameUpdatePlayer();
            target.GameMakeMove(5, 4, player);

            string Expect = "xxxxxxxx" +
                            "xxxxxxxx" +
                            "xxxxxxxx" +
                            "xxxbwxxx" +
                            "xxxwwwxx" +
                            "xxxxxxxx" +
                            "xxxxxxxx" +
                            "xxxxxxxx";

            Assert.AreEqual(Expect, target.GameGetBoardData(OthelloBoardType.String));


            player = target.GameUpdatePlayer();
            target.GameMakeMove(5, 3, player);

            Expect =    "xxxxxxxx" +
                        "xxxxxxxx" +
                        "xxxxxxxx" +
                        "xxxbbbxx" +
                        "xxxwwwxx" +
                        "xxxxxxxx" +
                        "xxxxxxxx" +
                        "xxxxxxxx";

            Assert.AreEqual(Expect, target.GameGetBoardData(OthelloBoardType.String));



            player = target.GameUpdatePlayer();
            target.GameMakeMove(3, 2, player);

            Expect =    "xxxxxxxx" +
                        "xxxxxxxx" +
                        "xxxwxxxx" +
                        "xxxwwbxx" +
                        "xxxwwwxx" +
                        "xxxxxxxx" +
                        "xxxxxxxx" +
                        "xxxxxxxx";

            Assert.AreEqual(Expect, target.GameGetBoardData(OthelloBoardType.String));


            player = target.GameUpdatePlayer();
            target.GameUndo();

            player = target.GameUpdatePlayer();
            target.GameMakeMove(5, 2, player);

            player = target.GameUpdatePlayer();
            target.GameRedo();

            Expect =    "xxxxxxxx" +
                        "xxxxxxxx" +
                        "xxxxxwxx" +
                        "xxxbwwxx" +
                        "xxxwwwxx" +
                        "xxxxxxxx" +
                        "xxxxxxxx" +
                        "xxxxxxxx";

            Assert.AreEqual(Expect, target.GameGetBoardData(OthelloBoardType.String));
        }

        // for JSON serialization and deserialization
        [TestCase]
        public static void CheckJSONSerializationAndDeserialization()
        {
            var oPlayerA = new OthelloGamePlayer(OthelloPlayerKind.White, "PlayerA");
            var oPlayerB = new OthelloGamePlayer(OthelloPlayerKind.Black, "PlayerB");
            var target = new OthelloGame(oPlayerA, oPlayerB, oPlayerA);

            //the first player is White and makes a move
            OthelloGamePlayer player = target.GameUpdatePlayer();
            target.GameMakeMove(5, 4, player);

            string json = target.GameGetJSON();
            OthelloGame game = target.GameGetGameFromJSON(json);
            player = game.GameUpdatePlayer();

            //the player should now be Black
            var Expect = OthelloPlayerKind.Black;
            Assert.AreEqual(Expect, player.PlayerKind);
        }

        [TestCase]
        public static void CheckJSONDeserializationRaw()
        {
            var oPlayerA = new OthelloGamePlayer(OthelloPlayerKind.White, "PlayerA");
            var oPlayerB = new OthelloGamePlayer(OthelloPlayerKind.Black, "PlayerB");
            var target = new OthelloGame(oPlayerA, oPlayerB, oPlayerA);

            string json = "\"AAEAAAD/////AQAAAAAAAAAEAQAAABxTeXN0ZW0uQ29sbGVjdGlvbnMuQXJyYXlMaXN0AwAAAAZfaXRlbXMFX" +
                "3NpemUIX3ZlcnNpb24FAAAICAkCAAAABwAAAAcAAAAQAgAAAAgAAAAJAwAAAAkEAAAACQUAAAAJBgAAAAkHAAAACQgAAAAJCQ" +
                "AAAAoMCgAAAD5PdGhlbGxvLCBWZXJzaW9uPTEuMC4wLjAsIEN1bHR1cmU9bmV1dHJhbCwgUHVibGljS2V5VG9rZW49bnVsbAU" +
                "DAAAAGU90aGVsbG8uT3RoZWxsb0dhbWVQbGF5ZXICAAAAGzxQbGF5ZXJLaW5kPmtfX0JhY2tpbmdGaWVsZClPdGhlbGxvUGVy" +
                "c29uKzxQbGF5ZXJOYW1lPmtfX0JhY2tpbmdGaWVsZAQBGU90aGVsbG8uT3RoZWxsb1BsYXllcktpbmQKAAAACgAAAAX1////G" +
                "U90aGVsbG8uT3RoZWxsb1BsYXllcktpbmQBAAAAB3ZhbHVlX18ACAoAAAABAAAABgwAAAAHUGxheWVyQQEEAAAAAwAAAAHz//" +
                "//9f///wAAAAAGDgAAAAdQbGF5ZXJCBQUAAAAUT3RoZWxsby5PdGhlbGxvU3RhdGUFAAAAFzxTY29yZVc+a19fQmFja2luZ0Z" +
                "pZWxkFzxTY29yZUI+a19fQmFja2luZ0ZpZWxkHjxDdXJyZW50UGxheWVyPmtfX0JhY2tpbmdGaWVsZBU8VHVybj5rX19CYWNr" +
                "aW5nRmllbGQJQm9hcmREYXRhAAAEAAQICBlPdGhlbGxvLk90aGVsbG9HYW1lUGxheWVyCgAAAAgUT3RoZWxsby5PdGhlbGxvQ" +
                "m9hcmQKAAAACgAAAAIAAAACAAAACQMAAAABAAAACRAAAAAMEQAAAElTeXN0ZW0sIFZlcnNpb249NC4wLjAuMCwgQ3VsdHVyZT" +
                "1uZXV0cmFsLCBQdWJsaWNLZXlUb2tlbj1iNzdhNWM1NjE5MzRlMDg5BQYAAAB6U3lzdGVtLkNvbGxlY3Rpb25zLkdlbmVyaWM" +
                "uU3RhY2tgMVtbT3RoZWxsby5PdGhlbGxvU3RhdGUsIE90aGVsbG8sIFZlcnNpb249MS4wLjAuMCwgQ3VsdHVyZT1uZXV0cmFs" +
                "LCBQdWJsaWNLZXlUb2tlbj1udWxsXV0DAAAABl9hcnJheQVfc2l6ZQhfdmVyc2lvbgQAABZPdGhlbGxvLk90aGVsbG9TdGF0Z" +
                "VtdCgAAAAgIEQAAAAkSAAAAAAAAAAAAAAABBwAAAAYAAAAJEgAAAAAAAAAAAAAABQgAAAAQT3RoZWxsby5HYW1lTW9kZQEAAA" +
                "AHdmFsdWVfXwAICgAAAAAAAAAFCQAAABtPdGhlbGxvLk90aGVsbG9HYW1lQWlTeXN0ZW0HAAAADV9jdXJyZW50U3RhdGUcPEh" +
                "1bWFuUGxheWVyPmtfX0JhY2tpbmdGaWVsZBk8QWlQbGF5ZXI+a19fQmFja2luZ0ZpZWxkE19tYXhQaXZvdEhldXJpc3RpY3MW" +
                "X21ham9yUGl2b3RzSGV1cmlzdGljcwpfbWF4UGl2b3RzDF9tYWpvclBpdm90cwQEBAMDBgYUT3RoZWxsby5PdGhlbGxvU3Rhd" +
                "GUKAAAAGU90aGVsbG8uT3RoZWxsb0dhbWVQbGF5ZXIKAAAAGU90aGVsbG8uT3RoZWxsb0dhbWVQbGF5ZXIKAAAAeVN5c3RlbS" +
                "5Db2xsZWN0aW9ucy5HZW5lcmljLkxpc3RgMVtbT3RoZWxsby5PdGhlbGxvVG9rZW4sIE90aGVsbG8sIFZlcnNpb249MS4wLjA" +
                "uMCwgQ3VsdHVyZT1uZXV0cmFsLCBQdWJsaWNLZXlUb2tlbj1udWxsXV15U3lzdGVtLkNvbGxlY3Rpb25zLkdlbmVyaWMuTGlz" +
                "dGAxW1tPdGhlbGxvLk90aGVsbG9Ub2tlbiwgT3RoZWxsbywgVmVyc2lvbj0xLjAuMC4wLCBDdWx0dXJlPW5ldXRyYWwsIFB1Y" +
                "mxpY0tleVRva2VuPW51bGxdXQoAAAAJEwAAAAkDAAAACQQAAAAJFgAAAAkXAAAACRgAAAAJGQAAAAUQAAAAFE90aGVsbG8uT3" +
                "RoZWxsb0JvYXJkAgAAAAZIYXNoSUQJYm9hcmREYXRhAQcCCgAAAAoJGgAAAAcSAAAAAAEAAAAAAAAABBRPdGhlbGxvLk90aGV" +
                "sbG9TdGF0ZQoAAAABEwAAAAUAAAACAAAAAgAAAAkDAAAAAQAAAAkcAAAABBYAAAB5U3lzdGVtLkNvbGxlY3Rpb25zLkdlbmVy" +
                "aWMuTGlzdGAxW1tPdGhlbGxvLk90aGVsbG9Ub2tlbiwgT3RoZWxsbywgVmVyc2lvbj0xLjAuMC4wLCBDdWx0dXJlPW5ldXRyY" +
                "WwsIFB1YmxpY0tleVRva2VuPW51bGxdXQMAAAAGX2l0ZW1zBV9zaXplCF92ZXJzaW9uBAAAFk90aGVsbG8uT3RoZWxsb1Rva2" +
                "VuW10KAAAACAgJHQAAAAQAAAAEAAAAARcAAAAWAAAACR4AAAAMAAAADAAAABEYAAAABAAAAAYfAAAAAzAsMAYgAAAAAzAsNwY" +
                "hAAAAAzcsMAYiAAAAAzcsNxEZAAAADAAAAAYjAAAAAzEsMQYkAAAAAzEsNgYlAAAAAzYsMQYmAAAAAzYsNgYnAAAAAzAsMQYo" +
                "AAAAAzAsNgYpAAAAAzEsMAYqAAAAAzEsNwYrAAAAAzYsMAYsAAAAAzYsNwYtAAAAAzcsMQYuAAAAAzcsNg8aAAAAEAAAAAIAA" +
                "AAAAAABwANAAAAAAAAAARwAAAAQAAAACgkvAAAABx0AAAAAAQAAAAQAAAAEFE90aGVsbG8uT3RoZWxsb1Rva2VuCgAAAAkwAA" +
                "AACTEAAAAJMgAAAAkzAAAABx4AAAAAAQAAABAAAAAEFE90aGVsbG8uT3RoZWxsb1Rva2VuCgAAAAk0AAAACTUAAAAJNgAAAAk" +
                "3AAAACTgAAAAJOQAAAAk6AAAACTsAAAAJPAAAAAk9AAAACT4AAAAJPwAAAA0EDy8AAAAQAAAAAgAAAAAAAAHAA0AAAAAAAAAF" +
                "MAAAABRPdGhlbGxvLk90aGVsbG9Ub2tlbgMAAAASPFg+a19fQmFja2luZ0ZpZWxkEjxZPmtfX0JhY2tpbmdGaWVsZBY8VG9rZ" +
                "W4+a19fQmFja2luZ0ZpZWxkAAAECAgWT3RoZWxsby5PdGhlbGxvQml0VHlwZQoAAAAKAAAAAAAAAAAAAAAFwP///xZPdGhlbG" +
                "xvLk90aGVsbG9CaXRUeXBlAQAAAAd2YWx1ZV9fAAgKAAAAAwAAAAExAAAAMAAAAAAAAAAHAAAAAb/////A////AwAAAAEyAAA" +
                "AMAAAAAcAAAAAAAAAAb7////A////AwAAAAEzAAAAMAAAAAcAAAAHAAAAAb3////A////AwAAAAE0AAAAMAAAAAEAAAABAAAA" +
                "Abz////A////AwAAAAE1AAAAMAAAAAEAAAAGAAAAAbv////A////AwAAAAE2AAAAMAAAAAYAAAABAAAAAbr////A////AwAAA" +
                "AE3AAAAMAAAAAYAAAAGAAAAAbn////A////AwAAAAE4AAAAMAAAAAAAAAABAAAAAbj////A////AwAAAAE5AAAAMAAAAAAAAA" +
                "AGAAAAAbf////A////AwAAAAE6AAAAMAAAAAEAAAAAAAAAAbb////A////AwAAAAE7AAAAMAAAAAEAAAAHAAAAAbX////A///" +
                "/AwAAAAE8AAAAMAAAAAYAAAAAAAAAAbT////A////AwAAAAE9AAAAMAAAAAYAAAAHAAAAAbP////A////AwAAAAE+AAAAMAAA" +
                "AAcAAAABAAAAAbL////A////AwAAAAE/AAAAMAAAAAcAAAAGAAAAAbH////A////AwAAAAs=\"";
            OthelloGame game = target.GameGetGameFromJSON(json);
            var player = game.GameUpdatePlayer();

            var Expect = OthelloPlayerKind.White;
            Assert.AreEqual(Expect, player.PlayerKind);
        }

        //Score Check
        [TestCase]
        public static void CheckScoresAreCorrect()
        {
            var oPlayerA = new OthelloGamePlayer(OthelloPlayerKind.White, "PlayerA");
            var oPlayerB = new OthelloGamePlayer(OthelloPlayerKind.Black, "PlayerB");
            var target = new OthelloGame(oPlayerA, oPlayerB, oPlayerA);

            OthelloGamePlayer player = target.GameUpdatePlayer();
            target.GameMakeMove(5, 4, player);

            player = target.GameUpdatePlayer();
            target.GameMakeMove(5, 3, player);

            player = target.GameUpdatePlayer();
            target.GameMakeMove(3, 2, player);

            player = target.GameUpdatePlayer();
            target.GameMakeMove(5, 5, player);

            player = target.GameUpdatePlayer();
            target.GameMakeMove(6, 6, player);

            Assert.AreEqual(2, target.GameGetScoreBlack());
            Assert.AreEqual(7, target.GameGetScoreWhite());
        }


        [TestCase(1, 0, OthelloBitType.Empty)]
        [TestCase(1,0, OthelloBitType.White)]
        [TestCase(0,0,OthelloBitType.Black)]
        public static void CheckOthelloTokenInstantiation(int x,int y, OthelloBitType type)
        {
            OthelloToken ot = new OthelloToken(x, y, type);

            Assert.AreEqual(x, ot.X);
            Assert.AreEqual(y, ot.Y);
            Assert.AreEqual(type,ot.Token);
        }

        [TestCase(OthelloBitType.White, ExpectedResult=OthelloBitType.Black)]
        [TestCase(OthelloBitType.Black, ExpectedResult=OthelloBitType.White)]
        public static OthelloBitType CheckOthelloTokenInverse(OthelloBitType type)
        {
            return OthelloToken.GetInverse(new OthelloToken(-1, -1, type)).Token;
        }

        #endregion

        #region AI TESTS
        [TestCase]
        public static void CheckAIRandomBestMove()
        {
            var oPlayerA = new OthelloGamePlayer(OthelloPlayerKind.White, "PlayerA");
            var oPlayerB = new OthelloGamePlayer(OthelloPlayerKind.Black, "PlayerB");
            var oGame = new OthelloGame(oPlayerA, oPlayerB, oPlayerA);
            var oAIPlayer = new OthelloGameAiSystem(oGame, oPlayerB, oPlayerA);

            string initboard =  "xxxxxxxx" +
                                "xxxxxxxx" +
                                "xxbwxxxx" +
                                "xxbwwxxx" +
                                "xxwbbxxx" +
                                "xwxxxxxx" +
                                "xxxxxxxx" +
                                "xxxxxxxx";

            oGame.GameSetBoardData(initboard);

            var oTarget = oAIPlayer.GetBestMove(oAIPlayer.AiPlayer,5);
            Assert.AreNotEqual(new OthelloToken(2, 5, OthelloBitType.Black), oTarget);
            Assert.AreNotEqual(new OthelloToken(1, 4, OthelloBitType.Black), oTarget);
            Assert.AreNotEqual(new OthelloToken(5, 2, OthelloBitType.Black), oTarget);
        }


        [TestCase(GameDifficultyMode.Default)]
        public static void CheckAIConfigLoaderSanity(GameDifficultyMode modes)
        {
            OthelloGamePlayer oPlayerA = new OthelloGamePlayer(OthelloPlayerKind.White, "Human");
            OthelloGamePlayer oPlayerB = new OthelloGamePlayer(OthelloPlayerKind.Black, "Computer");

            OthelloGame oGame = new OthelloGame(oPlayerA, oPlayerB, oPlayerA, false, true);

            //oGame.DebugLoadAIConfig();
            int? depth;
            float? alpha, beta;
            int[] turn = new int[64];
            for (int i = 0; i < turn.Length; i++)
                turn[i] = i + 1;

            foreach(int n in turn)
            {
                oGame.DebugGetTurnConfig(modes, n, oGame.DebugAIConfig(), out depth, out alpha, out beta);

                Assert.AreNotEqual(null, depth);
                Assert.AreNotEqual(null, alpha);
                Assert.AreNotEqual(null, beta);
            }

        }

        [TestCase]
        public static void CheckAISaveLoadGame()
        {
            var oPlayerA = new OthelloGamePlayer(OthelloPlayerKind.White, "PlayerA");
            var oPlayerB = new OthelloGamePlayer(OthelloPlayerKind.Black, "PlayerB");

            var target = new OthelloGame(oPlayerA, oPlayerB, oPlayerA, false, true);
            var oAIPlayer = new OthelloGameAiSystem(target, oPlayerB, oPlayerA);

            var customPath = @"..\..\..\OthelloSaveGames";

            string initboard = "xxxxxxxx" +
                                "xxxxxxxx" +
                                "xxbwxxxx" +
                                "xxbwwxxx" +
                                "xxwbbxxx" +
                                "xwxxxxxx" +
                                "xxxxxxxx" +
                                "xxxxxxxx";

            target.GameDisableLog();
            target.GameSetBoardData(initboard);
            var oTarget = oAIPlayer.GetBestMove(oAIPlayer.AiPlayer, 5);

            target.GameAIMakeMove();

            string expect = (string)target.GameGetBoardData(OthelloBoardType.String);

            target.GameSave(false, customPath);
            target.GameCreateNew(oPlayerA, oPlayerB, oPlayerB);
            target.GameLoad(false, customPath);

            Assert.AreEqual(expect, target.GameGetBoardData(OthelloBoardType.String));
        }


        #endregion
    }
}
