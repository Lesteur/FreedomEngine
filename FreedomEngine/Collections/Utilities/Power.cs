using System;

namespace FreedomEngine.Collections.Utilities
{
    public static class Power
    {
        public static float In(double value, int power)
        {
            return (float)Math.Pow(value, power);
        }

        public static float Out(double value, int power)
        {
            var sign = power % 2 == 0 ? -1 : 1;
            return (float)(sign * (Math.Pow(value - 1, power) + sign));
        }

        public static float InOut(double s, int power)
        {
            s *= 2;

            if (s < 1)
            {
                return In(s, power) / 2;
            }

            var sign = power % 2 == 0 ? -1 : 1;
            return (float)(sign / 2.0 * (Math.Pow(s - 2, power) + sign * 2));
        }
    }
}