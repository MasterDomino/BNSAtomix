using System.ComponentModel;
using System.Runtime.InteropServices;

namespace SmartEngine.Core.Math
{
    [StructLayout(LayoutKind.Sequential), TypeConverter(typeof(_GeneralTypeConverter<Ray>))]
    public struct Ray
    {
        internal Vec3 ri;
        internal Vec3 rJ;
        public Ray(Ray source)
        {
            ri = source.ri;
            rJ = source.rJ;
        }

        public Ray(Vec3 origin, Vec3 direction)
        {
            ri = origin;
            rJ = direction;
        }

        public Vec3 Origin
        {
            get
            {
                return ri;
            }
            set
            {
                ri = value;
            }
        }

        public Vec3 Direction
        {
            get
            {
                return rJ;
            }
            set
            {
                rJ = value;
            }
        }

        public override bool Equals(object obj)
        {
            return ((obj is Ray) && (this == ((Ray)obj)));
        }

        public override int GetHashCode()
        {
            return (ri.GetHashCode() ^ rJ.GetHashCode());
        }

        public static bool operator ==(Ray v1, Ray v2)
        {
            return ((v1.ri == v2.ri) && (v1.rJ == v2.rJ));
        }

        public static bool operator !=(Ray v1, Ray v2)
        {
            if (!(v1.ri != v2.ri))
            {
                return (v1.rJ != v2.rJ);
            }
            return true;
        }

        public bool Equals(Ray v, float epsilon)
        {
            if (!ri.Equals(v.ri, epsilon))
            {
                return false;
            }
            if (!rJ.Equals(v.rJ, epsilon))
            {
                return false;
            }
            return true;
        }

        public Vec3 GetPointOnRay(float t)
        {
            Vec3 vec;
            vec.x = ri.x + (rJ.x * t);
            vec.y = ri.y + (rJ.y * t);
            vec.z = ri.z + (rJ.z * t);
            return vec;
        }

        public void GetPointOnRay(float t, out Vec3 result)
        {
            result.x = ri.x + (rJ.x * t);
            result.y = ri.y + (rJ.y * t);
            result.z = ri.z + (rJ.z * t);
        }
    }
}
