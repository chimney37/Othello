using System;
using System.IO;
using System.Collections.Generic;
using NUnit.Framework;
using System.Linq;
using Othello;

namespace OthelloAdapters
{
    [TestFixture]
    public class OthelloAdapterTest
    {
        #region CONSTANTS
        public const string playerAName = "playerA";
        public const string playerBName = "playerB";
        #endregion

        /// <summary>
        /// Fix to change the test directory to the location of the assembly. Default is within VS's program directory
        /// </summary>
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var dir = Path.GetDirectoryName(typeof(OthelloTest).Assembly.Location);
            Environment.CurrentDirectory = dir;
        }

        [TestCase]
        public static void TestJSONDeserializeInstantiateOthelloAIGame()
        {
            var target = new OthelloAdapter();
            target.GameCreateNewHumanVSAI(playerAName, playerBName, OthelloPlayerKind.White);
            string json = target.GetGameJSON();
            target.GetGameFromJSON(json);
            var player = target.GameUpdatePlayer();

            var actual = OthelloPlayerKind.White;
            Assert.AreEqual(player.PlayerKind, actual);
        }

        [TestCase]
        public static void TestSaveAndLoadGameInstantiateOthelloGame()
        {
            var savetarget = new OthelloAdapter();
            savetarget.GameCreateNewHumanVSAI(playerAName, playerBName, OthelloPlayerKind.White);
            savetarget.GameSave();

            var loadtarget = new OthelloAdapter();
            loadtarget.GameLoad();
            var player = loadtarget.GameUpdatePlayer();
            var actual = OthelloPlayerKind.White;
            Assert.AreEqual(player.PlayerKind, actual);
        }


        [TestCase]
        public static void TestLoadGameInstantiateOthelloGame()
        {
            var target = new OthelloAdapter();

        }

        [TestCase]
        public static void CheckAISaveLoadGame()
        {
            var savetarget = new OthelloAdapter();
            savetarget.GameCreateNewHumanVSAI(playerAName, playerBName, OthelloPlayerKind.White);
            savetarget.GameSetDifficultyMode(GameDifficultyMode.Hard);
            savetarget.GameSave();

            var loadtarget = new OthelloAdapter();
            loadtarget.GameLoad();
            var actual = loadtarget.GameGetDifficultyMode();
            Assert.AreEqual(GameDifficultyMode.Hard, actual);
        }
    }
}