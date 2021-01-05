using ALifeUni.ALife.AgentPieces;
using ALifeUni.ALife.Scenarios;
using System;
using System.Collections.Generic;

namespace ALifeUni.ALife
{
    public sealed class Planet
    {
        #region Static/WorldBuilder stuff
        private static Planet instance;

        static Planet()
        {

        }

        private Planet(int seed, int height, int width, IScenario theScenario)
        {
            worldWidth = width;
            worldHeight = height;
            Seed = seed;
            NumberGen = new Random(seed);
            Scenario = theScenario;
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
            instance = new Planet(seed, height, width, new MazeScenario());

            //Initialize collision grid
            instance._collisionLevels.Add(ReferenceValues.CollisionLevelPhysical, new CollisionGrid<WorldObject>(height, width));
            instance.ZoneMap = new CollisionGrid<Zone>(height, width);

            instance.Scenario.PlanetSetup();
        }

        #region Instance Stuff

        public readonly Dictionary<String, Zone> Zones = new Dictionary<string, Zone>();


        //All objects which are active (This includes "dead" objects, which have not yet finished their activity (i.e. corpses are "active")
        public readonly List<WorldObject> AllActiveObjects = new List<WorldObject>();
        //All objects which were active last turn
        public readonly List<WorldObject> StableActiveObjects = new List<WorldObject>();
        //All new objects. Should be added to Stable Active Objects, should already be in AllActiveObjects
        public readonly List<WorldObject> NewActiveObjects = new List<WorldObject>();
        
        //All objects which have falled out of all active object scope
        public readonly List<WorldObject> InactiveObjects = new List<WorldObject>();
        //Objects which at the end of the turn should be removed from "AllActive", "StableActive" and "NewActive"
        public readonly List<WorldObject> ToRemoveObjects = new List<WorldObject>();
        
        
        public readonly int Seed;
        public readonly IScenario Scenario;

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
                AllActiveObjects.Remove(ToRemoveObjects[0]);
                StableActiveObjects.Remove(ToRemoveObjects[0]);
                NewActiveObjects.Remove(ToRemoveObjects[0]);

                InactiveObjects.Add(ToRemoveObjects[0]);
                
                ToRemoveObjects.RemoveAt(0);
            }

            GlobalEndOfTurnActions();
        }

        internal void GlobalEndOfTurnActions()
        {
            Scenario.GlobalEndOfTurnActions();
        }

        public List<Agent> BestXAgents = new List<Agent>();
        private int bestAgentCounter = 0;
        public void ReproduceBest()
        {
            if(BestXAgents.Count == 0)
            {
                return;
            }
            if(bestAgentCounter >= BestXAgents.Count)
            {
                bestAgentCounter = 0;
            }
            BestXAgents[bestAgentCounter].Reproduce();
            bestAgentCounter++;
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
