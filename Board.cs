using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessProject
{
    class Board
    {
        private Dictionary<(eWidthAlphabet, int), Piece> m_pieces = new ();

        public Piece GetPiece(eWidthAlphabet width, int height)
        {
            m_pieces.TryGetValue((width, height), out var selectPiece);
            return selectPiece;
        }

        public bool AddPiece<T>(T piece) where T : Piece
        {
            if (m_pieces.ContainsKey((piece.Width, piece.Height)))
            {
                Console.WriteLine("Add failed. duplicated position.");
                return false;
            }

            m_pieces.Add((piece.Width, piece.Height), piece);
            return true;
        }

        public bool MovePiece<T>(T piece) where T : Piece
        {
            var target = m_pieces
                .Where(p => p.Value.Color == piece.Color)
                .Where(p => p.Key.Item1 == piece.Width && p.Key.Item2 == piece.Height)
                .SingleOrDefault().Value;
            if (target != null)
            {
                return false;
            }

            if (!piece.Move(piece.Width, piece.Height))
            {
                return false;
            }

            m_pieces[(piece.Width, piece.Height)] = piece;

            return true;
        }

        public void PrintAllBoard()
        {
            Console.WriteLine("Print All Pieces.");

            foreach(var piece in m_pieces)
            {
                Console.WriteLine($"{piece.Value.Color}: {piece.Key.Item1}{piece.Key.Item2} {piece.Value.ToString()}");
            }
        }
    }
}
