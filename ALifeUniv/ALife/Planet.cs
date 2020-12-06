using ALifeUni.ALife.AgentPieces;
using ALifeUni.ALife.UtilityClasses;
using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife
{
    public sealed class Planet
    {
        #region Static/WorldBuilder stuff
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
                if(instance != null)
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
            CreateWorld(r.Next(), 1000, 880);
        }

        public static void CreateWorld(int height, int width)
        {
            Random r = new Random();

            CreateWorld(r.Next(), height, width);
        }
        #endregion

        public static void CreateWorld(int seed, int height, int width)
        {
            instance = new Planet(seed, height, width);

            //Initialize collision grid
            instance._collisionLevels.Add(ReferenceValues.CollisionLevelPhysical, new CollisionGrid<WorldObject>(height, width));
            instance.ZoneMap = new CollisionGrid<Zone>(height, width);

            //TODO: Put Planet Creation into the config
            //TODO: Create Special Objects from Config
            //TODO: Read new world agentnum from config

            //TODO: Add Zones Propertly to their visual layer.
            Zone red = new Zone("Red", "Random", Colors.Red, new Point(0, 0), 50, height);
            Zone blue = new Zone("Blue", "Random", Colors.Blue, new Point(width - 50, 0), 50, height);
            Zone green = new Zone("Green", "Random", Colors.Green, new Point(0, 0), width, 50);
            Zone orange = new Zone("Orange", "Random", Colors.Orange, new Point(0, height - 50), width, 50);
            instance.AddZone(red);
            instance.AddZone(blue);
            instance.AddZone(green);
            instance.AddZone(orange);

            int numAgents = 40;
            int agentRadius = 5;
            for(int i = 0; i < numAgents; i++)
            {
                Point redCP = red.Distributor.NextAgentCentre(agentRadius * 2, agentRadius * 2);
                Agent rag = new Agent(redCP, red, blue, Colors.Blue, 0);
                instance.AddObjectToWorld(rag);
                
                Point blueCP = blue.Distributor.NextAgentCentre(agentRadius * 2, agentRadius * 2);
                Agent bag = new Agent(blueCP, blue, red, Colors.Red, 180);
                instance.AddObjectToWorld(bag);
                
                Point greenCP = green.Distributor.NextAgentCentre(agentRadius * 2, agentRadius * 2);
                Agent gag = new Agent(greenCP, green, orange, Colors.Orange, 90);
                instance.AddObjectToWorld(gag);
                
                Point orangeCP = orange.Distributor.NextAgentCentre(agentRadius * 2, agentRadius * 2);
                Agent oag = new Agent(orangeCP, orange, green, Colors.Green, 270);
                instance.AddObjectToWorld(oag);
            }
        }

        #region Instance Stuff

        public readonly Dictionary<String, Zone> Zones = new Dictionary<string, Zone>();
        public readonly List<WorldObject> AllControlledObjects = new List<WorldObject>();
        public readonly List<WorldObject> AllActiveObjects = new List<WorldObject>();
        public readonly List<WorldObject> AllNewObjects = new List<WorldObject>();
        public readonly int Seed;

        internal ICollisionMap<Zone> ZoneMap;

        private Dictionary<string, ICollisionMap<WorldObject>> _collisionLevels = new Dictionary<string, ICollisionMap<WorldObject>>();
        internal IReadOnlyDictionary<string, ICollisionMap<WorldObject>> CollisionLevels
        {
            get { return _collisionLevels; }
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

        internal readonly Random NumberGen;
        
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

        public void ExecuteManyTurns(int numTurns)
        {
            for(int i = 0; i < numTurns; i++)
            {
                ExecuteOneTurn();
            }
        }

        public void ExecuteOneTurn()
        {
            turns++;
            int order = 0;
            foreach(WorldObject wo in AllActiveObjects)
            {
                wo.ExecutionOrder = order++;
                wo.ExecuteTurn();
            }
            while(AllNewObjects.Count > 0)
            {
                AllActiveObjects.Add(AllNewObjects[0]);
                AllNewObjects.RemoveAt(0);
            }

            GlobalEndOfTurnActions();
        }

        internal void GlobalEndOfTurnActions()
        {
            //TODO: Pull from Config
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
                _collisionLevels.Add(newLevel, new CollisionGrid<WorldObject>(WorldHeight, WorldWidth));
            }
            CollisionLevels[newLevel].Insert(mySelf);
        }

        internal void AddObjectToWorld(WorldObject toAdd)
        {
            if(!_collisionLevels.ContainsKey(toAdd.CollisionLevel))
            {
                _collisionLevels.Add(toAdd.CollisionLevel, new CollisionGrid<WorldObject>(WorldHeight, WorldWidth));
            }
            _collisionLevels[toAdd.CollisionLevel].Insert(toAdd);

            AllControlledObjects.Add(toAdd);
            AllNewObjects.Add(toAdd);
        }

        internal void AddZone(Zone toAdd)
        {
            Zones.Add(toAdd.Name, toAdd);
            ZoneMap.Insert(toAdd);
        }
        #endregion
    }
}
