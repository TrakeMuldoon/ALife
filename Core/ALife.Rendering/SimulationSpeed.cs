namespace ALife.Rendering
{
    /// <summary>
    /// The speed of the simulation
    /// </summary>
    public enum SimulationSpeed
    {
        /// <summary>
        /// 2 ticks per second
        /// </summary>
        VerySlow = 2,

        /// <summary>
        /// 30 ticks per second
        /// </summary>
        Slow = 30,

        /// <summary>
        /// 60 ticks per second
        /// </summary>
        Normal = 60,

        /// <summary>
        /// 120 ticks per second
        /// </summary>
        Fast = 120,

        /// <summary>
        /// 240 ticks per second
        /// </summary>
        VeryFast = 240,

        /// <summary>
        /// 480 ticks per second
        /// </summary>
        VeryVeryFast = 480,

        /// <summary>
        /// 10000 ticks per second. This would be great, but we likely won't hit it This is to basically simulate
        /// "unlimited" speed
        /// </summary>
        VeryVeryVeryFast = 10000,
    }

    /// <summary>
    /// Extensions and constants for the SimulationSpeed enum
    /// </summary>
    public static class SimulationSpeedExtensions
    {
        /// <summary>
        /// The default speed
        /// </summary>
        public const SimulationSpeed DEFAULT_SPEED = SimulationSpeed.Normal;

        /// <summary>
        /// The space per slider tick
        /// </summary>
        public const int SPACE_PER_SLIDER_TICK = 10;

        /// <summary>
        /// The maximum slider position
        /// </summary>
        public static int MAX_SLIDER_POSITION = Enum.GetValues(typeof(SimulationSpeed)).Length * SPACE_PER_SLIDER_TICK - SPACE_PER_SLIDER_TICK;

        /// <summary>
        /// The slider positions for speed
        /// </summary>
        public static Dictionary<SimulationSpeed, int> SLIDER_POSITIONS_FOR_SPEED;

        /// <summary>
        /// Initializes the <see cref="SimulationSpeedExtensions"/> class.
        /// </summary>
        static SimulationSpeedExtensions()
        {
            var speeds = Enum.GetValues(typeof(SimulationSpeed)).AsQueryable().Cast<SimulationSpeed>().OrderBy(x => (int)x);

            SLIDER_POSITIONS_FOR_SPEED = new();
            int position = 0;
            foreach(SimulationSpeed speed in speeds)
            {
                SLIDER_POSITIONS_FOR_SPEED.Add(speed, position);
                position += SPACE_PER_SLIDER_TICK;
            }
        }

        /// <summary>
        /// Gets the slider position.
        /// </summary>
        /// <param name="speed">The speed.</param>
        /// <returns></returns>
        public static int GetSliderPosition(this SimulationSpeed speed)
        {
            return SLIDER_POSITIONS_FOR_SPEED[speed];
        }

        public static SimulationSpeed SimulationSpeedFromSliderValue(this int value)
        {
            foreach(var pair in SLIDER_POSITIONS_FOR_SPEED)
            {
                if(pair.Value == value)
                {
                    return pair.Key;
                }
            }

            return 0;
        }

        /// <summary>
        /// Converts to double.
        /// </summary>
        /// <param name="speed">The speed.</param>
        /// <returns></returns>
        public static double ToDouble(this SimulationSpeed speed)
        {
            return (double)speed;
        }

        /// <summary>
        /// Converts to target milliseconds.
        /// </summary>
        /// <param name="speed">The speed.</param>
        /// <returns></returns>
        public static int ToTargetMS(this SimulationSpeed speed)
        {
            int ms = (int)Math.Round(1000 / (double)speed);
            return ms;
        }
    }
}
