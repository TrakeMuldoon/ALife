using ALife.Core.Geometry.Shapes;
using System;

namespace ALife.Core.WorldObjects
{
    public class SoundWave : WorldObject
    {
        static int SoundUniqueID = 1;

        public byte Intensity;
        public byte Pitch;
        public byte Timbre;

        public SoundWave(byte intensity, byte pitch, byte timbre, Point coords)
            : base(coords, new Circle(coords, intensity), "Sound", (SoundUniqueID++).ToString(), ReferenceValues.CollisionLevelSound, System.Drawing.Color.WhiteSmoke)
        {
            Intensity = intensity;
            Pitch = pitch;
            Timbre = timbre;
        }

        public override void Die()
        {
            throw new NotImplementedException();
        }

        public override void ExecuteAliveTurn()
        {
            TrashItem();
        }

        public override void ExecuteDeadTurn()
        {
            throw new NotImplementedException();
        }

        public override WorldObject Clone()
        {
            throw new NotImplementedException("SoundWaves should not be cloned");
        }

        public override WorldObject Reproduce()
        {
            throw new NotImplementedException("SoundWaves should not reproduce");
        }
    }
}
