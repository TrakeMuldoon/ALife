﻿using ALife.Core.Collision;
using ALife.Core.Geometry.Shapes;
using System;
using System.Collections.Generic;

namespace ALife.Core.WorldObjects.Agents.Senses
{

    public abstract class SenseCluster : IHasShape
    {
        public readonly string Name;
        public readonly List<SenseInput> SubInputs = new List<SenseInput>();
        public readonly string CollisionLevel;
        private readonly ICollisionMap<WorldObject> CollisionMap;
        readonly WorldObject parent;

        public abstract IShape Shape
        {
            get;
        }

        public SenseCluster(WorldObject parent, String name, string collisionLevel = ReferenceValues.CollisionLevelPhysical)
        {
            CollisionLevel = collisionLevel;
            CollisionMap = Planet.World.CollisionLevels[this.CollisionLevel];

            this.parent = parent;
            Name = name;
        }

        public virtual void Detect()
        {
            Shape.Reset();

            List<WorldObject> collisions = CollisionMap.DetectCollisions(this, parent);

            //This is for debug purposes. The shape of a "sense" is not viewable to other WorldObjects, so changing it
            // does not impact the Simulation Worldstate.
            Shape.Color = collisions.Count > 0 ? System.Drawing.Color.DodgerBlue : System.Drawing.Color.DarkBlue;

            //Set the value for each input based on the collisions detected.
            foreach(SenseInput si in SubInputs)
            {
                si.SetValue(collisions);
            }
        }

        public abstract SenseCluster CloneSense(WorldObject newParent);

        public abstract SenseCluster ReproduceSense(WorldObject newParent);

        //On/Off
        //How Many (0-255 for each)
        //Detect Magnitude (Volume for sound, power for smell, Vibrance for sight)
        //Detect Quality (0-255 for sound, 0-255 for smell, 0-255 for R, G and B for sight)
        //Detect Category (10 known categories?)
    }
}
