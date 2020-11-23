using ALifeUni.ALife.AgentPieces;
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
            Seed = seed;
            NumberGen = new Random(seed);
            AgentIDGenerator.Reset();
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
            //TODO Hardcoded world size
            CreateWorld(r.Next(), 1000, 800);
        }

        internal string NextUniqueAgentID()
        {
            throw new NotImplementedException();
        }

        public  static void CreateWorld(int height, int width)
        {
            Random r = new Random();

            CreateWorld(r.Next(), height, width);
        }


        public static void CreateWorld(int seed, int height, int width)
        {
            instance = new Planet(seed, height, width);

            //TODO: Put Planet Creation into the config

            //TODO: Create Special Objects from Config

            //TODO: Read new world agentnum from config

            //int locationMultiplier = 20;
            //for (int i = 0; i < 100; i++)
            //{
            //    int yPosBase = 2 + (i / 3);
            //    int xPosBase = 2 + ((i - 1) / 3) + (((i - 1) % 3) % 2);
            //    int xPos = xPosBase * locationMultiplier;
            //    int yPos = yPosBase * locationMultiplier;
            //    Agent ag = new Agent(new Point(xPos, yPos));
            //    instance.AddObjectToWorld(ag);
            //}

            Agent ag;
            ag = new Agent(new Point(200, 200));
            instance.AddObjectToWorld(ag);
            ag = new Agent(new Point(250, 200));
            instance.AddObjectToWorld(ag);
            ag = new Agent(new Point(200, 250));
            instance.AddObjectToWorld(ag);
            ag = new Agent(new Point(250, 250));
            instance.AddObjectToWorld(ag);
        }

        public readonly List<WorldObject> AllControlledObjects = new List<WorldObject>();
        public readonly List<WorldObject> AllActiveObjects = new List<WorldObject>();
        public readonly List<WorldObject> AllNewObjects = new List<WorldObject>();
        public readonly int Seed;

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
        private int turns = 0;
        public int Turns
        {
            get
            {
                return turns;
            }
        }

        private object UniqueLock = new object();
        private int uniqueInt = 0;
        public int NextUniqueID()
        {
            lock(UniqueLock)
            {
                return ++uniqueInt;
            }
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
            turns++;
            foreach (WorldObject wo in AllActiveObjects)
            {
                wo.ExecuteTurn();
            }
            while(AllNewObjects.Count > 0)
            {
                AllActiveObjects.Add(AllNewObjects[0]);
                AllNewObjects.RemoveAt(0);
            }
        }

        internal void RemoveWorldObject(WorldObject mySelf)
        {
            string collisionLevel = mySelf.CollisionLevel;
            CollisionLevels[collisionLevel].RemoveObject(mySelf);
            AllControlledObjects.Remove(mySelf);
            AllNewObjects.Remove(mySelf);
            AllActiveObjects.Remove(mySelf);
        }

        internal void ChangeCollisionLayerForObject(WorldObject mySelf, string newLevel)
        {
            string currCollisionLevel = mySelf.CollisionLevel;
            CollisionLevels[currCollisionLevel].RemoveObject(mySelf);
            if(!_collisionLevels.ContainsKey(newLevel))
            {
                _collisionLevels.Add(newLevel, new CollisionGrid(WorldHeight, WorldWidth));
            }
            CollisionLevels[newLevel].Insert(mySelf);
        }

        internal void AddObjectToWorld(WorldObject toAdd)
        {
            if (!_collisionLevels.ContainsKey(toAdd.CollisionLevel))
            {
                _collisionLevels.Add(toAdd.CollisionLevel, new CollisionGrid(WorldHeight, WorldWidth));
            }
            _collisionLevels[toAdd.CollisionLevel].Insert(toAdd);

            AllControlledObjects.Add(toAdd);
            AllNewObjects.Add(toAdd);
        }
    }
}
