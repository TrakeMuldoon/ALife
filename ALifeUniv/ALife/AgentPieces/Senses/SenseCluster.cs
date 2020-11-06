using ALifeUni.ALife.UtilityClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using Windows.UI;

namespace ALifeUni.ALife
{

    public abstract class SenseCluster : IHasShape
    {
        public readonly string Name;
        public readonly List<SenseInput> SubInputs = new List<SenseInput>();
        readonly string CollisionLevel = ReferenceValues.CollisionLevelPhysical;
        readonly Agent parent;

        public SenseCluster(Agent parent, String name)
        {
            this.parent = parent;
            Name = name;
        }

        public virtual void Detect()
        {
            ICollisionMap collider = Planet.World.CollisionLevels[this.CollisionLevel];

            IShape myShape = this.GetShape();

            myShape.Reset();
            BoundingBox bb = myShape.BoundingBox;
            List<WorldObject> collisions = collider.QueryForBoundingBoxCollisions(bb, parent);
            collisions = CollisionDetector.FineGrainedCollisionDetection(collisions, myShape);

            //if(collisions.Count > 0)
            //{
            //    GetShape().DebugColor = Colors.Green;
            //}
            //else
            //{
            //    GetShape().DebugColor = Colors.Yellow;
            //}

            foreach(SenseInput si in SubInputs)
            {
                si.SetValue(collisions);
            }
        }

        public abstract IShape GetShape();

        //On/Off
        //How Many (0-255 for each)
        //Detect Magnitude (Volume for sound, power for smell, Vibrance for sight)
        //Detect Quality (0-255 for sound, 0-255 for smell, 0-255 for R, G and B for sight)
        //Detect Category (10 known categories?)
    }
}
