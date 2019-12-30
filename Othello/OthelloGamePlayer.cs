using System;

namespace Othello
{
    /// <summary>
    /// Class to encapsulate player information for an Othello game.
    /// </summary>
    [Serializable]
    public class OthelloGamePlayer : OthelloPerson
    {
        #region PROPERTIES
        /// <summary>
        /// In Othello, either you are black, or white, as the PlayerKind
        /// </summary>
        public OthelloPlayerKind PlayerKind { get; set; }

        #endregion

        #region CONSTRUCTOR
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="oPKind"></param>
        /// <param name="PlayerName"></param>
        public OthelloGamePlayer(OthelloPlayerKind oPKind, string PlayerName)
        {
            this.PlayerKind = oPKind;
            this.PlayerName = PlayerName;
        }
        #endregion

        #region GETTERS
        /// <summary>
        /// Get the associated token type of a player (e.g. a player playing white tokens would get white)
        /// </summary>
        /// <returns></returns>
        public OthelloBitType GetPlayerOthelloToken()
        {
            return (this.PlayerKind == OthelloPlayerKind.Black) ?
                OthelloBitType.Black :
                OthelloBitType.White;
        }

        protected OthelloBitType GetBitType()
        {
            return this.GetPlayerOthelloToken();
        }

        protected OthelloBitType GetInverseBitType()
        {
            return this.GetPlayerOthelloToken() == OthelloBitType.White ? OthelloBitType.Black : OthelloBitType.White;
        }

        /// <summary>
        /// override method to return a string representation of a player
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.PlayerKind.ToString();
        }
        #endregion
    }
}
