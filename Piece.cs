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
        private eTeamColor m_color;
        private string m_keyword;
        protected eWidthAlphabet m_width;
        protected int m_height;

        public eTeamColor Color => m_color;
        public eWidthAlphabet Width => m_width;
        public int Height => m_height;

        public Piece(eTeamColor color, eWidthAlphabet width, int height, string keyword)
        {
            m_color = color;
            m_width = width;
            m_height = height;
            m_keyword = keyword;
        }

        public abstract bool Move(eWidthAlphabet width, int height);
        public abstract bool Attack(eWidthAlphabet width, int height);

        public virtual void AttackAreaExcept(List<(eWidthAlphabet, int)> list, eWidthAlphabet width, int height)
        {
            Console.Write("Attack except");
        }

        protected bool InvalidMove(eWidthAlphabet width, int height)
        {
            if (width < eWidthAlphabet.a || eWidthAlphabet.h < width) return true;
            if (height < 1 || 8 < height) return true;
            if (Width == width && Height == height) return false;

            return false;
        }

        public override string ToString()
        {
            return m_color == eTeamColor.White ? "w" + m_keyword : "b" + m_keyword;
        }
    }

    public class Pawn : Piece
    {
        public Pawn(eTeamColor color, eWidthAlphabet startWidth, int startHeight)
            : base(color, startWidth, startHeight, "P")
        {
        }

        public override bool Move(eWidthAlphabet width, int height)
        {
            if (InvalidMove(width, height)) return false;

            if (Color == eTeamColor.White)
            {
                if (height <= Height) return false;
                if (Height + 2 < height) return false;

                if (height == Height + 2)
                {
                    if (Height != Board.PAWN_W_START_HEIGHT)
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (Height <= height) return false;
                if (height < Height - 2) return false;

                if (height == Height - 2)
                {
                    if (Height != Board.PAWN_B_START_HEIGHT)
                    {
                        return false;
                    }
                }
            }

            if (Width != width)
            {
                if (!Attack(width, height))
                {
                    return false;
                }
            }

            Console.WriteLine($"{Width}{Height} -> {width}{height}. Pawn is moved.");

            m_width = width;
            m_height = height;

            return true;
        }

        public override bool Attack(eWidthAlphabet width, int height)
        {
            if (Color == eTeamColor.White)
            {
                if (height == Height + 1 && (width == Width - 1 || width == Width + 1))
                {
                    return true;
                }
            }
            else
            {
                if (height == Height - 1 && (width == Width - 1 || width == Width + 1))
                {
                    return true;
                }
            }

            return false;
        }

        public void Promotion()
        {
        }
    }

    public class Knight : Piece
    {
        public Knight(eTeamColor color, eWidthAlphabet width, int height)
            : base(color, width, height, "N")
        {
        }

        private bool DefaultCheck(eWidthAlphabet width, int height)
        {
            if (InvalidMove(width, height)) return false;
            if (Width == width) return false;
            if (Height == height) return false;
            if (Height + 1 == height && (Width + 1 == width || Width - 1 == width)) return false;
            if (Height + 2 == height && (Width + 2 == width || Width - 2 == width)) return false;
            if (Height - 1 == height && (Width + 1 == width || Width - 1 == width)) return false;
            if (Height - 2 == height && (Width + 2 == width || Width - 2 == width)) return false;
            if (Height + 2 < height || Height - 2 > height) return false;
            if (Width + 2 < width || Width - 2 > width) return false;

            return true;
        }

        public override bool Move(eWidthAlphabet width, int height)
        {
            if (!DefaultCheck(width, height)) return false;

            Console.WriteLine($"{Width}{Height} -> {width}{height}. Pawn is moved.");

            m_width = width;
            m_height = height;

            return true;
        }

        public override bool Attack(eWidthAlphabet width, int height)
        {
            return DefaultCheck(width, height);
        }

        public override void AttackAreaExcept(List<(eWidthAlphabet, int)> list, eWidthAlphabet width, int height)
        {
            list.RemoveAll(l => l.Item1 == width && l.Item2 == height);
        }
    }

    public class Bishop : Piece
    {
        public Bishop(eTeamColor color, eWidthAlphabet width, int height)
            : base(color, width, height, "B")
        {
        }

        private bool DefaultCheck(eWidthAlphabet width, int height)
        {
            if (InvalidMove(width, height)) return false;

            for (var i = 1; i < 9 - Height; i++)
            {
                if (Height + i == height && (Width + i == width || Width - i == width))
                {
                    return true;
                }
            }

            for (var i = 1; i < Height; i++)
            {
                if (Height - i == height && (Width + i == width || Width - i == width))
                {
                    return true;
                }
            }

            return false;
        }

        public override bool Move(eWidthAlphabet width, int height)
        {
            if (!DefaultCheck(width, height)) return false;

            Console.WriteLine($"{Width}{Height} -> {width}{height}. Bishop is moved.");

            m_width = width;
            m_height = height;

            return true;
        }

        public override bool Attack(eWidthAlphabet width, int height)
        {
            return DefaultCheck(width, height);
        }

        public override void AttackAreaExcept(List<(eWidthAlphabet, int)> list, eWidthAlphabet width, int height)
        {
            if (Width > width)
            {
                if (Height < height)
                {
                    list.RemoveAll(l => l.Item1 <= width && height <= l.Item2);
                }
                else
                {
                    list.RemoveAll(l => l.Item1 <= width && height >= l.Item2);
                }
            }
            else
            {
                if (Height < height)
                {
                    list.RemoveAll(l => l.Item1 >= width && height <= l.Item2);
                }
                else
                {
                    list.RemoveAll(l => l.Item1 >= width && height >= l.Item2);
                }
            }
        }
    }

    public class Rook : Piece
    {
        public Rook(eTeamColor color, eWidthAlphabet width, int height)
            :base(color, width, height, "R")
        {
        }

        private bool DefaultCheck(eWidthAlphabet width, int height)
        {
            if (InvalidMove(width, height)) return false;
            if (Width == width || Height == height) return true;

            return false;
        }

        public override bool Move(eWidthAlphabet width, int height)
        {
            if (!DefaultCheck(width, height)) return false;

            Console.WriteLine($"{Width}{Height} -> {width}{height}. Rook is moved.");

            m_width = width;
            m_height = height;

            return true;
        }

        public override bool Attack(eWidthAlphabet width, int height)
        {
            return DefaultCheck(width, height);
        }

        public override void AttackAreaExcept(List<(eWidthAlphabet, int)> list, eWidthAlphabet width, int height)
        {
            if (Width == width && Height == height) return;

            if (Width == width)
            {
                if (Height < height)
                {
                    list.RemoveAll(l => l.Item1 == width && height <= l.Item2);
                }
                else
                {
                    list.RemoveAll(l => l.Item1 == width && l.Item2 <= height);
                }
            }

            if (Height == height)
            {
                if (Width < width)
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
            : base(color, width, height, "Q")
        {
        }

        private bool DefaultCheck(eWidthAlphabet width, int height)
        {
            if (InvalidMove(width, height)) return false;
            if (Width == width || Height == height) return true;

            for (var i = 1; i < 9 - Height; i++)
            {
                if (Height + i == height && (Width + i == width || Width - i == width))
                {
                    return true;
                }
            }

            for (var i = 1; i < Height; i++)
            {
                if (Height - i == height && (Width + i == width || Width - i == width))
                {
                    return true;
                }
            }

            return false;
        }

        public override bool Move(eWidthAlphabet width, int height)
        {
            if (!DefaultCheck(width, height)) return false;

            Console.WriteLine($"{Width}{Height} -> {width}{height}. Queen is moved.");

            m_width = width;
            m_height = height;

            return true;
        }

        public override bool Attack(eWidthAlphabet width, int height)
        {
            return DefaultCheck(width, height);
        }

        public override void AttackAreaExcept(List<(eWidthAlphabet, int)> list, eWidthAlphabet width, int height)
        {
            if (Width == width && Height == height) return;

            if (Width > width)
            {
                if (Height < height)
                {
                    list.RemoveAll(l => l.Item1 < width && height < l.Item2);
                }
                else
                {
                    list.RemoveAll(l => l.Item1 < width && height > l.Item2);
                }
            }
            else
            {
                if (Height < height)
                {
                    list.RemoveAll(l => l.Item1 > width && height < l.Item2);
                }
                else
                {
                    list.RemoveAll(l => l.Item1 > width && height > l.Item2);
                }
            }

            if (Width == width)
            {
                if (Height < height)
                {
                    list.RemoveAll(l => l.Item1 == width && height <= l.Item2);
                }
                else
                {
                    list.RemoveAll(l => l.Item1 == width && l.Item2 <= height);
                }
            }

            if (Height == height)
            {
                if (Width < width)
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

    public class King : Piece
    {
        public King(eTeamColor color, eWidthAlphabet width, int height)
            : base(color, width, height, "K")
        {
        }

        private bool DefaultCheck(eWidthAlphabet width, int height)
        {
            if (InvalidMove(width, height)) return false;
            if (Width + 1 < width || Height + 1 < height) return false;
            if (Width - 1 > width || Height - 1 > height) return false;

            return true;
        }

        public override bool Move(eWidthAlphabet width, int height)
        {
            if (!DefaultCheck(width, height)) return false;

            Console.WriteLine($"{Width}{Height} -> {width}{height}. Queen is moved.");

            m_width = width;
            m_height = height;

            return true;
        }

        public override bool Attack(eWidthAlphabet width, int height)
        {
            return DefaultCheck(width, height);
        }
    }
}
