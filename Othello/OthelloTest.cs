﻿using System;
using System.IO;
using System.Collections.Generic;
using NUnit.Framework;
using System.Diagnostics;
using System.Globalization;

namespace Othello
{

    /// <summary>
    /// A test class used to test the validty of the Othello Game Engine during its development. 
    /// Contains both functional and performance tests
    /// </summary>
    [TestFixture]
    public class OthelloTest
    {
        private static Tuple<string, int, int>[] tlist;
        private static Tuple<string,int,int,string>[,] multilist;

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

            Assert.That(target.Token, Is.EqualTo(OthelloBitType.White));
        }

        [TestCase(3,2)]
        public static void CheckTokenMatrixCoordinateSystem(int x, int y)
        {
            var oPlayerA = new OthelloGamePlayer(OthelloPlayerKind.White, "PlayerA");
            var oPlayerB = new OthelloGamePlayer(OthelloPlayerKind.Black, "PlayerB");
            var oGame = new OthelloGame(oPlayerA, oPlayerB, oPlayerA);

            oGame.GameMakeMove(x, y, oPlayerA);
            OthelloToken[,] target = (OthelloToken[,])oGame.GameGetBoardData(OthelloBoardType.TokenMatrix);

            Assert.That(target[x, y].Token, Is.EqualTo(OthelloBitType.White));
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
            Assert.That(first.X, Is.EqualTo(0));
            Assert.That(first.Y, Is.EqualTo(0));

            count++;

            //check second
            oIter.MoveNext();
            Assert.That(oIter.Current.X, Is.EqualTo(1));
            Assert.That(oIter.Current.Y, Is.EqualTo(0));

            count++;

            //check coordinates don't go out of bounds
            while(oIter.MoveNext())
            {
                Assert.That(oIter.Current.X >= OthelloBoard.BoardSize, Is.False);
                Assert.That(oIter.Current.X < 0, Is.False);
                Assert.That(oIter.Current.Y >= OthelloBoard.BoardSize, Is.False);
                Assert.That(oIter.Current.Y < 0, Is.False);
                    
                count++;

                Trace.Write(string.Format(CultureInfo.CurrentCulture,"({0},{1})", oIter.Current.X, oIter.Current.Y));
            }

            //check movenext returns false after hitting end
            bool whatsnext = oIter.MoveNext();
            Assert.That(whatsnext, Is.EqualTo(false));

            //assert throwing from an anonymous delegate used to check that Current after hitting end throws exception (by IEnumerator specification)
            Assert.Throws<InvalidOperationException>(() => { OthelloToken ot = oIter.Current; });
            
            //check all tokens are counted
            Assert.That(count, Is.EqualTo(OthelloBoard.BoardSize * OthelloBoard.BoardSize));
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
            Assert.That(target.GameGetBoardData(OthelloBoardType.StringSequence), Is.EqualTo(tlist[ExpectIndex].Item1));
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
                Assert.That(target.GameGetBoardData(OthelloBoardType.StringSequence), Is.EqualTo(multilist[t, index].Item1));
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

                Assert.That(strfliptokens, Is.EqualTo(flips[i]));

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

            Assert.That(target.GameMakeMove(7, 7, oPlayerA).Count, Is.EqualTo(0));
            Assert.That(target.GameMakeMove(1, 1, oPlayerA).Count, Is.EqualTo(0));
            Assert.That(target.GameMakeMove(2, 3, oPlayerA).Count, Is.EqualTo(0));
            Assert.That(target.GameMakeMove(7, 6, oPlayerA).Count, Is.EqualTo(0));

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

            Assert.That(target.GameGetBoardData(OthelloBoardType.StringSequence), Is.EqualTo(expect));
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

            Assert.That(fliptokens.FindIndex(a => a.X == 5 && a.Y == 4 && a.Token == OthelloBitType.White), Is.GreaterThanOrEqualTo(0));
            Assert.That(fliptokens.FindIndex(a => a.X == 4 && a.Y == 4 && a.Token == OthelloBitType.White), Is.GreaterThanOrEqualTo(0));
            Assert.That(fliptokens.FindIndex(a => a.X == 4 && a.Y == 3 && a.Token == OthelloBitType.White), Is.GreaterThanOrEqualTo(0));
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

            Assert.That(target.GameGetBoardData(OthelloBoardType.StringSequence), Is.EqualTo(Expect));

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

            Assert.That(target.GameGetBoardData(OthelloBoardType.StringSequence), Is.EqualTo(Expect));

        }

        //check swtiching start player works
        [TestCase]
        public static void CheckStartPlayerSwitch()
        {
            var oPlayerA = new OthelloGamePlayer(OthelloPlayerKind.White, "PlayerA");
            var oPlayerB = new OthelloGamePlayer(OthelloPlayerKind.Black, "PlayerB");
            var oGame = new OthelloGame(oPlayerA, oPlayerB, oPlayerB, false);

            var targetPlayer = oGame.GameUpdatePlayer();

            Assert.That(targetPlayer.PlayerKind, Is.EqualTo(oPlayerB.PlayerKind));
        }

        //checks game can calculate turn properly given state
        [TestCase]
        public static void CheckCalculateTurn()
        {
            var oPlayerA = new OthelloGamePlayer(OthelloPlayerKind.White, "PlayerA");
            var oPlayerB = new OthelloGamePlayer(OthelloPlayerKind.Black, "PlayerB");
            var target = new OthelloGame(oPlayerA, oPlayerB, oPlayerA);
         
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Before ExecuteMoves. Turn = {0}", target.GameUpdateTurn()));

            OthelloGamePlayer player = target.GameUpdatePlayer();
            target.GameMakeMove(5, 4, player);

            player = target.GameUpdatePlayer();
            target.GameMakeMove(5, 3, player);

            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "After Execute 2 Moves. Current Turn = {0}", target.GameUpdateTurn()));

            Assert.That(target.GameUpdateTurn(), Is.EqualTo(3));
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

            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Calculated Turn = {0}", target.GameUpdateTurn()));

            player = target.GameUpdatePlayer();
            target.GameMakeMove(0, 7, player);

            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Calculated Turn = {0}", target.GameUpdateTurn()));

            Assert.That(target.GameIsEndGame(), Is.EqualTo(true));

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

            Assert.That(target.GameIsEndGame(), Is.EqualTo(true));
        }


        [TestCase]
        public static void CheckAllowedPlayerMoveCount()
        {
            var oPlayerA = new OthelloGamePlayer(OthelloPlayerKind.White, "PlayerA");
            var oPlayerB = new OthelloGamePlayer(OthelloPlayerKind.Black, "PlayerB");
            var oGame = new OthelloGame(oPlayerA, oPlayerB, oPlayerA);

            OthelloGamePlayer player = oGame.GameUpdatePlayer();
            int targetcount = oGame.GameGetPlayerAllowedMoves(player).Count;


            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Validated and Set Player to Move = {0}, allowed moves count = {1}", player, targetcount));

            Assert.That(targetcount, Is.EqualTo(4));
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
            Assert.That(player.PlayerKind, Is.EqualTo(OthelloPlayerKind.Black));
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
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Validated and Set Player to Move = {0}, allowed moves count = {1}", oPlayerB.PlayerKind, target.GameGetPlayerAllowedMoves(oPlayerB).Count));

            OthelloGamePlayer targetPlayer = target.GameUpdatePlayer();

            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Validated and Set Player to Move = {0}, allowed moves count = {1}", targetPlayer.PlayerKind, target.GameGetPlayerAllowedMoves(targetPlayer).Count));

            Assert.That(targetPlayer.PlayerKind, Is.EqualTo(OthelloPlayerKind.White));
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

            Assert.That(target.GameGetBoardData(OthelloBoardType.StringSequence), Is.EqualTo(Expect));
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

            Assert.That(target_size, Is.EqualTo(Expect));

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

            Assert.That(target.GameGetBoardData(OthelloBoardType.StringSequence), Is.EqualTo(Expect));
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

            Assert.That(target.GameGetBoardData(OthelloBoardType.StringSequence), Is.EqualTo(Expect));
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

            Assert.That(target.GameGetBoardData(OthelloBoardType.StringSequence), Is.EqualTo(Expect));


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

            Assert.That(target.GameGetBoardData(OthelloBoardType.StringSequence), Is.EqualTo(Expect));



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

            Assert.That(target.GameGetBoardData(OthelloBoardType.StringSequence), Is.EqualTo(Expect));


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

            Assert.That(target.GameGetBoardData(OthelloBoardType.StringSequence), Is.EqualTo(Expect));
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

            Assert.That(player.PlayerKind, Is.EqualTo(Expect));
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

            Assert.That(target.GameGetScoreBlack(), Is.EqualTo(2));
            Assert.That(target.GameGetScoreWhite(), Is.EqualTo(7));
        }


        [TestCase(1, 0, OthelloBitType.Empty)]
        [TestCase(1,0, OthelloBitType.White)]
        [TestCase(0,0,OthelloBitType.Black)]
        public static void CheckOthelloTokenInstantiation(int x,int y, OthelloBitType type)
        {
            OthelloToken ot = new OthelloToken(x, y, type);

            Assert.That(ot.X, Is.EqualTo(x));
            Assert.That(ot.Y, Is.EqualTo(y));
            Assert.That(ot.Token, Is.EqualTo(type));
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
            Assert.That(oTarget, Is.Not.EqualTo(new OthelloToken(2, 5, OthelloBitType.Black)));
            Assert.That(oTarget, Is.Not.EqualTo(new OthelloToken(1, 4, OthelloBitType.Black)));
            Assert.That(oTarget, Is.Not.EqualTo(new OthelloToken(5, 2, OthelloBitType.Black)));
        }


        [TestCase(GameDifficultyMode.Default)]
        public static void CheckAIConfigLoaderSanity(GameDifficultyMode modes)
        {
            OthelloGamePlayer oPlayerA = new OthelloGamePlayer(OthelloPlayerKind.White, "Human");
            OthelloGamePlayer oPlayerB = new OthelloGamePlayer(OthelloPlayerKind.Black, "Computer");
            OthelloGame oGame = new OthelloGame(oPlayerA, oPlayerB, oPlayerA, false, true);

            int? depth;
            float? alpha, beta;
            int[] turn = new int[64];
            for (int i = 0; i < turn.Length; i++)
                turn[i] = i + 1;

            foreach(int n in turn)
            {
                OthelloGame.DebugGetTurnConfig(modes, n, oGame.DebugAIConfig(), out depth, out alpha, out beta);

                Assert.That(depth, Is.Not.EqualTo(null));
                Assert.That(alpha, Is.Not.EqualTo(null));
                Assert.That(beta, Is.Not.EqualTo(null));
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

            OthelloGame.GameDisableLog();
            target.GameSetBoardData(initboard);
            var oTarget = oAIPlayer.GetBestMove(oAIPlayer.AiPlayer, 5);

            target.GameAIMakeMove();

            string expect = (string)target.GameGetBoardData(OthelloBoardType.StringSequence);

            target.GameSave(false, customPath);
            target.GameCreateNew(oPlayerA, oPlayerB, oPlayerB);
            target.GameLoad(false, customPath);

            Assert.That(target.GameGetBoardData(OthelloBoardType.StringSequence), Is.EqualTo(expect));
        }


        #endregion
    }
}
