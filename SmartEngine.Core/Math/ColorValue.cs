using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Drawing.Design;

namespace SmartEngine.Core.Math
{
    internal static class SpaceCharacter
    {
        public static char[] arrayWithOneSpaceCharacter = new char[] { ' ' };
    }

    [StructLayout(LayoutKind.Sequential), Editor(typeof(_ColorValueEditor), typeof(UITypeEditor)), LogicSystemBrowsable(true), TypeConverter(typeof(_ColorValueAsByteConverter))]
    public unsafe struct ColorValue
    {
        private float rc;
        private float g;
        private float b;
        private float a;
        public static readonly ColorValue Zero;
        public ColorValue(ColorValue source)
        {
            rc = source.rc;
            g = source.g;
            b = source.b;
            a = source.a;
        }

        public ColorValue(float r, float g, float b, float a)
        {
            rc = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public ColorValue(float r, float g, float b)
        {
            rc = r;
            this.g = g;
            this.b = b;
            a = 1f;
        }

        static ColorValue()
        {
            Zero = new ColorValue(0f, 0f, 0f, 0f);
        }

        [LogicSystemMethodDisplay("ColorValue( Single r, Single g, Single b, Single a )", "ColorValue( {0}, {1}, {2}, {3} )")]
        public static ColorValue Construct(float r, float g, float b, float a)
        {
            return new ColorValue(r, g, b, a);
        }

        [LogicSystemMethodDisplay("ColorValue( Single r, Single g, Single b )", "ColorValue( {0}, {1}, {2} )")]
        public static ColorValue Construct(float r, float g, float b)
        {
            return new ColorValue(r, g, b);
        }

        [LogicSystemBrowsable(true), DefaultValue(0f)]
        public float r
        {
            get
            {
                return rc;
            }
            set
            {
                rc = value;
            }
        }

        [LogicSystemBrowsable(true), DefaultValue(0f)]
        public float Green
        {
            get
            {
                return g;
            }
            set
            {
                g = value;
            }
        }

        [LogicSystemBrowsable(true), DefaultValue(0f)]
        public float Blue
        {
            get
            {
                return b;
            }
            set
            {
                b = value;
            }
        }

        [LogicSystemBrowsable(true), DefaultValue(0f)]
        public float Alpha
        {
            get
            {
                return a;
            }
            set
            {
                a = value;
            }
        }

        [LogicSystemBrowsable(true)]
        public static ColorValue Parse(string text)
        {
            ColorValue value2;
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException("The text parameter cannot be null or zero length.");
            }
            string[] strArray = text.Split(SpaceCharacter.arrayWithOneSpaceCharacter, StringSplitOptions.RemoveEmptyEntries);
            if ((strArray.Length != 3) && (strArray.Length != 4))
            {
                throw new FormatException(string.Format("Cannot parse the text '{0}' because it does not have 3 or 4 parts separated by spaces in the form (r g b [a]).", text));
            }
            try
            {
                value2 = new ColorValue(float.Parse(strArray[0]), float.Parse(strArray[1]), float.Parse(strArray[2]), (strArray.Length == 4) ? float.Parse(strArray[3].Trim()) : 1f);
            }
            catch (Exception)
            {
                throw new FormatException("The parts of the color must be decimal numbers.");
            }
            return value2;
        }

        public override string ToString()
        {
            if (a != 1f)
            {
                return string.Format("{0} {1} {2} {3}", new object[] { rc, g, b, a });
            }
            return string.Format("{0} {1} {2}", rc, g, b);
        }

        [LogicSystemBrowsable(true)]
        public string ToString(int precision)
        {
            string str = string.Empty;
            str = str.PadLeft(precision, '#');
            return string.Format("{0:0." + str + "} {1:0." + str + "} {2:0." + str + "} {3:0." + str + "}", new object[] { rc, g, b, a });
        }

        public override bool Equals(object obj)
        {
            return ((obj is ColorValue) && (this == ((ColorValue)obj)));
        }

        public override int GetHashCode()
        {
            return (((rc.GetHashCode() ^ g.GetHashCode()) ^ b.GetHashCode()) ^ a.GetHashCode());
        }

        public static ColorValue operator +(ColorValue v1, ColorValue v2)
        {
            ColorValue value2;
            value2.rc = v1.rc + v2.rc;
            value2.g = v1.g + v2.g;
            value2.b = v1.b + v2.b;
            value2.a = v1.a + v2.a;
            return value2;
        }

        public static ColorValue operator -(ColorValue v1, ColorValue v2)
        {
            ColorValue value2;
            value2.rc = v1.rc - v2.rc;
            value2.g = v1.g - v2.g;
            value2.b = v1.b - v2.b;
            value2.a = v1.a - v2.a;
            return value2;
        }

        public static ColorValue operator *(ColorValue v1, ColorValue v2)
        {
            ColorValue value2;
            value2.rc = v1.rc * v2.rc;
            value2.g = v1.g * v2.g;
            value2.b = v1.b * v2.b;
            value2.a = v1.a * v2.a;
            return value2;
        }

        [LogicSystemBrowsable(true)]
        public static ColorValue operator *(ColorValue v, float s)
        {
            ColorValue value2;
            value2.rc = v.rc * s;
            value2.g = v.g * s;
            value2.b = v.b * s;
            value2.a = v.a * s;
            return value2;
        }

        public static ColorValue operator *(float s, ColorValue v)
        {
            ColorValue value2;
            value2.rc = v.rc * s;
            value2.g = v.g * s;
            value2.b = v.b * s;
            value2.a = v.a * s;
            return value2;
        }

        public static ColorValue operator /(ColorValue v1, ColorValue v2)
        {
            ColorValue value2;
            value2.rc = v1.rc / v2.rc;
            value2.g = v1.g / v2.g;
            value2.b = v1.b / v2.b;
            value2.a = v1.a / v2.a;
            return value2;
        }

        [LogicSystemBrowsable(true)]
        public static ColorValue operator /(ColorValue v, float s)
        {
            ColorValue value2;
            float num = 1f / s;
            value2.rc = v.rc * num;
            value2.g = v.g * num;
            value2.b = v.b * num;
            value2.a = v.a * num;
            return value2;
        }

        public static ColorValue operator /(float s, ColorValue v)
        {
            ColorValue value2;
            value2.rc = s / v.rc;
            value2.g = s / v.g;
            value2.b = s / v.b;
            value2.a = s / v.a;
            return value2;
        }

        public static ColorValue operator -(ColorValue v)
        {
            ColorValue value2;
            value2.rc = -v.rc;
            value2.g = -v.g;
            value2.b = -v.b;
            value2.a = -v.a;
            return value2;
        }

        public static void Add(ref ColorValue v1, ref ColorValue v2, out ColorValue result)
        {
            result.rc = v1.rc + v2.rc;
            result.g = v1.g + v2.g;
            result.b = v1.b + v2.b;
            result.a = v1.a + v2.a;
        }

        public static void Subtract(ref ColorValue v1, ref ColorValue v2, out ColorValue result)
        {
            result.rc = v1.rc - v2.rc;
            result.g = v1.g - v2.g;
            result.b = v1.b - v2.b;
            result.a = v1.a - v2.a;
        }

        public static void Multiply(ref ColorValue v1, ref ColorValue v2, out ColorValue result)
        {
            result.rc = v1.rc * v2.rc;
            result.g = v1.g * v2.g;
            result.b = v1.b * v2.b;
            result.a = v1.a * v2.a;
        }

        public static void Multiply(ref ColorValue v, float s, out ColorValue result)
        {
            result.rc = v.rc * s;
            result.g = v.g * s;
            result.b = v.b * s;
            result.a = v.a * s;
        }

        public static void Multiply(float s, ref ColorValue v, out ColorValue result)
        {
            result.rc = v.rc * s;
            result.g = v.g * s;
            result.b = v.b * s;
            result.a = v.a * s;
        }

        public static void Divide(ref ColorValue v1, ref ColorValue v2, out ColorValue result)
        {
            result.rc = v1.rc / v2.rc;
            result.g = v1.g / v2.g;
            result.b = v1.b / v2.b;
            result.a = v1.a / v2.a;
        }

        public static void Divide(ref ColorValue v, float s, out ColorValue result)
        {
            float num = 1f / s;
            result.rc = v.rc * num;
            result.g = v.g * num;
            result.b = v.b * num;
            result.a = v.a * num;
        }

        public static void Divide(float s, ref ColorValue v, out ColorValue result)
        {
            result.rc = s / v.rc;
            result.g = s / v.g;
            result.b = s / v.b;
            result.a = s / v.a;
        }

        public static void Negate(ref ColorValue v, out ColorValue result)
        {
            result.rc = -v.rc;
            result.g = -v.g;
            result.b = -v.b;
            result.a = -v.a;
        }

        public static ColorValue Add(ColorValue v1, ColorValue v2)
        {
            ColorValue value2;
            value2.rc = v1.rc + v2.rc;
            value2.g = v1.g + v2.g;
            value2.b = v1.b + v2.b;
            value2.a = v1.a + v2.a;
            return value2;
        }

        public static ColorValue Subtract(ColorValue v1, ColorValue v2)
        {
            ColorValue value2;
            value2.rc = v1.rc - v2.rc;
            value2.g = v1.g - v2.g;
            value2.b = v1.b - v2.b;
            value2.a = v1.a - v2.a;
            return value2;
        }

        public static ColorValue Multiply(ColorValue v1, ColorValue v2)
        {
            ColorValue value2;
            value2.rc = v1.rc * v2.rc;
            value2.g = v1.g * v2.g;
            value2.b = v1.b * v2.b;
            value2.a = v1.a * v2.a;
            return value2;
        }

        public static ColorValue Multiply(ColorValue v, float s)
        {
            ColorValue value2;
            value2.rc = v.rc * s;
            value2.g = v.g * s;
            value2.b = v.b * s;
            value2.a = v.a * s;
            return value2;
        }

        public static ColorValue Multiply(float s, ColorValue v)
        {
            ColorValue value2;
            value2.rc = v.rc * s;
            value2.g = v.g * s;
            value2.b = v.b * s;
            value2.a = v.a * s;
            return value2;
        }

        public static ColorValue Divide(ColorValue v1, ColorValue v2)
        {
            ColorValue value2;
            value2.rc = v1.rc / v2.rc;
            value2.g = v1.g / v2.g;
            value2.b = v1.b / v2.b;
            value2.a = v1.a / v2.a;
            return value2;
        }

        public static ColorValue Divide(ColorValue v, float s)
        {
            ColorValue value2;
            float num = 1f / s;
            value2.rc = v.rc * num;
            value2.g = v.g * num;
            value2.b = v.b * num;
            value2.a = v.a * num;
            return value2;
        }

        public static ColorValue Divide(float s, ColorValue v)
        {
            ColorValue value2;
            value2.rc = s / v.rc;
            value2.g = s / v.g;
            value2.b = s / v.b;
            value2.a = s / v.a;
            return value2;
        }

        public static ColorValue Negate(ColorValue v)
        {
            ColorValue value2;
            value2.rc = -v.rc;
            value2.g = -v.g;
            value2.b = -v.b;
            value2.a = -v.a;
            return value2;
        }

        [LogicSystemBrowsable(true)]
        public static bool operator ==(ColorValue v1, ColorValue v2)
        {
            return ((((v1.rc == v2.rc) && (v1.g == v2.g)) && (v1.b == v2.b)) && (v1.a == v2.a));
        }

        [LogicSystemBrowsable(true)]
        public static bool operator !=(ColorValue v1, ColorValue v2)
        {
            if (((v1.rc == v2.rc) && (v1.g == v2.g)) && (v1.b == v2.b))
            {
                return (v1.a != v2.a);
            }
            return true;
        }

        [LogicSystemBrowsable(true)]
        public float this[int index]
        {
            get
            {
                if ((index < 0) || (index > 3))
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                fixed (float* numRef = &rc)
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
                fixed (float* numRef = &rc)
                {
                    numRef[index * 4] = value;
                }
            }
        }

        public bool Equals(ColorValue c, float epsilon)
        {
            if (System.Math.Abs(rc - c.rc) > epsilon)
            {
                return false;
            }
            if (System.Math.Abs(g - c.g) > epsilon)
            {
                return false;
            }
            if (System.Math.Abs(b - c.b) > epsilon)
            {
                return false;
            }
            if (System.Math.Abs(a - c.a) > epsilon)
            {
                return false;
            }
            return true;
        }

        public void Clamp(ColorValue min, ColorValue max)
        {
            if (rc < min.rc)
            {
                rc = min.rc;
            }
            else if (rc > max.rc)
            {
                rc = max.rc;
            }
            if (g < min.g)
            {
                g = min.g;
            }
            else if (g > max.g)
            {
                g = max.g;
            }
            if (b < min.b)
            {
                b = min.b;
            }
            else if (b > max.b)
            {
                b = max.b;
            }
            if (a < min.a)
            {
                a = min.a;
            }
            else if (a > max.a)
            {
                a = max.a;
            }
        }

        [LogicSystemBrowsable(true)]
        public void Saturate()
        {
            if (rc < 0f)
            {
                rc = 0f;
            }
            else if (rc > 1f)
            {
                rc = 1f;
            }
            if (g < 0f)
            {
                g = 0f;
            }
            else if (g > 1f)
            {
                g = 1f;
            }
            if (b < 0f)
            {
                b = 0f;
            }
            else if (b > 1f)
            {
                b = 1f;
            }
            if (a < 0f)
            {
                a = 0f;
            }
            else if (a > 1f)
            {
                a = 1f;
            }
        }

        [LogicSystemBrowsable(true)]
        public Vec4 ToVec4()
        {
            return new Vec4(rc, g, b, a);
        }
    }
}
