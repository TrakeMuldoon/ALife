using System.Globalization;
using ALife.Spatial.Demo;

// Optional arguments: the index to start with (1-9, as on the keys), how many circles, and a number of seconds to profile
// for, which prints the average timings and quits; "nonodes" after them starts with the index's nodes hidden, and "serial"
// with each step run before drawing rather than alongside it, "bounce" with bouncing on, and "shapes" or "compounds" with
// shapes rather than circles.
int kind = args.Length > 0 && int.TryParse(args[0], out int k) ? k : 1;
int count = args.Length > 1 && int.TryParse(args[1], out int n) ? n : 2000;
double profile = args.Length > 2 && double.TryParse(args[2], CultureInfo.InvariantCulture, out double p) ? p : 0;
using SpatialDemoGame game = new(kind, count, profile) { DrawNodes = !args.Contains("nonodes"), OverlapStep = !args.Contains("serial"),
    Bounce = args.Contains("bounce"), ShapeMode = args.Contains("compounds") ? 2 : args.Contains("shapes") ? 1 : 0 };
game.Run();
