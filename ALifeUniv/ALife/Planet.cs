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
            CreateWorld(r.Next(), 1000, 800);
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
            Zone start = new Zone("Start", "Random", Colors.Red, new Point(0, 50), 100, height - 100);
            Zone end = new Zone("End", "Random", Colors.Blue, new Point(width - 50, 50), 50, height - 100);
            Zone rocks = new Zone("Rocks", "Random", Colors.DarkGray, new Point(100, 0), width - 150, 50);
            Zone rockBottom = new Zone("Rock Bottom", "Random", Colors.Orchid, new Point(100, height - 50), width - 150, 50);
            instance.AddZone(start);
            instance.AddZone(end);
            instance.AddZone(rocks);
            instance.AddZone(rockBottom);

            //int numAgents = 100;
            //int agentRadius = 5;
            //for(int i = 0; i < numAgents; i++)
            //{
            //    Point nextCP = start.Distributor.NextAgentCentre(agentRadius * 2, agentRadius * 2);
            //    Agent ag = new Agent(nextCP, start);
            //    instance.AddObjectToWorld(ag);
            //}

            Point cp = new Point(100, 100);
            Circle cir = new Circle(cp, 12);
            Spinner soc = new Spinner(cp, cir, "Genie", "AAA", ReferenceValues.CollisionLevelPhysical, Colors.Blue);

            Point rp = new Point(500, 500);
            Rectangle rectangle = new Rectangle(rp, 80, 60, Colors.OldLace);
            Spinner sor = new Spinner(rp, rectangle, "Square", "ABBA", ReferenceValues.CollisionLevelPhysical, Colors.Red);

            instance.AddObjectToWorld(soc);
            instance.AddObjectToWorld(sor);
        }

        #region Instance Stuff

        public readonly Dictionary<String, Zone> Zones = new Dictionary<string, Zone>();
        public readonly List<WorldObject> AllControlledObjects = new List<WorldObject>();
        public readonly List<WorldObject> AllActiveObjects = new List<WorldObject>();
        public readonly List<WorldObject> AllNewObjects = new List<WorldObject>();
        public readonly int Seed;

        public ICollisionMap<Zone> ZoneMap;

        private Dictionary<string, ICollisionMap<WorldObject>> _collisionLevels = new Dictionary<string, ICollisionMap<WorldObject>>();
        public IReadOnlyDictionary<string, ICollisionMap<WorldObject>> CollisionLevels
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
            for(int i = 0; i < numTurns; i++)
            {
                ExecuteOneTurn();
            }
        }

        internal void ExecuteOneTurn()
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
