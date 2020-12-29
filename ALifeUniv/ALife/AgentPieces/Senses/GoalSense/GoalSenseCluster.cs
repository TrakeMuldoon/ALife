using ALifeUni.ALife.UtilityClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALifeUni.ALife.AgentPieces.Senses.GoalSense
{
    public class GoalSenseCluster : SenseCluster
    {
        public GoalSenseCluster(WorldObject parent, string name) : base(parent, name)
        {
        }

        public override IShape Shape => throw new NotImplementedException();


        public override void Detect()
        {
            base.Detect();
        }

        public override SenseCluster CloneSense(WorldObject newParent)
        {
            throw new NotImplementedException();
        }

        public override SenseCluster ReproduceSense(WorldObject newParent)
        {
            throw new NotImplementedException();
        }
    }
}
