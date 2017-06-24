using System;
using System.Threading;
namespace SmartEngine.Network
{
    public class RandomF
    {
        private readonly ThreadLocal<Random> random = new ThreadLocal<Random> (() => new Random(Guid.NewGuid().GetHashCode()));
        private readonly int last;
        public int Next(int min, int max)
        {
            if (max != int.MaxValue)
            {
                max++;
            }

                
            /*if (value == last && max > 10)
            {
                value = random.Next(min, max);
                if (value == last)
                {
                    value = random.Next(min, max);
                    if (value == last)
                        if (last == 0)
                        {
                            Logger.Log.Debug("Random function(min:" + min.ToString() + ",max:" + max + ") returning value:" + last.ToString() + " for three times!", null);
                            random = new Random(DateTime.Now.Millisecond);
                        }
                        else
                            last = value;
                }
                else
                    last = value;
            }
            else
                last = value;*/
            return random.Value.Next(min, max);
        }

        public int Next()
        {
            return random.Value.Next();
        }

        public int Next(int max)
        {
            return Next(0, max);
        }

        public double NextDouble()
        {
            return random.Value.NextDouble();
        }
    }
}
