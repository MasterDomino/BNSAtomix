using System.Collections.Generic;

namespace SmartEngine.Core.Math
{
    public class Curve
    {
        private readonly List<float> Rc = new List<float>();
        private int Rd = -1;
        private readonly List<Vec3> RD = new List<Vec3>();

        public int AddValue(float time, Vec3 value)
        {
            int indexForTime = GetIndexForTime(time);
            Rc.Insert(indexForTime, time);
            RD.Insert(indexForTime, value);
            return indexForTime;
        }

        public virtual Vec3 CalculateValueByTime(float time)
        {
            int indexForTime = GetIndexForTime(time);
            if (indexForTime >= RD.Count)
            {
                return RD[RD.Count - 1];
            }
            return RD[indexForTime];
        }

        public void Clear()
        {
            RD.Clear();
            Rc.Clear();
            Rd = -1;
        }

        public virtual Vec3 GetCurrentFirstDerivative(float time)
        {
            return Vec3.Zero;
        }

        protected int GetIndexForTime(float time)
        {
            if ((Rd >= 0) && (Rd <= Rc.Count))
            {
                if (Rd == 0)
                {
                    if (time <= Rc[Rd])
                    {
                        return Rd;
                    }
                }
                else if (Rd == Rc.Count)
                {
                    if (time > Rc[Rd - 1])
                    {
                        return Rd;
                    }
                }
                else
                {
                    if ((time > Rc[Rd - 1]) && (time <= Rc[Rd]))
                    {
                        return Rd;
                    }
                    if ((time > Rc[Rd]) && (((Rd + 1) == Rc.Count) || (time <= Rc[Rd + 1])))
                    {
                        Rd++;
                        return Rd;
                    }
                }
            }
            int count = Rc.Count;
            int num2 = count;
            int num3 = 0;
            int num4 = 0;
            while (num2 > 0)
            {
                num2 = count >> 1;
                if (time == Rc[num3 + num2])
                {
                    return (num3 + num2);
                }
                if (time > Rc[num3 + num2])
                {
                    num3 += num2;
                    count -= num2;
                    num4 = 1;
                }
                else
                {
                    count -= num2;
                    num4 = 0;
                }
            }
            Rd = num3 + num4;
            return Rd;
        }

        protected float GetSpeed(float time)
        {
            Vec3 currentFirstDerivative = GetCurrentFirstDerivative(time);
            float x = 0f;
            for (int i = 0; i < 3; i++)
            {
                x += currentFirstDerivative[i] * currentFirstDerivative[i];
            }
            return MathFunctions.Sqrt(x);
        }

        public void RemoveValue(int index)
        {
            RD.RemoveAt(index);
            Rc.RemoveAt(index);
        }

        /// <summary>
        /// <b>Don't modify</b>.
        /// </summary>
        public List<float> Times
        {
            get
            {
                return Rc;
            }
        }

        /// <summary>
        /// <b>Don't modify</b>.
        /// </summary>
        public List<Vec3> Values
        {
            get
            {
                return RD;
            }
        }
    }
}
