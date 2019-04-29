﻿using System;
using System.Collections.Generic;

namespace ALifeUni.ALife
{
    public class Brain
    {
        public IEnumerable<Behaviour> Behaviours
        {
            get
            {
                return behaviours;
            }
        }

        private List<Behaviour> behaviours;
        private Agent parent;


        public Brain(Agent parent)
        {
            this.parent = parent;
            //behaviours = new List<Behaviour>();
            ////TODO: Config this, for now, it'll be 10

            //for(int i = 0; i < 10; i ++)
            //{

            //}
        }

        internal void ExecuteTurn()
        {
            //TODO: Holy Crap this is bad
            double randNum = Planet.World.NumberGen.NextDouble();
            
            //Reset Colour
            parent.Actions["Color"].AttemptEnact(0.0099);
            if (randNum < 0.20)
            {
                parent.Actions["Rotate"].AttemptEnact(randNum * 3);
            }
            else if(randNum < 0.90)
            {
                parent.Actions["Move"].AttemptEnact((randNum - 0.33) * 3);
            }
            else
            {
                parent.Actions["Color"].AttemptEnact(0.99);
            }
        }
    }
}