using System;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace SmartEngine.Core.Math
{
    [StructLayout(LayoutKind.Sequential), TypeConverter(typeof(DegreeTypeConverter))]
    public struct Degree : IComparable<Degree>, IComparable<float>
    {
        public static readonly Degree Zero;
        private readonly float degree;
        public Degree(float r)
        {
            degree = r;
        }

        public Degree(int r)
        {
            degree = r;
        }

        public Degree(Degree d)
        {
            degree = d.degree;
        }

        public Degree(Radian r)
        {
            degree = r.InDegrees();
        }

        static Degree()
        {
            Zero = new Degree(0f);
        }

        public Radian InRadians()
        {
            return MathFunctions.DegToRad(degree);
        }

        public static implicit operator Degree(float value)
        {
            return new Degree(value);
        }

        public static implicit operator Degree(int value)
        {
            return new Degree((float)value);
        }

        public static implicit operator Degree(Radian value)
        {
            return new Degree(value);
        }

        public static implicit operator float(Degree value)
        {
            return value.degree;
        }

        public static Degree operator +(Degree left, float right)
        {
            return (left.degree + right);
        }

        public static Degree operator +(Degree left, int right)
        {
            return (left.degree + right);
        }

        public static Degree operator +(Degree left, Degree right)
        {
            return (left.degree + right.degree);
        }

        public static Degree operator +(Degree left, Radian right)
        {
            return (left + right.InDegrees());
        }

        public static Degree operator -(Degree r)
        {
            return -r.degree;
        }

        public static Degree operator -(Degree left, float right)
        {
            return (left.degree - right);
        }

        public static Degree operator -(Degree left, int right)
        {
            return (left.degree - right);
        }

        public static Degree operator -(Degree left, Degree right)
        {
            return (left.degree - right.degree);
        }

        public static Degree operator -(Degree left, Radian right)
        {
            return (left - right.InDegrees());
        }

        public static Degree operator *(Degree left, float right)
        {
            return (left.degree * right);
        }

        public static Degree operator *(Degree left, int right)
        {
            return (left.degree * right);
        }

        public static Degree operator *(float left, Degree right)
        {
            return (left * right.degree);
        }

        public static Degree operator *(int left, Degree right)
        {
            return (left * right.degree);
        }

        public static Degree operator *(Degree left, Degree right)
        {
            return (left.degree * right.degree);
        }

        public static Degree operator *(Degree left, Radian right)
        {
            return (left.degree * right.InDegrees());
        }

        public static Degree operator /(Degree left, float right)
        {
            return (left.degree / right);
        }

        public static bool operator <(Degree left, Degree right)
        {
            return (left.degree < right.degree);
        }

        public static bool operator ==(Degree left, Degree right)
        {
            return (left.degree == right.degree);
        }

        public static bool operator !=(Degree left, Degree right)
        {
            return (left.degree != right.degree);
        }

        public static bool operator >(Degree left, Degree right)
        {
            return (left.degree > right.degree);
        }

        public override bool Equals(object obj)
        {
            return ((obj is Degree) && (this == ((Degree)obj)));
        }

        public override int GetHashCode()
        {
            return degree.GetHashCode();
        }

        public int CompareTo(Degree other)
        {
            return degree.CompareTo(other);
        }

        public int CompareTo(Radian other)
        {
            return degree.CompareTo(other.InDegrees());
        }

        public int CompareTo(float other)
        {
            return degree.CompareTo(other);
        }

        [LogicSystemBrowsable(true)]
        public static Degree Parse(string text)
        {
            return new Degree(float.Parse(text));
        }

        public override string ToString()
        {
            return degree.ToString();
        }
    }
}
