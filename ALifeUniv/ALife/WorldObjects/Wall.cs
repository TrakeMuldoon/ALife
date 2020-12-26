using ALifeUni.ALife.UtilityClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife.Objects
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
                if(rec == null)
                {
                    throw new Exception("Cannot set a wall to be any other shape");
                }
                rShape = rec;
            }
        }

        public Wall(Point centrePoint, double Length, Angle orientation, string individualLabel)
            : base("Wall", individualLabel, ReferenceValues.CollisionLevelPhysical)
        {

            rShape = new Rectangle(Length, 5, Colors.DarkKhaki);
            Shape.CentrePoint = centrePoint;
            Shape.Orientation = orientation;
            Shape.Reset();
        }

        public override void Die()
        {
            throw new NotImplementedException();
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

        private static int SplitLength = 100;
        public static List<Wall> WallSplitter(Wall wall)
        {
            List<Wall> segments = new List<Wall>();
            int numSplits = (int)(wall.RShape.FBLength / SplitLength);
            double segmentLength = wall.RShape.FBLength / numSplits;
            for(int i = 1; i < numSplits+1; i++)
            {
                Angle ori = wall.Shape.Orientation.Clone();
                double indexer = i - ((numSplits + 1) / 2.0);

                Point p = ExtraMath.TranslateByVector(wall.Shape.CentrePoint, ori.Radians, segmentLength * indexer);
                Wall w = new Wall(p, segmentLength, ori, wall.IndividualLabel + "~" + (i + 1));
                segments.Add(w);
            }
            return segments;
        }
    }
}
