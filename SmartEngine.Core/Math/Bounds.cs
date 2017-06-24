using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace SmartEngine.Core.Math
{
    [StructLayout(LayoutKind.Sequential), TypeConverter(typeof(_GeneralTypeConverter<Bounds>))]
    public struct Bounds
    {
        internal Vec3 rf;
        internal Vec3 rG;
        public static readonly Bounds Zero;
        public static readonly Bounds Cleared;
        public Bounds(Bounds source)
        {
            rf = source.rf;
            rG = source.rG;
        }

        public Bounds(Vec3 minimum, Vec3 maximum)
        {
            rf = minimum;
            rG = maximum;
        }

        public Bounds(float minimumX, float minimumY, float minimumZ, float maximumX, float maximumY, float maximumZ)
        {
            rf = new Vec3(minimumX, minimumY, minimumZ);
            rG = new Vec3(maximumX, maximumY, maximumZ);
        }

        public Bounds(Vec3 v)
        {
            rf = v;
            rG = v;
        }

        static Bounds()
        {
            Zero = new Bounds(Vec3.Zero, Vec3.Zero);
            Cleared = new Bounds(new Vec3(1E+30f, 1E+30f, 1E+30f), new Vec3(-1E+30f, -1E+30f, -1E+30f));
        }

        [DefaultValue(typeof(Vec3), "0 0 0")]
        public Vec3 Minimum
        {
            get
            {
                return rf;
            }
            set
            {
                rf = value;
            }
        }

        [DefaultValue(typeof(Vec3), "0 0 0")]
        public Vec3 Maximum
        {
            get
            {
                return rG;
            }
            set
            {
                rG = value;
            }
        }

        public override bool Equals(object obj)
        {
            return ((obj is Bounds) && (this == ((Bounds)obj)));
        }

        public override int GetHashCode()
        {
            return (rf.GetHashCode() ^ rG.GetHashCode());
        }

        public static bool operator ==(Bounds v1, Bounds v2)
        {
            return ((v1.rf == v2.rf) && (v1.rG == v2.rG));
        }

        public static bool operator !=(Bounds v1, Bounds v2)
        {
            if (!(v1.rf != v2.rf))
            {
                return (v1.rG != v2.rG);
            }
            return true;
        }

        public unsafe Vec3 this[int index]
        {
            get
            {
                if ((index < 0) || (index > 1))
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                fixed (Vec3* vecRef = &rf)
                {
                    return vecRef[index];
                }
            }
            set
            {
                if ((index < 0) || (index > 1))
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                fixed (Vec3* vecRef = &rf)
                {
                    vecRef[index] = value;
                }
            }
        }

        public bool Equals(Bounds v, float epsilon)
        {
            if (!rf.Equals(ref v.rf, epsilon))
            {
                return false;
            }
            if (!rG.Equals(ref v.rG, epsilon))
            {
                return false;
            }
            return true;
        }

        public bool IsCleared()
        {
            return (rf.X > rG.X);
        }

        public Vec3 GetCenter()
        {
            Vec3 vec;
            vec.x = (rf.x + rG.x) * 0.5f;
            vec.y = (rf.y + rG.y) * 0.5f;
            vec.z = (rf.z + rG.z) * 0.5f;
            return vec;
        }

        public void GetCenter(out Vec3 result)
        {
            result.x = (rf.x + rG.x) * 0.5f;
            result.y = (rf.y + rG.y) * 0.5f;
            result.z = (rf.z + rG.z) * 0.5f;
        }

        public float GetRadius()
        {
            float x = 0f;
            for (int i = 0; i < 3; i++)
            {
                float num3 = System.Math.Abs(Minimum[i]);
                float num4 = System.Math.Abs(Maximum[i]);
                if (num3 > num4)
                {
                    x += num3 * num3;
                }
                else
                {
                    x += num4 * num4;
                }
            }
            return MathFunctions.Sqrt(x);
        }

        public float GetRadius(Vec3 center)
        {
            float x = 0f;
            for (int i = 0; i < 3; i++)
            {
                float num3 = System.Math.Abs(center[i] - Minimum[i]);
                float num4 = System.Math.Abs(Maximum[i] - center[i]);
                if (num3 > num4)
                {
                    x += num3 * num3;
                }
                else
                {
                    x += num4 * num4;
                }
            }
            return MathFunctions.Sqrt(x);
        }

        public float GetVolume()
        {
            Vec3.Subtract(ref rG, ref rf, out Vec3 vec);
            return ((vec.X * vec.Y) * vec.Z);
        }

        public void Add(Vec3 v)
        {
            if (v.X < rf.X)
            {
                rf.X = v.X;
            }
            if (v.X > rG.X)
            {
                rG.X = v.X;
            }
            if (v.Y < rf.Y)
            {
                rf.Y = v.Y;
            }
            if (v.Y > rG.Y)
            {
                rG.Y = v.Y;
            }
            if (v.Z < rf.Z)
            {
                rf.Z = v.Z;
            }
            if (v.Z > rG.Z)
            {
                rG.Z = v.Z;
            }
        }

        public void Add(ref Vec3 v)
        {
            if (v.X < rf.X)
            {
                rf.X = v.X;
            }
            if (v.X > rG.X)
            {
                rG.X = v.X;
            }
            if (v.Y < rf.Y)
            {
                rf.Y = v.Y;
            }
            if (v.Y > rG.Y)
            {
                rG.Y = v.Y;
            }
            if (v.Z < rf.Z)
            {
                rf.Z = v.Z;
            }
            if (v.Z > rG.Z)
            {
                rG.Z = v.Z;
            }
        }

        public void Add(Bounds v)
        {
            if (v.rf.X < rf.X)
            {
                rf.X = v.rf.X;
            }
            if (v.rf.Y < rf.Y)
            {
                rf.Y = v.rf.Y;
            }
            if (v.rf.Z < rf.Z)
            {
                rf.Z = v.rf.Z;
            }
            if (v.rG.X > rG.X)
            {
                rG.X = v.rG.X;
            }
            if (v.rG.Y > rG.Y)
            {
                rG.Y = v.rG.Y;
            }
            if (v.rG.Z > rG.Z)
            {
                rG.Z = v.rG.Z;
            }
        }

        public void Add(ref Bounds v)
        {
            if (v.rf.X < rf.X)
            {
                rf.X = v.rf.X;
            }
            if (v.rf.Y < rf.Y)
            {
                rf.Y = v.rf.Y;
            }
            if (v.rf.Z < rf.Z)
            {
                rf.Z = v.rf.Z;
            }
            if (v.rG.X > rG.X)
            {
                rG.X = v.rG.X;
            }
            if (v.rG.Y > rG.Y)
            {
                rG.Y = v.rG.Y;
            }
            if (v.rG.Z > rG.Z)
            {
                rG.Z = v.rG.Z;
            }
        }

        public Bounds Intersect(Bounds v)
        {
            Bounds bounds;
            bounds.rf.x = (v.rf.X > rf.X) ? v.rf.X : rf.X;
            bounds.rf.y = (v.rf.Y > rf.Y) ? v.rf.Y : rf.Y;
            bounds.rf.z = (v.rf.Z > rf.Z) ? v.rf.Z : rf.Z;
            bounds.rG.x = (v.rG.X < rG.X) ? v.rG.X : rG.X;
            bounds.rG.y = (v.rG.Y < rG.Y) ? v.rG.Y : rG.Y;
            bounds.rG.z = (v.rG.Z < rG.Z) ? v.rG.Z : rG.Z;
            return bounds;
        }

        public void Intersect(ref Bounds v, out Bounds result)
        {
            result.rf.x = (v.rf.X > rf.X) ? v.rf.X : rf.X;
            result.rf.y = (v.rf.Y > rf.Y) ? v.rf.Y : rf.Y;
            result.rf.z = (v.rf.Z > rf.Z) ? v.rf.Z : rf.Z;
            result.rG.x = (v.rG.X < rG.X) ? v.rG.X : rG.X;
            result.rG.y = (v.rG.Y < rG.Y) ? v.rG.Y : rG.Y;
            result.rG.z = (v.rG.Z < rG.Z) ? v.rG.Z : rG.Z;
        }

        public void Expand(float d)
        {
            rf.X -= d;
            rf.Y -= d;
            rf.Z -= d;
            rG.X += d;
            rG.Y += d;
            rG.Z += d;
        }

        public void Expand(Vec3 d)
        {
            rf.X -= d.X;
            rf.Y -= d.Y;
            rf.Z -= d.Z;
            rG.X += d.X;
            rG.Y += d.Y;
            rG.Z += d.Z;
        }

        public bool IsContainsPoint(Vec3 p)
        {
            return ((((p.X >= rf.X) && (p.Y >= rf.Y)) && ((p.Z >= rf.Z) && (p.X <= rG.X))) && ((p.Y <= rG.Y) && (p.Z <= rG.Z)));
        }

        public bool IsContainsBounds(Bounds v)
        {
            return ((((v.rf.X >= rf.X) && (v.rf.Y >= rf.Y)) && ((v.rf.Z >= rf.Z) && (v.rG.X <= rG.X))) && ((v.rG.Y <= rG.Y) && (v.rG.Z <= rG.Z)));
        }

        public bool IsContainsBounds(ref Bounds v)
        {
            return ((((v.rf.X >= rf.X) && (v.rf.Y >= rf.Y)) && ((v.rf.Z >= rf.Z) && (v.rG.X <= rG.X))) && ((v.rG.Y <= rG.Y) && (v.rG.Z <= rG.Z)));
        }

        public bool IsIntersectsBounds(Bounds v)
        {
            return ((((v.rG.X >= rf.X) && (v.rG.Y >= rf.Y)) && ((v.rG.Z >= rf.Z) && (v.rf.X <= rG.X))) && ((v.rf.Y <= rG.Y) && (v.rf.Z <= rG.Z)));
        }

        public bool IsIntersectsBounds(ref Bounds v)
        {
            return ((((v.rG.X >= rf.X) && (v.rG.Y >= rf.Y)) && ((v.rG.Z >= rf.Z) && (v.rf.X <= rG.X))) && ((v.rf.Y <= rG.Y) && (v.rf.Z <= rG.Z)));
        }

        public Vec3 GetSize()
        {
            Vec3.Subtract(ref rG, ref rf, out Vec3 vec);
            return vec;
        }

        public void GetSize(out Vec3 result)
        {
            Vec3.Subtract(ref rG, ref rf, out result);
        }

        public void ToPoints(ref Vec3[] points)
        {
            if ((points == null) || (points.Length < 8))
            {
                points = new Vec3[8];
            }
            for (int i = 0; i < 8; i++)
            {
                Vec3 vec = this[(i ^ (i >> 1)) & 1];
                points[i].X = vec.X;
                Vec3 vec2 = this[(i >> 1) & 1];
                points[i].Y = vec2.Y;
                Vec3 vec3 = this[(i >> 2) & 1];
                points[i].Z = vec3.Z;
            }
        }

        public static Bounds Parse(string text)
        {
            Bounds bounds;
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException("The text parameter cannot be null or zero length.");
            }
            string[] strArray = text.Split(SpaceCharacter.arrayWithOneSpaceCharacter, StringSplitOptions.RemoveEmptyEntries);
            if (strArray.Length != 6)
            {
                throw new FormatException(string.Format("Cannot parse the text '{0}' because it does not have 6 parts separated by spaces in the form (x y z x y z).", text));
            }
            try
            {
                bounds = new Bounds(float.Parse(strArray[0]), float.Parse(strArray[1]), float.Parse(strArray[2]), float.Parse(strArray[3]), float.Parse(strArray[4]), float.Parse(strArray[5]));
            }
            catch (Exception)
            {
                throw new FormatException("The parts of the vectors must be decimal numbers.");
            }
            return bounds;
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3} {4} {5}", new object[] { Minimum.X, Minimum.Y, Minimum.Z, Maximum.X, Maximum.Y, Maximum.Z });
        }

        public bool RayIntersection(ref Ray ray, out float scale)
        {
            Vec3 zero = Vec3.Zero;
            scale = 0f;
            int num2 = -1;
            int num6 = 0;
            for (int i = 0; i < 3; i++)
            {
                int num5;
                if (ray.ri[i] < Minimum[i])
                {
                    num5 = 0;
                }
                else if (ray.ri[i] > Maximum[i])
                {
                    num5 = 1;
                }
                else
                {
                    num6++;
                    continue;
                }
                if (ray.rJ[i] != 0f)
                {
                    Vec3 vec4 = this[num5];
                    float num7 = ray.ri[i] - vec4[i];
                    if ((num2 < 0) || (System.Math.Abs(num7) > System.Math.Abs(scale * ray.rJ[i])))
                    {
                        scale = -(num7 / ray.rJ[i]);
                        num2 = i;
                    }
                }
            }
            if ((scale < 0f) || (scale > 1f))
            {
                return false;
            }
            if (num2 < 0)
            {
                scale = 0f;
                return (num6 == 3);
            }
            int num3 = (num2 + 1) % 3;
            int num4 = (num2 + 2) % 3;
            zero[num3] = ray.ri[num3] + (scale * ray.rJ[num3]);
            zero[num4] = ray.ri[num4] + (scale * ray.rJ[num4]);
            return ((((zero[num3] >= Minimum[num3]) && (zero[num3] <= Maximum[num3])) && (zero[num4] >= Minimum[num4])) && (zero[num4] <= Maximum[num4]));
        }

        public bool RayIntersection(Ray ray, out float scale)
        {
            return RayIntersection(ref ray, out scale);
        }

        public bool RayIntersection(ref Ray ray)
        {
            return RayIntersection(ref ray, out float num);
        }

        public bool RayIntersection(Ray ray)
        {
            return RayIntersection(ref ray, out float num);
        }

        public static Bounds operator +(Bounds b, Vec3 v)
        {
            Bounds bounds;
            Vec3.Add(ref b.rf, ref v, out bounds.rf);
            Vec3.Add(ref b.rG, ref v, out bounds.rG);
            return bounds;
        }

        public static Bounds operator +(Vec3 v, Bounds b)
        {
            Bounds bounds;
            Vec3.Add(ref v, ref b.rf, out bounds.rf);
            Vec3.Add(ref v, ref b.rG, out bounds.rG);
            return bounds;
        }

        public static Bounds operator -(Bounds b, Vec3 v)
        {
            Bounds bounds;
            Vec3.Subtract(ref b.rf, ref v, out bounds.rf);
            Vec3.Subtract(ref b.rG, ref v, out bounds.rG);
            return bounds;
        }

        public static Bounds operator -(Vec3 v, Bounds b)
        {
            Bounds bounds;
            Vec3.Subtract(ref v, ref b.rf, out bounds.rf);
            Vec3.Subtract(ref v, ref b.rG, out bounds.rG);
            return bounds;
        }

        public static void Add(ref Bounds b, ref Vec3 v, out Bounds result)
        {
            Vec3.Add(ref b.rf, ref v, out result.rf);
            Vec3.Add(ref b.rG, ref v, out result.rG);
        }

        public static void Add(ref Vec3 v, ref Bounds b, out Bounds result)
        {
            Vec3.Add(ref v, ref b.rf, out result.rf);
            Vec3.Add(ref v, ref b.rG, out result.rG);
        }

        public static void Subtract(ref Bounds b, ref Vec3 v, out Bounds result)
        {
            Vec3.Subtract(ref b.rf, ref v, out result.rf);
            Vec3.Subtract(ref b.rG, ref v, out result.rG);
        }

        public static void Subtract(ref Vec3 v, ref Bounds b, out Bounds result)
        {
            Vec3.Subtract(ref v, ref b.rf, out result.rf);
            Vec3.Subtract(ref v, ref b.rG, out result.rG);
        }

        public static Bounds Add(ref Bounds b, ref Vec3 v)
        {
            Bounds bounds;
            Vec3.Add(ref b.rf, ref v, out bounds.rf);
            Vec3.Add(ref b.rG, ref v, out bounds.rG);
            return bounds;
        }

        public static Bounds Add(ref Vec3 v, ref Bounds b)
        {
            Bounds bounds;
            Vec3.Add(ref v, ref b.rf, out bounds.rf);
            Vec3.Add(ref v, ref b.rG, out bounds.rG);
            return bounds;
        }

        public static Bounds Subtract(ref Bounds b, ref Vec3 v)
        {
            Bounds bounds;
            Vec3.Subtract(ref b.rf, ref v, out bounds.rf);
            Vec3.Subtract(ref b.rG, ref v, out bounds.rG);
            return bounds;
        }

        public static Bounds Subtract(ref Vec3 v, ref Bounds b)
        {
            Bounds bounds;
            Vec3.Subtract(ref v, ref b.rf, out bounds.rf);
            Vec3.Subtract(ref v, ref b.rG, out bounds.rG);
            return bounds;
        }

        public Plane.Side GetPlaneSide(ref Plane plane)
        {
            GetCenter(out Vec3 vec);
            float distance = plane.GetDistance(ref vec);
            float num2 = (System.Math.Abs((Maximum.X - vec.X) * plane.RI) + System.Math.Abs((Maximum.Y - vec.Y) * plane.Ri)) + System.Math.Abs((Maximum.Z - vec.Z) * plane.RJ);
            if ((distance - num2) > 0f)
            {
                return Plane.Side.Positive;
            }
            if ((distance + num2) < 0f)
            {
                return Plane.Side.Negative;
            }
            return Plane.Side.No;
        }

        public Plane.Side GetPlaneSide(Plane plane)
        {
            return GetPlaneSide(ref plane);
        }

        public float GetPlaneDistance(ref Plane plane)
        {
            GetCenter(out Vec3 vec);
            float distance = plane.GetDistance(ref vec);
            float num2 = (System.Math.Abs((Maximum.X - vec.X) * plane.Normal.X) + System.Math.Abs((Maximum.Y - vec.Y) * plane.Normal.Y)) + System.Math.Abs((Maximum.Z - vec.Z) * plane.Normal.Z);
            if ((distance - num2) > 0f)
            {
                return (distance - num2);
            }
            if ((distance + num2) < 0f)
            {
                return (distance + num2);
            }
            return 0f;
        }

        public float GetPlaneDistance(Plane plane)
        {
            return GetPlaneDistance(ref plane);
        }

        internal bool A(ref Vec3 A, ref Vec3 a)
        {
            Vec3 vec3;
            GetCenter(out Vec3 vec);
            Vec3.Subtract(ref rG, ref vec, out Vec3 vec2);
            vec3.x = 0.5f * (a.x - A.x);
            vec3.y = 0.5f * (a.y - A.y);
            vec3.z = 0.5f * (a.z - A.z);
            Vec3.Add(ref A, ref vec3, out Vec3 vec4);
            Vec3.Subtract(ref vec4, ref vec, out Vec3 vec5);
            float num = System.Math.Abs(vec3.X);
            if (System.Math.Abs(vec5.X) > (vec2.X + num))
            {
                return false;
            }
            float num2 = System.Math.Abs(vec3.Y);
            if (System.Math.Abs(vec5.Y) > (vec2.Y + num2))
            {
                return false;
            }
            float num3 = System.Math.Abs(vec3.Z);
            if (System.Math.Abs(vec5.Z) > (vec2.Z + num3))
            {
                return false;
            }
            Vec3.Cross(ref vec3, ref vec5, out Vec3 vec6);
            if (System.Math.Abs(vec6.X) > ((vec2.Y * num3) + (vec2.Z * num2)))
            {
                return false;
            }
            if (System.Math.Abs(vec6.Y) > ((vec2.X * num3) + (vec2.Z * num)))
            {
                return false;
            }
            if (System.Math.Abs(vec6.Z) > ((vec2.X * num2) + (vec2.Y * num)))
            {
                return false;
            }
            return true;
        }

        public Rect ToRect()
        {
            Rect rect;
            rect.rA = rf.X;
            rect.ra = rf.Y;
            rect.rB = rG.X;
            rect.rb = rG.Y;
            return rect;
        }

        public void ToRect(out Rect result)
        {
            result.rA = rf.X;
            result.ra = rf.Y;
            result.rB = rG.X;
            result.rb = rG.Y;
        }

        public float GetPointDistance(Vec3 point)
        {
            float num;
            float num2;
            float num3;
            if (point.x < rf.x)
            {
                num = rf.x - point.x;
            }
            else if (point.x > rG.x)
            {
                num = point.x - rG.x;
            }
            else
            {
                num = 0f;
            }
            if (point.y < rf.y)
            {
                num2 = rf.y - point.y;
            }
            else if (point.y > rG.y)
            {
                num2 = point.y - rG.y;
            }
            else
            {
                num2 = 0f;
            }
            if (point.z < rf.z)
            {
                num3 = rf.z - point.z;
            }
            else if (point.z > rG.z)
            {
                num3 = point.z - rG.z;
            }
            else
            {
                num3 = 0f;
            }
            float x = ((num * num) + (num2 * num2)) + (num3 * num3);
            if (x == 0f)
            {
                return 0f;
            }
            return MathFunctions.Sqrt(x);
        }
    }
}
