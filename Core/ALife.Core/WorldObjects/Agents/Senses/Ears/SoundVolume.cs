using ALife.Core.Geometry.New;
using ALife.Core.Geometry.Shapes;
using ALife.Core.WorldObjects.Prebuilt;
using System;
using System.Collections.Generic;

namespace ALife.Core.WorldObjects.Agents.Senses.Ears
{
    public class SoundVolume : SenseInput<Double>
    {
        readonly IShape parentShape;

        public SoundVolume(string name, IShape parentShape) : base(name)
        {
            this.parentShape = parentShape;
        }

        public override void SetValue(List<WorldObject> collisions)
        {
            double sum = 0;
            //This is a weird custom damping algorithm. 

            //Each sound is dampened by its ratio to the loudest sound heard. 
            //So if the sounds are 100, 20, 1, then the total would be "100 * (100/100) + 20 * (20/100) + 1 * 1/100" or 100 + 4 + 0.1.
            //This approximates the fact that many small sounds can add up to being cacophonous, but they don't drown out the loud sound if there is one.

            double max = double.MinValue;
            foreach(WorldObject wo in collisions)
            {
                SoundWave sw = wo as SoundWave;
                if(sw is null)
                {
                    if(wo is SoundEmitter)
                    {
                        //This is a little hack, because we need to put the SoundEmitter on some level.
                        continue; //We're very close to success!!
                    }
                    else
                    {
                        //Unknown object shouldn't be here.
                        throw new Exception("Soundwaves are the only supported things for ears to hear at the moment.");
                    }
                }
                if(sw.Intensity > max)
                {
                    max = sw.Intensity;
                }
            }

            foreach(WorldObject wo in collisions)
            {
                SoundWave sw = wo as SoundWave;
                if(sw is null)
                {
                    if(wo is SoundEmitter)
                    {
                        //This is a little hack, because we need to put the SoundEmitter on some level.
                        continue; //We're very close to success!!
                    }
                }

                double distanceBetween = GeometryMath.DistanceBetweenTwoPoints(this.parentShape.CentrePoint, sw.Shape.CentrePoint);
                double volume = sw.Intensity - distanceBetween;
                sum += volume * (double)volume / max;
            }

            if(sum > 255)
            {
                sum = 255;
            }

            Value = sum / 255;
        }
    }
}
