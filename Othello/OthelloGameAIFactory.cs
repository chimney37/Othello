using System;
using System.Collections;

namespace Othello
{
    /// <summary>
    /// The concrete creator implementing the sub routines of the creator framework that creates the product (the othello AI)
    /// </summary>
    [Serializable]
    public class OthelloGameAIFactory : OthelloFactory
    {
        private ArrayList aiplayers = new ArrayList();

        public ArrayList AIPlayers
        {
            get { return aiplayers; }
        }

        protected override OthelloGameAISystemProduct CreateProduct(OthelloGame oGame, OthelloGamePlayer AIplayer, OthelloGamePlayer humanPlayer)
        {
            return new OthelloGameAiSystem(oGame,AIplayer,humanPlayer);
        }

        protected override void RegisterProduct(OthelloGameAISystemProduct AI)
        {
            if(AI == null)
            {
                throw new ArgumentNullException(nameof(AI));
            }
            aiplayers.Add(((OthelloGameAiSystem)AI).AiPlayer.PlayerName);
        }
    }

    /// <summary>
    /// Testing a new type of OthelloProduct, which is the observer. Allow other people to observe the game between 2 players.
    /// </summary>
    [Serializable]
    public class OthelloGameObserverFactory : OthelloFactory
    {
        private ArrayList observers = new ArrayList();

        public ArrayList Observers
        {
            get { return observers; }
        }

        protected override OthelloGameAISystemProduct CreateProduct(OthelloGame oGame, OthelloGamePlayer AIplayer, OthelloGamePlayer humanPlayer)
        {
            return new OthelloGameAiSystem(oGame, AIplayer, humanPlayer);
        }

        protected override void RegisterProduct(OthelloGameAISystemProduct AI)
        {
            OthelloExceptions.ThrowExceptionIfNull(AI);

            observers.Add(((OthelloGameAiSystem)AI).AiPlayer.PlayerName);
        }
    }

}
