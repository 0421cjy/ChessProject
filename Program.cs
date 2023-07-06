using System;

namespace ChessProject
{
    class Program
    {
        public const int PAWN_START_HEIGHT = 2;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello Chess World!");

            var board = new Board();

            for (var color = eTeamColor.White; color < eTeamColor.Max; color++)
            {
                for (int j = 0; j < 8; j++)
                {
                    board.AddPiece(new Pawn(color, (eWidthAlphabet)j, PAWN_START_HEIGHT + (5 * (int)color)));
                }

                board.AddPiece(new Rook(color, eWidthAlphabet.a, 1 + (7 * (int)color)));
                board.AddPiece(new Rook(color, eWidthAlphabet.h, 1 + (7 * (int)color)));

                board.AddPiece(new Knight(color, eWidthAlphabet.b, 1 + (7 * (int)color)));
                board.AddPiece(new Knight(color, eWidthAlphabet.g, 1 + (7 * (int)color)));

                board.AddPiece(new Bishop(color, eWidthAlphabet.c, 1 + (7 * (int)color)));
                board.AddPiece(new Bishop(color, eWidthAlphabet.f, 1 + (7 * (int)color)));

                board.AddPiece(new Queen(color, eWidthAlphabet.d, 1 + (7 * (int)color)));
                board.AddPiece(new King(color, eWidthAlphabet.e, 1 + (7 * (int)color)));
            }

            board.PrintAllBoard();

            var targetPiece = board.GetPiece(eWidthAlphabet.d, 2);
            board.MovePiece(targetPiece, eWidthAlphabet.d, 4);

            board.PrintAllBoard();
        }
    }
}
