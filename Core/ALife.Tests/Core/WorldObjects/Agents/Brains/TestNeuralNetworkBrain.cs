﻿using ALife.Core;
using ALife.Core.Scenarios.TestScenarios;
using ALife.Core.WorldObjects.Agents;
using System.Linq;

namespace ALife.Tests.Core.WorldObjects.Agents.Brains
{
    [TestClass]
    public class TestNeuralNetworkBrain
    {
        [TestMethod]
        public void TestExport()
        {
            Planet.CreateWorld(123, new NeuralNetScenario());

            Agent firstAgent = Planet.World.AllActiveObjects.OfType<Agent>().First();
            String export = firstAgent.MyBrain.ExportNewBrain();

            Assert.AreEqual(export, "blahBlah");
        }
    }
}