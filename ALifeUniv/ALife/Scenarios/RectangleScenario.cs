﻿using ALifeUni.ALife.AgentPieces;
using ALifeUni.ALife.AgentPieces.Brains;
using ALifeUni.ALife.AgentPieces.Brains.TesterBrain;
using ALifeUni.ALife.Brains.BehaviourBrains;
using ALifeUni.ALife.Utility;
using ALifeUni.ALife.UtilityClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife.Scenarios
{
    class RectangleScenario : BaseScenario
    {
        public override Agent CreateAgent(string genusName, Zone parentZone, Zone targetZone, Color color, double startOrientation)
        {
            Agent agent = new Agent(genusName
                                    , AgentIDGenerator.GetNextAgentId()
                                    , ReferenceValues.CollisionLevelPhysical);
            agent.Zone = parentZone;
            agent.TargetZone = targetZone;

            //TODO: FIX SO THAT THE SHAPE IS A PARAMETER
            Point centrePoint = parentZone.Distributor.NextAgentCentre(10, 10); //TODO: HARDCODED AGENT RADIUS

            IShape myShape = new Circle(centrePoint, 5);                        //TODO: HARDCODED AGENT RADIUS
            agent.StartOrientation = startOrientation;
            myShape.Orientation.Degrees = startOrientation;
            myShape.Color = color;
            agent.SetShape(myShape);

            List<SenseCluster> agentSenses = new List<SenseCluster>()
            {
                new SquareSenseCluster(agent, "Square")
            };

            List<PropertyInput> agentProperties = new List<PropertyInput>();
            List<StatisticInput> agentStatistics = new List<StatisticInput>();

            List<ActionCluster> agentActions = new List<ActionCluster>()
            {
                new MoveCluster(agent),
                new RotateCluster(agent)
            };

            agent.AttachAttributes(agentSenses, agentProperties, agentStatistics, agentActions);

            //IBrain newBrain = new TesterBrain(agent);
            IBrain newBrain = new BehaviourBrain(agent, "IF ALWAYS THEN Rotate.TurnRight AT [0.1]");

            agent.CompleteInitialization(null, 1, newBrain);

            return agent;
        }

        public override void EndOfTurnTriggers(Agent me)
        {

        }

        public override void AgentUpkeep(Agent me)
        {

        }

        public override void PlanetSetup()
        {
            Zone nullZone = new Zone("Null", "random", Colors.Black, new Point(0, 0), 1000, 1000);
            Planet.World.AddZone(nullZone);

            Point ap = new Point(30, 30);
            Agent a = AgentFactory.CreateAgent("Agent", nullZone, nullZone, Colors.Red, 0);
            a.Shape.CentrePoint = ap;

            Point bp = new Point(60, 60);
            Agent b = AgentFactory.CreateAgent("Agent", nullZone, nullZone, Colors.Red, 0);
            b.Shape.CentrePoint = bp;

            ICollisionMap<WorldObject> collider = Planet.World.CollisionLevels[a.CollisionLevel];
            collider.MoveObject(a);
            collider.MoveObject(b);
        }
    }
}
