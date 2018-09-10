namespace Othello
{
    /// <summary>
    /// The creator framework that defines the top level routines to create a product. This is the factory design pattern.
    /// </summary>
    public abstract class OthelloFactory
    {
        public OthelloGameAISystemProduct Create(OthelloGame oGame, OthelloGamePlayer AIplayer, OthelloGamePlayer humanPlayer)
        {
            OthelloGameAISystemProduct pdt = CreateProduct(oGame, AIplayer, humanPlayer);
            RegisterProduct(pdt);
            return pdt;
        }

        protected abstract OthelloGameAISystemProduct CreateProduct(OthelloGame oGame, OthelloGamePlayer AIplayer, OthelloGamePlayer humanPlayer);
        protected abstract void RegisterProduct(OthelloGameAISystemProduct pdt);
    }

}
