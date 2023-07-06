using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessProject
{
    class Board
    {
        public const int PAWN_W_START_HEIGHT = 2;
        public const int PAWN_B_START_HEIGHT = 7;
        private const int MAX_BOARD_WIDTH = 8;

        private List<Piece> m_pieces = new ();
        protected Stack<(eWidthAlphabet, int, Piece)> m_history = new ();

        public Board()
        {
            //PlaceDefaultPosition();
            AddPiece(new Knight(eTeamColor.White, eWidthAlphabet.d, 4));
        }

        private void PlaceDefaultPosition()
        {
            for (var color = eTeamColor.White; color < eTeamColor.Max; color++)
            {
                for (int width = 0; width < MAX_BOARD_WIDTH; width++)
                {
                    AddPiece(new Pawn(color, (eWidthAlphabet)width, PAWN_W_START_HEIGHT + (5 * (int)color)));
                }

                AddPiece(new Rook(color, eWidthAlphabet.a, 1 + (PAWN_B_START_HEIGHT * (int)color)));
                AddPiece(new Rook(color, eWidthAlphabet.h, 1 + (PAWN_B_START_HEIGHT * (int)color)));

                AddPiece(new Knight(color, eWidthAlphabet.b, 1 + (PAWN_B_START_HEIGHT * (int)color)));
                AddPiece(new Knight(color, eWidthAlphabet.g, 1 + (PAWN_B_START_HEIGHT * (int)color)));

                AddPiece(new Bishop(color, eWidthAlphabet.c, 1 + (PAWN_B_START_HEIGHT * (int)color)));
                AddPiece(new Bishop(color, eWidthAlphabet.f, 1 + (PAWN_B_START_HEIGHT * (int)color)));

                AddPiece(new Queen(color, eWidthAlphabet.d, 1 + (PAWN_B_START_HEIGHT * (int)color)));
                AddPiece(new King(color, eWidthAlphabet.e, 1 + (PAWN_B_START_HEIGHT * (int)color)));
            }
        }

        private bool AddPiece<T>(T piece) where T : Piece
        {
            if (m_pieces.Any(p => p.Width == piece.Width && p.Height == piece.Height))
            {
                Console.WriteLine("Add failed. duplicated position.");
                return false;
            }

            m_pieces.Add(piece);
            return true;
        }

        public Piece GetPiece(eWidthAlphabet width, int height)
        {
            var selectPiece = m_pieces.Where(p => p.Width == width && p.Height == height).SingleOrDefault();
            if (selectPiece == null)
            {
                return null;
            }

            return selectPiece;
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

        public bool AttackPiece<T>(T piece, eWidthAlphabet newWidth, int newHeight) where T : Piece
        {
            var target = m_pieces
                .Where(p => p.Color == piece.Color)
                .Where(p => p.Width == newWidth && p.Height == newHeight)
                .SingleOrDefault();
            if (target != null)
            {
                return false;
            }

            if (!piece.Attack(newWidth, newHeight))
            {
                return false;
            }

            return true;
        }

        public void PrintAllBoard()
        {
            for (eWidthAlphabet i = 0; i < eWidthAlphabet.Max; i++)
            {
                Console.Write("+---");
            }

            Console.WriteLine("+");

            for (var i = 8; 0 < i; i--)
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

        public void PrintAttackArea(List<(eWidthAlphabet, int)> attackList)
        {
            for (eWidthAlphabet i = 0; i < eWidthAlphabet.Max; i++)
            {
                Console.Write("+---");
            }

            Console.WriteLine("+");

            for (var i = 8; 0 < i; i--)
            {
                for (var j = eWidthAlphabet.a; j < eWidthAlphabet.Max; j++)
                {
                    if (attackList.Any(p => p.Item1 == j && p.Item2 == i))
                    {
                        Console.Write($"| X ");
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

        public void SelectAttackArea(eWidthAlphabet width, int height)
        {
            var areaList = new List<(eWidthAlphabet, int)>();
            var piece = GetPiece(width, height);

            for (var i = eWidthAlphabet.a; i < eWidthAlphabet.Max; i++)
            {
                for (var j = 8; 0 < j; j--)
                {
                    if (AttackPiece(piece, i, j))
                    {
                        areaList.Add((i, j));
                    }
                }
            }

            PrintAttackArea(areaList);
        }

        public void Run()
        {
            Console.WriteLine("Hello Chess World!");

            SelectAttackArea(eWidthAlphabet.d, 4);

            while (true)
            {
                PrintAllBoard();

                Console.Write("insert width height and new width, new height : ");

                var input = Console.ReadLine();
                if (input == "exit" || input == "EXIT") break;

                if (input.Length < 4) continue;

                if (!Int32.TryParse(input[1].ToString(), out var height))
                {
                    continue;
                }

                if (!Int32.TryParse(input[3].ToString(), out var newHeight))
                {
                    continue;
                }

                var width = input[0] switch
                {
                    'a' => 0,
                    'b' => 1,
                    'c' => 2,
                    'd' => 3,
                    'e' => 4,
                    'f' => 5,
                    'g' => 6,
                    'h' => 7,
                    _ => -1,
                };

                var newWidth = input[2] switch
                {
                    'a' => 0,
                    'b' => 1,
                    'c' => 2,
                    'd' => 3,
                    'e' => 4,
                    'f' => 5,
                    'g' => 6,
                    'h' => 7,
                    _ => -1,
                };

                var targetPiece = GetPiece((eWidthAlphabet)width, height);
                if (!MovePiece(targetPiece, (eWidthAlphabet)newWidth, newHeight))
                {
                    Console.WriteLine($"moved failed {targetPiece.Width}{targetPiece.Height} -> {(eWidthAlphabet)newWidth}{newHeight}");
                }
            }
        }
    }
}
