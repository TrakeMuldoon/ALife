using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlifeUniversal.ALife
{
    public sealed class Environment
    {
        private static Environment instance;

        static Environment()
        {

        }

        private Environment(int seed)
        {
            NumberGen = new Random(seed);
        }

        public static Environment World
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
            instance = new Environment(seed);
        }


        public readonly Random NumberGen;

        public readonly List<WorldObject> AllControlledObjects = new List<WorldObject>();

        public readonly Dictionary<string, List<WorldObject>> CollisionLevels = new Dictionary<string, List<WorldObject>>();

        internal void RemoveWorldObject(WorldObject mySelf)
        {
            string collisionLevel = mySelf.CollisionLevel;
            CollisionLevels[collisionLevel].Remove(mySelf);
        }
    }
}
