using ALifeUni.ALife.UtilityClasses;
using System;
using System.Collections.Generic;
using Windows.UI;

namespace ALifeUni.ALife
{
    public class AgentShadow : WorldObject
    {
        public readonly List<SenseCluster> Senses = new List<SenseCluster>();

        public AgentShadow(Agent self) : base(self.CentrePoint, self.Radius, self.GenusLabel, "sha:" + self.IndividualLabel, self.CollisionLevel, self.Color)
        {
            DebugColor = Colors.Yellow;
            Orientation = new Angle(self.Orientation.Degrees);
            foreach(SenseCluster sc in self.Senses)
            {
                Senses.Add(sc.Clone(this));
            }
        }

        public override void Die()
        {
            throw new NotImplementedException();
        }

        public override void ExecuteAliveTurn()
        {
            throw new NotImplementedException();
        }

        public override void ExecuteDeadTurn()
        {
            throw new NotImplementedException();
        }

        public override WorldObject Reproduce(bool exactCopy)
        {
            throw new NotImplementedException();
        }
    }
}
