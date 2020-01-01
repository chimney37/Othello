using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Globalization;

namespace Othello
{
    /// <summary>
    /// Class representing the state of a board that encapsulates the game rules and mechanics of Othello.
    /// It contains primary functions such as the following:
    ///     Encapsulates the underlying Othello board data
    ///     Identify that the current player is a valid player
    ///     Calculate the turn of a Othello game given its state
    ///     Compute the token flips of a given token when placed on a board given its state
    ///     Compute the allowed moves of a player
    ///     Several helper methods to aid the mechanics of above
    /// </summary>
    [Serializable]
    public class OthelloState : IOthelloPrototypeState
    {
        #region PROPERTIES AND FIELDS
        public int ScoreW { get; set; }
        public int ScoreB { get; set; }
        public OthelloGamePlayer CurrentPlayer { get; set; }
        public int Turn { get; set; }

        private OthelloBoard BoardData;
        #endregion

        #region CONSTRUCTORS AND INTIALIZATION
        /// <summary>
        /// Primary constructor
        /// </summary>
        /// <param name="currentPlayer"></param>
        /// <param name="turn"></param>
        /// <param name="IsAlternate"></param>
        public OthelloState(OthelloGamePlayer currentPlayer, int turn, bool IsAlternate = false)
        {
            this.Initialize(currentPlayer,turn, IsAlternate);
        }

        /// <summary>
        /// Copy constructor
        /// 
        /// </summary>
        /// <param name="prevState"></param>
        protected OthelloState(OthelloState prevState)
        {
            OthelloExceptions.ThrowExceptionIfNull(prevState);

            //do a member wise bit copy of all fields
            this.ScoreW = prevState.ScoreW;
            this.ScoreB = prevState.ScoreB;

            //BoardData = new OthelloBoard(prevState.BoardData);
            BoardData = (OthelloBoard)prevState.BoardData.Clone();

            this.CurrentPlayer = prevState.CurrentPlayer;
            this.Turn = prevState.Turn;
        }

        /// <summary>
        /// initialization method
        /// </summary>
        /// <param name="currentPlayer"></param>
        /// <param name="turn"></param>
        /// <param name="IsAlternate"></param>
        private void Initialize(OthelloGamePlayer currentPlayer, int turn, bool IsAlternate = false)
        {
            //this.ChildIDs = new List<string>();
            this.BoardData = new OthelloBoard(IsAlternate);
            this.ScoreB = 2;
            this.ScoreW = 2;

            this.CurrentPlayer = currentPlayer;
            this.Turn = turn;
        }

        /// <summary>
        /// Clone member. Deep clone version using protected copy constructor
        /// http://developerscon.blogspot.jp/2008/06/c-object-clone-wars.html
        /// </summary>
        /// <returns></returns>
        public IOthelloPrototypeState Clone()
        {
            return new OthelloState(this);
        }

        #endregion

        #region STATE VALIDATORS, GETTERS AND SETTERS

        /// <summary>
        /// validate player to check that player has valid moves. also returns the current playable player
        /// this is often used when loading a game state uncontrolled by OthelloGame
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool IsValidPlayer(OthelloGamePlayer player)
        {
            if (!this.GetAllowedMoves(player).Any())
            {
                return false;
            }
            return true ;
        }

        /// <summary>
        /// Calculate the turn of a Othello State
        /// </summary>
        /// <returns></returns>
        public int CalculateTurn()
        {
            //initial state of board has 4 tokens and turn equals 1, so (4-1) must be subtracted
            return GetBoardBlackCount() + GetBoardWhiteCount() - 3;
        }

        /// <summary>
        /// Get the counts of white tokens
        /// </summary>
        /// <returns></returns>
        public int GetBoardWhiteCount()
        {
            return this.BoardData.GetTokenCount(OthelloBitType.White);
        }

        /// <summary>
        /// Gets the count of black tokens
        /// </summary>
        /// <returns></returns>
        public int GetBoardBlackCount()
        {
            return this.BoardData.GetTokenCount(OthelloBitType.Black);
        }

        public int GetBoardCount(OthelloBitType t)
        {
            if (t == OthelloBitType.Black)
            {
                return GetBoardBlackCount();
            }
            else
            {
                return GetBoardWhiteCount();
            }
        }

        /// <summary>
        /// get the underlying board data
        /// </summary>
        /// <returns></returns>
        public OthelloBoard GetBoardData()
        {
            return BoardData;
        }

        /// <summary>
        /// set the board data given a charBoard
        /// </summary>
        /// <param name="charBoard"></param>
        public void SetBoardData(char[,] charBoard)
        {
            BoardData.SetBoardData(charBoard);
        }

        /// <summary>
        /// set the board data given a tokenBoard
        /// </summary>
        /// <param name="tokenBoard"></param>
        public void SetBoardData(OthelloToken[,] tokenBoard)
        {
            BoardData.SetBoardData(tokenBoard);
        }

        /// <summary>
        /// sets the board data given a stringBoard
        /// </summary>
        /// <param name="stringBoard"></param>
        public void SetBoardData(string stringBoard)
        {
            BoardData.SetBoardData(stringBoard);
        }


        /// <summary>
        /// evalute move validity given a player
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool IsValidMove(int x, int y, OthelloGamePlayer player)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));

            //get the token at the cell where a player is trying to place
            OthelloToken obTry = this.BoardData.GetCell(x, y);

            //invalid if its not emtpy OR out of bounds
            if (obTry.Token != OthelloBitType.Empty ||
                obTry.Token == OthelloBitType.OOB)
            {
#if DEBUG
                Debug.WriteLine("IsValidMove:(FAIL) Invalid as cell not empty or OOB.");
#endif
                return false;
            }

            OthelloToken ob = new OthelloToken(x, y, player.GetPlayerOthelloToken());
            OthelloToken obOpposition = OthelloToken.GetInverse(ob);

            //check 8 paths: if no opposing token in between to flip
            if (IsNoOpposingTokenToFlip(ob, obOpposition))
            {
#if DEBUG
                Debug.WriteLine("IsValidMove:(FAIL) no opposing token in between to flip");
#endif
                return false;
            }

#if DEBUG
            Debug.WriteLine("IsValidMove: Is Valid Move.");
#endif
            return true;
        }

        #endregion

        #region QUERYABLES


        /// <summary>
        /// get a total list of tokens to flip given a placement token at x,y
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public List<OthelloToken> GetAllFlipsTokens(int x, int y, OthelloGamePlayer player)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));

            OthelloToken ob = new OthelloToken(x, y, player.GetPlayerOthelloToken());
            OthelloToken obOpposition = OthelloToken.GetInverse(ob);

            List<OthelloToken> FlipTokens = new List<OthelloToken>();

            List<List<OthelloToken>> paths = GetAllDirectionalPaths(ob);
            paths = GetValidPaths(ob, obOpposition, paths);

            //Get opposing bits in each path and reverse
            for (var index = 0; index < paths.Count; index++)
            {
                List<OthelloToken> path = paths[index];

                //note: changed Concat to AddRange for better performance
                FlipTokens.AddRange(GetFlipTokenFromPath(ob, path));
            }

            return FlipTokens;
        }

        /// <summary>
        /// get a list of token w/ coordinates from a given path to flip
        /// </summary>
        /// <param name="ob"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<OthelloToken> GetFlipTokenFromPath(OthelloToken ob, List<OthelloToken> path)
        {
            if (ob == null)
                throw new ArgumentNullException(nameof(ob));
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            List<OthelloToken> FlipTokens = new List<OthelloToken>();

            //search start index is always 0 (by design)
            //get index of any empty that is bigger than the startindex
            int indexmidEmpty = path.FindIndex(1, a => a.Token == OthelloBitType.Empty);

            //get end index (of the same token type) as the one at origin along path
            int indexendOb = path.FindIndex(1, a => a.Token == ob.Token);

            //Check: if any empty token index that is in between start and end index            
            if (indexmidEmpty > 0 && indexmidEmpty < indexendOb)
            {
                return FlipTokens;
            }

            //check end index is 2 or bigger
            if (indexendOb > 1)
            {
                FlipTokens = path.GetRange(1, indexendOb - 1);
            }
            return FlipTokens;
        }

        /// <summary>
        /// get a list of available moves on the board given the current state
        /// Maybe can be further improved by keeping a list of placable positions and updating it as game progresses. Don't need to search entire board from placement?
        /// However, this approach seems complicated as we need to keep updating this list of placable positions which seems on the same order as searching the entire board.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public List<OthelloToken> GetAllowedMoves(OthelloGamePlayer player)
        {
            List<OthelloToken> moves = new List<OthelloToken>();

            //new routine to use iterator instead of traditional for loop
            OthelloBoardIterator oIter = BoardData.CreateIterator();

            while (oIter.MoveNext())
            {
                if (IsValidMove(oIter.Current.X, oIter.Current.Y, player))
                    moves.Add(oIter.Current);
            }

            return moves;
        }
        #endregion

        #region HELPERS QUERYABLES
  
        /// <summary>
        /// get a path of othello tokens starting from the placement bit all the way until edge of board or empty token is encountered   
        /// </summary>
        /// <param name="placementob"></param>
        /// <param name="odir"></param>
        /// <returns></returns>
        protected List<OthelloToken> GetPath(OthelloToken placementob, OthelloDirection odir)
        {
            if (placementob == null)
                throw new ArgumentNullException(nameof(placementob));

            //initialize new token at origin of path
            List<OthelloToken> path = new List<OthelloToken>();

            //using a temp variable is faster than using the variable given to the stack
            OthelloToken ob = placementob;
                   
            //note: add OthelloBitType Empty checks b/c we don't need a path for these either
            //adding this additional condition reduced times for IsValidMove from about 515ms to 275 ms (87% faster)
            do
            {
                path.Add(ob);
                ob = this.BoardData.GetAdjacentCell(ob.X, ob.Y, odir);
            } while (ob.Token != OthelloBitType.OOB &&
                ob.Token != OthelloBitType.Empty);

            return path;
        }

        /// <summary>
        /// get paths in all direction: regardless of validness
        /// </summary>
        /// <param name="ob"></param>
        /// <returns></returns>
        protected List<List<OthelloToken>> GetAllDirectionalPaths(OthelloToken ob)
        {
            List<List<OthelloToken>> paths = new List<List<OthelloToken>>();

            paths.Add(GetPath(ob, OthelloDirection.Deg0));
            paths.Add(GetPath(ob, OthelloDirection.Deg45));
            paths.Add(GetPath(ob, OthelloDirection.Deg90));
            paths.Add(GetPath(ob, OthelloDirection.Deg135));
            paths.Add(GetPath(ob, OthelloDirection.Deg180));
            paths.Add(GetPath(ob, OthelloDirection.Deg225));
            paths.Add(GetPath(ob, OthelloDirection.Deg270));
            paths.Add(GetPath(ob, OthelloDirection.Deg315));
            return paths;
        }

        /// <summary>
        /// get a list of valid paths. only take paths that has tokens to flip
        /// </summary>
        /// <param name="ob"></param>
        /// <param name="obOpposition"></param>
        /// <param name="paths"></param>
        /// <returns></returns>
        protected static List<List<OthelloToken>> GetValidPaths(OthelloToken ob, OthelloToken obOpposition, List<List<OthelloToken>> paths)
        {
            if (ob == null)
                throw new ArgumentNullException(nameof(ob));
            if (obOpposition == null)
                throw new ArgumentNullException(nameof(obOpposition));
            if (paths == null)
                throw new ArgumentNullException(nameof(paths));

            List<List<OthelloToken>> validPaths = new List<List<OthelloToken>>();

            //check if at least 1 opposing color and between 2 of the color by ob (player)
            // w -> b -> b -> w : valid
            // w -> w : invalid
            // w -> b -> e : invalid
            // w : invalid
            // w -> b -> b -> e -> w : invalid

            for (int i = 0; i < paths.Count; i++)
            {            
                //Case 1: Path length is 1 or less (meaning it's an alone token)
                    //optmization: if either of this condition fufills, it's not valid, so continue the loop
                    //using this continues improve IsValidMove from about 270ms down to about 248ms (9% faster)
                    //we also don't need IsPathInvalid, as this is part of the sandwich test. further down to about 219ms (23% faster).
                //Case 2: Check for invalidity : a sandwitch is like "bwwb": true. "bw":false "bwe": false, "bweb": false
                if (paths[i].Count <= 1 || 
                    //IsPathInvalid(ob, paths[i]) ||
                    IsNotValidSandwich(ob, obOpposition, paths[i]))
                    continue;

                validPaths.Add(paths[i]);
            }

            return validPaths;
        }


        #endregion

        #region HELPER VALIDATORS
        /// <summary>
        /// check if no opposing tokens in path (no sandwiching)
        /// </summary>
        /// <param name="ob"></param>
        /// <param name="obOpposition"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        protected static bool IsNotValidSandwich(OthelloToken ob, OthelloToken obOpposition, List<OthelloToken> path)
        {
            if (ob == null)
                throw new ArgumentNullException(nameof(ob));
            if (obOpposition == null)
                throw new ArgumentNullException(nameof(obOpposition));
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            //index of origin token is 0 (it's 0 by design)
            //find index of the end token that encloses a valid section
            int obIndexEnd = path.FindIndex(1, a => a.Token == ob.Token);

            //get index of any empty that is bigger than the startindex
            int indexmidEmpty = path.FindIndex(1, a => a.Token == OthelloBitType.Empty);

            //find the index of the opposite token in between the origin and end token
            int oboppIndex = path.FindIndex(1, a => a.Token == obOpposition.Token);

            //Case 1: if there is no end index  (meaning it can be the same one as origin token (it won't be found)
            //Case 2: if the opposite token index is not found, there is no sandwich
            //Case 3: if the opposite token index is not in between opindex start and end, no sandwich
            //Case 4: if there is an empty cell in between start and end, we don't have a valid sandwich
            if (obIndexEnd < 0 ||
                oboppIndex < 0 ||
                oboppIndex > obIndexEnd ||
                (indexmidEmpty >= 0 && indexmidEmpty <= obIndexEnd))
                return true;
            
            return false;
        }


        /// <summary>
        /// check if there is no opposing tokens to flip given an origin token and an opposing token
        /// </summary>
        /// <param name="ob"></param>
        /// <param name="obOpposition"></param>
        /// <returns></returns>
        protected bool IsNoOpposingTokenToFlip(OthelloToken ob, OthelloToken obOpposition)
        {
            List<List<OthelloToken>> paths = GetAllDirectionalPaths(ob);

            paths = GetValidPaths(ob, obOpposition, paths);

            //if no valid paths, there is no opposing token to flip
            if (paths.Count == 0)
                return true;
            
            return false;
        }
        #endregion

        #region DEBUG OUTPUT
        public void DebugState()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Hash: {0}\t{1}\t{2}\n", this.BoardData.HashID, this.ScoreW, this.ScoreB));
        }
        protected static string DebugPath(List<OthelloToken> path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            StringBuilder sb = new StringBuilder();
            foreach (OthelloToken obit in path)
                sb.Append(string.Format(CultureInfo.InvariantCulture, "[{0}({1},{2})]", obit.Token, obit.X, obit.Y));          
            sb.Append("\n");
            return sb.ToString();
        }
        #endregion
    }
}
