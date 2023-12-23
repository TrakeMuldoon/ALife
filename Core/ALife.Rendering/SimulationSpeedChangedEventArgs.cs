namespace ALife.Rendering
{
    public class SimulationSpeedChangedEventArgs : EventArgs
    {
        public SimulationSpeedChangedEventArgs(SimulationSpeed oldSpeed, SimulationSpeed newSpeed)
        {
            OldSpeed = oldSpeed;
            NewSpeed = newSpeed;
        }

        public SimulationSpeed NewSpeed { get; }
        public SimulationSpeed OldSpeed { get; }
    }
}
