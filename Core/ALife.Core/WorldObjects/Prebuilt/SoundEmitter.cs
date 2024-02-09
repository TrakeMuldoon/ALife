using ALife.Core.Geometry.Shapes;
using ALife.Core.Utility.Colours;
using System;

namespace ALife.Core.WorldObjects.Prebuilt
{
    public class SoundEmitter : WorldObject
    {
        private static int EmitterCount = 1;
        public SoundEmitter(Point centrePoint) :
            base(centrePoint, new Circle(1), "SoundEmitter", (++EmitterCount).ToString(), ReferenceValues.CollisionLevelSound, Colour.Black)
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
            if(turnCount % 10 == 0)
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
