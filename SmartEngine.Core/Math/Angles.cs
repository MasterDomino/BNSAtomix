using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace SmartEngine.Core.Math
{
    [StructLayout(LayoutKind.Sequential), TypeConverter(typeof(_GeneralTypeConverter<Angles>))]
    public struct Angles
    {
        internal float roll;
        internal float pitch;
        internal float yaw;
        public static readonly Angles Zero;
        public Angles(Vec3 v)
        {
            roll = v.Z;
            pitch = v.X;
            yaw = v.Y;
        }

        public Angles(float roll, float pitch, float yaw)
        {
            this.roll = roll;
            this.pitch = pitch;
            this.yaw = yaw;
        }

        public Angles(Angles source)
        {
            roll = source.roll;
            pitch = source.pitch;
            yaw = source.yaw;
        }

        static Angles()
        {
            Zero = new Angles(0f, 0f, 0f);
        }

        [DefaultValue(0f)]
        public float Roll
        {
            get
            {
                return roll;
            }
            set
            {
                roll = value;
            }
        }

        [DefaultValue(0f)]
        public float Pitch
        {
            get
            {
                return pitch;
            }
            set
            {
                pitch = value;
            }
        }

        [DefaultValue(0f)]
        public float Yaw
        {
            get
            {
                return yaw;
            }
            set
            {
                yaw = value;
            }
        }

        public static Angles Parse(string text)
        {
            Angles angles;
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException("The text parameter cannot be null or zero length.");
            }
            string[] strArray = text.Split(SpaceCharacter.arrayWithOneSpaceCharacter, StringSplitOptions.RemoveEmptyEntries);
            if (strArray.Length != 3)
            {
                throw new FormatException(string.Format("Cannot parse the text '{0}' because it does not have 4 parts separated by spaces in the form (pitch yaw roll).", text));
            }
            try
            {
                angles = new Angles(float.Parse(strArray[0]), float.Parse(strArray[1]), float.Parse(strArray[2]));
            }
            catch (Exception)
            {
                throw new FormatException("The parts of the angles must be decimal numbers.");
            }
            return angles;
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", roll, pitch, yaw);
        }

        public string ToString(int precision)
        {
            string str = string.Empty;
            str = str.PadLeft(precision, '#');
            return string.Format("{0:0." + str + "} {1:0." + str + "} {2:0." + str + "}", roll, pitch, yaw);
        }

        public override bool Equals(object obj)
        {
            return ((obj is Angles) && (this == ((Angles)obj)));
        }

        public override int GetHashCode()
        {
            return ((roll.GetHashCode() ^ pitch.GetHashCode()) ^ yaw.GetHashCode());
        }

        public unsafe float this[int index]
        {
            get
            {
                if ((index < 0) || (index > 2))
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                fixed (float* numRef = &roll)
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
                fixed (float* numRef = &roll)
                {
                    numRef[index * 4] = value;
                }
            }
        }

        public static bool operator ==(Angles a, Angles b)
        {
            return (((a.roll == b.roll) && (a.pitch == b.pitch)) && (a.yaw == b.yaw));
        }

        public static bool operator !=(Angles a, Angles b)
        {
            if ((a.roll == b.roll) && (a.pitch == b.pitch))
            {
                return (a.yaw != b.yaw);
            }
            return true;
        }

        public bool Equals(Angles a, float epsilon)
        {
            if (System.Math.Abs(roll - a.roll) > epsilon)
            {
                return false;
            }
            if (System.Math.Abs(pitch - a.pitch) > epsilon)
            {
                return false;
            }
            if (System.Math.Abs(yaw - a.yaw) > epsilon)
            {
                return false;
            }
            return true;
        }

        public static Angles operator -(Angles a)
        {
            return new Angles(-a.roll, -a.pitch, -a.yaw);
        }

        public static Angles operator +(Angles a, Angles b)
        {
            return new Angles(a.roll + b.roll, a.pitch + b.pitch, a.yaw + b.yaw);
        }

        public static Angles operator -(Angles a, Angles b)
        {
            return new Angles(a.roll - b.roll, a.pitch - b.pitch, a.yaw - b.yaw);
        }

        public static Angles operator *(Angles a, float b)
        {
            return new Angles(a.roll * b, a.pitch * b, a.yaw * b);
        }

        public static Angles operator /(Angles a, float b)
        {
            float num = 1f / b;
            return new Angles(a.roll * num, a.pitch * num, a.yaw * num);
        }

        public static Angles operator *(float a, Angles b)
        {
            float num = 1f / a;
            return new Angles(b.roll * num, b.pitch * num, b.yaw * num);
        }

        public void Clamp(Angles min, Angles max)
        {
            if (roll < min.roll)
            {
                roll = min.roll;
            }
            else if (roll > max.roll)
            {
                roll = max.roll;
            }
            if (pitch < min.pitch)
            {
                pitch = min.pitch;
            }
            else if (pitch > max.pitch)
            {
                pitch = max.pitch;
            }
            if (yaw < min.yaw)
            {
                yaw = min.yaw;
            }
            else if (yaw > max.yaw)
            {
                yaw = max.yaw;
            }
        }

        public Quat ToQuat()
        {
            float a = MathFunctions.DegToRad(yaw) * 0.5f;
            float num2 = MathFunctions.Sin(a);
            float num3 = MathFunctions.Cos(a);
            a = MathFunctions.DegToRad(pitch) * 0.5f;
            float num4 = MathFunctions.Sin(a);
            float num5 = MathFunctions.Cos(a);
            a = MathFunctions.DegToRad(roll) * 0.5f;
            float num6 = MathFunctions.Sin(a);
            float num7 = MathFunctions.Cos(a);
            float num8 = num6 * num5;
            float num9 = num7 * num5;
            float num10 = num6 * num4;
            float num11 = num7 * num4;
            return new Quat((num11 * num2) - (num8 * num3), (-num11 * num3) - (num8 * num2), (num10 * num3) - (num9 * num2), (num9 * num3) + (num10 * num2));
        }

        public unsafe void Normalize360()
        {
            for (int i = 0; i < 3; i++)
            {
                if ((this[i] >= 360f) || (this[i] < 0f))
                {
                    this[i] = this[i] - (MathFunctions.Floor(this[i] / 360f) * 360f);
                    if (this[i] >= 360f)
                    {
                        this[i] = this[i] - 360f;
                    }
                    if (this[i] < 0f)
                    {
                        this[i] = this[i] + 360f;
                    }
                }
            }
        }

        public void Normalize180()
        {
            Normalize360();
            if (pitch > 180f)
            {
                pitch -= 360f;
            }
            if (yaw > 180f)
            {
                yaw -= 360f;
            }
            if (roll > 180f)
            {
                roll -= 360f;
            }
        }
    }
}
