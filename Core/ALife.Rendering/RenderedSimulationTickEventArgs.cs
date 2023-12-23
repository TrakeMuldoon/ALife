namespace ALife.Rendering
{
    public class RenderedSimulationTickEventArgs : EventArgs
    {
        public RenderedSimulationTickEventArgs(DateTime time, int turn)
        {
            Time = time;
            Turn = turn;
        }

        public DateTime Time { get; }

        public int Turn { get; }
    }
}
