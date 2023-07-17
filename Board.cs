using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ChessProject
{
    class Board
    {
        public const int PAWN_W_START_HEIGHT = 2;
        public const int PAWN_B_START_HEIGHT = 7;
        private const int MAX_BOARD_WIDTH = 8;

        private List<Piece> m_pieces = new ();
        protected Stack<(eFile, int, Piece)> m_history = new ();

        public Board()
        {
            PlaceDefaultPosition();
        }

        private void PlaceDefaultPosition()
        {
            for (var color = eColor.White; color < eColor.Max; color++)
            {
                for (int width = 0; width < MAX_BOARD_WIDTH; width++)
                {
                    AddPiece(new Pawn(color, (eFile)width, PAWN_W_START_HEIGHT + (5 * (int)color)));
                }

                AddPiece(new Rook(color, eFile.a, 1 + (PAWN_B_START_HEIGHT * (int)color)));
                AddPiece(new Rook(color, eFile.h, 1 + (PAWN_B_START_HEIGHT * (int)color)));

                AddPiece(new Knight(color, eFile.b, 1 + (PAWN_B_START_HEIGHT * (int)color)));
                AddPiece(new Knight(color, eFile.g, 1 + (PAWN_B_START_HEIGHT * (int)color)));

                AddPiece(new Bishop(color, eFile.c, 1 + (PAWN_B_START_HEIGHT * (int)color)));
                AddPiece(new Bishop(color, eFile.f, 1 + (PAWN_B_START_HEIGHT * (int)color)));

                AddPiece(new Queen(color, eFile.d, 1 + (PAWN_B_START_HEIGHT * (int)color)));
                AddPiece(new King(color, eFile.e, 1 + (PAWN_B_START_HEIGHT * (int)color)));
            }
        }

        private bool AddPiece<T>(T piece) where T : Piece
        {
            Debug.Assert(eFile.a <= piece.File && piece.File < eFile.Max, "invalid file.");
            Debug.Assert(1 <= piece.Rank && piece.Rank < 9, "invalid rank.");

            if (m_pieces.Any(p => p.File == piece.File && p.Rank == piece.Rank))
            {
                Console.WriteLine("Add failed. duplicated position.");
                return false;
            }

            m_pieces.Add(piece);
            return true;
        }

        public Piece GetPiece(eFile file, int rank)
        {
            return m_pieces.Where(p => p.File == file && p.Rank == rank).SingleOrDefault();
        }

        public bool MovePiece<T>(T piece, eFile targetFile, int targetRank) where T : Piece
        {
            var sameColorPiece = m_pieces
                .Where(p => p.Color == piece.Color)
                .Where(p => p.File == targetFile && p.Rank == targetRank)
                .SingleOrDefault();
            if (sameColorPiece != null)
            {
                return false;
            }

            if (!piece.Move(targetFile, targetRank))
            {
                return false;
            }

            var area = GetMoveArea(piece);
            if (!area.Contains((targetFile, targetRank)))
            {
                return false;
            }

            m_history.Push((piece.File, piece.Rank, piece));

            Console.WriteLine($"{piece.File}{piece.Rank} -> {targetFile}{targetRank}. {piece} is moved.");

            piece.SetFile = targetFile;
            piece.SetRank = targetRank;

            var target = m_pieces
                .Where(p => p.Color != piece.Color)
                .Where(p => p.File == targetFile && p.Rank == targetRank)
                .SingleOrDefault();
            if (target != null)
            {
                m_pieces.Remove(target);
            }

            return true;
        }

        public void PrintAllBoard()
        {
            for (eFile i = 0; i < eFile.Max; i++)
            {
                Console.Write("+---");
            }

            Console.WriteLine("+");

            for (var i = 8; 0 < i; i--)
            {
                for (var j = eFile.a; j < eFile.Max; j++)
                {
                    var piece = m_pieces.Where(p => p.File == j && p.Rank == i).SingleOrDefault();
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

                for (var j = eFile.a; j < eFile.Max; j++)
                {
                    Console.Write("+---");
                }

                Console.WriteLine("+");
            }
        }

        private void SelectAttackArea(eFile width, int height)
        {
            var piece = GetPiece(width, height);
            var area = GetMoveArea(piece);

            PrintAttackArea(area);
        }

        private List<(eFile, int)> GetMoveArea(Piece piece)
        {
            var areaList = new List<(eFile, int)>();

            for (var i = eFile.a; i < eFile.Max; i++)
            {
                for (var j = 8; 0 < j; j--)
                {
                    if (piece.Move(i, j))
                    {
                        areaList.Add((i, j));
                    }
                }
            }

            var copyArea = areaList.ToList();

            foreach (var area in areaList)
            {
                var targetPiece = GetPiece(area.Item1, area.Item2);
                if (targetPiece != null)
                {
                    piece.AttackAreaExcept(copyArea, targetPiece.File, targetPiece.Rank);
                }
            }

            return copyArea;
        }

        private void PrintAttackArea(List<(eFile, int)> attackList)
        {
            for (eFile i = 0; i < eFile.Max; i++)
            {
                Console.Write("+---");
            }

            Console.WriteLine("+");

            for (var i = 8; 0 < i; i--)
            {
                for (var j = eFile.a; j < eFile.Max; j++)
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

                for (var j = eFile.a; j < eFile.Max; j++)
                {
                    Console.Write("+---");
                }

                Console.WriteLine("+");
            }
        }

        public void Run()
        {
            SelectAttackArea(eFile.d, 2);

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

                var targetPiece = GetPiece((eFile)width, height);
                if (!MovePiece(targetPiece, (eFile)newWidth, newHeight))
                {
                    Console.WriteLine($"moved failed {targetPiece.File}{targetPiece.Rank} -> {(eFile)newWidth}{newHeight}");
                }
            }
        }
    }
}
