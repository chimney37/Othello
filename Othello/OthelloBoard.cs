﻿#define BITBOARD

using System;
using System.Text;
using System.Diagnostics;
using System.Globalization;

namespace Othello
{
    /// <summary>
    /// A default Othelloboard internal data (bitBoard) is represented by 16 bytes as opposed to a matrix representation.
    /// In this representation, each byte stores 4 tokens.
    /// This saves memory requirements for a single board layout by 64 - 16 = 48 bytes (25% of original or 4 times more space efficient)
    ///  
    /// Below is an example (x,y) token, followed by byte index indicating which byte the token belongs.
    /// 0,0 : byte 0
    /// 0,1 : byte 0
    /// 0,2 : byte 0
    /// 0,3 : byte 0
    /// 0,4 : byte 1
    /// 0,5 : byte 1
    /// 0,6 : byte 1
    /// 0.7 : byte 1
    /// 1,0 : byte 2
    /// 
    /// 2 bits represent 1 token. 
    /// Below table (binary versus othello type)
    /// 00: x (empty)
    /// 01: b (black)
    /// 11: w (white)
    /// 
    /// A byte contains 4 tokens placed side by side.
    /// Example: of a byte : 11110100 => wwbx
    /// 
    /// Other supported board representations are the CharBoard, StringBoard and Token as defined by OthelloBoardType.
    /// OthelloBoardType contains following definitions : TokenMatrix, CharMatrix, String, Bit
    /// 
    /// CharMatrix : each cell can be the following ('e':empty, 'b': black, 'w': white) 
    /// String : uses same definition as char board, except the whole board is represented using single string with no returns at end of each row
    /// TokenMatrix : uses OthelloToken class representation for each cell.
    /// 
    /// Coordinate Systems
    /// Othello User view Coordinates has the coordinate system V(x,y) where 
    /// x : left to right (columns)
    /// y : top to bottom (rows)
    /// 
    /// TokenMatrix
    /// T(i,j) = V(x,y)
    /// 
    /// </summary>
    [Serializable]
    public class OthelloBoard : IOthelloAbstractBoardCollection, ICloneable
    {
        #region PROPERTIES AND FIELDS
        public string HashID { get; set; }
        public static readonly int BoardSize = 8;
        protected static readonly int BoardDataLength = 16;
        protected static readonly int[] ShiftMask = { 0xfc, 0xf3, 0xcf, 0x3f };    //masking table: e.g. "11111100" -> 0xfc. Masks out first 2 bits
        private static int[] Shifter;     //precomputed values of shifters
        private static int[,] Indexer;     //precomputed values of indexers
        private byte[] boardData;

        private const char cEmpty = 'x';
        private const char cWhite = 'w';
        private const char cBlack = 'b';
        private const int bitsmask = 0x3;

        #endregion

        #region CONSTRUCTORS AND INITIALIZATION

        /// <summary>
        /// Default
        /// </summary>
        /// <param name="alternate"></param>
        public OthelloBoard(bool alternate = false)
        {
            //precomputes shifters and indexers
            Precomputes();

            boardData = new byte[BoardDataLength];

            InitializeBoard(alternate);
        }

        /// <summary>
        /// Precomputes values for performance improvement.
        /// </summary>
        private static void Precomputes()
        {
            Shifter = new int[BoardSize];
            Indexer = new int[BoardSize, BoardSize];

            //precompute shifter
            for (int i = 0; i < BoardSize; i++)
                Shifter[i] = ComputeShift(i);

            //precompute indexer
            for (int i = 0; i < BoardSize; i++)
                for (int j = 0; j < BoardSize; j++)
                    Indexer[j, i] = ComputeIndex(j, i);
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="prevBoard"></param>
        protected OthelloBoard(OthelloBoard prevBoard)
        {
            OthelloExceptions.ThrowExceptionIfNull(prevBoard);

            this.boardData = new byte[BoardDataLength];
            for(int i=0; i < boardData.Length; i++)
                this.boardData[i] = prevBoard.boardData[i];
        }
       
        /// <summary>
        /// initialize board: pattern 1 if alternate = true (default), pattern 2 otherwise
        /// </summary>
        /// <param name="alternate"></param>
        protected void InitializeBoard(bool alternate = false)
        {
            if (alternate)
            {
                SetCell(OthelloBitType.White, 4, 4);
                SetCell(OthelloBitType.Black, 3, 4);
                SetCell(OthelloBitType.White, 3, 3);
                SetCell(OthelloBitType.Black, 4, 3);
            }
            else
            {
                SetCell(OthelloBitType.Black, 4, 4);
                SetCell(OthelloBitType.White, 3, 4);
                SetCell(OthelloBitType.Black, 3, 3);
                SetCell(OthelloBitType.White, 4, 3);
            }
        }

        /// <summary>
        /// Create a deep clone of OthelloBoard using a protected copy constructor
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new OthelloBoard(this);
        }

        #endregion

        #region DATA CONVERTERS
        
        /// <summary>
        /// Converts a char representation into bit type 
        /// </summary>
        /// <param name="boardRep"></param>
        /// <returns></returns>
        public static byte[] ConvertCharToBitBoard(char[,] boardRep)
        {
            OthelloExceptions.ThrowExceptionIfNull(boardRep);

            byte[] bData = new byte[BoardDataLength];

            for (int i = 0; i < BoardSize; i++)
                for (int j = 0; j < BoardSize; j++)
                    ComputeByteIndexShifts(boardRep[i,j], bData, i, j);
            
            return bData;
        }

        /// <summary>
        /// converts a string board (no returns at end of row)  to a char board
        /// </summary>
        /// <param name="strBoard"></param>
        /// <returns></returns>
        public static char[,] ConvertStringToCharBoard(string strBoard)
        {
            OthelloExceptions.ThrowExceptionIfNull(strBoard);

            char[,] boardRep = new char[BoardSize, BoardSize];
            for (int i = 0; i < BoardSize; i++)
                for (int j = 0; j < BoardSize; j++)
                    boardRep[j, i] = strBoard[i * BoardSize + j];

            return boardRep;
        }

        /// <summary>
        /// converts a string board to a bit board
        /// </summary>
        /// <param name="strBoard"></param>
        /// <returns></returns>
        public static byte[] ConvertStringToBitBoard(string strBoard)
        {
            OthelloExceptions.ThrowExceptionIfNull(strBoard);

            byte[] bData = new byte[BoardDataLength];

            for (int i = 0; i < BoardSize; i++)
                for (int j = 0; j < BoardSize; j++)
                    ComputeByteIndexShifts(strBoard[i * BoardSize + j], bData, i, j);

            return bData;
        }

        /// <summary>
        /// converts a bit board to TokenMatrix form
        /// </summary>
        /// <param name="bitboard"></param>
        /// <returns></returns>
        protected static OthelloToken[,] ConvertBitToOthelloTokenBoard(byte[] bitboard)
        {
            OthelloExceptions.ThrowExceptionIfNull(bitboard);

            OthelloToken[,] tokenBoard = new OthelloToken[BoardSize, BoardSize];

            for (int i = 0; i < BoardSize; i++)
                for (int j = 0; j < BoardSize; j++)
                    switch ((int)((bitboard[Indexer[i, j]] >> Shifter[j]) & bitsmask))
                    {
                        case (int)OthelloBitType.Empty:
                            tokenBoard[j, i] = new OthelloToken(i, j, OthelloBitType.Empty);
                            break;
                        case (int)OthelloBitType.Black:
                            tokenBoard[j, i] = new OthelloToken(i, j, OthelloBitType.Black);
                            break;
                        case (int)OthelloBitType.White:
                            tokenBoard[j, i] = new OthelloToken(i, j, OthelloBitType.White);
                            break;
                        default:
                            throw new Exception(string.Format(CultureInfo.CurrentCulture, "OthelloBoard [Corrupt Bit Data]"));
                    }

            return tokenBoard;
        }

        /// <summary>
        /// converts a bit board to string form
        /// </summary>
        /// <param name="bitboard"></param>
        /// <returns></returns>
        protected static string ConvertBitToStringBoard(byte[] bitboard)
        {
            OthelloExceptions.ThrowExceptionIfNull(bitboard);

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < BoardSize; i++)
                for (int j = 0; j < BoardSize; j++)
                    switch ((int)((bitboard[Indexer[i, j]] >> Shifter[j]) & bitsmask))
                    {
                        case (int)OthelloBitType.Empty:
                            sb.Append(cEmpty);
                            break;
                        case (int)OthelloBitType.Black:
                            sb.Append(cBlack);
                            break;
                        case (int)OthelloBitType.White:
                            sb.Append(cWhite);
                            break;
                        default:
                            throw new Exception(string.Format(CultureInfo.InvariantCulture, "OthelloBoard [Corrupt Bit Data]"));
                    }
                         
            return sb.ToString();
        }

        /// <summary>
        /// converts a bit board to CharMatrix form
        /// </summary>
        /// <param name="bitboard"></param>
        /// <returns></returns>
        protected static char[,] ConvertBitToCharBoard(byte[] bitboard)
        {
            //it's not optimal, but it's relatively cheap to write this way and no big scenarios to do this.
            return ConvertStringToCharBoard(ConvertBitToStringBoard(bitboard));
        }

        /// <summary>
        /// Convert TokenMatrix to bit form
        /// </summary>
        /// <param name="tokenBoard"></param>
        /// <returns></returns>
        protected static byte[] ConvertOthelloTokenToBitBoard(OthelloToken[,] tokenBoard)
        {
            byte[] bData = new byte[BoardDataLength];

            for (int i = 0; i < BoardSize; i++)
                for (int j = 0; j < BoardSize; j++)                
                    ComputeByteIndexShifts(tokenBoard, bData, i, j);

            return bData;
        }

        /// <summary>
        /// helper in conversion to get byte indexer and calculate bit shifts given a char
        /// </summary>
        /// <param name="c"></param>
        /// <param name="bData"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        protected static void ComputeByteIndexShifts(char c, byte[] bData, int i, int j)
        {
            OthelloExceptions.ThrowExceptionIfNull(bData);

            int index = Indexer[i, j];
            int shift = Shifter[j];

            //first erase the bits that is to be set using the masking table.
            bData[index] &= (byte)((int)ShiftMask[shift / 2]);

            switch (c)
            {
                case cEmpty:
                    bData[index] |= (byte)((int)OthelloBitType.Empty << shift);
                    break;
                case cBlack:
                    bData[index] |= (byte)((int)OthelloBitType.Black << shift);
                    break;
                case cWhite:
                    bData[index] |= (byte)((int)OthelloBitType.White << shift);
                    break;
                default:
                    throw new Exception(string.Format(CultureInfo.InvariantCulture, "broken board data."));
            }
        }

        /// <summary>
        /// Conversion to get byte indexer and calculate bit shifts given a token
        /// </summary>
        /// <param name="tokenBoard"></param>
        /// <param name="bData"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        protected static void ComputeByteIndexShifts(OthelloToken[,] tokenBoard, byte[] bData, int i, int j)
        {
            OthelloExceptions.ThrowExceptionIfNull(tokenBoard);
            OthelloExceptions.ThrowExceptionIfNull(bData);

            OthelloBitType obitType = tokenBoard[i, j].Token;

            ComputeByteIndexShiftsSubHelper(bData, i, j, obitType);
        }
        #endregion

        #region SETs

        /// <summary>
        /// Setting board data using a charBoard
        /// </summary>
        /// <param name="charBoard"></param>
        public void SetBoardData(char[,] charBoard)
        {
            this.boardData = OthelloBoard.ConvertCharToBitBoard(charBoard);
        }

        /// <summary>
        /// Setting board data using a stringBoard
        /// </summary>
        /// <param name="stringBoard"></param>
        public void SetBoardData(string stringBoard)
        {
            this.boardData = OthelloBoard.ConvertStringToBitBoard(stringBoard);
        }

        /// <summary>
        /// Setting board data using a tokenBoard
        /// </summary>
        /// <param name="tokenBoard"></param>
        public void SetBoardData(OthelloToken[,] tokenBoard)
        {
            this.boardData = OthelloBoard.ConvertOthelloTokenToBitBoard(tokenBoard);
        }

        /// <summary>
        /// Setting a cell with a token type
        /// </summary>
        /// <param name="oTokenType"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetCell(OthelloBitType oTokenType, int x, int y)
        {
            if (x < 0 || y < 0 || x >= BoardSize || y >= BoardSize)
                throw new Exception(string.Format(CultureInfo.InvariantCulture, "trying to set invalid cell."));

            ComputeByteIndexShiftsSubHelper(boardData, x, y, oTokenType);
        }


        #endregion

        #region GETs

        /// <summary>
        /// Getting a cell token
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public OthelloToken GetCell(int x, int y)
        {
            if(x < 0 || y < 0 || x >= BoardSize || y >= BoardSize)
                return new OthelloToken(-1,-1,OthelloBitType.OOB);
            
            //1# get byte on index of x and y to correspond to the right byte
            //right shift 1# depending on the value of x
            //note: high order bits are zero filled ()
            //precompute a shift table so we can do a O(1) fetch. Gives about 6666 ms in PerfSpeedMakeMove (6688ms)
            switch ((int)((boardData[Indexer[y, x]] >> Shifter[x]) & bitsmask))
            {
                case (int)OthelloBitType.Empty:
                    return new OthelloToken(x, y, OthelloBitType.Empty);
                case (int)OthelloBitType.Black:
                    return new OthelloToken(x, y, OthelloBitType.Black);
                case (int)OthelloBitType.White:
                    return new OthelloToken(x, y, OthelloBitType.White);
                default:
                    throw new Exception(string.Format(CultureInfo.InvariantCulture, "binary data format is incorrect."));
            }
        }
 
        /// <summary>
        /// Get an adjacent othello cell token bit in any 8 directions.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="odir"></param>
        /// <returns></returns>
        public OthelloToken GetAdjacentCell(int x, int y,OthelloDirection odir)
        {
            switch(odir)
            {
                case OthelloDirection.Deg0:
                    return GetCell(x + 1, y);
                case OthelloDirection.Deg45:
                    return GetCell(x + 1, y - 1);
                case OthelloDirection.Deg90:
                    return GetCell(x, y - 1);
                case OthelloDirection.Deg135:
                    return GetCell(x - 1, y - 1);
                case OthelloDirection.Deg180:
                    return GetCell(x - 1, y);
                case OthelloDirection.Deg225:
                    return GetCell(x - 1, y + 1);
                case OthelloDirection.Deg270:
                    return GetCell(x, y + 1);
                case OthelloDirection.Deg315:
                    return GetCell(x + 1, y + 1);
                default:
                    return new OthelloToken(-1,-1, OthelloBitType.OOB);
            }
        }

        /// <summary>
        /// Getting a Token Count of a specific type
        /// </summary>
        /// <param name="obitType"></param>
        /// <returns></returns>
        public int GetTokenCount(OthelloBitType obitType)
        {
            //huge optimizations on using bit type comparer rather than using matrix kind. Original time on looping 100000 on GetToken count took 1070ms.
            //with this code it takes 200ms. (435% faster)

            int wCount = 0;
            for (int i = 0; i < BoardDataLength; i++)
                for (int j = 0; j < 4; j++)
                {
                    int shift = j * 2;
                    wCount += (int)((boardData[i] >> shift) & bitsmask) == (int)obitType ? 1 : 0;
                }
            return wCount;
        }

        /// <summary>
        /// Get 3 types of board data depending on the type specified.
        /// </summary>
        /// <param name="boardType"></param>
        /// <returns></returns>
        public object GetOthelloBoard(OthelloBoardType boardType)
        {
            switch(boardType)
            {
                case OthelloBoardType.Bit:
                    return this.boardData;
                case OthelloBoardType.CharMatrix:
                    return OthelloBoard.ConvertBitToCharBoard(this.boardData);
                case OthelloBoardType.StringSequence:
                    return OthelloBoard.ConvertBitToStringBoard(this.boardData);
                case OthelloBoardType.TokenMatrix:
                    return OthelloBoard.ConvertBitToOthelloTokenBoard(this.boardData);
                default:
                    return null;
            }
        }

        public byte[] GetOthelloBoardBytes()
        {
            return boardData;
        }

        public char[,] GetOthelloBoardCharMatrix()
        {
            return ConvertBitToCharBoard(boardData);
        }

        public string GetOthelloBoardString()
        {
            return ConvertBitToStringBoard(boardData);
        }

        public OthelloToken[,] GetOthelloBoardTokenMatrix()
        {
            return ConvertBitToOthelloTokenBoard(boardData);
        }

        /// <summary>
        /// override method to get string representation of board
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return OthelloBoard.ConvertBitToStringBoard(this.boardData);
        }
        #endregion

        #region ABSTRACTION HELPERS

        /// <summary>
        /// Sub helper in conversion to get byte indexer
        /// </summary>
        /// <param name="bData"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="obitType"></param>
        private static void ComputeByteIndexShiftsSubHelper(byte[] bData, int i, int j, OthelloBitType obitType)
        {
            int index = Indexer[j, i];
            int shift = Shifter[i];

            //first erase the bits that is to be set using the masking table.
            bData[index] &= (byte)((int)ShiftMask[shift / 2]);

            switch (obitType)
            {
                case OthelloBitType.Empty:
                    bData[index] |= (byte)((int)OthelloBitType.Empty << shift);
                    break;
                case OthelloBitType.Black:
                    bData[index] |= (byte)((int)OthelloBitType.Black << shift);
                    break;
                case OthelloBitType.White:
                    bData[index] |= (byte)((int)OthelloBitType.White << shift);
                    break;
            }
        }

        /// <summary>
        /// Computes a shift given a column index i of a matrix where i = x in V(x,y). Used to get and set individual 2-bits of each token bit representation
        /// E.g. 
        /// column 0, BoardSize=8 will give us shift = 6
        /// column 1, BoardSize=8 => shift = 4
        /// column 2, BoardSize=8 => shift = 2
        /// column 3, BoardSize=8 => shift = 0
        /// Speed up is achieved by precomputing a shift table so we can do a O(1) fetch from memory upon creation
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        protected static int ComputeShift(int column)
        {
            return (Math.Abs((column - (BoardSize - 1))) % 4) * 2;
        }

        /// <summary>
        /// Compute a indexer of the byte that contains the token bit representation. See byte layout example of class description
        /// E.g. (x,y) followed by byte index.
        /// 0,0 : byte 0
        /// 0,1 : byte 0
        /// 0,2 : byte 0
        /// 0,3 : byte 0
        /// 0,4 : byte 1
        /// 0,5 : byte 1
        /// 0,6 : byte 1
        /// 0.7 : byte 1
        /// 1,0 : byte 2
        /// Speed up is achieved by a precomputed index for all combinations of row and column for O(1) fetch instead of arithmetic
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        protected static int ComputeIndex(int row, int column)
        {
            return (row * 2) + (column) / 4;
        }
        #endregion

        #region ITERATOR
        public OthelloBoardIterator CreateIterator()
        {
            return new OthelloBoardIterator(this);
        }

        #endregion

        #region DEBUG OUTS FOR TESTING

        public string DebugOutputBitBoard(bool outAxis=true, bool outConsole=true)
        {
            StringBuilder sb = new StringBuilder();
            string output;

            //create x-axis
            if (outAxis)
            {
                sb.Append("\t ");
                for (int i = 0; i < BoardSize; i++)
                {
                    sb.Append(Convert.ToString(i, CultureInfo.InvariantCulture));
                }
                sb.Append("\n");
            }

            for (int row = 0; row < BoardSize; row++)
            {
                //create y-axis
                if (outAxis)
                {
                    sb.Append(string.Format(CultureInfo.InvariantCulture,"\t{0}", Convert.ToString(row, CultureInfo.InvariantCulture)));
                }

                for (int column = 0; column < BoardSize; column++)
                {
                    int index = ComputeIndex(row, column);
                    int shift = ComputeShift(column);
                    int Token = (int)((boardData[index] >> shift) & bitsmask);

                    //Trace.WriteLine(string.Format("({0},{1}):{2},{3},{4:00000000}", row, column, index, shift, Convert.ToInt32(Convert.ToString(Token, 2))));

                    switch (Token)
                    {
                        case (int)OthelloBitType.Empty:
                            sb.Append("x");
                            break;
                        case (int)OthelloBitType.Black:
                            sb.Append("b");
                            break;
                        case (int)OthelloBitType.White:
                            sb.Append("w");
                            break;
                        default:
                            throw new Exception(string.Format(CultureInfo.InvariantCulture, "binary data format is incorrect."));
                    }
                }
                sb.Append("\n");
            }
            output = sb.ToString();
            if (outConsole)
            {
                Console.Write(output);
                Trace.Write(output);
            }
            sb.Clear();

            return output;
        }
        public string DebugOutputBitBoardBytes()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < boardData.Length; i++)
            {
                sb.Append(string.Format(CultureInfo.InvariantCulture,"byte{0}: {1:00000000} ",
                    i, 
                    Convert.ToInt32(Convert.ToString(boardData[i], 2), CultureInfo.InvariantCulture)));

                if ((i + 1) % 2 == 0)
                {
                    sb.Append("\n");
                }
            }
            return sb.ToString();
        }

        #endregion 
    }
}
