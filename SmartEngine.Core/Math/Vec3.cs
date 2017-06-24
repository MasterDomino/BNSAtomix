using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace SmartEngine.Core.Math
{
    [StructLayout(LayoutKind.Sequential), LogicSystemBrowsable(true), TypeConverter(typeof(_GeneralTypeConverter<Vec3>))]
    public unsafe struct Vec3
    {
        internal float x;
        internal float y;
        internal float z;
        public static readonly Vec3 Zero;
        public static readonly Vec3 XAxis;
        public static readonly Vec3 YAxis;
        public static readonly Vec3 ZAxis;
        public Vec3(Vec3 source)
        {
            x = source.x;
            y = source.y;
            z = source.z;
        }

        public Vec3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        static Vec3()
        {
            Zero = new Vec3(0f, 0f, 0f);
            XAxis = new Vec3(1f, 0f, 0f);
            YAxis = new Vec3(0f, 1f, 0f);
            ZAxis = new Vec3(0f, 0f, 1f);
        }

        [LogicSystemMethodDisplay("Vec3( Single x, Single y, Single z )", "Vec3( {0}, {1}, {2} )")]
        public static Vec3 Construct(float x, float y, float z)
        {
            return new Vec3(x, y, z);
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

        [LogicSystemBrowsable(true), DefaultValue(0f)]
        public float Z
        {
            get
            {
                return z;
            }
            set
            {
                z = value;
            }
        }

        [LogicSystemBrowsable(true)]
        public static Vec3 Parse(string text)
        {
            Vec3 vec;
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException("The text parameter cannot be null or zero length.");
            }
            string[] strArray = text.Split(SpaceCharacter.arrayWithOneSpaceCharacter, StringSplitOptions.RemoveEmptyEntries);
            if (strArray.Length != 3)
            {
                throw new FormatException(string.Format("Cannot parse the text '{0}' because it does not have 3 parts separated by spaces in the form (x y z).", text));
            }
            try
            {
                vec = new Vec3(float.Parse(strArray[0]), float.Parse(strArray[1]), float.Parse(strArray[2]));
            }
            catch (Exception)
            {
                throw new FormatException("The parts of the vectors must be decimal numbers.");
            }
            return vec;
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", x, y, z);
        }

        [LogicSystemBrowsable(true)]
        public string ToString(int precision)
        {
            string str = string.Empty;
            str = str.PadLeft(precision, '#');
            return string.Format("{0:0." + str + "} {1:0." + str + "} {2:0." + str + "}", x, y, z);
        }

        public override bool Equals(object obj)
        {
            return ((obj is Vec3) && (this == ((Vec3)obj)));
        }

        public override int GetHashCode()
        {
            return ((x.GetHashCode() ^ y.GetHashCode()) ^ z.GetHashCode());
        }

        [LogicSystemBrowsable(true)]
        public static Vec3 operator +(Vec3 v1, Vec3 v2)
        {
            Vec3 vec;
            vec.x = v1.x + v2.x;
            vec.y = v1.y + v2.y;
            vec.z = v1.z + v2.z;
            return vec;
        }

        [LogicSystemBrowsable(true)]
        public static Vec3 operator -(Vec3 v1, Vec3 v2)
        {
            Vec3 vec;
            vec.x = v1.x - v2.x;
            vec.y = v1.y - v2.y;
            vec.z = v1.z - v2.z;
            return vec;
        }

        [LogicSystemBrowsable(true)]
        public static Vec3 operator *(Vec3 v1, Vec3 v2)
        {
            Vec3 vec;
            vec.x = v1.x * v2.x;
            vec.y = v1.y * v2.y;
            vec.z = v1.z * v2.z;
            return vec;
        }

        [LogicSystemBrowsable(true)]
        public static Vec3 operator *(Vec3 v, float s)
        {
            Vec3 vec;
            vec.x = v.x * s;
            vec.y = v.y * s;
            vec.z = v.z * s;
            return vec;
        }

        public static Vec3 operator *(float s, Vec3 v)
        {
            Vec3 vec;
            vec.x = s * v.x;
            vec.y = s * v.y;
            vec.z = s * v.z;
            return vec;
        }

        [LogicSystemBrowsable(true)]
        public static Vec3 operator /(Vec3 v1, Vec3 v2)
        {
            Vec3 vec;
            vec.x = v1.x / v2.x;
            vec.y = v1.y / v2.y;
            vec.z = v1.z / v2.z;
            return vec;
        }

        [LogicSystemBrowsable(true)]
        public static Vec3 operator /(Vec3 v, float s)
        {
            Vec3 vec;
            float num = 1f / s;
            vec.x = v.x * num;
            vec.y = v.y * num;
            vec.z = v.z * num;
            return vec;
        }

        public static Vec3 operator /(float s, Vec3 v)
        {
            Vec3 vec;
            vec.x = s / v.x;
            vec.y = s / v.y;
            vec.z = s / v.z;
            return vec;
        }

        [LogicSystemBrowsable(true)]
        public static Vec3 operator -(Vec3 v)
        {
            Vec3 vec;
            vec.x = -v.x;
            vec.y = -v.y;
            vec.z = -v.z;
            return vec;
        }

        public static void Add(ref Vec3 v1, ref Vec3 v2, out Vec3 result)
        {
            result.x = v1.x + v2.x;
            result.y = v1.y + v2.y;
            result.z = v1.z + v2.z;
        }

        public static void Subtract(ref Vec3 v1, ref Vec3 v2, out Vec3 result)
        {
            result.x = v1.x - v2.x;
            result.y = v1.y - v2.y;
            result.z = v1.z - v2.z;
        }

        public static void Multiply(ref Vec3 v1, ref Vec3 v2, out Vec3 result)
        {
            result.x = v1.x * v2.x;
            result.y = v1.y * v2.y;
            result.z = v1.z * v2.z;
        }

        public static void Multiply(ref Vec3 v, float s, out Vec3 result)
        {
            result.x = v.x * s;
            result.y = v.y * s;
            result.z = v.z * s;
        }

        public static void Multiply(float s, ref Vec3 v, out Vec3 result)
        {
            result.x = v.x * s;
            result.y = v.y * s;
            result.z = v.z * s;
        }

        public static void Divide(ref Vec3 v1, ref Vec3 v2, out Vec3 result)
        {
            result.x = v1.x / v2.x;
            result.y = v1.y / v2.y;
            result.z = v1.z / v2.z;
        }

        public static void Divide(ref Vec3 v, float s, out Vec3 result)
        {
            float num = 1f / s;
            result.x = v.x * num;
            result.y = v.y * num;
            result.z = v.z * num;
        }

        public static void Divide(float s, ref Vec3 v, out Vec3 result)
        {
            result.x = s / v.x;
            result.y = s / v.y;
            result.z = s / v.z;
        }

        public static void Negate(ref Vec3 v, out Vec3 result)
        {
            result.x = -v.x;
            result.y = -v.y;
            result.z = -v.z;
        }

        public static Vec3 Add(Vec3 v1, Vec3 v2)
        {
            Vec3 vec;
            vec.x = v1.x + v2.x;
            vec.y = v1.y + v2.y;
            vec.z = v1.z + v2.z;
            return vec;
        }

        public static Vec3 Subtract(Vec3 v1, Vec3 v2)
        {
            Vec3 vec;
            vec.x = v1.x - v2.x;
            vec.y = v1.y - v2.y;
            vec.z = v1.z - v2.z;
            return vec;
        }

        public static Vec3 Multiply(Vec3 v1, Vec3 v2)
        {
            Vec3 vec;
            vec.x = v1.x * v2.x;
            vec.y = v1.y * v2.y;
            vec.z = v1.z * v2.z;
            return vec;
        }

        public static Vec3 Multiply(Vec3 v, float s)
        {
            Vec3 vec;
            vec.x = v.x * s;
            vec.y = v.y * s;
            vec.z = v.z * s;
            return vec;
        }

        public static Vec3 Multiply(float s, Vec3 v)
        {
            Vec3 vec;
            vec.x = v.x * s;
            vec.y = v.y * s;
            vec.z = v.z * s;
            return vec;
        }

        public static Vec3 Divide(Vec3 v1, Vec3 v2)
        {
            Vec3 vec;
            vec.x = v1.x / v2.x;
            vec.y = v1.y / v2.y;
            vec.z = v1.z / v2.z;
            return vec;
        }

        public static Vec3 Divide(Vec3 v, float s)
        {
            Vec3 vec;
            float num = 1f / s;
            vec.x = v.x * num;
            vec.y = v.y * num;
            vec.z = v.z * num;
            return vec;
        }

        public static Vec3 Divide(float s, Vec3 v)
        {
            Vec3 vec;
            vec.x = s / v.x;
            vec.y = s / v.y;
            vec.z = s / v.z;
            return vec;
        }

        public static Vec3 Negate(Vec3 v)
        {
            Vec3 vec;
            vec.x = -v.x;
            vec.y = -v.y;
            vec.z = -v.z;
            return vec;
        }

        [LogicSystemBrowsable(true)]
        public static bool operator ==(Vec3 v1, Vec3 v2)
        {
            return (((v1.x == v2.x) && (v1.y == v2.y)) && (v1.z == v2.z));
        }

        [LogicSystemBrowsable(true)]
        public static bool operator !=(Vec3 v1, Vec3 v2)
        {
            if ((v1.x == v2.x) && (v1.y == v2.y))
            {
                return (v1.z != v2.z);
            }
            return true;
        }

        [LogicSystemBrowsable(true)]
        public float this[int index]
        {
            get
            {
                if ((index < 0) || (index > 2))
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
                if ((index < 0) || (index > 2))
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
        public static float Dot(Vec3 v1, Vec3 v2)
        {
            return (((v1.x * v2.x) + (v1.y * v2.y)) + (v1.z * v2.z));
        }

        internal static float A(ref Vec3 A, ref Vec3 a)
        {
            return (((A.x * a.x) + (A.y * a.y)) + (A.z * a.z));
        }

        public static void Dot(ref Vec3 v1, ref Vec3 v2, out float result)
        {
            result = ((v1.x * v2.x) + (v1.y * v2.y)) + (v1.z * v2.z);
        }

        [LogicSystemBrowsable(true)]
        public static Vec3 Cross(Vec3 v1, Vec3 v2)
        {
            return new Vec3((v1.y * v2.z) - (v1.z * v2.y), (v1.z * v2.x) - (v1.x * v2.z), (v1.x * v2.y) - (v1.y * v2.x));
        }

        public static void Cross(ref Vec3 v1, ref Vec3 v2, out Vec3 result)
        {
            result.x = (v1.y * v2.z) - (v1.z * v2.y);
            result.y = (v1.z * v2.x) - (v1.x * v2.z);
            result.z = (v1.x * v2.y) - (v1.y * v2.x);
        }

        public bool Equals(Vec3 v, float epsilon)
        {
            if (System.Math.Abs(x - v.x) > epsilon)
            {
                return false;
            }
            if (System.Math.Abs(y - v.y) > epsilon)
            {
                return false;
            }
            if (System.Math.Abs(z - v.z) > epsilon)
            {
                return false;
            }
            return true;
        }

        public bool Equals(ref Vec3 v, float epsilon)
        {
            if (System.Math.Abs(x - v.x) > epsilon)
            {
                return false;
            }
            if (System.Math.Abs(y - v.y) > epsilon)
            {
                return false;
            }
            if (System.Math.Abs(z - v.z) > epsilon)
            {
                return false;
            }
            return true;
        }

        [LogicSystemBrowsable(true)]
        public float Length()
        {
            return MathFunctions.Sqrt(((x * x) + (y * y)) + (z * z));
        }

        public float LengthFast()
        {
            return MathFunctions.Sqrt16(((x * x) + (y * y)) + (z * z));
        }

        public float LengthSqr()
        {
            return (((x * x) + (y * y)) + (z * z));
        }

        public void Clamp(Vec3 min, Vec3 max)
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
            if (z < min.z)
            {
                z = min.z;
            }
            else if (z > max.z)
            {
                z = max.z;
            }
        }

        [LogicSystemBrowsable(true)]
        public float Normalize()
        {
            float x = ((this.x * this.x) + (y * y)) + (z * z);
            float num2 = MathFunctions.InvSqrt(x);
            this.x *= num2;
            y *= num2;
            z *= num2;
            return (num2 * x);
        }

        public float NormalizeFast()
        {
            float x = ((this.x * this.x) + (y * y)) + (z * z);
            float num2 = MathFunctions.InvSqrt16(x);
            this.x *= num2;
            y *= num2;
            z *= num2;
            return (num2 * x);
        }

        public static Vec3 Normalize(Vec3 v)
        {
            float num = MathFunctions.InvSqrt(((v.x * v.x) + (v.y * v.y)) + (v.z * v.z));
            return new Vec3(v.x * num, v.y * num, v.z * num);
        }

        public static Vec3 NormalizeFast(Vec3 v)
        {
            float num = MathFunctions.InvSqrt16(((v.x * v.x) + (v.y * v.y)) + (v.z * v.z));
            return new Vec3(v.x * num, v.y * num, v.z * num);
        }

        public static void Normalize(ref Vec3 v, out Vec3 result)
        {
            float num = MathFunctions.InvSqrt(((v.x * v.x) + (v.y * v.y)) + (v.z * v.z));
            result.x = v.x * num;
            result.y = v.y * num;
            result.z = v.z * num;
        }

        public static void NormalizeFast(ref Vec3 v, out Vec3 result)
        {
            float num = MathFunctions.InvSqrt16(((v.x * v.x) + (v.y * v.y)) + (v.z * v.z));
            result.x = v.x * num;
            result.y = v.y * num;
            result.z = v.z * num;
        }

        public Vec3 GetNormalize()
        {
            float num = MathFunctions.InvSqrt(((x * x) + (y * y)) + (z * z));
            return new Vec3(x * num, y * num, z * num);
        }

        public Vec3 GetNormalizeFast()
        {
            float num = MathFunctions.InvSqrt16(((x * x) + (y * y)) + (z * z));
            return new Vec3(x * num, y * num, z * num);
        }

        [LogicSystemBrowsable(true)]
        public Vec2 ToVec2()
        {
            Vec2 vec;
            vec.x = x;
            vec.y = y;
            return vec;
        }

        public static Vec3 Lerp(Vec3 v1, Vec3 v2, float amount)
        {
            Vec3 vec;
            vec.x = v1.x + ((v2.x - v1.x) * amount);
            vec.y = v1.y + ((v2.y - v1.y) * amount);
            vec.z = v1.z + ((v2.z - v1.z) * amount);
            return vec;
        }

        public static void Lerp(ref Vec3 v1, ref Vec3 v2, float amount, out Vec3 result)
        {
            result.x = v1.x + ((v2.x - v1.x) * amount);
            result.y = v1.y + ((v2.y - v1.y) * amount);
            result.z = v1.z + ((v2.z - v1.z) * amount);
        }

        public void Saturate()
        {
            MathFunctions.Saturate(ref x);
            MathFunctions.Saturate(ref y);
            MathFunctions.Saturate(ref z);
        }
    }
}
