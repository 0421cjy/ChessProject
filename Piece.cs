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

        public virtual void Promotion()
        {
            Console.Write("Promotion");
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
            if (height <= m_height) return false;
            if (m_height + 2 < height) return false;

            if (height == m_height + 2)
            {
                if (m_height != Program.PAWN_START_HEIGHT)
                {
                    return false;
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

        public bool Attack(eWidthAlphabet width, int height)
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

        public override bool Move(eWidthAlphabet width, int height)
        {
            return true;
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

        public override bool Move(eWidthAlphabet width, int height)
        {
            return true;
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

        public override bool Move(eWidthAlphabet width, int height)
        {
            return true;
        }

        public override string ToString()
        {
            return m_color == eTeamColor.White ? "w" + "R" : "b" + "R";
        }
    }

    public class Queen : Piece
    {
        public Queen(eTeamColor color, eWidthAlphabet width, int height)
            : base(color, width, height)
        {
        }

        public override bool Move(eWidthAlphabet width, int height)
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

        public override bool Move(eWidthAlphabet width, int height)
        {
            return true;
        }

        public override string ToString()
        {
            return m_color == eTeamColor.White ? "w" + "K" : "b" + "K";
        }
    }
}
