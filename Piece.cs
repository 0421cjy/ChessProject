using System;
using System.Collections.Generic;

namespace ChessProject
{
    public enum eTeamColor
    {
        White,
        Black,
        Max,
    }

    public enum eWidthAlphabet
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
        protected eTeamColor m_color;
        protected eWidthAlphabet m_width;
        protected int m_height;

        public eTeamColor Color => m_color;
        public eWidthAlphabet Width => m_width;
        public int Height => m_height;

        public Piece(eTeamColor color, eWidthAlphabet width, int height)
        {
            m_color = color;
            m_width = width;
            m_height = height;
        }

        public abstract bool Move(eWidthAlphabet width, int height);
        public abstract bool Attack(eWidthAlphabet width, int height);

        public virtual void AttackAreaExcept(List<(eWidthAlphabet, int)> list, eWidthAlphabet width, int height)
        {
            Console.Write("Attack except");
        }

        public virtual void Promotion()
        {
            Console.Write("Promotion");
        }

        protected bool InvalidMove(eWidthAlphabet width, int height)
        {
            if (width < eWidthAlphabet.a || eWidthAlphabet.h < width) return true;
            if (height < 1 || 8 < height) return true;
            if (m_height == height && m_width == width) return false;

            return false;
        }
    }

    public class Pawn : Piece
    {
        public Pawn(eTeamColor color, eWidthAlphabet startWidth, int startHeight)
            : base(color, startWidth, startHeight)
        {
        }

        public override bool Move(eWidthAlphabet width, int height)
        {
            if (InvalidMove(width, height)) return false;

            if (m_color == eTeamColor.White)
            {
                if (height <= m_height) return false;
                if (m_height + 2 < height) return false;

                if (height == m_height + 2)
                {
                    if (m_height != Board.PAWN_W_START_HEIGHT)
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (m_height <= height) return false;
                if (height < m_height - 2) return false;

                if (height == m_height - 2)
                {
                    if (m_height != Board.PAWN_B_START_HEIGHT)
                    {
                        return false;
                    }
                }
            }

            if (m_width != width)
            {
                if (!Attack(width, height))
                {
                    return false;
                }
            }

            Console.WriteLine($"{m_width}{m_height} -> {width}{height}. Pawn is moved.");

            m_width = width;
            m_height = height;

            return true;
        }

        public override bool Attack(eWidthAlphabet width, int height)
        {
            if (width != m_width + 1 && width != m_width - 1)
            {
                return false;
            }

            return true;
        }

        public override void Promotion()
        {
            base.Promotion();
        }

        public override string ToString()
        {
            return m_color == eTeamColor.White ? "w" + "P" : "b" + "P";
        }
    }

    public class Knight : Piece
    {
        public Knight(eTeamColor color, eWidthAlphabet width, int height)
            : base(color, width, height)
        {
        }

        private bool DefaultCheck(eWidthAlphabet width, int height)
        {
            if (InvalidMove(width, height)) return false;
            if (m_width == width) return false;
            if (m_height == height) return false;
            if (m_height + 1 == height && (m_width + 1 == width || m_width - 1 == width)) return false;
            if (m_height + 2 == height && (m_width + 2 == width || m_width - 2 == width)) return false;
            if (m_height - 1 == height && (m_width + 1 == width || m_width - 1 == width)) return false;
            if (m_height - 2 == height && (m_width + 2 == width || m_width - 2 == width)) return false;
            if (m_height + 2 < height || m_height - 2 > height) return false;
            if (m_width + 2 < width || m_width - 2 > width) return false;

            return true;
        }

        public override bool Move(eWidthAlphabet width, int height)
        {
            if (!DefaultCheck(width, height)) return false;

            Console.WriteLine($"{m_width}{m_height} -> {width}{height}. Pawn is moved.");

            m_width = width;
            m_height = height;

            return true;
        }

        public override bool Attack(eWidthAlphabet width, int height)
        {
            return DefaultCheck(width, height);
        }

        public override string ToString()
        {
            return m_color == eTeamColor.White ? "w" + "N" : "b" + "N";
        }
    }

    public class Bishop : Piece
    {
        public Bishop(eTeamColor color, eWidthAlphabet width, int height)
            : base(color, width, height)
        {
        }

        private bool DefaultCheck(eWidthAlphabet width, int height)
        {
            if (InvalidMove(width, height)) return false;
            if (m_width == width) return false;
            if (m_height == height) return false;

            for(var i = eWidthAlphabet.h - width; i < (int)eWidthAlphabet.h; i++)
            {
                for (var j = 8 - m_height; 0 < i; i--)
                {
                    if (m_height == height + j)
                    {
                        return true;
                    }
                }
            }

            return true;
        }

        public override bool Move(eWidthAlphabet width, int height)
        {
            if (!DefaultCheck(width, height)) return false;

            Console.WriteLine($"{m_width}{m_height} -> {width}{height}. Bishop is moved.");

            m_width = width;
            m_height = height;

            return true;
        }

        public override bool Attack(eWidthAlphabet width, int height)
        {
            return DefaultCheck(width, height);
        }

        public override string ToString()
        {
            return m_color == eTeamColor.White ? "w" + "B" : "b" + "B";
        }
    }

    public class Rook : Piece
    {
        public Rook(eTeamColor color, eWidthAlphabet width, int height)
            :base(color, width, height)
        {
        }

        private bool DefaultCheck(eWidthAlphabet width, int height)
        {
            if (InvalidMove(width, height)) return false;
            if (m_width != width && m_height != height) return false;

            return true;
        }

        public override bool Move(eWidthAlphabet width, int height)
        {
            if (!DefaultCheck(width, height)) return false;

            Console.WriteLine($"{m_width}{m_height} -> {width}{height}. Rook is moved.");

            m_width = width;
            m_height = height;

            return true;
        }

        public override bool Attack(eWidthAlphabet width, int height)
        {
            return DefaultCheck(width, height);
        }

        public override string ToString()
        {
            return m_color == eTeamColor.White ? "w" + "R" : "b" + "R";
        }

        public override void AttackAreaExcept(List<(eWidthAlphabet, int)> list, eWidthAlphabet width, int height)
        {
            if (m_width == width && m_height == height) return;

            if (m_width == width)
            {
                if (m_height < height)
                {
                    list.RemoveAll(l => l.Item1 == width && height <= l.Item2);
                }
                else
                {
                    list.RemoveAll(l => l.Item1 == width && l.Item2 <= height);
                }
            }

            if (m_height == height)
            {
                if (m_width < width)
                {
                    list.RemoveAll(l => l.Item2 == height && width <= l.Item1);
                }
                else
                {
                    list.RemoveAll(l => l.Item2 == height && l.Item1 <= width);
                }
            }
        }
    }

    public class Queen : Piece
    {
        public Queen(eTeamColor color, eWidthAlphabet width, int height)
            : base(color, width, height)
        {
        }

        private bool DefaultCheck(eWidthAlphabet width, int height)
        {
            if (InvalidMove(width, height)) return false;

            return true;
        }

        public override bool Move(eWidthAlphabet width, int height)
        {
            if (!DefaultCheck(width, height)) return false;

            Console.WriteLine($"{m_width}{m_height} -> {width}{height}. Queen is moved.");

            m_width = width;
            m_height = height;

            return true;
        }

        public override bool Attack(eWidthAlphabet width, int height)
        {
            return true;
        }

        public override string ToString()
        {
            return m_color == eTeamColor.White ? "w" + "Q" : "b" + "Q";
        }
    }

    public class King : Piece
    {
        public King(eTeamColor color, eWidthAlphabet width, int height)
            : base(color, width, height)
        {
        }

        private bool DefaultCheck(eWidthAlphabet width, int height)
        {
            if (InvalidMove(width, height)) return false;

            return true;
        }

        public override bool Move(eWidthAlphabet width, int height)
        {
            if (!DefaultCheck(width, height)) return false;

            Console.WriteLine($"{m_width}{m_height} -> {width}{height}. Queen is moved.");

            m_width = width;
            m_height = height;

            return true;
        }

        public override bool Attack(eWidthAlphabet width, int height)
        {
            return true;
        }

        public override string ToString()
        {
            return m_color == eTeamColor.White ? "w" + "K" : "b" + "K";
        }
    }
}
