using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlifeUniversal.ALife
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

            //TODO: Put PLanet Creation into the config

            //TODO: Create Special Objects from Config

            //TODO: Read new world agentnum from config
            for(int i = 0; i < 10; i++)
            {


            }
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
        }


        public readonly Random NumberGen;

        private int uniqueInt = 0;
        public int NextUniqueID()
        {
            return ++uniqueInt;
        }

        public readonly List<WorldObject> AllControlledObjects = new List<WorldObject>();
        public readonly Dictionary<string, List<WorldObject>> CollisionLevels = new Dictionary<string, List<WorldObject>>();

        internal void AddObjectToWorld(WorldObject toAdd, String collisionLevel)
        {
            if(!CollisionLevels.ContainsKey(collisionLevel))
            {
                CollisionLevels.Add(collisionLevel, new List<WorldObject>());
            }
            CollisionLevels[collisionLevel].Add(toAdd);

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
