using System;
using System.Collections.Generic;
using System.Linq;

namespace ALife.Core
{
    /// <summary>
    /// A performance counter for indicating the performance of a component of the simulation.
    /// </summary>
    public class PerformanceCounter
    {
        /// <summary>
        /// The maximum samples
        /// </summary>
        public const int MAXIMUM_SAMPLES = 100;

        /// <summary>
        /// The sample buffer
        /// </summary>
        private Queue<double> _sampleBuffer = new(MAXIMUM_SAMPLES + 1);

        /// <summary>
        /// Gets the average frames per ticks.
        /// </summary>
        /// <value>The average frames per ticks.</value>
        public double AverageFramesPerTicks { get; private set; }

        /// <summary>
        /// Gets the current frames per ticks.
        /// </summary>
        /// <value>The current frames per ticks.</value>
        public double CurrentFramesPerTicks { get; private set; }

        /// <summary>
        /// Gets the total seconds.
        /// </summary>
        /// <value>The total seconds.</value>
        public double TotalSeconds { get; private set; }

        /// <summary>
        /// Gets the total ticks.
        /// </summary>
        /// <value>The total frames.</value>
        public ulong TotalTicks { get; private set; }

        /// <summary>
        /// The last time
        /// </summary>
        private DateTime _lastTime = DateTime.Now;

        /// <summary>
        /// Updates the specified delta time.
        /// </summary>
        /// <param name="deltaTime">The delta time.</param>
        public void Update()
        {
            DateTime currentTime = DateTime.Now;
            double deltaTime = (currentTime - _lastTime).TotalSeconds;
            CurrentFramesPerTicks = 1.0f / deltaTime;

            _sampleBuffer.Enqueue(CurrentFramesPerTicks);

            if(_sampleBuffer.Count > MAXIMUM_SAMPLES)
            {
                _sampleBuffer.Dequeue();
                AverageFramesPerTicks = _sampleBuffer.Average(i => i);
            }
            else
            {
                AverageFramesPerTicks = CurrentFramesPerTicks;
            }

            TotalTicks++;
            TotalSeconds += deltaTime;
            _lastTime = currentTime;
        }

        /// <summary>
        /// Clears the buffer.
        /// </summary>
        public void ClearBuffer()
        {
            _sampleBuffer.Clear();
        }
    }
}
