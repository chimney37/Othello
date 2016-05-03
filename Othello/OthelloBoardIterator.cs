using System;
using System.Collections;

namespace Othello
{
    /// <summary>
    /// The concrete version of the IEnumerator 
    /// scans through the board coordinates x,y where (x,y) = (0,0), (1,0),...,(0,1),...,(1,0),...,(7,0),...,(7,7)
    /// </summary>
    public sealed class OthelloBoardIterator : IEnumerator
    {
        #region ATTRIBUTES AND PROPERTIES
        private OthelloBoard oBoard;
        private int _xcurrent = 0;
        private int _ycurrent = 0;
        private int oBoardSizeModulo;

        /// <summary>
        /// Determines if there is a next value to be returned
        /// </summary>
        private bool hasNext
        {
            get { return !(_xcurrent >= oBoardSizeModulo * oBoardSizeModulo); }
        }

        /// <summary>
        /// 
        /// </summary>
        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        /// <summary>
        /// Returns the current item in the iterator
        /// </summary>
        public OthelloToken Current
        {
            get 
            {
                if (!hasNext)
                    throw new InvalidOperationException();

                return oBoard.GetCell(_xcurrent % oBoardSizeModulo, _ycurrent);
            }
        }
        #endregion

        #region CONSTRUCTOR
        /// <summary>
        /// Iterator Constructor
        /// </summary>
        /// <param name="b"></param>
        public OthelloBoardIterator(OthelloBoard b)
        {
            this.oBoard = b;
            this.oBoardSizeModulo = OthelloBoard.BoardSize;
        }

        #endregion

        #region MEMBERS
        /// <summary>
        /// Moves the Enumerator to the next element, returning true if more OthelloTokens are available
        /// </summary>
        /// <returns>bool </returns>
        public bool MoveNext()
        {
            _ycurrent = ((_xcurrent + 1) % oBoardSizeModulo == 0) ? _ycurrent + 1 : _ycurrent;
            _xcurrent = ++_xcurrent;

            return hasNext;
        }

        /// <summary>
        /// Reset Enumerator
        /// </summary>
        public void Reset()
        {
            _xcurrent = _ycurrent = 0;
        }

        #endregion
    }
}
