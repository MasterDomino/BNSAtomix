﻿using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace SmartEngine.Core.Math
{
    [StructLayout(LayoutKind.Sequential), TypeConverter(typeof(_GeneralTypeConverter<Plane>))]
    public struct Plane
    {
        internal float RI;
        internal float Ri;
        internal float RJ;
        internal float Rj;
        public Plane(Plane source)
        {
            RI = source.RI;
            Ri = source.Ri;
            RJ = source.RJ;
            Rj = source.Rj;
        }

        public Plane(float a, float b, float c, float d)
        {
            RI = a;
            Ri = b;
            RJ = c;
            Rj = d;
        }

        public Plane(Vec3 normal, float distance)
        {
            RI = normal.X;
            Ri = normal.Y;
            RJ = normal.Z;
            Rj = -distance;
        }

        public static Plane Parse(string text)
        {
            Plane plane;
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException("The text parameter cannot be null or zero length.");
            }
            string[] strArray = text.Split(SpaceCharacter.arrayWithOneSpaceCharacter, StringSplitOptions.RemoveEmptyEntries);
            if (strArray.Length != 4)
            {
                throw new FormatException(string.Format("Cannot parse the text '{0}' because it does not have 4 parts separated by spaces in the form (x y z w).", text));
            }
            try
            {
                plane = new Plane(float.Parse(strArray[0]), float.Parse(strArray[1]), float.Parse(strArray[2]), float.Parse(strArray[3]));
            }
            catch (Exception)
            {
                throw new FormatException("The parts of the vectors must be decimal numbers.");
            }
            return plane;
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3}", new object[] { RI, Ri, RJ, Rj });
        }

        public string ToString(int precision)
        {
            string str = string.Empty;
            str = str.PadLeft(precision, '#');
            return string.Format("{0:0." + str + "} {1:0." + str + "} {2:0." + str + "} {3:0." + str + "}", new object[] { RI, Ri, RJ, Rj });
        }

        public override bool Equals(object obj)
        {
            return ((obj is Plane) && (this == ((Plane)obj)));
        }

        public override int GetHashCode()
        {
            return (((RI.GetHashCode() ^ Ri.GetHashCode()) ^ RJ.GetHashCode()) ^ Rj.GetHashCode());
        }

        public static Plane operator +(Plane p0, Plane p1)
        {
            Plane plane;
            plane.RI = p0.RI + p1.RI;
            plane.Ri = p0.Ri + p1.Ri;
            plane.RJ = p0.RJ + p1.RJ;
            plane.Rj = p0.Rj + p1.Rj;
            return plane;
        }

        public static Plane operator -(Plane p0, Plane p1)
        {
            Plane plane;
            plane.RI = p0.RI - p1.RI;
            plane.Ri = p0.Ri - p1.Ri;
            plane.RJ = p0.RJ - p1.RJ;
            plane.Rj = p0.Rj - p1.Rj;
            return plane;
        }

        public static Plane operator -(Plane p)
        {
            Plane plane;
            plane.RI = -p.RI;
            plane.Ri = -p.Ri;
            plane.RJ = -p.RJ;
            plane.Rj = -p.Rj;
            return plane;
        }

        public static void Add(Plane p0, Plane p1, out Plane result)
        {
            result.RI = p0.RI + p1.RI;
            result.Ri = p0.Ri + p1.Ri;
            result.RJ = p0.RJ + p1.RJ;
            result.Rj = p0.Rj + p1.Rj;
        }

        public static void Subtract(Plane p0, Plane p1, out Plane result)
        {
            result.RI = p0.RI - p1.RI;
            result.Ri = p0.Ri - p1.Ri;
            result.RJ = p0.RJ - p1.RJ;
            result.Rj = p0.Rj - p1.Rj;
        }

        public static void Negate(ref Plane p, out Plane result)
        {
            result.RI = -p.RI;
            result.Ri = -p.Ri;
            result.RJ = -p.RJ;
            result.Rj = -p.Rj;
        }

        public static Plane Add(Plane p0, Plane p1)
        {
            Plane plane;
            plane.RI = p0.RI + p1.RI;
            plane.Ri = p0.Ri + p1.Ri;
            plane.RJ = p0.RJ + p1.RJ;
            plane.Rj = p0.Rj + p1.Rj;
            return plane;
        }

        public static Plane Subtract(Plane p0, Plane p1)
        {
            Plane plane;
            plane.RI = p0.RI - p1.RI;
            plane.Ri = p0.Ri - p1.Ri;
            plane.RJ = p0.RJ - p1.RJ;
            plane.Rj = p0.Rj - p1.Rj;
            return plane;
        }

        public static Plane Negate(Plane p)
        {
            Plane plane;
            plane.RI = -p.RI;
            plane.Ri = -p.Ri;
            plane.RJ = -p.RJ;
            plane.Rj = -p.Rj;
            return plane;
        }

        public static bool operator ==(Plane p0, Plane p1)
        {
            return ((((p0.RI == p1.RI) && (p0.Ri == p1.Ri)) && (p0.RJ == p1.RJ)) && (p0.Rj == p1.Rj));
        }

        public static bool operator !=(Plane p0, Plane p1)
        {
            if (((p0.RI == p1.RI) && (p0.Ri == p1.Ri)) && (p0.RJ == p1.RJ))
            {
                return (p0.Rj != p1.Rj);
            }
            return true;
        }

        public unsafe float this[int index]
        {
            get
            {
                if ((index < 0) || (index > 3))
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                fixed (float* numRef = &RI)
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
                fixed (float* numRef = &RI)
                {
                    numRef[index * 4] = value;
                }
            }
        }

        public bool Equals(Plane p, float epsilon)
        {
            if (System.Math.Abs(RI - p.RI) > epsilon)
            {
                return false;
            }
            if (System.Math.Abs(Ri - p.Ri) > epsilon)
            {
                return false;
            }
            if (System.Math.Abs(RJ - p.RJ) > epsilon)
            {
                return false;
            }
            if (System.Math.Abs(Rj - p.Rj) > epsilon)
            {
                return false;
            }
            return true;
        }

        public bool Equals(ref Plane p, float epsilon)
        {
            if (System.Math.Abs(RI - p.RI) > epsilon)
            {
                return false;
            }
            if (System.Math.Abs(Ri - p.Ri) > epsilon)
            {
                return false;
            }
            if (System.Math.Abs(RJ - p.RJ) > epsilon)
            {
                return false;
            }
            if (System.Math.Abs(Rj - p.Rj) > epsilon)
            {
                return false;
            }
            return true;
        }

        public bool Equals(Plane p, float normalEpsilon, float distanceEpsilon)
        {
            if (System.Math.Abs(Rj - p.Rj) > distanceEpsilon)
            {
                return false;
            }
            Vec3 normal = Normal;
            Vec3 v = p.Normal;
            if (!normal.Equals(ref v, normalEpsilon))
            {
                return false;
            }
            return true;
        }

        public bool Equals(ref Plane p, float normalEpsilon, float distanceEpsilon)
        {
            if (System.Math.Abs(Rj - p.Rj) > distanceEpsilon)
            {
                return false;
            }
            Vec3 normal = Normal;
            Vec3 v = p.Normal;
            if (!normal.Equals(ref v, normalEpsilon))
            {
                return false;
            }
            return true;
        }

        public Vec3 Normal
        {
            get
            {
                Vec3 vec;
                vec.x = RI;
                vec.y = Ri;
                vec.z = RJ;
                return vec;
            }
            set
            {
                RI = value.X;
                Ri = value.Y;
                RJ = value.Z;
            }
        }

        public float Distance
        {
            get
            {
                return -Rj;
            }
            set
            {
                Rj = -value;
            }
        }

        public float Normalize()
        {
            Vec3 normal = Normal;
            float num = normal.Normalize();
            Normal = normal;
            return num;
        }

        public Plane GetNormalize()
        {
            Plane plane;
            Vec3 normal = Normal;
            normal.Normalize();
            plane.RI = normal.x;
            plane.Ri = normal.y;
            plane.RJ = normal.z;
            plane.Rj = Rj;
            return plane;
        }

        public float GetDistance(Vec3 v)
        {
            return ((((RI * v.X) + (Ri * v.Y)) + (RJ * v.Z)) + Rj);
        }

        public float GetDistance(ref Vec3 v)
        {
            return ((((RI * v.X) + (Ri * v.Y)) + (RJ * v.Z)) + Rj);
        }

        public Side GetSide(Vec3 point)
        {
            float distance = GetDistance(ref point);
            if (distance > 0f)
            {
                return Side.Positive;
            }
            if (distance < 0f)
            {
                return Side.Negative;
            }
            return Side.No;
        }

        public Side GetSide(ref Vec3 point)
        {
            float distance = GetDistance(ref point);
            if (distance > 0f)
            {
                return Side.Positive;
            }
            if (distance < 0f)
            {
                return Side.Negative;
            }
            return Side.No;
        }

        public Side GetSide(Vec3 point, float epsilon)
        {
            float distance = GetDistance(ref point);
            if (distance > epsilon)
            {
                return Side.Positive;
            }
            if (distance < -epsilon)
            {
                return Side.Negative;
            }
            return Side.No;
        }

        public Side GetSide(ref Vec3 point, float epsilon)
        {
            float distance = GetDistance(ref point);
            if (distance > epsilon)
            {
                return Side.Positive;
            }
            if (distance < -epsilon)
            {
                return Side.Negative;
            }
            return Side.No;
        }

        public Vec4 ToVec4()
        {
            Vec4 vec;
            vec.x = RI;
            vec.y = Ri;
            vec.z = RJ;
            vec.w = Rj;
            return vec;
        }

        public static Plane FromPoints(Vec3 point0, Vec3 point1, Vec3 point2)
        {
            FromPoints(ref point0, ref point1, ref point2, out Plane plane);
            return plane;
        }

        public static void FromPoints(ref Vec3 point0, ref Vec3 point1, ref Vec3 point2, out Plane result)
        {
            Vec3.Subtract(ref point1, ref point0, out Vec3 vec);
            Vec3.Subtract(ref point2, ref point0, out Vec3 vec2);
            Vec3.Cross(ref vec, ref vec2, out Vec3 vec3);
            vec3.Normalize();
            result.RI = vec3.x;
            result.Ri = vec3.y;
            result.RJ = vec3.z;
            result.Rj = -(((vec3.X * point0.X) + (vec3.Y * point0.Y)) + (vec3.Z * point0.Z));
        }

        public static Plane FromVectors(Vec3 dir1, Vec3 dir2, Vec3 p)
        {
            Plane plane;
            Vec3.Cross(ref dir1, ref dir2, out Vec3 vec);
            vec.Normalize();
            plane.RI = vec.x;
            plane.Ri = vec.y;
            plane.RJ = vec.z;
            plane.Rj = -(((vec.X * p.X) + (vec.Y * p.Y)) + (vec.Z * p.Z));
            return plane;
        }

        public static void FromVectors(ref Vec3 dir1, ref Vec3 dir2, ref Vec3 p, out Plane result)
        {
            Vec3.Cross(ref dir1, ref dir2, out Vec3 vec);
            vec.Normalize();
            result.RI = vec.x;
            result.Ri = vec.y;
            result.RJ = vec.z;
            result.Rj = -(((vec.X * p.X) + (vec.Y * p.Y)) + (vec.Z * p.Z));
        }

        public static Plane FromPointAndNormal(Vec3 point, Vec3 normal)
        {
            return new Plane(normal, Vec3.A(ref point, ref normal));
        }

        public static void FromPointAndNormal(ref Vec3 point, ref Vec3 normal, out Plane plane)
        {
            float distance = Vec3.A(ref point, ref normal);
            plane = new Plane(normal, distance);
        }

        public bool RayIntersection(Ray ray, out float scale)
        {
            float num = (((RI * ray.ri.x) + (Ri * ray.ri.y)) + (RJ * ray.ri.z)) + Rj;
            float num2 = ((RI * ray.rJ.x) + (Ri * ray.rJ.y)) + (RJ * ray.rJ.z);
            if (num2 == 0f)
            {
                scale = 0f;
                return false;
            }
            scale = -(num / num2);
            return true;
        }

        public bool RayIntersection(Ray ray, out Vec3 intersectionPoint)
        {
            float num = (((RI * ray.ri.x) + (Ri * ray.ri.y)) + (RJ * ray.ri.z)) + Rj;
            float num2 = ((RI * ray.rJ.x) + (Ri * ray.rJ.y)) + (RJ * ray.rJ.z);
            if (num2 == 0f)
            {
                intersectionPoint = Vec3.Zero;
                return false;
            }
            float t = -(num / num2);
            intersectionPoint = ray.GetPointOnRay(t);
            return true;
        }

        public bool LineIntersection(ref Vec3 start, ref Vec3 end, out float scale)
        {
            scale = 0f;
            float num = (((RI * start.x) + (Ri * start.y)) + (RJ * start.z)) + Rj;
            float num2 = (((RI * end.x) + (Ri * end.y)) + (RJ * end.z)) + Rj;
            if (num == num2)
            {
                return false;
            }
            if ((num > 0f) && (num2 > 0f))
            {
                return false;
            }
            if ((num < 0f) && (num2 < 0f))
            {
                return false;
            }
            float num3 = num / (num - num2);
            if ((num3 < 0f) || (num3 > 1f))
            {
                return false;
            }
            scale = num3;
            return true;
        }

        public bool LineIntersection(Vec3 start, Vec3 end, out float scale)
        {
            return LineIntersection(ref start, ref end, out scale);
        }

        public bool LineIntersection(Vec3 start, Vec3 end)
        {
            return LineIntersection(start, end, out float num);
        }

        /// <summary>
        /// Returns which side of the plane that the given box lies on.
        /// The box is defined as centre/half-size pairs for effectively.
        /// </summary>
        /// <param name="boundsCenter"></param>
        /// <param name="boundsHalfSize"></param>
        /// <returns></returns>
        public Side GetSide(ref Vec3 boundsCenter, ref Vec3 boundsHalfSize)
        {
            float num = (((RI * boundsCenter.x) + (Ri * boundsCenter.y)) + (RJ * boundsCenter.z)) + Rj;
            float num2 = (System.Math.Abs(RI * boundsHalfSize.x) + System.Math.Abs(Ri * boundsHalfSize.y)) + System.Math.Abs(RJ * boundsHalfSize.z);
            if (num < -num2)
            {
                return Side.Negative;
            }
            if (num > num2)
            {
                return Side.Positive;
            }
            return Side.No;
        }

        /// <summary>
        /// Returns which side of the plane that the given box lies on.
        /// The box is defined as centre/half-size pairs for effectively.
        /// </summary>
        /// <param name="boundsCenter"></param>
        /// <param name="boundsHalfSize"></param>
        /// <returns></returns>
        public Side GetSide(Vec3 boundsCenter, Vec3 boundsHalfSize)
        {
            return GetSide(ref boundsCenter, ref boundsHalfSize);
        }

        public Side GetSide(ref Bounds bounds)
        {
            bounds.GetCenter(out Vec3 vec);
            Vec3.Subtract(ref vec, ref bounds.rf, out Vec3 vec2);
            return GetSide(ref vec, ref vec2);
        }

        public Side GetSide(Bounds bounds)
        {
            return GetSide(ref bounds);
        }

        public enum Side
        {
            Negative,
            No,
            Positive
        }
    }
}
