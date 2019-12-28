using System;
using System.IO;
using NUnit.Framework;
using System.Diagnostics;
using System.Reflection;
using MethodTimer;

namespace Othello
{
    /// <summary>
    /// Perf Testing. For time and memory performance
    /// </summary>
    [TestFixture]
    class OthelloPerfTest
    {
        /// <summary>
        /// A logger class used to log the output of a method timer framework (fody)
        /// </summary>
        public static class MethodTimeLogger
        {
            /// <summary>
            ///  Do some logging here
            /// </summary>
            /// <param name="methodBase"></param>
            /// <param name="milliseconds"></param>
            public static void Log(MethodBase methodBase, long milliseconds)
            {
                Trace.WriteLine(string.Format("{0}: {1} ms", methodBase.Name, milliseconds));
                Console.WriteLine(string.Format("{0}: {1} ms", methodBase.Name, milliseconds));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var dir = Path.GetDirectoryName(typeof(OthelloTest).Assembly.Location);
            Environment.CurrentDirectory = dir;
        }

        #region PERFORMANCE and MEMORY TESTS

        [TestCase]
        public static void PerfSpeedMakeMove()
        {
            ExecuteLoopMakeMove();
            ExecuteLoopMakeMove();
        }

        [Time]
        private static void ExecuteLoopMakeMove()
        {
            var oPlayerA = new OthelloGamePlayer(OthelloPlayerKind.White, "PlayerA");
            var oPlayerB = new OthelloGamePlayer(OthelloPlayerKind.Black, "PlayerB");

            var target = new OthelloGame(oPlayerA, oPlayerB, oPlayerA);
            target.GameDisableLog();

            for (int i = 0; i < 100000; i++)
            {     
                target.GameMakeMove(5, 3, oPlayerA);
                target.GameMakeMove(5, 4, oPlayerA);
                target.GameUndo();
                target.GameUndo();
            }
        }

        [TestCase]
        public static void PerfSpeedIsValidMoves()
        {
            var oState = new OthelloState(new OthelloGamePlayer(OthelloPlayerKind.White, "PlayerA"), 1, false);

            OthelloLogger.Disable();

            ExecuteLoopIsValidMove(oState, oState.CurrentPlayer);
            ExecuteLoopIsValidMove(oState, oState.CurrentPlayer);
        }

        [Time]
        private static void ExecuteLoopIsValidMove(OthelloState s, OthelloGamePlayer p)
        {
            for (int i = 0; i < 100000; i++)
            {
                s.IsValidMove(5, 4, p);
            }
        }

        [TestCase]
        [Time]
        public static void PerfSpeedCalculateBoardScore()
        {
            var target = new OthelloBoard();

            for (int i = 0; i < 100000; i++)
            {
                target.GetTokenCount(OthelloBitType.Black);
                target.GetTokenCount(OthelloBitType.White);
                target.GetTokenCount(OthelloBitType.Empty);
            }

        }

        #endregion

    }
}
