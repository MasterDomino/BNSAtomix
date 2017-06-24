using System;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace SmartEngine.Core.Math
{
    [StructLayout(LayoutKind.Sequential), TypeConverter(typeof(_GeneralTypeConverter<Vec2>)), LogicSystemBrowsable(true)]
    public unsafe struct Vec2
    {
        internal float x;
        internal float y;
        public static readonly Vec2 Zero;
        public static readonly Vec2 XAxis;
        public static readonly Vec2 YAxis;
        public Vec2(Vec2 source)
        {
            x = source.x;
            y = source.y;
        }

        public Vec2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        static Vec2()
        {
            Zero = new Vec2(0f, 0f);
            XAxis = new Vec2(1f, 0f);
            YAxis = new Vec2(0f, 1f);
        }

        [LogicSystemMethodDisplay("Vec2( Single x, Single y )", "Vec2( {0}, {1} )")]
        public static Vec2 Construct(float x, float y)
        {
            return new Vec2(x, y);
        }

        [LogicSystemBrowsable(true), DefaultValue(0f)]
        public float X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
            }
        }

        [DefaultValue(0f), LogicSystemBrowsable(true)]
        public float Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
            }
        }

        [LogicSystemBrowsable(true)]
        public static Vec2 Parse(string text)
        {
            Vec2 vec;
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException("The text parameter cannot be null or zero length.");
            }
            string[] strArray = text.Split(SpaceCharacter.arrayWithOneSpaceCharacter, StringSplitOptions.RemoveEmptyEntries);
            if (strArray.Length != 2)
            {
                throw new FormatException(string.Format("Cannot parse the text '{0}' because it does not have 2 parts separated by spaces in the form (x y).", text));
            }
            try
            {
                vec = new Vec2(float.Parse(strArray[0]), float.Parse(strArray[1]));
            }
            catch (Exception)
            {
                throw new FormatException("The parts of the vectors must be decimal numbers.");
            }
            return vec;
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", x, y);
        }

        [LogicSystemBrowsable(true)]
        public string ToString(int precision)
        {
            string str = string.Empty;
            str = str.PadLeft(precision, '#');
            return string.Format("{0:0." + str + "} {1:0." + str + "}", x, y);
        }

        public override bool Equals(object obj)
        {
            return ((obj is Vec2) && (this == ((Vec2)obj)));
        }

        public override int GetHashCode()
        {
            return (x.GetHashCode() ^ y.GetHashCode());
        }

        [LogicSystemBrowsable(true)]
        public static Vec2 operator +(Vec2 v1, Vec2 v2)
        {
            Vec2 vec;
            vec.x = v1.x + v2.x;
            vec.y = v1.y + v2.y;
            return vec;
        }

        [LogicSystemBrowsable(true)]
        public static Vec2 operator -(Vec2 v1, Vec2 v2)
        {
            Vec2 vec;
            vec.x = v1.x - v2.x;
            vec.y = v1.y - v2.y;
            return vec;
        }

        [LogicSystemBrowsable(true)]
        public static Vec2 operator *(Vec2 v1, Vec2 v2)
        {
            Vec2 vec;
            vec.x = v1.x * v2.x;
            vec.y = v1.y * v2.y;
            return vec;
        }

        [LogicSystemBrowsable(true)]
        public static Vec2 operator *(Vec2 v, float s)
        {
            Vec2 vec;
            vec.x = v.x * s;
            vec.y = v.y * s;
            return vec;
        }

        public static Vec2 operator *(float s, Vec2 v)
        {
            Vec2 vec;
            vec.x = s * v.x;
            vec.y = s * v.y;
            return vec;
        }

        [LogicSystemBrowsable(true)]
        public static Vec2 operator /(Vec2 v1, Vec2 v2)
        {
            Vec2 vec;
            vec.x = v1.x / v2.x;
            vec.y = v1.y / v2.y;
            return vec;
        }

        [LogicSystemBrowsable(true)]
        public static Vec2 operator /(Vec2 v, float s)
        {
            Vec2 vec;
            float num = 1f / s;
            vec.x = v.x * num;
            vec.y = v.y * num;
            return vec;
        }

        public static Vec2 operator /(float s, Vec2 v)
        {
            Vec2 vec;
            vec.x = s / v.x;
            vec.y = s / v.y;
            return vec;
        }

        [LogicSystemBrowsable(true)]
        public static Vec2 operator -(Vec2 v)
        {
            Vec2 vec;
            vec.x = -v.x;
            vec.y = -v.y;
            return vec;
        }

        public static void Add(ref Vec2 v1, ref Vec2 v2, out Vec2 result)
        {
            result.x = v1.x + v2.x;
            result.y = v1.y + v2.y;
        }

        public static void Subtract(ref Vec2 v1, ref Vec2 v2, out Vec2 result)
        {
            result.x = v1.x - v2.x;
            result.y = v1.y - v2.y;
        }

        public static void Multiply(ref Vec2 v1, ref Vec2 v2, out Vec2 result)
        {
            result.x = v1.x * v2.x;
            result.y = v1.y * v2.y;
        }

        public static void Multiply(ref Vec2 v, float s, out Vec2 result)
        {
            result.x = v.x * s;
            result.y = v.y * s;
        }

        public static void Multiply(float s, ref Vec2 v, out Vec2 result)
        {
            result.x = v.x * s;
            result.y = v.y * s;
        }

        public static void Divide(ref Vec2 v1, ref Vec2 v2, out Vec2 result)
        {
            result.x = v1.x / v2.x;
            result.y = v1.y / v2.y;
        }

        public static void Divide(ref Vec2 v, float s, out Vec2 result)
        {
            float num = 1f / s;
            result.x = v.x * num;
            result.y = v.y * num;
        }

        public static void Divide(float s, ref Vec2 v, out Vec2 result)
        {
            result.x = s / v.x;
            result.y = s / v.y;
        }

        public static void Negate(ref Vec2 v, out Vec2 result)
        {
            result.x = -v.x;
            result.y = -v.y;
        }

        public static Vec2 Add(Vec2 v1, Vec2 v2)
        {
            Vec2 vec;
            vec.x = v1.x + v2.x;
            vec.y = v1.y + v2.y;
            return vec;
        }

        public static Vec2 Subtract(Vec2 v1, Vec2 v2)
        {
            Vec2 vec;
            vec.x = v1.x - v2.x;
            vec.y = v1.y - v2.y;
            return vec;
        }

        public static Vec2 Multiply(Vec2 v1, Vec2 v2)
        {
            Vec2 vec;
            vec.x = v1.x * v2.x;
            vec.y = v1.y * v2.y;
            return vec;
        }

        public static Vec2 Multiply(Vec2 v, float s)
        {
            Vec2 vec;
            vec.x = v.x * s;
            vec.y = v.y * s;
            return vec;
        }

        public static Vec2 Multiply(float s, Vec2 v)
        {
            Vec2 vec;
            vec.x = v.x * s;
            vec.y = v.y * s;
            return vec;
        }

        public static Vec2 Divide(Vec2 v1, Vec2 v2)
        {
            Vec2 vec;
            vec.x = v1.x / v2.x;
            vec.y = v1.y / v2.y;
            return vec;
        }

        public static Vec2 Divide(Vec2 v, float s)
        {
            Vec2 vec;
            float num = 1f / s;
            vec.x = v.x * num;
            vec.y = v.y * num;
            return vec;
        }

        public static Vec2 Divide(float s, Vec2 v)
        {
            Vec2 vec;
            vec.x = s / v.x;
            vec.y = s / v.y;
            return vec;
        }

        public static Vec2 Negate(Vec2 v)
        {
            Vec2 vec;
            vec.x = -v.x;
            vec.y = -v.y;
            return vec;
        }

        [LogicSystemBrowsable(true)]
        public static bool operator ==(Vec2 v1, Vec2 v2)
        {
            return ((v1.x == v2.x) && (v1.y == v2.y));
        }

        [LogicSystemBrowsable(true)]
        public static bool operator !=(Vec2 v1, Vec2 v2)
        {
            if (v1.x == v2.x)
            {
                return (v1.y != v2.y);
            }
            return true;
        }

        [LogicSystemBrowsable(true)]
        public float this[int index]
        {
            get
            {
                if ((index < 0) || (index > 1))
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                fixed (float* numRef = &x)
                {
                    return numRef[index * 4];
                }
            }
            set
            {
                if ((index < 0) || (index > 1))
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                fixed (float* numRef = &x)
                {
                    numRef[index * 4] = value;
                }
            }
        }

        [LogicSystemBrowsable(true)]
        public static float Dot(Vec2 v1, Vec2 v2)
        {
            return ((v1.x * v2.x) + (v1.y * v2.y));
        }

        public static void Dot(ref Vec2 v1, ref Vec2 v2, out float result)
        {
            result = (v1.x * v2.x) + (v1.y * v2.y);
        }

        [LogicSystemBrowsable(true)]
        public static Vec2 Cross(Vec2 v1, Vec2 v2)
        {
            return new Vec2((v1.y * v2.x) - (v1.x * v2.y), (v1.x * v2.y) - (v1.y * v2.x));
        }

        public static void Cross(ref Vec2 v1, ref Vec2 v2, out Vec2 result)
        {
            result.x = (v1.y * v2.x) - (v1.x * v2.y);
            result.y = (v1.x * v2.y) - (v1.y * v2.x);
        }

        public bool Equals(Vec2 v, float epsilon)
        {
            if (System.Math.Abs(x - v.x) > epsilon)
            {
                return false;
            }
            if (System.Math.Abs(y - v.y) > epsilon)
            {
                return false;
            }
            return true;
        }

        [LogicSystemBrowsable(true)]
        public float Length()
        {
            return MathFunctions.Sqrt((x * x) + (y * y));
        }

        public float LengthFast()
        {
            return MathFunctions.Sqrt16((x * x) + (y * y));
        }

        public float LengthSqr()
        {
            return ((x * x) + (y * y));
        }

        public void Clamp(Vec2 min, Vec2 max)
        {
            if (x < min.x)
            {
                x = min.x;
            }
            else if (x > max.x)
            {
                x = max.x;
            }
            if (y < min.y)
            {
                y = min.y;
            }
            else if (y > max.y)
            {
                y = max.y;
            }
        }

        [LogicSystemBrowsable(true)]
        public float Normalize()
        {
            float x = (this.x * this.x) + (y * y);
            float num2 = MathFunctions.InvSqrt(x);
            this.x *= num2;
            y *= num2;
            return (num2 * x);
        }

        public float NormalizeFast()
        {
            float x = (this.x * this.x) + (y * y);
            float num2 = MathFunctions.InvSqrt16(x);
            this.x *= num2;
            y *= num2;
            return (num2 * x);
        }

        public static Vec2 Normalize(Vec2 v)
        {
            float num = MathFunctions.InvSqrt((v.x * v.x) + (v.y * v.y));
            return new Vec2(v.x * num, v.y * num);
        }

        public static Vec2 NormalizeFast(Vec2 v)
        {
            float num = MathFunctions.InvSqrt16((v.x * v.x) + (v.y * v.y));
            return new Vec2(v.x * num, v.y * num);
        }

        public static void Normalize(ref Vec2 v, out Vec2 result)
        {
            float num = MathFunctions.InvSqrt((v.x * v.x) + (v.y * v.y));
            result.x = v.x * num;
            result.y = v.y * num;
        }

        public static void NormalizeFast(ref Vec2 v, out Vec2 result)
        {
            float num = MathFunctions.InvSqrt16((v.x * v.x) + (v.y * v.y));
            result.x = v.x * num;
            result.y = v.y * num;
        }

        public Vec2 GetNormalize()
        {
            float num = MathFunctions.InvSqrt((x * x) + (y * y));
            return new Vec2(x * num, y * num);
        }

        public Vec2 GetNormalizeFast()
        {
            float num = MathFunctions.InvSqrt16((x * x) + (y * y));
            return new Vec2(x * num, y * num);
        }

        public static Vec2 Lerp(Vec2 v1, Vec2 v2, float amount)
        {
            Vec2 vec;
            vec.x = v1.x + ((v2.x - v1.x) * amount);
            vec.y = v1.y + ((v2.y - v1.y) * amount);
            return vec;
        }

        public static void Lerp(ref Vec2 v1, ref Vec2 v2, float amount, out Vec2 result)
        {
            result.x = v1.x + ((v2.x - v1.x) * amount);
            result.y = v1.y + ((v2.y - v1.y) * amount);
        }

        public void Saturate()
        {
            MathFunctions.Saturate(ref x);
            MathFunctions.Saturate(ref y);
        }
    }
}
