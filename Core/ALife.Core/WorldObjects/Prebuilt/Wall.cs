using ALife.Core.Geometry;
using ALife.Core.Geometry.Shapes;
using ALife.Core.Utility;
using ALife.Core.Utility.Colours;
using ALife.Core.Utility.Maths;
using System;
using System.Collections.Generic;

namespace ALife.Core.WorldObjects.Prebuilt
{
    public class Wall : WorldObject
    {
        private Rectangle rShape;
        public Rectangle RShape
        {
            get
            {
                return rShape;
            }
        }

        public override IShape Shape
        {
            get { return rShape; }
            protected set
            {
                Rectangle rec = value as Rectangle;
                rShape = rec ?? throw new Exception("Cannot set a wall to be any other shape");
            }
        }

        public Wall(Geometry.Shapes.Point centrePoint, double Length, Angle orientation, string individualLabel)
            : base("Wall", individualLabel, ReferenceValues.CollisionLevelPhysical)
        {

            rShape = new Rectangle(Length, 5, Colour.DarkKhaki);
            Shape.CentrePoint = centrePoint;
            Shape.Orientation = orientation;
            Shape.Reset();
        }

        public override void Die()
        {
            //TODO: Abstract this out
            this.Alive = false;
            Planet.World.RemoveWorldObject(this);
        }

        public override void ExecuteAliveTurn()
        {
            //Do nothing. It's a wall.
        }

        public override void ExecuteDeadTurn()
        {
            throw new NotImplementedException();
        }

        public override WorldObject Reproduce()
        {
            throw new NotImplementedException();
        }
        public override WorldObject Clone()
        {
            throw new NotImplementedException();
        }

        private const int SplitLength = 100;
        public static List<Wall> WallSplitter(Wall wall)
        {
            List<Wall> segments = new List<Wall>();
            int numSplits = (int)(wall.RShape.FBLength / SplitLength);
            double segmentLength = wall.RShape.FBLength / numSplits;
            for(int i = 1; i < numSplits + 1; i++)
            {
                Angle ori = wall.Shape.Orientation.Clone();
                double indexer = i - ((numSplits + 1) / 2.0);

                Geometry.Shapes.Point p = GeometryMaths.TranslateByVector(wall.Shape.CentrePoint, ori, segmentLength * indexer);
                Wall w = new Wall(p, segmentLength, ori, wall.IndividualLabel + "~" + (i + 1));
                segments.Add(w);
            }
            return segments;
        }
    }
}
