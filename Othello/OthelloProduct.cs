﻿using System;

namespace Othello
{
    /// <summary>
    /// The abstract class that defines the product role in the factory design pattern framework 
    /// </summary>
    [Serializable]
    public abstract class OthelloProduct
    {
        public abstract string GetAIPlayerName();
        public abstract OthelloToken GetBestMove(OthelloPlayer currentPlayer, int remainDepth, float alpha, float beta);
    }
}
