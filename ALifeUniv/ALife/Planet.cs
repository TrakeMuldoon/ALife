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
            //1622137501
            instance = new Planet(seed, height, width);

            //Initialize collision grid
            instance._collisionLevels.Add(ReferenceValues.CollisionLevelPhysical, new CollisionGrid<WorldObject>(height, width));
            instance.ZoneMap = new CollisionGrid<Zone>(height, width);

            //TODO: Put Planet Creation into the config
            //TODO: Create Special Objects from Config
            //TODO: Read new world agentnum from config

            #region SQUARE TEST
            //Zone nullZone = new Zone("Null", "random", Colors.Black, new Point(0, 0), 1, 1);
            //instance.AddZone(nullZone);

            //Point ap = new Point(20, 20);
            //Agent a = new Agent(ap, nullZone, null, Colors.Red, 0);
            //instance.AddObjectToWorld(a);

            //Point bp = new Point(40, 20);
            //Agent b = new Agent(bp, nullZone, null, Colors.Red, 0);
            //instance.AddObjectToWorld(b);
            #endregion

            Zone red = new Zone("Red(Blue)", "Random", Colors.Red, new Point(0, 0), 50, height);
            Zone blue = new Zone("Blue(Red)", "Random", Colors.Blue, new Point(width - 50, 0), 50, height);
            red.OppositeZone = blue;
            red.OrientationDegrees = 0;
            blue.OppositeZone = red;
            blue.OrientationDegrees = 180;

            Zone green = new Zone("Green(Orange)", "Random", Colors.Green, new Point(0, 0), width, 100);
            Zone orange = new Zone("Orange(Green)", "Random", Colors.Orange, new Point(0, height - 40), width, 40);
            green.OppositeZone = orange;
            green.OrientationDegrees = 90;
            orange.OppositeZone = green;
            orange.OrientationDegrees = 270;

            instance.AddZone(red);
            instance.AddZone(blue);
            instance.AddZone(green);
            instance.AddZone(orange);

            int numAgents = 50;
            for(int i = 0; i < numAgents; i++)
            {
                Agent rag = AgentFactory.CreateAgent("Agent", red, blue, Colors.Blue, 0);
                Agent bag = AgentFactory.CreateAgent("Agent", blue, red, Colors.Red, 180);
                Agent gag = AgentFactory.CreateAgent("Agent", green, orange, Colors.Orange, 90);
                Agent oag = AgentFactory.CreateAgent("Agent", orange, green, Colors.Green, 270);
            }

            Point rockCP = new Point((width / 2) - (width / 15), height / 2);
            Circle cir = new Circle(rockCP, 30);
            FallingRock fr = new FallingRock(rockCP, cir, Colors.Black);
            instance.AddObjectToWorld(fr);

            Point rockRCP = new Point((width / 2), (height / 2) - (height / 15));
            Rectangle rec = new Rectangle(rockRCP, 10, 30, Colors.BurlyWood);
            FallingRock frR = new FallingRock(rockRCP, rec, Colors.BurlyWood);
            instance.AddObjectToWorld(frR);
        }

        #region Instance Stuff

        public readonly Dictionary<String, Zone> Zones = new Dictionary<string, Zone>();
        public readonly List<WorldObject> AllActiveObjects = new List<WorldObject>();
        public readonly List<WorldObject> StableActiveObjects = new List<WorldObject>();
        public readonly List<WorldObject> NewActiveObjects = new List<WorldObject>();
        public readonly List<WorldObject> InactiveObjects = new List<WorldObject>();
        public readonly List<WorldObject> ToRemoveObjects = new List<WorldObject>();
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
            foreach(WorldObject wo in StableActiveObjects)
            {
                wo.ExecutionOrder = order++;
                wo.ExecuteTurn();
            }
            while(NewActiveObjects.Count > 0)
            {
                StableActiveObjects.Add(NewActiveObjects[0]);
                NewActiveObjects.RemoveAt(0);
            }
            while(ToRemoveObjects.Count > 0)
            {
                StableActiveObjects.Remove(ToRemoveObjects[0]);
                InactiveObjects.Add(ToRemoveObjects[0]);
                ToRemoveObjects.RemoveAt(0);
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
            NewActiveObjects.Remove(mySelf);
            ToRemoveObjects.Add(mySelf);
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

            AllActiveObjects.Add(toAdd);
            NewActiveObjects.Add(toAdd);
        }

        internal void AddZone(Zone toAdd)
        {
            Zones.Add(toAdd.Name, toAdd);
            ZoneMap.Insert(toAdd);
        }
        #endregion
    }
}
