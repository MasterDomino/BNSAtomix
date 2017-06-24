using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace SmartEngine.Core.Math
{
    [StructLayout(LayoutKind.Sequential), TypeConverter(typeof(_GeneralTypeConverter<Rect>)), LogicSystemBrowsable(true)]
    public struct Rect
    {
        internal float rA;
        internal float ra;
        internal float rB;
        internal float rb;
        public static readonly Rect Zero;
        public static readonly Rect Cleared;
        public Rect(Rect source)
        {
            rA = source.rA;
            ra = source.ra;
            rB = source.rB;
            rb = source.rb;
        }

        public Rect(float left, float top, float right, float bottom)
        {
            rA = left;
            ra = top;
            rB = right;
            rb = bottom;
        }

        public Rect(Vec2 leftTop, Vec2 rightBottom)
        {
            rA = leftTop.X;
            ra = leftTop.Y;
            rB = rightBottom.X;
            rb = rightBottom.Y;
        }

        public Rect(Vec2 v)
        {
            rA = v.X;
            rB = v.X;
            ra = v.Y;
            rb = v.Y;
        }

        static Rect()
        {
            Zero = new Rect(0f, 0f, 0f, 0f);
            Cleared = new Rect(new Vec2(1E+30f, 1E+30f), new Vec2(-1E+30f, -1E+30f));
        }

        [LogicSystemMethodDisplay("Rect( Single left, Single top, Single right, Single bottom )", "Rect( {0}, {1}, {2}, {3} )")]
        public static Rect Construct(float left, float top, float right, float bottom)
        {
            return new Rect(left, top, right, bottom);
        }

        [LogicSystemBrowsable(true), DefaultValue(0f)]
        public float Left
        {
            get
            {
                return rA;
            }
            set
            {
                rA = value;
            }
        }

        [DefaultValue(0f), LogicSystemBrowsable(true)]
        public float Top
        {
            get
            {
                return ra;
            }
            set
            {
                ra = value;
            }
        }

        [DefaultValue(0f), LogicSystemBrowsable(true)]
        public float Right
        {
            get
            {
                return rB;
            }
            set
            {
                rB = value;
            }
        }

        [LogicSystemBrowsable(true), DefaultValue(0f)]
        public float Bottom
        {
            get
            {
                return rb;
            }
            set
            {
                rb = value;
            }
        }

        [LogicSystemBrowsable(true)]
        public static Rect Parse(string text)
        {
            Rect rect;
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException("The text parameter cannot be null or zero length.");
            }
            string[] strArray = text.Split(SpaceCharacter.arrayWithOneSpaceCharacter, StringSplitOptions.RemoveEmptyEntries);
            if (strArray.Length != 4)
            {
                throw new FormatException(string.Format("Cannot parse the text '{0}' because it does not have 4 parts separated by spaces in the form (left top right bottom).", text));
            }
            try
            {
                rect = new Rect(float.Parse(strArray[0]), float.Parse(strArray[1]), float.Parse(strArray[2]), float.Parse(strArray[3]));
            }
            catch (Exception)
            {
                throw new FormatException("The parts of the vectors must be decimal numbers.");
            }
            return rect;
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3}", new object[] { rA, ra, rB, rb });
        }

        [LogicSystemBrowsable(true)]
        public string ToString(int precision)
        {
            string str = string.Empty;
            str = str.PadLeft(precision, '#');
            return string.Format("{0:0." + str + "} {1:0." + str + "} {2:0." + str + "} {3:0." + str + "}", new object[] { rA, ra, rB, rb });
        }

        public override bool Equals(object obj)
        {
            return ((obj is Rect) && (this == ((Rect)obj)));
        }

        public override int GetHashCode()
        {
            return (((rA.GetHashCode() ^ ra.GetHashCode()) ^ rB.GetHashCode()) ^ rb.GetHashCode());
        }

        [LogicSystemBrowsable(true)]
        public static Rect operator +(Rect r, Vec2 v)
        {
            Rect rect;
            rect.rA = r.rA + v.X;
            rect.ra = r.ra + v.Y;
            rect.rB = r.rB + v.X;
            rect.rb = r.rb + v.Y;
            return rect;
        }

        [LogicSystemBrowsable(true)]
        public static Rect operator +(Vec2 v, Rect r)
        {
            Rect rect;
            rect.rA = v.X + r.rA;
            rect.ra = v.Y + r.ra;
            rect.rB = v.X + r.rB;
            rect.rb = v.Y + r.rb;
            return rect;
        }

        [LogicSystemBrowsable(true)]
        public static Rect operator -(Rect r, Vec2 v)
        {
            Rect rect;
            rect.rA = r.rA - v.X;
            rect.ra = r.ra - v.Y;
            rect.rB = r.rB - v.X;
            rect.rb = r.rb - v.Y;
            return rect;
        }

        [LogicSystemBrowsable(true)]
        public static Rect operator -(Vec2 v, Rect r)
        {
            Rect rect;
            rect.rA = v.X - r.rA;
            rect.ra = v.Y - r.ra;
            rect.rB = v.X - r.rB;
            rect.rb = v.Y - r.rb;
            return rect;
        }

        public static Rect Add(Rect r, Vec2 v)
        {
            Rect rect;
            rect.rA = r.rA + v.X;
            rect.ra = r.ra + v.Y;
            rect.rB = r.rB + v.X;
            rect.rb = r.rb + v.Y;
            return rect;
        }

        public static Rect Add(Vec2 v, Rect r)
        {
            Rect rect;
            rect.rA = v.X + r.rA;
            rect.ra = v.Y + r.ra;
            rect.rB = v.X + r.rB;
            rect.rb = v.Y + r.rb;
            return rect;
        }

        public static Rect Subtract(Rect r, Vec2 v)
        {
            Rect rect;
            rect.rA = r.rA - v.X;
            rect.ra = r.ra - v.Y;
            rect.rB = r.rB - v.X;
            rect.rb = r.rb - v.Y;
            return rect;
        }

        public static Rect Subtract(Vec2 v, Rect r)
        {
            Rect rect;
            rect.rA = v.X - r.rA;
            rect.ra = v.Y - r.ra;
            rect.rB = v.X - r.rB;
            rect.rb = v.Y - r.rb;
            return rect;
        }

        public static void Add(Rect r, Vec2 v, out Rect result)
        {
            result.rA = r.rA + v.X;
            result.ra = r.ra + v.Y;
            result.rB = r.rB + v.X;
            result.rb = r.rb + v.Y;
        }

        public static void Add(Vec2 v, Rect r, out Rect result)
        {
            result.rA = v.X + r.rA;
            result.ra = v.Y + r.ra;
            result.rB = v.X + r.rB;
            result.rb = v.Y + r.rb;
        }

        public static void Subtract(Rect r, Vec2 v, out Rect result)
        {
            result.rA = r.rA - v.X;
            result.ra = r.ra - v.Y;
            result.rB = r.rB - v.X;
            result.rb = r.rb - v.Y;
        }

        public static void Subtract(Vec2 v, Rect r, out Rect result)
        {
            result.rA = v.X - r.rA;
            result.ra = v.Y - r.ra;
            result.rB = v.X - r.rB;
            result.rb = v.Y - r.rb;
        }

        [LogicSystemBrowsable(true)]
        public static bool operator ==(Rect v1, Rect v2)
        {
            return ((((v1.rA == v2.rA) && (v1.ra == v2.ra)) && (v1.rB == v2.rB)) && (v1.rb == v2.rb));
        }

        [LogicSystemBrowsable(true)]
        public static bool operator !=(Rect v1, Rect v2)
        {
            if (((v1.rA == v2.rA) && (v1.ra == v2.ra)) && (v1.rB == v2.rB))
            {
                return (v1.rb != v2.rb);
            }
            return true;
        }

        [LogicSystemBrowsable(true)]
        public unsafe float this[int index]
        {
            get
            {
                if ((index < 0) || (index > 3))
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                fixed (float* numRef = &rA)
                {
                    return numRef[index * 4];
                }
            }
            set
            {
                if ((index < 0) || (index > 3))
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                fixed (float* numRef = &rA)
                {
                    numRef[index * 4] = value;
                }
            }
        }

        public bool Equals(Rect v, float epsilon)
        {
            if (System.Math.Abs(rA - v.rA) > epsilon)
            {
                return false;
            }
            if (System.Math.Abs(ra - v.ra) > epsilon)
            {
                return false;
            }
            if (System.Math.Abs(rB - v.rB) > epsilon)
            {
                return false;
            }
            if (System.Math.Abs(rb - v.rb) > epsilon)
            {
                return false;
            }
            return true;
        }

        [LogicSystemBrowsable(true), Browsable(false)]
        public Vec2 Size
        {
            get
            {
                Vec2 vec;
                vec.x = rB - rA;
                vec.y = rb - ra;
                return vec;
            }
            set
            {
                rB = rA + value.X;
                rb = ra + value.Y;
            }
        }

        public Vec2 GetSize()
        {
            Vec2 vec;
            vec.x = rB - rA;
            vec.y = rb - ra;
            return vec;
        }

        public void GetSize(out Vec2 result)
        {
            result.x = rB - rA;
            result.y = rb - ra;
        }

        [Browsable(false), LogicSystemBrowsable(true)]
        public Vec2 LeftTop
        {
            get
            {
                Vec2 vec;
                vec.x = rA;
                vec.y = ra;
                return vec;
            }
            set
            {
                rA = value.X;
                ra = value.Y;
            }
        }

        [LogicSystemBrowsable(true), Browsable(false)]
        public Vec2 RightTop
        {
            get
            {
                Vec2 vec;
                vec.x = rB;
                vec.y = ra;
                return vec;
            }
            set
            {
                rB = value.X;
                ra = value.Y;
            }
        }

        [Browsable(false), LogicSystemBrowsable(true)]
        public Vec2 LeftBottom
        {
            get
            {
                Vec2 vec;
                vec.x = rA;
                vec.y = rb;
                return vec;
            }
            set
            {
                rA = value.X;
                rb = value.Y;
            }
        }

        [Browsable(false), LogicSystemBrowsable(true)]
        public Vec2 RightBottom
        {
            get
            {
                Vec2 vec;
                vec.x = rB;
                vec.y = rb;
                return vec;
            }
            set
            {
                rB = value.X;
                rb = value.Y;
            }
        }

        [LogicSystemBrowsable(true), Browsable(false)]
        public Vec2 Minimum
        {
            get
            {
                Vec2 vec;
                vec.x = rA;
                vec.y = ra;
                return vec;
            }
            set
            {
                rA = value.X;
                ra = value.Y;
            }
        }

        [Browsable(false), LogicSystemBrowsable(true)]
        public Vec2 Maximum
        {
            get
            {
                Vec2 vec;
                vec.x = rB;
                vec.y = rb;
                return vec;
            }
            set
            {
                rB = value.X;
                rb = value.Y;
            }
        }

        public bool IsInvalid()
        {
            if (rB >= rA)
            {
                return (rb < ra);
            }
            return true;
        }

        [LogicSystemBrowsable(true)]
        public bool IsContainsPoint(Vec2 p)
        {
            return (((p.X >= rA) && (p.Y >= ra)) && ((p.X <= rB) && (p.Y <= rb)));
        }

        public bool IsContainsRect(Rect v)
        {
            return (((v.rA >= rA) && (v.ra >= ra)) && ((v.rB <= rB) && (v.rb <= rb)));
        }

        public bool IsContainsRect(ref Rect v)
        {
            return (((v.rA >= rA) && (v.ra >= ra)) && ((v.rB <= rB) && (v.rb <= rb)));
        }

        public bool IsIntersectsRect(Rect v)
        {
            return (((v.rB >= rA) && (v.rb >= ra)) && ((v.rA <= rB) && (v.ra <= rb)));
        }

        public bool IsIntersectsRect(ref Rect v)
        {
            return (((v.rB >= rA) && (v.rb >= ra)) && ((v.rA <= rB) && (v.ra <= rb)));
        }

        [LogicSystemBrowsable(true)]
        public void Add(Vec2 v)
        {
            if (v.X < rA)
            {
                rA = v.X;
            }
            if (v.X > rB)
            {
                rB = v.X;
            }
            if (v.Y < ra)
            {
                ra = v.Y;
            }
            if (v.Y > rb)
            {
                rb = v.Y;
            }
        }

        public void Add(Rect a)
        {
            if (a.rA < rA)
            {
                rA = a.rA;
            }
            if (a.ra < ra)
            {
                ra = a.ra;
            }
            if (a.rB > rB)
            {
                rB = a.rB;
            }
            if (a.rb > rb)
            {
                rb = a.rb;
            }
        }

        [LogicSystemBrowsable(true)]
        public void Expand(float d)
        {
            rA -= d;
            ra -= d;
            rB += d;
            rb += d;
        }

        [LogicSystemBrowsable(true)]
        public void Expand(Vec2 d)
        {
            rA -= d.X;
            ra -= d.Y;
            rB += d.X;
            rb += d.Y;
        }

        [LogicSystemBrowsable(true)]
        public bool IsCleared()
        {
            return (rA > rB);
        }

        public Rect Intersect(Rect v)
        {
            Rect rect;
            rect.rA = (v.rA > rA) ? v.rA : rA;
            rect.ra = (v.ra > ra) ? v.ra : ra;
            rect.rB = (v.rB < rB) ? v.rB : rB;
            rect.rb = (v.rb < rb) ? v.rb : rb;
            return rect;
        }

        public void Intersect(ref Rect v, out Rect result)
        {
            result.rA = (v.rA > rA) ? v.rA : rA;
            result.ra = (v.ra > ra) ? v.ra : ra;
            result.rB = (v.rB < rB) ? v.rB : rB;
            result.rb = (v.rb < rb) ? v.rb : rb;
        }
    }
}
