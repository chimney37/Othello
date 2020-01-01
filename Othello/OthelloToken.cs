using System;
using System.Collections.Generic;
using System.Globalization;

namespace Othello
{
    /// <summary>
    /// Class used to represent a single othello token.
    /// An othello token can be defined using:
    ///     Token type : defined by OthelloBitType: Empty, Black, White, OOB (out-of-bounds
    ///     x : left to right (columns) -  x in Othello User view Coordinates has the coordinate system V(x,y)
    ///     y : top to bottom (rows) - y in Othello User view Coordinates has the coordinate system V(x,y)
    ///     
    /// Generally it is used as part of the TokenMatrix represented by:
    /// T(i,j) = V(x,y)
    /// </summary>
    [Serializable]
    public sealed class OthelloToken
    {
        #region PROPERTIES
        public int X { get; set; }
        public int Y { get; set; }
        public OthelloBitType Token { get; set; }
        #endregion

        #region CONSTRUCTOR

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="oBit"></param>
        public OthelloToken(int x, int y, OthelloBitType oBit)
        {
            this.X = x;
            this.Y = y;
            this.Token = oBit;
        }
        #endregion

        #region GETTERS

        /// <summary>
        /// Get the opposite token (white if black is set, or vice-versa)
        /// by design: all other token type : OOB, empty, would return Black
        /// </summary>
        /// <param name="ob"></param>
        /// <returns></returns>
        public static OthelloToken GetInverse(OthelloToken ob)
        {
            if (ob == null)
                throw new ArgumentNullException(nameof(ob));

            return new OthelloToken(ob.X,ob.Y,(ob.Token == OthelloBitType.Black) ? OthelloBitType.White : OthelloBitType.Black);
        }

        /// <summary>
        /// override method for string representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "[{0}:{1},{2}]",((OthelloBitType)Token).ToString(), X.ToString(CultureInfo.InvariantCulture), Y.ToString(CultureInfo.InvariantCulture));
        }
        #endregion
    }

    /// <summary>
    /// Equality comparer when used in a generic collection
    /// </summary>
    public class OthelloTokenEqualityComparer : IEqualityComparer<OthelloToken>
    {
        /// <summary>
        /// Equals operator to check if 2 tokens are identical in Othello Space. If either Tokens are null, return false
        /// </summary>
        /// <param name="oToken1"></param>
        /// <param name="oToken2"></param>
        /// <returns></returns>
        public bool Equals(OthelloToken oToken1, OthelloToken oToken2)
        {
            if(oToken1 != null && 
                oToken2 != null && 
                oToken1.Token == oToken2.Token &&
                oToken1.X == oToken2.X &&
                oToken1.Y == oToken2.Y)
                return true;

            return false;
        }

        /// <summary>
        /// Get a hashcode of a token. Assumed Unique given a token type, x and y
        /// </summary>
        /// <param name="oToken"></param>
        /// <returns></returns>
        public int GetHashCode(OthelloToken oToken)
        {
            OthelloExceptions.ThrowExceptionIfNull(oToken);

            int hCode = (int)oToken.Token ^ oToken.X ^ oToken.Y;
            return hCode.GetHashCode();
        }
    }
}
