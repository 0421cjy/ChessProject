using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessProject
{
    class Board
    {
        private List<Piece> m_pieces = new ();
        protected Stack<(eWidthAlphabet, int, Piece)> m_history = new ();

        public Piece GetPiece(eWidthAlphabet width, int height)
        {
            var selectPiece = m_pieces.Where(p => p.Width == width && p.Height == height).SingleOrDefault();
            if (selectPiece == null)
            {
                return null;
            }

            return selectPiece;
        }

        public bool AddPiece<T>(T piece) where T : Piece
        {
            if (m_pieces.Any(p => p.Width == piece.Width && p.Height == piece.Height))
            {
                Console.WriteLine("Add failed. duplicated position.");
                return false;
            }

            m_pieces.Add(piece);
            return true;
        }

        public bool MovePiece<T>(T piece, eWidthAlphabet newWidth, int newHeight) where T : Piece
        {
            var target = m_pieces
                .Where(p => p.Color == piece.Color)
                .Where(p => p.Width == newWidth && p.Height == newHeight)
                .SingleOrDefault();
            if (target != null)
            {
                return false;
            }

            var prevWidth = piece.Width;
            var prevHeight = piece.Height;

            if (!piece.Move(newWidth, newHeight))
            {
                return false;
            }

            m_history.Push((prevWidth, prevHeight, piece));
            return true;
        }

        public void PrintAllBoard()
        {
            for (eWidthAlphabet i = 0; i < eWidthAlphabet.Max; i++)
            {
                Console.Write("+---");
            }

            Console.WriteLine("+");

            for (int i = 8; i > 0; i--)
            {
                for (var j = eWidthAlphabet.a; j < eWidthAlphabet.Max; j++)
                {
                    var piece = m_pieces.Where(p => p.Width == j && p.Height == i).SingleOrDefault();
                    if (piece != null)
                    {
                        Console.Write($"| {piece}");
                    }
                    else
                    {
                        Console.Write($"|   ");
                    }
                }

                Console.WriteLine("|");

                for (var j = eWidthAlphabet.a; j < eWidthAlphabet.Max; j++)
                {
                    Console.Write("+---");
                }

                Console.WriteLine("+");
            }
        }
    }
}
