using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ALifeUni.ALife
{
    public sealed class Planet
    {
        private static Planet instance;

        static Planet()
        {

        }

        private Planet(int seed)
        {
            NumberGen = new Random(seed);
        }

        public static Planet World
        {
            get
            {
                if (instance != null)
                {
                    return instance;
                }
                else
                {
                    throw new NullReferenceException();
                }
            }
        }

        public static void CreateWorld()
        {
            Random r = new Random();
            CreateWorld(r.Next());
        }

        public static void CreateWorld(int seed)
        {
            instance = new Planet(seed);

            //TODO: Put PLanet Creation into the config

            //TODO: Create Special Objects from Config

            //TODO: Read new world agentnum from config

            int locationMultiplier = 12;
            for (int i = 0; i < 100; i++)
            {
                int yPosBase = 1 + (i / 3);
                int xPosBase = 1 + ((i - 1) / 3) + (((i - 1) % 3) % 2);
                int xPos = xPosBase * locationMultiplier;
                int yPos = yPosBase * locationMultiplier;
                Agent ag = new Agent(new Vector2((float)xPos, (float)yPos));
                instance.AddObjectToWorld(ag);
            }
        }


        public readonly Random NumberGen;

        private int uniqueInt = 0;
        public int NextUniqueID()
        {
            return ++uniqueInt;
        }

        public readonly List<WorldObject> AllControlledObjects = new List<WorldObject>();
        public readonly Dictionary<string, List<WorldObject>> CollisionLevels = new Dictionary<string, List<WorldObject>>();

        internal void AddObjectToWorld(WorldObject toAdd)
        {
            if(!CollisionLevels.ContainsKey(toAdd.CollisionLevel))
            {
                CollisionLevels.Add(toAdd.CollisionLevel, new List<WorldObject>());
            }
            CollisionLevels[toAdd.CollisionLevel].Add(toAdd);

            AllControlledObjects.Add(toAdd);
        }

        internal void RemoveWorldObject(WorldObject mySelf)
        {
            string collisionLevel = mySelf.CollisionLevel;
            CollisionLevels[collisionLevel].Remove(mySelf);
            AllControlledObjects.Remove(mySelf);
        }
    }
}
