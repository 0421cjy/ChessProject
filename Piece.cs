using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessProject
{
    public enum eColor
    {
        White,
        Black,
        Max,
    }

    public enum eFile
    {
        a,
        b,
        c,
        d,
        e,
        f,
        g,
        h,
        Max,
    }

    public abstract class Piece
    {
        private readonly eColor m_color;
        private string m_symbol;
        protected eFile m_file;
        protected int m_rank;
        protected int m_moveCount;

        public eColor Color => m_color;
        public eFile File => m_file;
        public int Rank => m_rank;
        public String Symbol => m_symbol;

        public eFile SetFile 
        { 
            set { m_file = value; }
        }

        public int SetRank
        {
            set { m_rank = value; }
        }

        public Piece(eColor color, eFile file, int rank, string symbol)
        {
            m_color = color;
            m_file = file;
            m_rank = rank;
            m_symbol = symbol;
            m_moveCount = 0;
        }

        public abstract bool Move(eFile file, int rank);
        public virtual void ExceptBlockArea(List<(eFile, int)> moveList, eFile file, int rank) { }
        public virtual void ExceptEmptyArea(List<(eFile, int)> moveList, eFile emptyFile, int emptyRank) { }
        public virtual bool CanPromote() { return false; }
        public virtual bool IsCheck() { return false; }
        public virtual void AdditionalMoveArea(List<(eFile, int)> moveList, List<Piece> pieceList) { }
        public virtual void ExceptAttackedArea(List<(eFile, int)> moveList, Func<eColor, List<(eFile, int)>> func) { }

        protected bool CheckDefaultMovePostion(eFile file, int rank)
        {
            if (File == file && Rank == rank) return false;
            if (file < eFile.a || eFile.h < file) return false;
            if (rank < 1 || 8 < rank) return false;

            return true;
        }

        public void AddMoveCount() { m_moveCount++; }
        public bool IsFirstMove() { return m_moveCount == 0; }

        public override string ToString()
        {
            return (m_color == eColor.White ? "w" : "b") + m_symbol;
        }
    }

    public class Pawn : Piece
    {
        public Pawn(eColor color, eFile file, int rank)
            : base(color, file, rank, "P")
        {
        }

        public override bool Move(eFile file, int rank)
        {
            if (!CheckDefaultMovePostion(file, rank)) return false;

            if (Color == eColor.White)
            {
                if (rank <= Rank) return false;
                if (Rank + 2 < rank) return false;
                if (File + 1 < file || File - 1 > file) return false;
                if ((File + 1 == file || File - 1 == file) && (rank == Rank + 2)) return false;
            }

            if (Color == eColor.Black)
            {
                if (Rank <= rank) return false;
                if (Rank - 2 > rank) return false;
                if (File + 1 < file || File - 1 > file) return false;
                if ((File + 1 == file || File - 1 == file) && (rank == Rank - 2)) return false;
            }

            if (rank == Rank + 2 || Rank - 2 == rank)
            {
                if (m_moveCount != 0) return false;
            }

            return true;
        }

        public override void ExceptBlockArea(List<(eFile, int)> list, eFile file, int rank)
        {
            if (File == file)
            {
                if (Color == eColor.White && Rank + 1 == rank)
                {
                    list.RemoveAll(l => l.Item1 == File && l.Item2 > Rank);
                }
                
                if (Color == eColor.Black && Rank - 1 == rank)
                {
                    list.RemoveAll(l => l.Item1 == File && l.Item2 < Rank);
                }

                if (Color == eColor.White && Rank + 2 == rank)
                {
                    list.RemoveAll(l => l.Item1 == File && l.Item2 > Rank + 1);
                }

                if (Color == eColor.Black && Rank - 2 == rank)
                {
                    list.RemoveAll(l => l.Item1 == File && l.Item2 < Rank - 1);
                }
            }
        }

        public override void ExceptEmptyArea(List<(eFile, int)> list, eFile emptyFile, int emptyRank)
        {
            if (m_file == emptyFile) return;

            list.RemoveAll(l => l.Item1 == emptyFile && l.Item2 == emptyRank);
        }

        public override bool CanPromote()
        {
            if (Color == eColor.White && Rank == Board.END_RANK) return true;
            if (Color == eColor.Black && Rank == Board.START_RANK) return true;

            return false;
        }
    }

    public class Knight : Piece
    {
        public Knight(eColor color, eFile file, int rank)
            : base(color, file, rank, "N")
        {
        }

        public override bool Move(eFile file, int rank)
        {
            if (!CheckDefaultMovePostion(file, rank)) return false;
            if (File == file) return false;
            if (Rank == rank) return false;
            if (Rank + 1 == rank && (File + 1 == file || File - 1 == file)) return false;
            if (Rank + 2 == rank && (File + 2 == file || File - 2 == file)) return false;
            if (Rank - 1 == rank && (File + 1 == file || File - 1 == file)) return false;
            if (Rank - 2 == rank && (File + 2 == file || File - 2 == file)) return false;
            if (Rank + 2 < rank || Rank - 2 > rank) return false;
            if (File + 2 < file || File - 2 > file) return false;

            return true;
        }
    }

    public class Bishop : Piece
    {
        public Bishop(eColor color, eFile file, int rank)
            : base(color, file, rank, "B")
        {
        }

        public override bool Move(eFile file, int rank)
        {
            if (!CheckDefaultMovePostion(file, rank)) return false;

            for (var i = 1; i < 9 - Rank; i++)
            {
                if (Rank + i == rank && (File + i == file || File - i == file))
                {
                    return true;
                }
            }

            for (var i = 1; i < Rank; i++)
            {
                if (Rank - i == rank && (File + i == file || File - i == file))
                {
                    return true;
                }
            }

            return false;
        }

        public override void ExceptBlockArea(List<(eFile, int)> list, eFile file, int rank)
        {
            for (var i = 1; i < 9 - Rank; i++)
            {
                if (Rank + i == rank && File - i == file)
                {
                    list.RemoveAll(l => l.Item1 < file && l.Item2 > rank);
                }

                if (Rank + i == rank && File + i == file)
                {
                    list.RemoveAll(l => l.Item1 > file && l.Item2 > rank);
                }
            }

            for (var i = 1; i < Rank; i++)
            {
                if (Rank - i == rank && File - i == file)
                {
                    list.RemoveAll(l => l.Item1 < file && l.Item2 < rank);
                }

                if (Rank - i == rank && File + i == file)
                {
                    list.RemoveAll(l => l.Item1 > file && l.Item2 < rank);
                }
            }
        }
    }

    public class Rook : Piece
    {
        public Rook(eColor color, eFile file, int rank)
            :base(color, file, rank, "R")
        {
        }

        public override bool Move(eFile file, int rank)
        {
            if (!CheckDefaultMovePostion(file, rank)) return false;
            if (File == file || Rank == rank) return true;

            return false;
        }

        public override void ExceptBlockArea(List<(eFile, int)> list, eFile file, int rank)
        {
            if (File == file && Rank == rank) return;

            if (File == file)
            {
                if (Rank < rank)
                {
                    list.RemoveAll(l => l.Item1 == file && l.Item2 > rank);
                }
                else
                {
                    list.RemoveAll(l => l.Item1 == file && l.Item2 < rank);
                }
            }

            if (Rank == rank)
            {
                if (File < file)
                {
                    list.RemoveAll(l => l.Item2 == rank && l.Item1 > file);
                }
                else
                {
                    list.RemoveAll(l => l.Item2 == rank && l.Item1 < file);
                }
            }
        }
    }

    public class Queen : Piece
    {
        public Queen(eColor color, eFile file, int rank)
            : base(color, file, rank, "Q")
        {
        }

        public override bool Move(eFile file, int rank)
        {
            if (!CheckDefaultMovePostion(file, rank)) return false;
            if (File == file || Rank == rank) return true;

            for (var i = 1; i < 9 - Rank; i++)
            {
                if (Rank + i == rank && (File + i == file || File - i == file))
                {
                    return true;
                }
            }

            for (var i = 1; i < Rank; i++)
            {
                if (Rank - i == rank && (File + i == file || File - i == file))
                {
                    return true;
                }
            }

            return false;
        }

        public override void ExceptBlockArea(List<(eFile, int)> list, eFile file, int rank)
        {
            if (File == file && Rank == rank) return;

            for (var i = 1; i < 9 - Rank; i++)
            {
                if (Rank + i == rank && File - i == file)
                {
                    list.RemoveAll(l => l.Item1 < file && l.Item2 > rank);
                }

                if (Rank + i == rank && File + i == file)
                {
                    list.RemoveAll(l => l.Item1 > file && l.Item2 > rank);
                }
            }

            for (var i = 1; i < Rank; i++)
            {
                if (Rank - i == rank && File - i == file)
                {
                    list.RemoveAll(l => l.Item1 < file && l.Item2 < rank);
                }

                if (Rank - i == rank && File + i == file)
                {
                    list.RemoveAll(l => l.Item1 > file && l.Item2 < rank);
                }
            }

            if (File == file)
            {
                if (Rank < rank)
                {
                    list.RemoveAll(l => l.Item1 == file && l.Item2 > rank);
                }
                else
                {
                    list.RemoveAll(l => l.Item1 == file && l.Item2 < rank);
                }
            }

            if (Rank == rank)
            {
                if (File < file)
                {
                    list.RemoveAll(l => l.Item2 == rank && l.Item1 > file);
                }
                else
                {
                    list.RemoveAll(l => l.Item2 == rank && l.Item1 < file);
                }
            }
        }
    }

    public class King : Piece
    {
        public King(eColor color, eFile file, int rank)
            : base(color, file, rank, "K")
        {
        }

        public override bool Move(eFile file, int rank)
        {
            if (!CheckDefaultMovePostion(file, rank)) return false;
            if (File + 2 < file || Rank + 1 < rank) return false;
            if (File - 2 > file || Rank - 1 > rank) return false;
            if ((File + 2 == file || File - 2 == file) && (Rank + 1 == rank || Rank - 1 == rank)) return false;

            if (0 < m_moveCount)
            {
                if (File + 1 < file || File - 1 > file) return false;
            }

            return true;
        }

        public override void ExceptBlockArea(List<(eFile, int)> list, eFile file, int rank)
        {

        }

        public override bool IsCheck()
        {
            Console.WriteLine($"{Color} is checked");

            return true;
        }

        public override void AdditionalMoveArea(List<(eFile, int)> moveList, List<Piece> pieceList)
        {
            if (m_moveCount != 0) return;

            var rookList = pieceList.Where(p => p.Color == Color && p.Rank == Rank && p.IsFirstMove()).ToList();
            foreach (var rook in rookList)
            {
                if (File < rook.File)
                {
                    moveList.Add((File + 2, Rank));
                }

                if (File > rook.File)
                {
                    moveList.Add((File - 2, Rank));
                }
            }
        }

        public override void ExceptAttackedArea(List<(eFile, int)> moveList, Func<eColor, List<(eFile, int)>> func)
        {
            var attackList = func(Color == eColor.White ? eColor.Black : eColor.White);
            moveList.RemoveAll(m => attackList.Any(e => e.Item1 == m.Item1 && e.Item2 == m.Item2));

            foreach (var attack in attackList)
            {
                ExceptBlockArea(moveList, attack.Item1, attack.Item2);
            }
        }
    }
}
