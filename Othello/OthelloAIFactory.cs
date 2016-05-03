using System;
using System.Collections;

namespace Othello
{
    /// <summary>
    /// The creator framework that defines the top level routines to create a product
    /// </summary>
    public abstract class OthelloFactory
    {
        public OthelloProduct Create(OthelloGame oGame, OthelloPlayer AIplayer, OthelloPlayer humanPlayer)
        {
            OthelloProduct ai = CreateProduct(oGame, AIplayer, humanPlayer);
            RegisterProduct(ai);
            return ai;
        }

        protected abstract OthelloProduct CreateProduct(OthelloGame oGame, OthelloPlayer AIplayer, OthelloPlayer humanPlayer);
        protected abstract void RegisterProduct(OthelloProduct ai);
    }

    /// <summary>
    /// The concrete creator implementing the sub routines of the creator framework that creates the product (the othello AI)
    /// </summary>
    [Serializable]
    public class OthelloAIFactory : OthelloFactory
    {
        private ArrayList aiplayers = new ArrayList();

        public ArrayList AIPlayers
        {
            get { return aiplayers; }
        }

        protected override OthelloProduct CreateProduct(OthelloGame oGame, OthelloPlayer AIplayer, OthelloPlayer humanPlayer)
        {
            return new OthelloGameAi(oGame,AIplayer,humanPlayer);
        }

        protected override void RegisterProduct(OthelloProduct AI)
        {
            aiplayers.Add(((OthelloGameAi)AI).AiPlayer.PlayerName);
        }
    }

}
