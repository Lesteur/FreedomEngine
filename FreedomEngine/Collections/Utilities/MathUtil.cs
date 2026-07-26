using System;

using Microsoft.Xna.Framework;

namespace FreedomEngine.Collections.Utilities
{
    static public class MathUtil
    {
        public static Vector2 FromPolar(float angle, float magnitude)
        {
            return magnitude * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        public static float Approach(float start, float target, float maxStep)
        {
            if (start < target)
            {
                return Math.Min(start + maxStep, target);
            }
            else
            {
                return Math.Max(start - maxStep, target);
            }
        }
    }
}
