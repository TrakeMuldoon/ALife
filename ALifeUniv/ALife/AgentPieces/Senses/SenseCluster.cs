using ALifeUni.ALife.UtilityClasses;
using System;
using System.Collections.Generic;
using Windows.UI;

namespace ALifeUni.ALife
{

    public abstract class SenseCluster : IHasShape
    {
        public readonly string Name;
        public readonly List<SenseInput> SubInputs = new List<SenseInput>();
        readonly string CollisionLevel = ReferenceValues.CollisionLevelPhysical;
        readonly WorldObject parent;

        public virtual IShape Shape
        {
            get; 
        }

        public SenseCluster(WorldObject parent, String name)
        {
            this.parent = parent;
            Name = name;
        }

        public virtual void Detect()
        {
            //TODO: Factor this out. The SenseClusters shouldn't need to know the details of the collision detection
            ICollisionMap collider = Planet.World.CollisionLevels[this.CollisionLevel];

            Shape.Reset();
            BoundingBox bb = Shape.BoundingBox;
            List<WorldObject> collisions = collider.QueryForBoundingBoxCollisions(bb, parent);
            collisions = CollisionDetector.FineGrainedCollisionDetection(collisions, Shape);

            //Shape.DebugColor = collisions.Count > 0 ?  Colors.Red : Colors.Transparent;
            Shape.Color = collisions.Count > 0 ? Colors.DodgerBlue : Colors.DarkBlue;
            foreach(SenseInput si in SubInputs)
            {
                si.SetValue(collisions);
            }
        }

        public abstract SenseCluster CloneSense(WorldObject newParent);

        //On/Off
        //How Many (0-255 for each)
        //Detect Magnitude (Volume for sound, power for smell, Vibrance for sight)
        //Detect Quality (0-255 for sound, 0-255 for smell, 0-255 for R, G and B for sight)
        //Detect Category (10 known categories?)
    }
}
