using System;
using System.Collections.Generic;

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
        }

        public abstract bool Move(eFile file, int rank);

        public virtual void AttackAreaExcept(List<(eFile, int)> list, eFile file, int rank) { }

        protected bool CheckMovePostion(eFile file, int rank)
        {
            if (file < eFile.a || eFile.h < file) return false;
            if (rank < 1 || 8 < rank) return false;
            if (File == file && Rank == rank) return false;

            return true;
        }

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
            if (!CheckMovePostion(file, rank)) return false;
            if (File != file) return false;

            if (Color == eColor.White)
            {
                if (rank <= Rank) return false;
                if (Rank + 2 < rank) return false;

                if (rank == Rank + 2)
                {
                    if (Rank != Board.PAWN_W_START_HEIGHT)
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (Rank <= rank) return false;
                if (rank < Rank - 2) return false;

                if (rank == Rank - 2)
                {
                    if (Rank != Board.PAWN_B_START_HEIGHT)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public override void AttackAreaExcept(List<(eFile, int)> list, eFile file, int rank)
        {
            if (File == file)
            {
                if (Color == eColor.White && Rank + 1 == rank)
                {
                    list.RemoveAll(l => l.Item1 == File && l.Item2 > Rank + 1);
                }
                else if (Color == eColor.Black && Rank - 1 == rank)
                {
                    list.RemoveAll(l => l.Item1 == File && l.Item2 < Rank - 1);
                }
            }
            else
            {
                if (Color == eColor.White)
                {
                    if (rank == Rank + 1 && (file == File - 1 || file == File + 1))
                    {
                        list.Add((file, rank));
                    }
                }
                else
                {
                    if (rank == Rank - 1 && (file == File - 1 || file == File + 1))
                    {
                        list.Add((file, rank));
                    }
                }
            }
        }

        public void Promotion()
        {
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
            if (!CheckMovePostion(file, rank)) return false;
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
            if (!CheckMovePostion(file, rank)) return false;

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

        public override void AttackAreaExcept(List<(eFile, int)> list, eFile file, int rank)
        {
            if (File > file)
            {
                if (Rank < rank)
                {
                    list.RemoveAll(l => l.Item1 < file && l.Item2 > rank);
                }
                else
                {
                    list.RemoveAll(l => l.Item1 < file && l.Item2 < rank);
                }
            }
            else
            {
                if (Rank < rank)
                {
                    list.RemoveAll(l => l.Item1 > file && l.Item2 > rank);
                }
                else
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
            if (!CheckMovePostion(file, rank)) return false;
            if (File == file || Rank == rank) return true;

            return false;
        }

        public override void AttackAreaExcept(List<(eFile, int)> list, eFile file, int rank)
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
            if (!CheckMovePostion(file, rank)) return false;
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

        public override void AttackAreaExcept(List<(eFile, int)> list, eFile file, int rank)
        {
            if (File == file && Rank == rank) return;

            if (File > file)
            {
                if (Rank < rank)
                {
                    list.RemoveAll(l => l.Item1 < file && l.Item2 > rank);
                }
                else
                {
                    list.RemoveAll(l => l.Item1 < file && l.Item2 < rank);
                }
            }
            else
            {
                if (Rank < rank)
                {
                    list.RemoveAll(l => l.Item1 > file && l.Item2 > rank);
                }
                else
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
            if (!CheckMovePostion(file, rank)) return false;
            if (File + 1 < file || Rank + 1 < rank) return false;
            if (File - 1 > file || Rank - 1 > rank) return false;

            return true;
        }
    }
}
