using ALifeUni.ALife.UtilityClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace ALifeUni.ALife
{
    public sealed class Planet
    {
        private static Planet instance;



        static Planet()
        {

        }

        private Planet(int seed, int height, int width)
        {
            worldWidth = width;
            worldHeight = height;
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
            CreateWorld(r.Next(), 1000, 1000);
        }
        public  static void CreateWorld(int height, int width)
        {
            Random r = new Random();

            CreateWorld(r.Next(), height, width);
        }


        public static void CreateWorld(int seed, int height, int width)
        {
            instance = new Planet(seed, height, width);

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
                Agent ag = new Agent(new Point(xPos, yPos));
                instance.AddObjectToWorld(ag);
            }
        }


        public readonly List<WorldObject> AllControlledObjects = new List<WorldObject>();

        public IReadOnlyDictionary<string, string> my = new Dictionary<string, string>();

        private Dictionary<string, ICollisionMap> _collisionLevels = new Dictionary<string, ICollisionMap>();
        public IReadOnlyDictionary<string, ICollisionMap> CollisionLevels
        {
            get { return (Dictionary<string, ICollisionMap>) _collisionLevels; }
        }
        private int worldWidth;
        public int WorldWidth
        {
            get
            {
                return worldWidth;
            }
        }

        private int worldHeight;
        public int WorldHeight
        {
            get
            {
                return worldHeight;
            }
        }

        public readonly Random NumberGen;

        private int uniqueInt = 0;
        public int NextUniqueID()
        {
            return ++uniqueInt;
        }



        internal void ExecuteManyTurns(int numTurns)
        {
            for (int i = 0; i < numTurns; i++)
            {
                ExecuteOneTurn();
            }
        }

        internal void ExecuteOneTurn()
        {
            foreach (WorldObject wo in AllControlledObjects)
            {
                wo.ExecuteTurn();
            }
        }



        internal void RemoveWorldObject(WorldObject mySelf)
        {
            string collisionLevel = mySelf.CollisionLevel;
            CollisionLevels[collisionLevel].RemoveObject(mySelf);
            AllControlledObjects.Remove(mySelf);
        }

        internal void AddObjectToWorld(WorldObject toAdd)
        {
            if (!_collisionLevels.ContainsKey(toAdd.CollisionLevel))
            {
                _collisionLevels.Add(toAdd.CollisionLevel, new CollisionGrid(WorldHeight, WorldWidth));
            }
            _collisionLevels[toAdd.CollisionLevel].Insert(toAdd);

            AllControlledObjects.Add(toAdd);
        }
    }
}
