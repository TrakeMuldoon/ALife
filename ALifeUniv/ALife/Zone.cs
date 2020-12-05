using ALifeUni.ALife.UtilityClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife
{
    [DebuggerDisplay("Zone: {Name}")]
    public class Zone : AARectangle, IHasShape
    {
        public String Name
        {
            get;
            private set;
        }

        public IShape Shape
        {
            get
            {
                return this;
            }
        }

        public readonly AgentDistributor Distributor;
        public readonly List<Agent> MyAgents = new List<Agent>();

        public Zone(String name, String distributorType, Color color
                    ,  Point topLeft, double xWidth, double yHeight) : base(topLeft, xWidth, yHeight, color)
        {
            Color lowAlpha = Color;
            lowAlpha.A = 50;
            Color = lowAlpha;

            Name = name;

            //Distributor type is currently unused but will be used later I guess?
            Distributor = new RandomAgentDistributor(this, true, ReferenceValues.CollisionLevelPhysical);
        }
    }
}
