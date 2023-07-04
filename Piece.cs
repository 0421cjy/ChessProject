using System;

namespace ChessProject
{
    public enum eTeamColor
    {
        White,
        Black,
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
    }

    public abstract class Piece
    {
        private eTeamColor m_color;
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
            if (m_width != width) return false;
            if (height <= m_height) return false;

            if (height == m_height + 2 && height != 2)
            {
                return false;
            }
            else if (height != m_height + 1)
            {
                return false;
            }
            

            m_width = width;
            m_height = height;

            Console.WriteLine($"{width}{height}, Pawn is moved");

            return true;
        }

        public bool Attack(Piece target)
        {
            return true;
        }

        public override void Promotion()
        {
            base.Promotion();
        }
    }
}
