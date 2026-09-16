using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreedomEngine.Collections.Interfaces
{
    public interface ITween : IControllableProcess
    {
        /// <summary>
        /// Gets the total duration of the tween.
        /// </summary>
        public TimeSpan Duration { get; }

        /// <summary>
        /// Gets the amount of time that has elapsed since the tween started.
        /// </summary>
        public TimeSpan Elapsed { get; }
    }
}
