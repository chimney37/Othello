//#define SINGLE_THREAD_SEARCH

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using MethodTimer;

namespace Othello
{
    //TODO: support fetching of pre-computed moves to speed up searches
    /// <summary>
    /// The concrete class that implements the concrete product, A.I for Othello, instatiated via the concrete implementation OthelloAIFactory
    /// </summary>
    [Serializable]
    public class OthelloGameAiSystem : OthelloGameAISystemProduct
    {
        #region PROPERTIES AND FIELDS
        OthelloState _currentState;
        public OthelloGamePlayer HumanPlayer {get;set;}
        public OthelloGamePlayer AiPlayer { get; set; }

        private List<OthelloToken> _maxPivotHeuristics;
        private List<OthelloToken> _majorPivotsHeuristics;

        readonly string[] _maxPivots = { "0,0", "0,7", "7,0", "7,7" };
        readonly string[] _majorPivots = {"1,1","1,6","6,1","6,6","0,1","0,6","1,0","1,7","6,0","6,7","7,1","7,6"};

        static readonly Comparison<Tuple<OthelloToken, float>> Comparison = ((a, b) =>
            b.Item2.CompareTo(a.Item2));
        #endregion

        #region CONSTRUCTORS INITIALIZERS

        /// <summary>
        /// 
        /// Constructor that can only be accessed from the current assembly.
        /// This is intended to be instantiated from a factory, thus intended to be called from only within this library
        /// http://msdn.microsoft.com/en-us/library/7c5ka91b.aspx
        /// 
        /// </summary>
        /// <param name="oGame"></param>
        /// <param name="aIplayer"></param>
        /// <param name="humanPlayer"></param>
        internal OthelloGameAiSystem(OthelloGame oGame, OthelloGamePlayer aIplayer, OthelloGamePlayer humanPlayer)
        {
            Initialize(oGame, aIplayer, humanPlayer);
        }

        public void Initialize(OthelloGame oGame, OthelloGamePlayer aIplayer, OthelloGamePlayer humanPlayer)
        {
            _currentState = (OthelloState)((IOthelloGameAiAccessor)oGame).GetCurrentState().Clone();
            this.HumanPlayer = humanPlayer;
            this.AiPlayer = aIplayer;

            _maxPivotHeuristics = new List<OthelloToken>();
            _majorPivotsHeuristics = new List<OthelloToken>();

            AddHeuristics(_maxPivotHeuristics, _maxPivots);
            AddHeuristics(_majorPivotsHeuristics, _majorPivots);
        }

        private void AddHeuristics(List<OthelloToken> list, string[] values)
        {
            foreach (string s in values)
                list.Add(new OthelloToken(Convert.ToInt32(s.Split(',')[0]), Convert.ToInt32(s.Split(',')[1]), OthelloBitType.White));
        }
        #endregion

        #region GETTERS AND SETTERS
        public void SetCurrentState(OthelloState oState)
        {
            _currentState = (OthelloState)oState.Clone();
        }

        #endregion 

        #region MAIN AI ROUTINES
        /// <summary>
        /// gets a random move when evaluation score turns out the same.
        /// </summary>
        /// <param name="currentPlayer"></param>
        /// <param name="remainDepth"></param>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <returns></returns>
        public override OthelloToken GetBestMove(OthelloGamePlayer currentPlayer, int remainDepth, float alpha = 0f, float beta = 0.51f)
        {

            //get a list of moves
            //TODO: implement a multiple key mapper of _currentState, alpha and beta values that can fetch a pre-computed version of oCalculatedMoves

            List<Tuple<OthelloToken, float>> calculatedMoves = GetMoves(currentPlayer, remainDepth, alpha, beta);
            //TODO: store this oCalculatedMoves if we can, epresented by a unique map of tegether with _currentState, alpha and beta values


            //handle the case where Calculated moves is 0. or else index woul be -1 and would cause exception
            if (calculatedMoves.Count == 0)
                return null;

            //determine max score
            float maxScore = calculatedMoves[0].Item2;

            //debug each score
            foreach (Tuple<OthelloToken, float> t in calculatedMoves)
            {
#if TRACE
                Trace.WriteLine(string.Format("[GetBestMove]({0},{1}),S={2}", t.Item1.X, t.Item1.Y, t.Item2));
#endif
            }

            //get list of max scores
            List<Tuple<OthelloToken, float>> maxScorelist = calculatedMoves.FindAll(a => a.Item2 == maxScore);

            //choose out of random
            Random rand = new Random((int)DateTime.Now.ToBinary());
            int index = rand.Next(maxScorelist.Count - 1);

            return maxScorelist[index].Item1;
        }

        /// <summary>
        /// GetMoves a list of evaluated moves given a current player
        /// </summary>
        /// <param name="currentPlayer"></param>
        /// <param name="remainDepth"></param>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <returns></returns>
        /// 
        /// TODO: optimize using Iterative Deepening Depth-First-Search (IDDFS) as part of AlphaBeta.
        // TODO: expose time limit variable to the class so we can set a limit when instantiating the game
        [Time]
        private List<Tuple<OthelloToken, float>> GetMoves(OthelloGamePlayer currentPlayer, int remainDepth, float alpha =0f, float beta = 0.51f)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            int totalmoves = 0;
            List<OthelloToken> allowedMoves = _currentState.GetAllowedMoves(currentPlayer);
     
            List<Tuple<OthelloToken, float>> oCalculatedMoves = new List<Tuple<OthelloToken, float>>();

            //since t is each a distinct and independent search, multithread this, but keep the original single threaded code for baseline purpose.

#if (SINGLE_THREAD_SEARCH)
           Trace.WriteLine("Single-Threaded Search.");
           foreach(OthelloToken t in allowedMoves)
            {
                OthelloState oNextState = GetNextState(t, CurrentState, currentPlayer);
                //movescore = Minimax(oNextState.CurrentPlayer, oNextState, remainDepth, ref totalmoves);
                movescore = AlphaBeta(oNextState.CurrentPlayer, oNextState, alpha, beta, remainDepth, ref totalmoves);
                bestValue = Math.Max(bestValue, movescore);

                Tuple<OthelloToken, float> scoretuple = new Tuple<OthelloToken, float>(t, movescore);
                oCalculatedMoves.Add(scoretuple);
            } 
#else
            //TODO: implement 1) compare current state to see if this can be loaded from a previous computation


            //for creating a monitor
            Trace.WriteLine("Parallel Search.");
            var syncObject = new object();

            //Multi-threaded approach. May speed things up a lot. For example, the first move (3,5) by human would take this particular machine
            //to calculate 19539 moves for a particular depth. In the single threaded implementation, this took 4170msec.
            //In the multi-threaded implementation this took 2556msec.
            //Similarly, 408408 moves for a particular state (single threaded : 80612msec, multithreaded : 53033msec)
            //Not robustly tested, it seems to give around ~61%,65% of the original time, approx a 130%+ speed increase even for a lower bound. 
            //Number of moves calculation and move score has been made local variables (thread Local variables as according to MSDN documentation:
            //http://msdn.microsoft.com/en-us/library/dd460713(v=vs.110).aspx
            //in this case the local variables and its assignments are thread safe because each local variable is a unique memory location.
            //scoretuple is a local variable that is thread-safe.
            //assignment to totalmoves is not thread safe. adding to a list is also not thread-safe. These are locked.
            // removing the locks as much as possible is especially performance improving in large processing (Alpha-Beta) generating many moves.
            // making local variables inside of Parallel instead of outside had basically improved speed by 8% observed in one case.
            Parallel.ForEach(allowedMoves, (t, state) =>
            {
                OthelloState oNextState = GetNextState(t, _currentState, currentPlayer);
                int move = 0;
#if (USE_MIN_MAX)
                float movescore = Minimax(oNextState.CurrentPlayer, oNextState, remainDepth, ref totalmoves);
#else
                float movescore = AlphaBeta(oNextState.CurrentPlayer, oNextState, alpha, beta, remainDepth, ref move);
#endif

                Tuple<OthelloToken, float> scoretuple = new Tuple<OthelloToken, float>(t, movescore);

                //critical section : multithreaded access
                lock(syncObject)
                {
                    totalmoves += move;
                    oCalculatedMoves.Add(scoretuple);

                    if (stopWatch.Elapsed.TotalSeconds > 5)
                    {
                        Trace.WriteLine("Breaking loop after 5 seconds");

                        // See this article about trying to stop a parallel loop
                        // https://stackoverflow.com/questions/14720014/immediately-exit-a-parallel-for-loop-in-c-sharp
                        state.Stop();
                    }
                }
            });
#endif

            oCalculatedMoves.Sort(Comparison);
         
            Trace.WriteLine(string.Format("ComputedMoveCount={0}", totalmoves));
            return oCalculatedMoves;
        }

#if (USE_MIN_MAX)
        /// <summary>
        /// Mini-max or min-max implementation. Attempts to minimize the maximum loss assuming player is perfect in their moves.
        /// At the base condition (at the leaf node), is heuristic evaluation function CalculateStateSCore returns the score of the state in the perspective of the mazimizing player (A.I.)
        /// 
        /// At each level, depending on whether the turn is by the AI or the human, the maximizing and minimizing score is returned, correspondingly.
        /// If it's the maximizing player's turn, the maximum value will be returned.
        /// If it's the minimizing player's turn, the minimum value will be returned. (From the minimum player perspective, it is trying to maximize its score actually)
        /// http://en.wikipedia.org/wiki/Minimax
        /// At the root, whre it chooses to move with the largest score, this is the move that the computer should make to minimize the chances of the human winning
        /// and correspondingly, maximizing its own chances to win. It's actually the maximum of the minimum score as a result that the opponent (human) might make..
        /// </summary>
        /// <param name="currentPlayer"></param>
        /// <param name="currentState"></param>
        /// <param name="remainDepth"></param>
        /// <param name="totalmoves"></param>
        /// <returns></returns>
        private float Minimax(OthelloPlayer currentPlayer, OthelloState currentState, int remainDepth, ref int totalmoves)
        {
            totalmoves++;
            float movescore = 0;
            List<OthelloToken> allowedMoves = currentState.GetAllowedMoves(currentPlayer);

            //Heuristic evaluation
            if( remainDepth == 0 || allowedMoves.Count() == 0)
            {
                return CalcuateStateScore(currentState);
            }

            //if MAX player, return the result which has the best score out of all moves. 
            //This score is the maximizing score of the MAX player
            // This is the player running the algorithm which is the A.I.
            if(currentPlayer == AiPlayer)
            {
                float bestValue = -100.0f;
                foreach (OthelloToken t in allowedMoves)
                {
                    OthelloState oNextState = GetNextState(t, currentState, currentPlayer);
                    movescore = Minimax(oNextState.CurrentPlayer, oNextState, remainDepth - 1, ref totalmoves);
                    bestValue = Math.Max(bestValue, movescore);
                }

                return bestValue;
            }
            // MIN player, return the result which has the worst score out of all moves.
            // THis score is the minizing score of the minmizing plaer, which is the human player in this case.
            else
            {
                float bestValue = 100.0f;
                foreach (OthelloToken t in allowedMoves)
                {
                    OthelloState oNextState = GetNextState(t, currentState, currentPlayer);
                    movescore = Minimax(oNextState.CurrentPlayer, oNextState, remainDepth - 1, ref totalmoves);
                    bestValue = Math.Min(bestValue, movescore);
                }

                return bestValue;
            }
        }
#endif
     
        /// <summary>
        /// An algorithm that seeks to decrease the number of nodes that are evaluated by the minimax algorithm.
        /// http://en.wikipedia.org/wiki/Alpha%E2%80%93beta_pruning
        /// It attempts to prune away branches that cannot possibly influence the final decision.
        /// It stops completely evaluating a move when at least one possibility has been found to prove the move to be worse than the previous examined move.
        /// The algorithm maintains 2 values alpha and beta. 
        /// alpha: represents the max score that maximizing player ia assured of. E.g. a = 0.
        /// beta: the minimum score that the minimizing player is assured of. E.g. b = 0.5.
        /// 
        /// In this code, alpha is set to a relatively small value and beta to a relatively large value. Both players start with lowest possible score.
        /// It can happen that when choosing a certain branch for minimum player, the minimizing score can fall lower than the maximizing score assured for maximum player (beta <= alpha)
        /// E.g., it finds that one of the nodes during a minimizing search in a certain branch produces beta is -5 while the parent holds an alpha value 0.
        /// In such case, the parent node will assume this is the worst possible node and other nodes need not be searched because that is assumed to be equally as bad and need not be searched.
        /// beta then becomes the minimum score that minimizing player is assured of at that level.
        /// 
        /// Similary, when choosing a certain branch for maximum player, the maximizing score can be larger than the maximizing score assured for minimizing player (alpha >= beta)
        /// E.g. finds alpha is 0.6 while beta holds a value 0.5.
        /// In such case, parent node will assume this is the best possible node and other nodes need not be searched as it "satifies" the best condition. Other nodes are assumed
        /// equally as good or less. 
        /// alpha then becomes the maximizing score that the maximizing player is assured of at that level.
        /// 
        /// Tuning the alpha beta values is important as it determines the cut-off point for stopping search. 
        /// If the alpha beta values do not describe well the maximum or minimizing score, it can result in important nodes not being searched.
        /// 
        /// Also, it is better that the ordering of search is tuned to the more "promising branches", that looks to exceed the range of the a and b values. 
        /// example being that if a = 0, branches that should be searched is likely to give b something less than 0.
        /// By doing that, we can find cut-off points quicker, saving time for deeper search of the promising branches.
        /// TODO: try to order the search to try to find the more "promising branches" that gives us a cut-off quicker, using some ordering hints. E.g. choose a move that allows maximizing player, the A.I to calculate score for the major pivots first
        /// </summary>
        /// <param name="currentPlayer"></param>
        /// <param name="currentState"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="remainDepth"></param>
        /// <param name="totalmoves"></param>
        /// <returns></returns>
        private float AlphaBeta(OthelloGamePlayer currentPlayer, OthelloState currentState, float a, float b, int remainDepth, ref int totalmoves)
        {

            //lock (syncObj)
            totalmoves++;

            List<OthelloToken> allowedMoves = currentState.GetAllowedMoves(currentPlayer);

            //Calculate the heuristic score of the base state
            if (remainDepth == 0 || !allowedMoves.Any())
                return CalcuateStateScore(currentState);           

            //MAX : maximizing the current player (A>I.)
            if (currentPlayer == AiPlayer)
            {
                foreach (OthelloToken t in allowedMoves)
                {
                    OthelloState oNextState = GetNextState(t, currentState, currentPlayer);
                    a = Math.Max(a, AlphaBeta(oNextState.CurrentPlayer, oNextState,a,b, remainDepth - 1, ref totalmoves));
                    if( b <= a)
                        break;                
                }

                return a;
            }
            // MIN player (the human)
            else
            {
                foreach (OthelloToken t in allowedMoves)
                {
                    OthelloState oNextState = GetNextState(t, currentState, currentPlayer);
                    b = Math.Min(b, AlphaBeta(oNextState.CurrentPlayer, oNextState, a, b, remainDepth - 1, ref totalmoves));
                    if (b <= a)
                        break;                  
                }

                return b;
            }
        }
        #endregion

        #region EVALUATION FUNCS, MISCELLENEOUS AND HELPERS

        /// <summary>
        /// Get AI Player name
        /// </summary>
        /// <returns></returns>
        public override string GetAIPlayerName()
        {
            return this.AiPlayer.PlayerName;
        }


        //evaluation function of a given state. Used in conjunction with mini-max/alpha-beta pruning
        private float CalcuateStateScore(OthelloState oState)
        {
            //calculate basic score for this move given perspective of the A.I
            float score = oState.GetBoardCount(AiPlayer.GetPlayerOthelloToken()) / (float)oState.Turn;

            //if this is end game, boost or negate scores accordingly
            if(oState.Turn == 61)
            {
                if (oState.CurrentPlayer.PlayerKind == AiPlayer.PlayerKind &&
                    score > 0.5)
                    score += 10.0f;
                else
                    score -= 10.0f;
            }

            return score + GetHeuristicScore(oState);
        }

        //used by the evaluation function to get a better evaluation of the state of the otello, from experience.
        private float GetHeuristicScore(OthelloState oState)
        {
            float score = 0.0f;

            //adjust score on max pivots. These change the flow of game alot
            foreach (OthelloToken t in _maxPivotHeuristics)
            {
                if (oState.GetBoardData().GetCell(t.X, t.Y).Token == AiPlayer.GetPlayerOthelloToken())
                    score += 2.009f;
                else if (oState.GetBoardData().GetCell(t.X, t.Y).Token == HumanPlayer.GetPlayerOthelloToken())
                    score += -2.009f;
            }

            foreach (OthelloToken t in _majorPivotsHeuristics)
            {
                if (oState.GetBoardData().GetCell(t.X, t.Y).Token == AiPlayer.GetPlayerOthelloToken())
                    score += -1.004f;
            }

            return score;
        }

        //create the next state given a game
        private OthelloState GetNextState(OthelloToken t, OthelloState currentState, OthelloGamePlayer player)
        {
            //create deep copy of current state
            OthelloState oNextState = (OthelloState)currentState.Clone();

            //get a list of tokens to flip given a move at x,y
            List<OthelloToken> fliplist = oNextState.GetAllFlipsTokens(t.X, t.Y, player);

            //get OthelloBitType
            OthelloBitType bt = player.GetPlayerOthelloToken();

            //Set the cell where player placed it
            oNextState.GetBoardData().SetCell(bt, t.X, t.Y);

            //Set the cells for all cell tokens that need flipping
            foreach (OthelloToken token in fliplist)
                oNextState.GetBoardData().SetCell(bt, token.X, token.Y);

            //validate and switch players
            oNextState.CurrentPlayer = UpdatePlayer(oNextState, HumanPlayer, player, AiPlayer);

            //go to next turn
            oNextState.Turn++;

            return oNextState;
        }

        //update a player to the next player given a state and player
        private static OthelloGamePlayer UpdatePlayer(OthelloState state, OthelloGamePlayer humanPlayer, OthelloGamePlayer player, OthelloGamePlayer AIPlayer)
        {
            OthelloGamePlayer updatedplayer = player.PlayerKind == AIPlayer.PlayerKind ? humanPlayer : AIPlayer;

            if (!state.IsValidPlayer(updatedplayer))
                updatedplayer = updatedplayer.PlayerKind == AIPlayer.PlayerKind ? humanPlayer : AIPlayer;
            
            return updatedplayer;
        }


        #endregion

        #region INTERFACES
        /// <summary>
        /// This is a way to overcome the friends problem in C#.
        /// Below is an interface that will be accessible within this class or its inherited classes
        /// </summary>
        protected internal interface IOthelloGameAiAccessor
        {
            OthelloState GetCurrentState();
        }
        #endregion
    }
}
