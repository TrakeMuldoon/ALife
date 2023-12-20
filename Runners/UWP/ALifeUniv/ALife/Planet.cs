using ALifeUni.ALife.Collision;
using ALifeUni.ALife.Scenarios;
using ALifeUni.ALife.Utility;
using ALifeUni.ALife.WorldObjects.Agents;
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
            if(theScenario.FixedWidthHeight
                && (theScenario.WorldHeight != height
                    || theScenario.WorldWidth != width))
            {
                throw new ArgumentException("Do not set width and height on fixed dimension scenarios");
            }

            worldWidth = width;
            worldHeight = height;
            Seed = seed;
            NumberGen = new FastRandom(seed);
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


        public static void CreateWorld(IScenario scenario)
        {
            Random r = new Random();
            CreateWorld(r.Next(), scenario, scenario.WorldHeight, scenario.WorldWidth);
        }

        public static void CreateWorld(int seed, IScenario scenario)
        {
            CreateWorld(seed, scenario, scenario.WorldHeight, scenario.WorldWidth);
        }

        public static void CreateWorld(IScenario scenario, int height, int width)
        {
            Random r = new Random();
            CreateWorld(r.Next(), scenario, height, width);
        }

        public static void CreateWorld(int seed, IScenario scenario, int height, int width)
        {
            instance = new Planet(seed, height, width, scenario);

            //Initialize collision grid
            string colLevel = ReferenceValues.CollisionLevelPhysical;
            instance._collisionLevels.Add(colLevel, new CollisionGrid<WorldObject>(height, width, colLevel));
            instance.ZoneMap = new CollisionGrid<Zone>(height, width, "Zone");

            instance.Scenario.PlanetSetup();
        }
        #endregion

        #region Instance Stuff

        public readonly Dictionary<String, Zone> Zones = new Dictionary<string, Zone>();
        public List<string> MessagePump = new List<string>();

        //All objects which are active
        //Active in this context means they will still execute a turn. For example, if dead agents become "corpses" which can be eaten, or smelled, they remain "active"
        //because the object still takes a turn. 
        public readonly List<WorldObject> AllActiveObjects = new List<WorldObject>();

        //All objects which were active last turn
        public readonly List<WorldObject> StableActiveObjects = new List<WorldObject>();

        //All new objects. Should be added to Stable Active Objects, should already be in AllActiveObjects
        public readonly List<WorldObject> NewActiveObjects = new List<WorldObject>();

        //All objects which have fallen out of all active object scope.
        //They do not take turns.
        //We might want to eventually clear this list. Probably via config setting.
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

        internal readonly FastRandom NumberGen;

        private int turns = 0;
        public int Turns
        {
            get
            {
                return turns;
            }
        }

        //TODO: This is a singlethreaded application. Why am I locking here
        //DNB: This is for a future in which the application is parallelized.
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
            ++turns;
            int order = 0;
            //Iterate through all the active objects, and execute their turn.
            //Note, some of them may have "died" in the meantime. They still "execute" a turn. 
            foreach(WorldObject wo in StableActiveObjects)
            {
                wo.ExecutionOrder = order++;
                wo.ExecuteTurn();
            }

            //Add all the new objects into the Stable list
            if(NewActiveObjects.Count > 0)
            {
                StableActiveObjects.AddRange(NewActiveObjects);
                NewActiveObjects.Clear();
            }
            // Remove objects as necessary
            while(ToRemoveObjects.Count > 0)
            {
                AllActiveObjects.Remove(ToRemoveObjects[0]);
                StableActiveObjects.Remove(ToRemoveObjects[0]);

                //It needs to be added to the InactiveObjects list, for statistics reasons. 
                InactiveObjects.Add(ToRemoveObjects[0]);

                ToRemoveObjects.RemoveAt(0);
            }

            GlobalEndOfTurnActions();
        }

        internal void GlobalEndOfTurnActions()
        {
            Scenario.GlobalEndOfTurnActions();
        }

        //TODO: Move this out to a Scenario helper. The Planet shouldn't have a concept of "BEST"
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
            //TODO: If I initialize the collision levels at the scenario level, then I don't need to constantly check if the layer exists.
            //Further Thoughts: This is only called on the death of a world-object right now. So not very frequently. I think safety outweighs any miniscule performance gain.
            if(!_collisionLevels.ContainsKey(newLevel))
            {
                _collisionLevels.Add(newLevel, new CollisionGrid<WorldObject>(WorldHeight, WorldWidth, newLevel));
            }
            CollisionLevels[newLevel].Insert(mySelf);
        }

        internal void AddObjectToWorld(WorldObject toAdd)
        {
            if(!_collisionLevels.ContainsKey(toAdd.CollisionLevel))
            {
                _collisionLevels.Add(toAdd.CollisionLevel, new CollisionGrid<WorldObject>(WorldHeight, WorldWidth, toAdd.CollisionLevel));
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
