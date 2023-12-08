using ALifeUni.ALife.Geometry;
using ALifeUni.ALife.Shapes;
using ALifeUni.ALife.Utility;
using System;
using System.Collections.Generic;
using Windows.Foundation;

namespace ALifeUni.ALife
{
    public abstract class WorldObjectDistributor
    {
        protected readonly Zone StartZone;
        protected readonly bool TrackCollisions;
        protected readonly string CollisionLevel;

        protected WorldObjectDistributor(Zone startZone, bool trackCollisions, string collisionLevel)
        {
            StartZone = startZone;
            TrackCollisions = trackCollisions;
            CollisionLevel = collisionLevel;
        }

        public abstract Point NextAgentCentre(double BBLength, double BBHeight);
    }

    public readonly struct StraightLineDistributorConfig
    {
        readonly public Angle Direction;
        readonly public int LineDepth;
        readonly public double Separation;
        readonly public Point StartPoint;
        readonly public bool WrapAround;
        readonly public int Length;
        readonly public bool Initialized;

        public StraightLineDistributorConfig(Angle direction, double separation, Point startPoint) : this()
        {
            Direction = direction;
            Separation = separation;
            StartPoint = startPoint;

            Length = 0;
            WrapAround = true;
            LineDepth = 1;
            Initialized = true;
        }
        public StraightLineDistributorConfig(Angle direction, double separation, Point startPoint, int lineDepth) : this()
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
        private StraightLineDistributorConfig Config;

        public StraightLineAgentDistributor(Zone startZone, bool trackCollisions, string collisionLevel, StraightLineDistributorConfig config) : base(startZone, trackCollisions, collisionLevel)
        {
            if(!config.Initialized)
            {
                throw new ArgumentException("Uninitialized Config is being utilized. This is unacceptable.");
            }
            Point sp = config.StartPoint;
            if(sp.X < startZone.BoundingBox.MinX
                || sp.X > startZone.BoundingBox.MaxX
                || sp.Y < startZone.BoundingBox.MinY
                || sp.Y > startZone.BoundingBox.MaxY)
            {
                throw new ArgumentOutOfRangeException("StartPoint is outside of the zone.");
            }

            Point nextPoint = ExtraMath.TranslateByVector(config.StartPoint, config.Direction, config.Separation);
            separationPoint = new Point(nextPoint.X - config.StartPoint.X, nextPoint.Y - config.StartPoint.Y);
            deltaStart = new Point(config.StartPoint.X - startZone.TopLeft.X, config.StartPoint.Y - startZone.TopLeft.Y);
        }

        private int counter = 0;
        private Point separationPoint;
        private Point deltaStart;

        public override Point NextAgentCentre(double BBLength, double BBHeight)
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
                return new Point(newX, newY);
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

    public class RandomAgentDistributor : WorldObjectDistributor
    {
        public RandomAgentDistributor(Zone startZone, bool trackCollisions, string collisionLevel) : base(startZone, trackCollisions, collisionLevel)
        {
        }

        public override Point NextAgentCentre(double BBLength, double BBHeight)
        {
            double halfLength = BBLength / 2;
            double halfHeight = BBHeight / 2;

            double xMin = StartZone.TopLeft.X + halfLength;
            double xMax = StartZone.TopLeft.X + StartZone.XWidth - halfLength;
            double yMin = StartZone.TopLeft.Y + halfHeight;
            double yMax = StartZone.TopLeft.Y + StartZone.YHeight - halfHeight;

            //If we aren't tracking collisions, then any point in the area is valid
            if(!TrackCollisions)
            {
                double X = Planet.World.NumberGen.Next((int)xMin, (int)xMax);
                double Y = Planet.World.NumberGen.Next((int)yMin, (int)yMax);
                return new Point(X, Y);
            }

            int attempts = 0;
            List<WorldObject> collisions;
            double newX, newY;
            do
            {
                newX = Planet.World.NumberGen.Next((int)xMin, (int)xMax);
                newY = Planet.World.NumberGen.Next((int)yMin, (int)yMax);

                BoundingBox bb = new BoundingBox(newX - halfLength, newY - halfHeight, newX + halfLength, newY + halfHeight);
                collisions = Planet.World.CollisionLevels[CollisionLevel].QueryForBoundingBoxCollisions(bb);
                attempts++;
            } while(collisions.Count > 0
                    && attempts < 15); //TODO: number of attempts is hardcoded here

            if(collisions.Count == 0)
            {
                return new Point(newX, newY);
            }
            else
            {
                throw new Exception("Unable to place Agent (random)");
            }
        }
    }
}
