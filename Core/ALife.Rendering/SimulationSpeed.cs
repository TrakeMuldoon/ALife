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
}
