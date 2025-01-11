using System;
using System.IO;
using NUnit.Framework;
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
            target.GameCreateNewHumanVSAI(playerAName, playerBName);
            string json = target.GetGameJSON();
            target.GetGameFromJSON(json);
            var player = target.GameUpdatePlayer();

            var actual = OthelloPlayerKind.White;
            Assert.That(actual, Is.EqualTo(player.PlayerKind));
        }

        [TestCase]
        public static void TestJSONDeserializeInstantiateAndMakeMoveOthelloAIGame()
        {
            var target = new OthelloAdapter();
            target.GameCreateNewHumanVSAI(playerAName, playerBName);
            var player = target.GameUpdatePlayer();
            target.GameMakeMove(3,2, player, out bool IsInvalid);
            var expectedboard = target.GameDebugGetBoardInString();
            string json = target.GetGameJSON();
            var actual = new OthelloAdapter();
            actual.GetGameFromJSON(json);
            var actualboard = target.GameDebugGetBoardInString();
            Assert.That(actualboard, Is.EqualTo(expectedboard));
        }

        [TestCase]
        public static void TestSaveAndLoadGameInstantiateOthelloGame()
        {
            var savetarget = new OthelloAdapter();
            savetarget.GameCreateNewHumanVSAI(playerAName, playerBName);
            savetarget.GameSave();

            var loadtarget = new OthelloAdapter();
            loadtarget.GameLoad();
            var player = loadtarget.GameUpdatePlayer();
            var actual = OthelloPlayerKind.White;
            Assert.That(actual, Is.EqualTo(player.PlayerKind));
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
            savetarget.GameCreateNewHumanVSAI(playerAName, playerBName);
            savetarget.GameSetDifficultyMode(GameDifficultyMode.Hard);
            savetarget.GameSave();

            var loadtarget = new OthelloAdapter();
            loadtarget.GameLoad();
            var actual = loadtarget.GameGetDifficultyMode();
            Assert.That(actual, Is.EqualTo(GameDifficultyMode.Hard));
        }
    }
}