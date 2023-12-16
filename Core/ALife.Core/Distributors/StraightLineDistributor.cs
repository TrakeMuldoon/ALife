using ALife.Core.Geometry;
using ALife.Core.Geometry.Shapes;
using ALife.Core.Utility;
using ALife.Core.WorldObjects;

namespace ALife.Core.Distributors
{
    public readonly struct StraightLineDistributorConfig
    {
        readonly public Angle Direction;
        readonly public int LineDepth;
        readonly public double Separation;
        readonly public Geometry.Shapes.Point StartPoint;
        readonly public bool WrapAround;
        readonly public int Length;
        readonly public bool Initialized;

        public StraightLineDistributorConfig(Angle direction, double separation, Geometry.Shapes.Point startPoint) : this()
        {
            Direction = direction;
            Separation = separation;
            StartPoint = startPoint;

            Length = 0;
            WrapAround = true;
            LineDepth = 1;
            Initialized = true;
        }
        public StraightLineDistributorConfig(Angle direction, double separation, Geometry.Shapes.Point startPoint, int lineDepth) : this()
        {
            Direction = direction;
            Separation = separation;
            StartPoint = startPoint;
            LineDepth = lineDepth;

            Length = 0;
            WrapAround = true;
            Initialized = true;
        }
    }

    public class StraightLineAgentDistributor : WorldObjectDistributor
    {
        private readonly StraightLineDistributorConfig Config;

        public StraightLineAgentDistributor(Zone startZone, bool trackCollisions, string collisionLevel, StraightLineDistributorConfig config) : base(startZone, trackCollisions, collisionLevel)
        {
            if(!config.Initialized)
            {
                throw new ArgumentException("Uninitialized Config is being utilized. This is unacceptable.");
            }
            Geometry.Shapes.Point sp = config.StartPoint;
            if(sp.X < startZone.BoundingBox.MinX
                || sp.X > startZone.BoundingBox.MaxX
                || sp.Y < startZone.BoundingBox.MinY
                || sp.Y > startZone.BoundingBox.MaxY)
            {
                throw new ArgumentOutOfRangeException("StartPoint is outside of the zone.");
            }

            Geometry.Shapes.Point nextPoint = ExtraMath.TranslateByVector(config.StartPoint, config.Direction, config.Separation);
            separationPoint = new Geometry.Shapes.Point(nextPoint.X - config.StartPoint.X, nextPoint.Y - config.StartPoint.Y);
            deltaStart = new Geometry.Shapes.Point(config.StartPoint.X - startZone.TopLeft.X, config.StartPoint.Y - startZone.TopLeft.Y);
        }

        private int counter = 0;
        private Geometry.Shapes.Point separationPoint;
        private Geometry.Shapes.Point deltaStart;

        public override Geometry.Shapes.Point NextObjectCentre(double BBLength, double BBHeight)
        {
            double halfLength = BBLength / 2;
            double halfHeight = BBHeight / 2;
            List<WorldObject> collisions;

            double newX;
            double newY;


            int attempts = 0;
            do
            {
                newX = CalculateNextPos(counter, separationPoint.X, StartZone.XWidth, deltaStart.X, Config.StartPoint.X);
                newY = CalculateNextPos(counter, separationPoint.Y, StartZone.YHeight, deltaStart.Y, Config.StartPoint.Y);


                BoundingBox bb = new BoundingBox(newX - halfLength, newY - halfHeight, newX + halfLength, newY + halfHeight);
                collisions = Planet.World.CollisionLevels[CollisionLevel].QueryForBoundingBoxCollisions(bb);
                counter++;
                attempts++;
            } while(collisions.Count > 0
                    && attempts < 10); //TODO: number of attempts is hardcoded here

            if(collisions.Count == 0)
            {
                return new Geometry.Shapes.Point(newX, newY);
            }
            else
            {
                throw new Exception("Unable to place Agent (straight line)");
            }
        }

        public static double CalculateNextPos(int counter, double separationValue, double modValue, double deltaStartValue, double startValue)
        {
            double fullDelta = (counter * separationValue);
            double modDelta = fullDelta % modValue;
            double posDelta = modDelta < 0 ? modDelta + modValue : modDelta;

            double offset = deltaStartValue + posDelta;
            double delta = offset % modValue;

            return delta + startValue;
        }
    }
}
