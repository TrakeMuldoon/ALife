﻿using ALifeUni.ALife.Geometry;
using ALifeUni.ALife.Shapes;
using ALifeUni.ALife.Utility;
using ALifeUni.ALife.WorldObjects.Agents.Senses.Generic;
using System;
using Windows.UI;

namespace ALifeUni.ALife.WorldObjects.Agents.Senses
{
    class SquareSenseCluster : SenseCluster
    {
        private ChildRectangle myShape;
        public override IShape Shape
        {
            get
            {
                return myShape;
            }
        }

        public SquareSenseCluster(WorldObject parent, string name, double FBLength, double RLWidth)
            : base(parent, name)
        {
            myShape = new ChildRectangle(parent.Shape, new Angle(45), 5.0, FBLength, RLWidth);

            SubInputs.Add(new AnyInput(name + ".SomethingClose"));
            SubInputs.Add(new CountInput(name + ".HowMany"));
        }

        //TODO: Implement SquareSenseCluster with EvoNumbers... whoops
        [Obsolete("SquareSenseClusterDefault is deprecated, please use SquareSenseCluster with EvoNumbers instead.")]
        public SquareSenseCluster(WorldObject parent, string name)
            : this(parent, name, 80, 30)
        {
        }

        public SquareSenseCluster(WorldObject parent, string name, double FBLength, double RLWidth, Color myColor)
            : this(parent, name, FBLength, RLWidth)
        {
            myShape.Color = myColor;
        }

        public override SenseCluster CloneSense(WorldObject newParent)
        {
            return new SquareSenseCluster(newParent, Name, myShape.FBLength, myShape.RLWidth, myShape.Color.Clone());
        }

        public override SenseCluster ReproduceSense(WorldObject newParent)
        {
            return new SquareSenseCluster(newParent, Name, myShape.FBLength, myShape.RLWidth, myShape.Color.Clone());
        }
    }
}