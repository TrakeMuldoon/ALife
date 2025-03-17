using System;
using Microsoft.Xna.Framework;

static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        using(Game g = new Game())
        {
            new GraphicsDeviceManager(g);
            g.Run();
        }
    }
}