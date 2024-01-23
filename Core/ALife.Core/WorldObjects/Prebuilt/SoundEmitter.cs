using ALife.Core.Geometry.Shapes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALife.Core.WorldObjects.Prebuilt
{
    public class SoundEmitter : WorldObject
    {
        private static int EmitterCount = 1;
        public SoundEmitter(Geometry.Shapes.Point centrePoint) :
            base(centrePoint, new Circle(1), "SoundEmitter", (++EmitterCount).ToString(), ReferenceValues.CollisionLevelSound, Color.Black)
        {
        }

        public override WorldObject Clone()
        {
            throw new NotImplementedException("Sound Emitters Should Not Be Cloned");
        }

        public override void Die()
        {
            throw new NotImplementedException("Sound Emitters Should Not Die");
        }

        private int turnCount = 0;
        public override void ExecuteAliveTurn()
        {
            turnCount += 1;
            if(turnCount % 3 == 0)
            {
                SoundWave sw = new SoundWave(255, 0, 0, Shape.CentrePoint);
                Planet.World.AddObjectToWorld(sw);
            }
        }

        public override void ExecuteDeadTurn()
        {
            throw new NotImplementedException("Sound Emitters Can't be Dead");
        }

        public override WorldObject Reproduce()
        {
            throw new NotImplementedException("Sound Emitters are lonely folk");
        }
    }
}
