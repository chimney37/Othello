using System;
using System.IO;
using System.Collections.Generic;
using NUnit.Framework;
using System.Linq;
using Othello;

namespace OthelloAdapter
{
    [TestFixture]
    public class OthelloAdapterTest
    {
        #region CONSTANTS
        public const string playerAName = "playerA";
        public const string playerBName = "playerB";
        public const string testjsonblurb = "\"AAEAAAD/////AQAAAAAAAAAEAQAAABxTeXN0ZW0uQ29sbGVjdGlvbnMuQXJyYXlMaXN0AwAAAAZfaXRlbXMFX" +
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
        public static void TestJSONDeserializeInstantiateOthelloGame()
        {
            var target = new OthelloAdapter();

            string json = testjsonblurb;
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