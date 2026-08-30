using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreedomEngine.Graphics
{
    public abstract class AnimatedResource
    {
        #region Properties

        public virtual int Length { get; }

        /// <summary>
        /// Gets the amount of time to delay before moving to the next frame.
        /// </summary>
        public TimeSpan Delay { get; }

        /// <summary>
        /// Gets the list of delays for each frame in the animation, allowing for variable frame timing.
        /// </summary>
        public TimeSpan[] Delays { get; }

        /// <summary>
        /// Gets a value indicating whether this sprite uses a single frame delay for all frames (true) or individual delays per frame (false).
        /// </summary>
        public bool MonoFrameDelay { get; }

        #endregion

        #region Constructors

        public AnimatedResource(TimeSpan delay)
        {
            Delay = delay;
            MonoFrameDelay = true;
        }

        public AnimatedResource(TimeSpan[] delays)
        {
            Delays = delays;
            MonoFrameDelay = false;
        }

        #endregion

        #region Public Methods

        public int GetNextFrame(int currentFrameIndex, TimeSpan elapsedTime, out TimeSpan newElapsedTime)
        {
            if (MonoFrameDelay)
            {
                if (elapsedTime >= Delay)
                {
                    newElapsedTime = elapsedTime - Delay;
                    return (currentFrameIndex + 1) % Length;
                }
            }
            else
            {
                if (elapsedTime >= Delays[currentFrameIndex])
                {
                    newElapsedTime = elapsedTime - Delays[currentFrameIndex];
                    return (currentFrameIndex + 1) % Length;
                }
            }

            newElapsedTime = elapsedTime;
            return currentFrameIndex;
        }

        #endregion
    }
}
