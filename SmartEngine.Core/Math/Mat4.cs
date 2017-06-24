using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace SmartEngine.Core.Math
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Mat4
    {
        public static readonly Mat4 Zero;
        public static readonly Mat4 Identity;
        internal Vec4 Rp;
        internal Vec4 RQ;
        internal Vec4 Rq;
        internal Vec4 RR;
        public Mat4(float xx, float xy, float xz, float xw, float yx, float yy, float yz, float yw, float zx, float zy, float zz, float zw, float wx, float wy, float wz, float ww)
        {
            Rp = new Vec4(xx, xy, xz, xw);
            RQ = new Vec4(yx, yy, yz, yw);
            Rq = new Vec4(zx, zy, zz, zw);
            RR = new Vec4(wx, wy, wz, ww);
        }

        public Mat4(Vec4 x, Vec4 y, Vec4 z, Vec4 w)
        {
            Rp = x;
            RQ = y;
            Rq = z;
            RR = z;
        }

        public Mat4(Mat4 source)
        {
            Rp = source.Rp;
            RQ = source.RQ;
            Rq = source.Rq;
            RR = source.RR;
        }

        public Mat4(Mat3 rotation, Vec3 translation)
        {
            Rp.x = rotation.Rt.x;
            Rp.y = rotation.Rt.y;
            Rp.z = rotation.Rt.z;
            Rp.w = 0f;
            RQ.x = rotation.RU.x;
            RQ.y = rotation.RU.y;
            RQ.z = rotation.RU.z;
            RQ.w = 0f;
            Rq.x = rotation.Ru.x;
            Rq.y = rotation.Ru.y;
            Rq.z = rotation.Ru.z;
            Rq.w = 0f;
            RR.x = translation.x;
            RR.y = translation.y;
            RR.z = translation.z;
            RR.w = 1f;
        }

        public Mat4(float[] array)
        {
            Rp = new Vec4(array[0], array[1], array[2], array[3]);
            RQ = new Vec4(array[4], array[5], array[6], array[7]);
            Rq = new Vec4(array[8], array[9], array[10], array[11]);
            RR = new Vec4(array[12], array[13], array[14], array[15]);
        }

        static Mat4()
        {
            Zero = new Mat4(0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f);
            Identity = new Mat4(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f);
        }

        public unsafe Vec4 this[int index]
        {
            get
            {
                if ((index < 0) || (index > 3))
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                fixed (Vec4* vecRef = &Rp)
                {
                    return vecRef[index];
                }
            }
            set
            {
                if ((index < 0) || (index > 3))
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                fixed (Vec4* vecRef = &Rp)
                {
                    vecRef[index] = value;
                }
            }
        }

        public override int GetHashCode()
        {
            return (((Rp.GetHashCode() ^ RQ.GetHashCode()) ^ Rq.GetHashCode()) ^ RR.GetHashCode());
        }

        public static Mat4 operator +(Mat4 v1, Mat4 v2)
        {
            Mat4 mat;
            mat.Rp.x = v1.Rp.x + v2.Rp.x;
            mat.Rp.y = v1.Rp.y + v2.Rp.y;
            mat.Rp.z = v1.Rp.z + v2.Rp.z;
            mat.Rp.w = v1.Rp.w + v2.Rp.w;
            mat.RQ.x = v1.RQ.x + v2.RQ.x;
            mat.RQ.y = v1.RQ.y + v2.RQ.y;
            mat.RQ.z = v1.RQ.z + v2.RQ.z;
            mat.RQ.w = v1.RQ.w + v2.RQ.w;
            mat.Rq.x = v1.Rq.x + v2.Rq.x;
            mat.Rq.y = v1.Rq.y + v2.Rq.y;
            mat.Rq.z = v1.Rq.z + v2.Rq.z;
            mat.Rq.w = v1.Rq.w + v2.Rq.w;
            mat.RR.x = v1.RR.x + v2.RR.x;
            mat.RR.y = v1.RR.y + v2.RR.y;
            mat.RR.z = v1.RR.z + v2.RR.z;
            mat.RR.w = v1.RR.w + v2.RR.w;
            return mat;
        }

        public static Mat4 operator -(Mat4 v1, Mat4 v2)
        {
            Mat4 mat;
            mat.Rp.x = v1.Rp.x - v2.Rp.x;
            mat.Rp.y = v1.Rp.y - v2.Rp.y;
            mat.Rp.z = v1.Rp.z - v2.Rp.z;
            mat.Rp.w = v1.Rp.w - v2.Rp.w;
            mat.RQ.x = v1.RQ.x - v2.RQ.x;
            mat.RQ.y = v1.RQ.y - v2.RQ.y;
            mat.RQ.z = v1.RQ.z - v2.RQ.z;
            mat.RQ.w = v1.RQ.w - v2.RQ.w;
            mat.Rq.x = v1.Rq.x - v2.Rq.x;
            mat.Rq.y = v1.Rq.y - v2.Rq.y;
            mat.Rq.z = v1.Rq.z - v2.Rq.z;
            mat.Rq.w = v1.Rq.w - v2.Rq.w;
            mat.RR.x = v1.RR.x - v2.RR.x;
            mat.RR.y = v1.RR.y - v2.RR.y;
            mat.RR.z = v1.RR.z - v2.RR.z;
            mat.RR.w = v1.RR.w - v2.RR.w;
            return mat;
        }

        public static Mat4 operator *(Mat4 m, float s)
        {
            Mat4 mat;
            mat.Rp.x = m.Rp.x * s;
            mat.Rp.y = m.Rp.y * s;
            mat.Rp.z = m.Rp.z * s;
            mat.Rp.w = m.Rp.w * s;
            mat.RQ.x = m.RQ.x * s;
            mat.RQ.y = m.RQ.y * s;
            mat.RQ.z = m.RQ.z * s;
            mat.RQ.w = m.RQ.w * s;
            mat.Rq.x = m.Rq.x * s;
            mat.Rq.y = m.Rq.y * s;
            mat.Rq.z = m.Rq.z * s;
            mat.Rq.w = m.Rq.w * s;
            mat.RR.x = m.RR.x * s;
            mat.RR.y = m.RR.y * s;
            mat.RR.z = m.RR.z * s;
            mat.RR.w = m.RR.w * s;
            return mat;
        }

        public static Mat4 operator *(float s, Mat4 m)
        {
            Mat4 mat;
            mat.Rp.x = m.Rp.x * s;
            mat.Rp.y = m.Rp.y * s;
            mat.Rp.z = m.Rp.z * s;
            mat.Rp.w = m.Rp.w * s;
            mat.RQ.x = m.RQ.x * s;
            mat.RQ.y = m.RQ.y * s;
            mat.RQ.z = m.RQ.z * s;
            mat.RQ.w = m.RQ.w * s;
            mat.Rq.x = m.Rq.x * s;
            mat.Rq.y = m.Rq.y * s;
            mat.Rq.z = m.Rq.z * s;
            mat.Rq.w = m.Rq.w * s;
            mat.RR.x = m.RR.x * s;
            mat.RR.y = m.RR.y * s;
            mat.RR.z = m.RR.z * s;
            mat.RR.w = m.RR.w * s;
            return mat;
        }

        public static Ray operator *(Mat4 m, Ray r)
        {
            Ray ray;
            Multiply(ref r.ri, ref m, out ray.ri);
            Vec3.Add(ref r.ri, ref r.rJ, out Vec3 vec);
            Multiply(ref vec, ref m, out Vec3 vec2);
            Vec3.Subtract(ref vec2, ref ray.ri, out ray.rJ);
            return ray;
        }

        public static Ray operator *(Ray r, Mat4 m)
        {
            Ray ray;
            Multiply(ref r.ri, ref m, out ray.ri);
            Vec3.Add(ref r.ri, ref r.rJ, out Vec3 vec);
            Multiply(ref vec, ref m, out Vec3 vec2);
            Vec3.Subtract(ref vec2, ref ray.ri, out ray.rJ);
            return ray;
        }

        public static Vec3 operator *(Mat4 m, Vec3 v)
        {
            Vec3 vec;
            vec.x = (((m.Rp.x * v.X) + (m.RQ.x * v.Y)) + (m.Rq.x * v.Z)) + m.RR.x;
            vec.y = (((m.Rp.y * v.X) + (m.RQ.y * v.Y)) + (m.Rq.y * v.Z)) + m.RR.y;
            vec.z = (((m.Rp.z * v.X) + (m.RQ.z * v.Y)) + (m.Rq.z * v.Z)) + m.RR.z;
            return vec;
        }

        public static Vec3 operator *(Vec3 v, Mat4 m)
        {
            Vec3 vec;
            vec.x = (((m.Rp.x * v.X) + (m.RQ.x * v.Y)) + (m.Rq.x * v.Z)) + m.RR.x;
            vec.y = (((m.Rp.y * v.X) + (m.RQ.y * v.Y)) + (m.Rq.y * v.Z)) + m.RR.y;
            vec.z = (((m.Rp.z * v.X) + (m.RQ.z * v.Y)) + (m.Rq.z * v.Z)) + m.RR.z;
            return vec;
        }

        public static Vec4 operator *(Mat4 m, Vec4 v)
        {
            Vec4 vec;
            vec.x = (((m.Rp.x * v.X) + (m.RQ.x * v.Y)) + (m.Rq.x * v.Z)) + (m.RR.x * v.W);
            vec.y = (((m.Rp.y * v.X) + (m.RQ.y * v.Y)) + (m.Rq.y * v.Z)) + (m.RR.y * v.W);
            vec.z = (((m.Rp.z * v.X) + (m.RQ.z * v.Y)) + (m.Rq.z * v.Z)) + (m.RR.z * v.W);
            vec.w = (((m.Rp.w * v.X) + (m.RQ.w * v.Y)) + (m.Rq.w * v.Z)) + (m.RR.w * v.W);
            return vec;
        }

        public static Vec4 operator *(Vec4 v, Mat4 m)
        {
            Vec4 vec;
            vec.x = (((m.Rp.x * v.X) + (m.RQ.x * v.Y)) + (m.Rq.x * v.Z)) + (m.RR.x * v.W);
            vec.y = (((m.Rp.y * v.X) + (m.RQ.y * v.Y)) + (m.Rq.y * v.Z)) + (m.RR.y * v.W);
            vec.z = (((m.Rp.z * v.X) + (m.RQ.z * v.Y)) + (m.Rq.z * v.Z)) + (m.RR.z * v.W);
            vec.w = (((m.Rp.w * v.X) + (m.RQ.w * v.Y)) + (m.Rq.w * v.Z)) + (m.RR.w * v.W);
            return vec;
        }

        public static Mat4 operator *(Mat4 v1, Mat4 v2)
        {
            Mat4 mat;
            mat.Rp.x = (((v1.Rp.x * v2.Rp.x) + (v1.RQ.x * v2.Rp.y)) + (v1.Rq.x * v2.Rp.z)) + (v1.RR.x * v2.Rp.w);
            mat.Rp.y = (((v1.Rp.y * v2.Rp.x) + (v1.RQ.y * v2.Rp.y)) + (v1.Rq.y * v2.Rp.z)) + (v1.RR.y * v2.Rp.w);
            mat.Rp.z = (((v1.Rp.z * v2.Rp.x) + (v1.RQ.z * v2.Rp.y)) + (v1.Rq.z * v2.Rp.z)) + (v1.RR.z * v2.Rp.w);
            mat.Rp.w = (((v1.Rp.w * v2.Rp.x) + (v1.RQ.w * v2.Rp.y)) + (v1.Rq.w * v2.Rp.z)) + (v1.RR.w * v2.Rp.w);
            mat.RQ.x = (((v1.Rp.x * v2.RQ.x) + (v1.RQ.x * v2.RQ.y)) + (v1.Rq.x * v2.RQ.z)) + (v1.RR.x * v2.RQ.w);
            mat.RQ.y = (((v1.Rp.y * v2.RQ.x) + (v1.RQ.y * v2.RQ.y)) + (v1.Rq.y * v2.RQ.z)) + (v1.RR.y * v2.RQ.w);
            mat.RQ.z = (((v1.Rp.z * v2.RQ.x) + (v1.RQ.z * v2.RQ.y)) + (v1.Rq.z * v2.RQ.z)) + (v1.RR.z * v2.RQ.w);
            mat.RQ.w = (((v1.Rp.w * v2.RQ.x) + (v1.RQ.w * v2.RQ.y)) + (v1.Rq.w * v2.RQ.z)) + (v1.RR.w * v2.RQ.w);
            mat.Rq.x = (((v1.Rp.x * v2.Rq.x) + (v1.RQ.x * v2.Rq.y)) + (v1.Rq.x * v2.Rq.z)) + (v1.RR.x * v2.Rq.w);
            mat.Rq.y = (((v1.Rp.y * v2.Rq.x) + (v1.RQ.y * v2.Rq.y)) + (v1.Rq.y * v2.Rq.z)) + (v1.RR.y * v2.Rq.w);
            mat.Rq.z = (((v1.Rp.z * v2.Rq.x) + (v1.RQ.z * v2.Rq.y)) + (v1.Rq.z * v2.Rq.z)) + (v1.RR.z * v2.Rq.w);
            mat.Rq.w = (((v1.Rp.w * v2.Rq.x) + (v1.RQ.w * v2.Rq.y)) + (v1.Rq.w * v2.Rq.z)) + (v1.RR.w * v2.Rq.w);
            mat.RR.x = (((v1.Rp.x * v2.RR.x) + (v1.RQ.x * v2.RR.y)) + (v1.Rq.x * v2.RR.z)) + (v1.RR.x * v2.RR.w);
            mat.RR.y = (((v1.Rp.y * v2.RR.x) + (v1.RQ.y * v2.RR.y)) + (v1.Rq.y * v2.RR.z)) + (v1.RR.y * v2.RR.w);
            mat.RR.z = (((v1.Rp.z * v2.RR.x) + (v1.RQ.z * v2.RR.y)) + (v1.Rq.z * v2.RR.z)) + (v1.RR.z * v2.RR.w);
            mat.RR.w = (((v1.Rp.w * v2.RR.x) + (v1.RQ.w * v2.RR.y)) + (v1.Rq.w * v2.RR.z)) + (v1.RR.w * v2.RR.w);
            return mat;
        }

        public static Mat4 operator -(Mat4 v)
        {
            Mat4 mat;
            mat.Rp.x = -v.Rp.x;
            mat.Rp.y = -v.Rp.y;
            mat.Rp.z = -v.Rp.z;
            mat.Rp.w = -v.Rp.w;
            mat.RQ.x = -v.RQ.x;
            mat.RQ.y = -v.RQ.y;
            mat.RQ.z = -v.RQ.z;
            mat.RQ.w = -v.RQ.w;
            mat.Rq.x = -v.Rq.x;
            mat.Rq.y = -v.Rq.y;
            mat.Rq.z = -v.Rq.z;
            mat.Rq.w = -v.Rq.w;
            mat.RR.x = -v.RR.x;
            mat.RR.y = -v.RR.y;
            mat.RR.z = -v.RR.z;
            mat.RR.w = -v.RR.w;
            return mat;
        }

        public static void Add(ref Mat4 v1, ref Mat4 v2, out Mat4 result)
        {
            result.Rp.x = v1.Rp.x + v2.Rp.x;
            result.Rp.y = v1.Rp.y + v2.Rp.y;
            result.Rp.z = v1.Rp.z + v2.Rp.z;
            result.Rp.w = v1.Rp.w + v2.Rp.w;
            result.RQ.x = v1.RQ.x + v2.RQ.x;
            result.RQ.y = v1.RQ.y + v2.RQ.y;
            result.RQ.z = v1.RQ.z + v2.RQ.z;
            result.RQ.w = v1.RQ.w + v2.RQ.w;
            result.Rq.x = v1.Rq.x + v2.Rq.x;
            result.Rq.y = v1.Rq.y + v2.Rq.y;
            result.Rq.z = v1.Rq.z + v2.Rq.z;
            result.Rq.w = v1.Rq.w + v2.Rq.w;
            result.RR.x = v1.RR.x + v2.RR.x;
            result.RR.y = v1.RR.y + v2.RR.y;
            result.RR.z = v1.RR.z + v2.RR.z;
            result.RR.w = v1.RR.w + v2.RR.w;
        }

        public static void Subtract(ref Mat4 v1, ref Mat4 v2, out Mat4 result)
        {
            result.Rp.x = v1.Rp.x - v2.Rp.x;
            result.Rp.y = v1.Rp.y - v2.Rp.y;
            result.Rp.z = v1.Rp.z - v2.Rp.z;
            result.Rp.w = v1.Rp.w - v2.Rp.w;
            result.RQ.x = v1.RQ.x - v2.RQ.x;
            result.RQ.y = v1.RQ.y - v2.RQ.y;
            result.RQ.z = v1.RQ.z - v2.RQ.z;
            result.RQ.w = v1.RQ.w - v2.RQ.w;
            result.Rq.x = v1.Rq.x - v2.Rq.x;
            result.Rq.y = v1.Rq.y - v2.Rq.y;
            result.Rq.z = v1.Rq.z - v2.Rq.z;
            result.Rq.w = v1.Rq.w - v2.Rq.w;
            result.RR.x = v1.RR.x - v2.RR.x;
            result.RR.y = v1.RR.y - v2.RR.y;
            result.RR.z = v1.RR.z - v2.RR.z;
            result.RR.w = v1.RR.w - v2.RR.w;
        }

        public static void Multiply(ref Mat4 m, float s, out Mat4 result)
        {
            result.Rp.x = m.Rp.x * s;
            result.Rp.y = m.Rp.y * s;
            result.Rp.z = m.Rp.z * s;
            result.Rp.w = m.Rp.w * s;
            result.RQ.x = m.RQ.x * s;
            result.RQ.y = m.RQ.y * s;
            result.RQ.z = m.RQ.z * s;
            result.RQ.w = m.RQ.w * s;
            result.Rq.x = m.Rq.x * s;
            result.Rq.y = m.Rq.y * s;
            result.Rq.z = m.Rq.z * s;
            result.Rq.w = m.Rq.w * s;
            result.RR.x = m.RR.x * s;
            result.RR.y = m.RR.y * s;
            result.RR.z = m.RR.z * s;
            result.RR.w = m.RR.w * s;
        }

        public static void Multiply(float s, ref Mat4 m, out Mat4 result)
        {
            result.Rp.x = m.Rp.x * s;
            result.Rp.y = m.Rp.y * s;
            result.Rp.z = m.Rp.z * s;
            result.Rp.w = m.Rp.w * s;
            result.RQ.x = m.RQ.x * s;
            result.RQ.y = m.RQ.y * s;
            result.RQ.z = m.RQ.z * s;
            result.RQ.w = m.RQ.w * s;
            result.Rq.x = m.Rq.x * s;
            result.Rq.y = m.Rq.y * s;
            result.Rq.z = m.Rq.z * s;
            result.Rq.w = m.Rq.w * s;
            result.RR.x = m.RR.x * s;
            result.RR.y = m.RR.y * s;
            result.RR.z = m.RR.z * s;
            result.RR.w = m.RR.w * s;
        }

        public static void Multiply(ref Mat4 m, ref Ray r, out Ray result)
        {
            Multiply(ref r.ri, ref m, out result.ri);
            Vec3.Add(ref r.ri, ref r.rJ, out Vec3 vec);
            Multiply(ref vec, ref m, out Vec3 vec2);
            Vec3.Subtract(ref vec2, ref result.ri, out result.rJ);
        }

        public static void Multiply(ref Ray r, ref Mat4 m, out Ray result)
        {
            Multiply(ref r.ri, ref m, out result.ri);
            Vec3.Add(ref r.ri, ref r.rJ, out Vec3 vec);
            Multiply(ref vec, ref m, out Vec3 vec2);
            Vec3.Subtract(ref vec2, ref result.ri, out result.rJ);
        }

        public static void Multiply(ref Mat4 m, ref Vec3 v, out Vec3 result)
        {
            result.x = (((m.Rp.x * v.X) + (m.RQ.x * v.Y)) + (m.Rq.x * v.Z)) + m.RR.x;
            result.y = (((m.Rp.y * v.X) + (m.RQ.y * v.Y)) + (m.Rq.y * v.Z)) + m.RR.y;
            result.z = (((m.Rp.z * v.X) + (m.RQ.z * v.Y)) + (m.Rq.z * v.Z)) + m.RR.z;
        }

        public static void Multiply(ref Vec3 v, ref Mat4 m, out Vec3 result)
        {
            result.x = (((m.Rp.x * v.X) + (m.RQ.x * v.Y)) + (m.Rq.x * v.Z)) + m.RR.x;
            result.y = (((m.Rp.y * v.X) + (m.RQ.y * v.Y)) + (m.Rq.y * v.Z)) + m.RR.y;
            result.z = (((m.Rp.z * v.X) + (m.RQ.z * v.Y)) + (m.Rq.z * v.Z)) + m.RR.z;
        }

        public static void Multiply(ref Mat4 m, ref Vec4 v, out Vec4 result)
        {
            result.x = (((m.Rp.x * v.X) + (m.RQ.x * v.Y)) + (m.Rq.x * v.Z)) + (m.RR.x * v.W);
            result.y = (((m.Rp.y * v.X) + (m.RQ.y * v.Y)) + (m.Rq.y * v.Z)) + (m.RR.y * v.W);
            result.z = (((m.Rp.z * v.X) + (m.RQ.z * v.Y)) + (m.Rq.z * v.Z)) + (m.RR.z * v.W);
            result.w = (((m.Rp.w * v.X) + (m.RQ.w * v.Y)) + (m.Rq.w * v.Z)) + (m.RR.w * v.W);
        }

        public static void Multiply(ref Vec4 v, ref Mat4 m, out Vec4 result)
        {
            result.x = (((m.Rp.x * v.X) + (m.RQ.x * v.Y)) + (m.Rq.x * v.Z)) + (m.RR.x * v.W);
            result.y = (((m.Rp.y * v.X) + (m.RQ.y * v.Y)) + (m.Rq.y * v.Z)) + (m.RR.y * v.W);
            result.z = (((m.Rp.z * v.X) + (m.RQ.z * v.Y)) + (m.Rq.z * v.Z)) + (m.RR.z * v.W);
            result.w = (((m.Rp.w * v.X) + (m.RQ.w * v.Y)) + (m.Rq.w * v.Z)) + (m.RR.w * v.W);
        }

        public static void Multiply(ref Mat4 v1, ref Mat4 v2, out Mat4 result)
        {
            result.Rp.x = (((v1.Rp.x * v2.Rp.x) + (v1.RQ.x * v2.Rp.y)) + (v1.Rq.x * v2.Rp.z)) + (v1.RR.x * v2.Rp.w);
            result.Rp.y = (((v1.Rp.y * v2.Rp.x) + (v1.RQ.y * v2.Rp.y)) + (v1.Rq.y * v2.Rp.z)) + (v1.RR.y * v2.Rp.w);
            result.Rp.z = (((v1.Rp.z * v2.Rp.x) + (v1.RQ.z * v2.Rp.y)) + (v1.Rq.z * v2.Rp.z)) + (v1.RR.z * v2.Rp.w);
            result.Rp.w = (((v1.Rp.w * v2.Rp.x) + (v1.RQ.w * v2.Rp.y)) + (v1.Rq.w * v2.Rp.z)) + (v1.RR.w * v2.Rp.w);
            result.RQ.x = (((v1.Rp.x * v2.RQ.x) + (v1.RQ.x * v2.RQ.y)) + (v1.Rq.x * v2.RQ.z)) + (v1.RR.x * v2.RQ.w);
            result.RQ.y = (((v1.Rp.y * v2.RQ.x) + (v1.RQ.y * v2.RQ.y)) + (v1.Rq.y * v2.RQ.z)) + (v1.RR.y * v2.RQ.w);
            result.RQ.z = (((v1.Rp.z * v2.RQ.x) + (v1.RQ.z * v2.RQ.y)) + (v1.Rq.z * v2.RQ.z)) + (v1.RR.z * v2.RQ.w);
            result.RQ.w = (((v1.Rp.w * v2.RQ.x) + (v1.RQ.w * v2.RQ.y)) + (v1.Rq.w * v2.RQ.z)) + (v1.RR.w * v2.RQ.w);
            result.Rq.x = (((v1.Rp.x * v2.Rq.x) + (v1.RQ.x * v2.Rq.y)) + (v1.Rq.x * v2.Rq.z)) + (v1.RR.x * v2.Rq.w);
            result.Rq.y = (((v1.Rp.y * v2.Rq.x) + (v1.RQ.y * v2.Rq.y)) + (v1.Rq.y * v2.Rq.z)) + (v1.RR.y * v2.Rq.w);
            result.Rq.z = (((v1.Rp.z * v2.Rq.x) + (v1.RQ.z * v2.Rq.y)) + (v1.Rq.z * v2.Rq.z)) + (v1.RR.z * v2.Rq.w);
            result.Rq.w = (((v1.Rp.w * v2.Rq.x) + (v1.RQ.w * v2.Rq.y)) + (v1.Rq.w * v2.Rq.z)) + (v1.RR.w * v2.Rq.w);
            result.RR.x = (((v1.Rp.x * v2.RR.x) + (v1.RQ.x * v2.RR.y)) + (v1.Rq.x * v2.RR.z)) + (v1.RR.x * v2.RR.w);
            result.RR.y = (((v1.Rp.y * v2.RR.x) + (v1.RQ.y * v2.RR.y)) + (v1.Rq.y * v2.RR.z)) + (v1.RR.y * v2.RR.w);
            result.RR.z = (((v1.Rp.z * v2.RR.x) + (v1.RQ.z * v2.RR.y)) + (v1.Rq.z * v2.RR.z)) + (v1.RR.z * v2.RR.w);
            result.RR.w = (((v1.Rp.w * v2.RR.x) + (v1.RQ.w * v2.RR.y)) + (v1.Rq.w * v2.RR.z)) + (v1.RR.w * v2.RR.w);
        }

        public static void Negate(ref Mat4 m, out Mat4 result)
        {
            result.Rp.x = -m.Rp.x;
            result.Rp.y = -m.Rp.y;
            result.Rp.z = -m.Rp.z;
            result.Rp.w = -m.Rp.w;
            result.RQ.x = -m.RQ.x;
            result.RQ.y = -m.RQ.y;
            result.RQ.z = -m.RQ.z;
            result.RQ.w = -m.RQ.w;
            result.Rq.x = -m.Rq.x;
            result.Rq.y = -m.Rq.y;
            result.Rq.z = -m.Rq.z;
            result.Rq.w = -m.Rq.w;
            result.RR.x = -m.RR.x;
            result.RR.y = -m.RR.y;
            result.RR.z = -m.RR.z;
            result.RR.w = -m.RR.w;
        }

        public static Mat4 Add(Mat4 v1, Mat4 v2)
        {
            Add(ref v1, ref v2, out Mat4 mat);
            return mat;
        }

        public static Mat4 Subtract(Mat4 v1, Mat4 v2)
        {
            Subtract(ref v1, ref v2, out Mat4 mat);
            return mat;
        }

        public static Mat4 Multiply(Mat4 m, float s)
        {
            Multiply(ref m, s, out Mat4 mat);
            return mat;
        }

        public static Mat4 Multiply(float s, Mat4 m)
        {
            Multiply(ref m, s, out Mat4 mat);
            return mat;
        }

        public static Ray Multiply(Mat4 m, Ray r)
        {
            Multiply(ref m, ref r, out Ray ray);
            return ray;
        }

        public static Ray Multiply(Ray r, Mat4 m)
        {
            Multiply(ref m, ref r, out Ray ray);
            return ray;
        }

        public static Vec3 Multiply(Mat4 m, Vec3 v)
        {
            Multiply(ref m, ref v, out Vec3 vec);
            return vec;
        }

        public static Vec3 Multiply(Vec3 v, Mat4 m)
        {
            Multiply(ref m, ref v, out Vec3 vec);
            return vec;
        }

        public static Vec4 Multiply(Mat4 m, Vec4 v)
        {
            Multiply(ref m, ref v, out Vec4 vec);
            return vec;
        }

        public static Vec4 Multiply(Vec4 v, Mat4 m)
        {
            Multiply(ref m, ref v, out Vec4 vec);
            return vec;
        }

        public static Mat4 Multiply(Mat4 v1, Mat4 v2)
        {
            Multiply(ref v1, ref v2, out Mat4 mat);
            return mat;
        }

        public static Mat4 Negate(Mat4 m)
        {
            Negate(ref m, out Mat4 mat);
            return mat;
        }

        public float GetTrace()
        {
            return (((Rp.X + RQ.Y) + Rq.Z) + RR.W);
        }

        public void Transpose()
        {
            float rv = Rp.y;
            Rp.y = RQ.x;
            RQ.x = rv;
            rv = Rp.z;
            Rp.z = Rq.x;
            Rq.x = rv;
            rv = Rp.w;
            Rp.w = RR.x;
            RR.x = rv;
            rv = RQ.z;
            RQ.z = Rq.y;
            Rq.y = rv;
            rv = RQ.w;
            RQ.w = RR.y;
            RR.y = rv;
            rv = Rq.w;
            Rq.w = RR.z;
            RR.z = rv;
        }

        public Mat4 GetTranspose()
        {
            Mat4 mat = this;
            mat.Transpose();
            return mat;
        }

        public void GetTranspose(out Mat4 result)
        {
            result = this;
            result.Transpose();
        }

        public bool Inverse()
        {
            float num3 = (Rp.x * RQ.y) - (Rp.y * RQ.x);
            float num4 = (Rp.x * RQ.z) - (Rp.z * RQ.x);
            float num5 = (Rp.x * RQ.w) - (Rp.w * RQ.x);
            float num6 = (Rp.y * RQ.z) - (Rp.z * RQ.y);
            float num7 = (Rp.y * RQ.w) - (Rp.w * RQ.y);
            float num8 = (Rp.z * RQ.w) - (Rp.w * RQ.z);
            float num9 = ((Rq.x * num6) - (Rq.y * num4)) + (Rq.z * num3);
            float num10 = ((Rq.x * num7) - (Rq.y * num5)) + (Rq.w * num3);
            float num11 = ((Rq.x * num8) - (Rq.z * num5)) + (Rq.w * num4);
            float num12 = ((Rq.y * num8) - (Rq.z * num7)) + (Rq.w * num6);
            double num = (((-num12 * RR.x) + (num11 * RR.y)) - (num10 * RR.z)) + (num9 * RR.w);
            if (System.Math.Abs(num) < 1E-14)
            {
                return false;
            }
            double num2 = 1.0 / num;
            float num13 = (Rp.x * RR.y) - (Rp.y * RR.x);
            float num14 = (Rp.x * RR.z) - (Rp.z * RR.x);
            float num15 = (Rp.x * RR.w) - (Rp.w * RR.x);
            float num16 = (Rp.y * RR.z) - (Rp.z * RR.y);
            float num17 = (Rp.y * RR.w) - (Rp.w * RR.y);
            float num18 = (Rp.z * RR.w) - (Rp.w * RR.z);
            float num19 = (RQ.x * RR.y) - (RQ.y * RR.x);
            float num20 = (RQ.x * RR.z) - (RQ.z * RR.x);
            float num21 = (RQ.x * RR.w) - (RQ.w * RR.x);
            float num22 = (RQ.y * RR.z) - (RQ.z * RR.y);
            float num23 = (RQ.y * RR.w) - (RQ.w * RR.y);
            float num24 = (RQ.z * RR.w) - (RQ.w * RR.z);
            float num25 = ((Rq.x * num16) - (Rq.y * num14)) + (Rq.z * num13);
            float num26 = ((Rq.x * num17) - (Rq.y * num15)) + (Rq.w * num13);
            float num27 = ((Rq.x * num18) - (Rq.z * num15)) + (Rq.w * num14);
            float num28 = ((Rq.y * num18) - (Rq.z * num17)) + (Rq.w * num16);
            float num29 = ((Rq.x * num22) - (Rq.y * num20)) + (Rq.z * num19);
            float num30 = ((Rq.x * num23) - (Rq.y * num21)) + (Rq.w * num19);
            float num31 = ((Rq.x * num24) - (Rq.z * num21)) + (Rq.w * num20);
            float num32 = ((Rq.y * num24) - (Rq.z * num23)) + (Rq.w * num22);
            float num33 = ((RR.x * num6) - (RR.y * num4)) + (RR.z * num3);
            float num34 = ((RR.x * num7) - (RR.y * num5)) + (RR.w * num3);
            float num35 = ((RR.x * num8) - (RR.z * num5)) + (RR.w * num4);
            float num36 = ((RR.y * num8) - (RR.z * num7)) + (RR.w * num6);
            Rp.x = (float)(-num32 * num2);
            RQ.x = (float)(num31 * num2);
            Rq.x = (float)(-num30 * num2);
            RR.x = (float)(num29 * num2);
            Rp.y = (float)(num28 * num2);
            RQ.y = (float)(-num27 * num2);
            Rq.y = (float)(num26 * num2);
            RR.y = (float)(-num25 * num2);
            Rp.z = (float)(num36 * num2);
            RQ.z = (float)(-num35 * num2);
            Rq.z = (float)(num34 * num2);
            RR.z = (float)(-num33 * num2);
            Rp.w = (float)(-num12 * num2);
            RQ.w = (float)(num11 * num2);
            Rq.w = (float)(-num10 * num2);
            RR.w = (float)(num9 * num2);
            return true;
        }

        public Mat4 GetInverse()
        {
            Mat4 mat = this;
            mat.Inverse();
            return mat;
        }

        public void GetInverse(out Mat4 result)
        {
            result = this;
            result.Inverse();
        }

        public override bool Equals(object obj)
        {
            return ((obj is Mat4) && (this == ((Mat4)obj)));
        }

        public static bool operator ==(Mat4 v1, Mat4 v2)
        {
            return ((((v1.Rp == v2.Rp) && (v1.RQ == v2.RQ)) && (v1.Rq == v2.Rq)) && (v1.RR == v2.RR));
        }

        public static bool operator !=(Mat4 v1, Mat4 v2)
        {
            if ((!(v1.Rp != v2.Rp) && !(v1.RQ != v2.RQ)) && !(v1.Rq != v2.Rq))
            {
                return (v1.RR != v2.RR);
            }
            return true;
        }

        public unsafe float this[int row, int column]
        {
            get
            {
                if ((row < 0) || (row > 3))
                {
                    throw new ArgumentOutOfRangeException("row");
                }
                if ((column < 0) || (column > 3))
                {
                    throw new ArgumentOutOfRangeException("column");
                }
                fixed (float* numRef = &Rp.x)
                {
                    return numRef[((row * 4) + column) * 4];
                }
            }
            set
            {
                if ((row < 0) || (row > 3))
                {
                    throw new ArgumentOutOfRangeException("row");
                }
                if ((column < 0) || (column > 3))
                {
                    throw new ArgumentOutOfRangeException("column");
                }
                fixed (float* numRef = &Rp.x)
                {
                    numRef[((row * 4) + column) * 4] = value;
                }
            }
        }

        public bool Equals(Mat4 v, float epsilon)
        {
            if (!Rp.Equals(ref v.Rp, epsilon))
            {
                return false;
            }
            if (!RQ.Equals(ref v.RQ, epsilon))
            {
                return false;
            }
            if (!Rq.Equals(ref v.Rq, epsilon))
            {
                return false;
            }
            if (!RR.Equals(ref v.RR, epsilon))
            {
                return false;
            }
            return true;
        }

        public static Mat4 LookAt(Vec3 eye, Vec3 dir, Vec3 up)
        {
            LookAt(ref eye, ref dir, ref up, out Mat4 mat);
            return mat;
        }

        public static void LookAt(ref Vec3 eye, ref Vec3 dir, ref Vec3 up, out Mat4 result)
        {
            Vec3.Subtract(ref eye, ref dir, out Vec3 vec);
            vec.Normalize();
            Vec3.Cross(ref up, ref vec, out Vec3 vec2);
            vec2.Normalize();
            Vec3.Cross(ref vec, ref vec2, out Vec3 vec3);
            vec3.Normalize();
            result.Rp.x = vec2.x;
            result.RQ.x = vec2.y;
            result.Rq.x = vec2.z;
            result.RR.x = 0f;
            result.Rp.y = vec3.x;
            result.RQ.y = vec3.y;
            result.Rq.y = vec3.z;
            result.RR.y = 0f;
            result.Rp.z = vec.x;
            result.RQ.z = vec.y;
            result.Rq.z = vec.z;
            result.RR.z = 0f;
            result.Rp.w = 0f;
            result.RQ.w = 0f;
            result.Rq.w = 0f;
            result.RR.w = 1f;
            Mat4 identity = Identity;
            identity[3, 0] = -eye.X;
            identity[3, 1] = -eye.Y;
            identity[3, 2] = -eye.Z;
            result *= identity;
        }

        public static Mat4 Perspective(float fov, float aspect, float znear, float zfar)
        {
            Perspective(fov, aspect, znear, zfar, out Mat4 mat);
            return mat;
        }

        public static void Perspective(float fov, float aspect, float znear, float zfar, out Mat4 result)
        {
            float num = MathFunctions.Tan((fov * 3.141593f) / 360f);
            float num2 = num * aspect;
            result.Rp.x = 1f / num2;
            result.RQ.x = 0f;
            result.Rq.x = 0f;
            result.RR.x = 0f;
            result.Rp.y = 0f;
            result.RQ.y = 1f / num;
            result.Rq.y = 0f;
            result.RR.y = 0f;
            result.Rp.z = 0f;
            result.RQ.z = 0f;
            result.Rq.z = -(zfar + znear) / (zfar - znear);
            result.RR.z = -((2f * zfar) * znear) / (zfar - znear);
            result.Rp.w = 0f;
            result.RQ.w = 0f;
            result.Rq.w = -1f;
            result.RR.w = 0f;
        }

        public static Mat4 FromTranslate(Vec3 translation)
        {
            Mat4 mat;
            mat.Rp.x = 1f;
            mat.Rp.y = 0f;
            mat.Rp.z = 0f;
            mat.Rp.w = 0f;
            mat.RQ.x = 0f;
            mat.RQ.y = 1f;
            mat.RQ.z = 0f;
            mat.RQ.w = 0f;
            mat.Rq.x = 0f;
            mat.Rq.y = 0f;
            mat.Rq.z = 1f;
            mat.Rq.w = 0f;
            mat.RR.x = translation.x;
            mat.RR.y = translation.y;
            mat.RR.z = translation.z;
            mat.RR.w = 1f;
            return mat;
        }

        public static void FromTranslate(ref Vec3 translation, out Mat4 result)
        {
            result.Rp.x = 1f;
            result.Rp.y = 0f;
            result.Rp.z = 0f;
            result.Rp.w = 0f;
            result.RQ.x = 0f;
            result.RQ.y = 1f;
            result.RQ.z = 0f;
            result.RQ.w = 0f;
            result.Rq.x = 0f;
            result.Rq.y = 0f;
            result.Rq.z = 1f;
            result.Rq.w = 0f;
            result.RR.x = translation.x;
            result.RR.y = translation.y;
            result.RR.z = translation.z;
            result.RR.w = 1f;
        }

        public Mat3 ToMat3()
        {
            Mat3 mat;
            mat.Rt.x = Rp.x;
            mat.Rt.y = Rp.y;
            mat.Rt.z = Rp.z;
            mat.RU.x = RQ.x;
            mat.RU.y = RQ.y;
            mat.RU.z = RQ.z;
            mat.Ru.x = Rq.x;
            mat.Ru.y = Rq.y;
            mat.Ru.z = Rq.z;
            return mat;
        }

        public void ToMat3(out Mat3 result)
        {
            result.Rt.x = Rp.x;
            result.Rt.y = Rp.y;
            result.Rt.z = Rp.z;
            result.RU.x = RQ.x;
            result.RU.y = RQ.y;
            result.RU.z = RQ.z;
            result.Ru.x = Rq.x;
            result.Ru.y = Rq.y;
            result.Ru.z = Rq.z;
        }

        public static Mat4 Parse(string text)
        {
            Mat4 mat;
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException("The text parameter cannot be null or zero length.");
            }
            string[] strArray = text.Split(SpaceCharacter.arrayWithOneSpaceCharacter, StringSplitOptions.RemoveEmptyEntries);
            if (strArray.Length != 16)
            {
                throw new FormatException(string.Format("Cannot parse the text '{0}' because it does not have 16 parts separated by spaces.", text));
            }
            try
            {
                mat = new Mat4(float.Parse(strArray[0]), float.Parse(strArray[1]), float.Parse(strArray[2]), float.Parse(strArray[3]), float.Parse(strArray[4]), float.Parse(strArray[5]), float.Parse(strArray[6]), float.Parse(strArray[7]), float.Parse(strArray[8]), float.Parse(strArray[9]), float.Parse(strArray[10]), float.Parse(strArray[11]), float.Parse(strArray[12]), float.Parse(strArray[13]), float.Parse(strArray[14]), float.Parse(strArray[15]));
            }
            catch (Exception)
            {
                throw new FormatException("The parts of the vectors must be decimal numbers.");
            }
            return mat;
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3}", new object[] { Rp.ToString(), RQ.ToString(), Rq.ToString(), RR.ToString() });
        }

        [Browsable(false)]
        public Vec4 Item0
        {
            get
            {
                return Rp;
            }
        }

        [Browsable(false)]
        public Vec4 Item1
        {
            get
            {
                return RQ;
            }
        }

        [Browsable(false)]
        public Vec4 Item2
        {
            get
            {
                return Rq;
            }
        }

        [Browsable(false)]
        public Vec4 Item3
        {
            get
            {
                return RR;
            }
        }
    }
}
