using System;

namespace ChessProject
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello Chess World!");

            var board = new Board();

            for (int i = 0; i < 8; i++)
            {
                board.AddPiece<Pawn>(new Pawn(eTeamColor.White, (eWidthAlphabet)i, 2));
            }

            board.PrintAllBoard();
        }
    }
}
