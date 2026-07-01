using System;

using Microsoft.Xna.Framework;

namespace FreedomEngine.Collections.Utilities
{
    public static class EasingFunctions
    {
        #region Linear

        public static float Linear(float value) => value;

        #endregion

        #region Cubic

        public static float CubicIn(float value) => Power.In(value, 3);

        public static float CubicOut(float value) => Power.Out(value, 3);

        public static float CubicInOut(float value) => Power.InOut(value, 3);

        #endregion

        #region Quadratic

        public static float QuadraticIn(float value) => Power.In(value, 2);

        public static float QuadraticOut(float value) => Power.Out(value, 2);

        public static float QuadraticInOut(float value) => Power.InOut(value, 2);

        #endregion

        #region Quartic

        public static float QuarticIn(float value) => Power.In(value, 4);

        public static float QuarticOut(float value) => Power.Out(value, 4);

        public static float QuarticInOut(float value) => Power.InOut(value, 4);

        #endregion

        #region Quintic

        public static float QuinticIn(float value) => Power.In(value, 5);

        public static float QuinticOut(float value) => Power.Out(value, 5);

        public static float QuinticInOut(float value) => Power.InOut(value, 5);

        #endregion

        #region Sine

        public static float SineIn(float value) => (float)Math.Sin(value * MathHelper.PiOver2 - MathHelper.PiOver2) + 1;

        public static float SineOut(float value) => (float)Math.Sin(value * MathHelper.PiOver2);

        public static float SineInOut(float value) => (float)(Math.Sin(value * MathHelper.Pi - MathHelper.PiOver2) + 1) / 2;

        #endregion

        #region Exponential

        public static float ExponentialIn(float value) => (float)Math.Pow(2, 10 * (value - 1));

        public static float ExponentialOut(float value) => Out(value, ExponentialIn);

        public static float ExponentialInOut(float value) => InOut(value, ExponentialIn);

        #endregion

        #region Circle

        public static float CircleIn(float value) => (float)-(Math.Sqrt(1 - value * value) - 1);

        public static float CircleOut(float value) => (float)Math.Sqrt(1 - (value - 1) * (value - 1));

        public static float CircleInOut(float value) => (float)(value <= .5 ? (Math.Sqrt(1 - value * value * 4) - 1) / -2 : (Math.Sqrt(1 - (value * 2 - 2) * (value * 2 - 2)) + 1) / 2);

        #endregion

        #region Elastic

        public static float ElasticIn(float value)
        {
            const int oscillations = 1;
            const float springiness = 3f;
            var e = (Math.Exp(springiness * value) - 1) / (Math.Exp(springiness) - 1);
            return (float)(e * Math.Sin((MathHelper.PiOver2 + MathHelper.TwoPi * oscillations) * value));
        }

        public static float ElasticOut(float value) => Out(value, ElasticIn);

        public static float ElasticInOut(float value) => InOut(value, ElasticIn);

        #endregion

        #region Back

        public static float BackIn(float value)
        {
            const float amplitude = 1f;
            return (float)(Math.Pow(value, 3) - value * amplitude * Math.Sin(value * MathHelper.Pi));
        }

        public static float BackOut(float value) => Out(value, BackIn);

        public static float BackInOut(float value) => InOut(value, BackIn);

        #endregion

        #region Bounce

        public static float BounceIn(float value)
        {
            const float bounceConst1 = 2.75f;
            var bounceConst2 = (float)Math.Pow(bounceConst1, 2);

            value = 1 - value; // flip x-axis

            if (value < 1 / bounceConst1) // big bounce
                return 1f - bounceConst2 * value * value;

            if (value < 2 / bounceConst1)
                return 1 - (float)(bounceConst2 * Math.Pow(value - 1.5f / bounceConst1, 2) + .75);

            if (value < 2.5 / bounceConst1)
                return 1 - (float)(bounceConst2 * Math.Pow(value - 2.25f / bounceConst1, 2) + .9375);

            // small bounce
            return 1f - (float)(bounceConst2 * Math.Pow(value - 2.625f / bounceConst1, 2) + .984375);
        }

        public static float BounceOut(float value) => Out(value, BounceIn);

        public static float BounceInOut(float value) => InOut(value, BounceIn);

        #endregion

        #region Private Helper Methods

        private static float Out(float value, Func<float, float> function)
        {
            return 1 - function(1 - value);
        }

        private static float InOut(float value, Func<float, float> function)
        {
            if (value < 0.5f)
            {
                return 0.5f * function(value * 2);
            }

            return 1f - 0.5f * function(2 - value * 2);
        }

        #endregion
    }
}