using ALife.Core;
using ALife.Core.Scenarios.TestScenarios;
using ALife.Core.WorldObjects.Agents;
using ALife.Core.WorldObjects.Agents.Brains;

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
            NeuralNetworkBrain brain = (NeuralNetworkBrain) firstAgent.MyBrain;

            string exportString = brain.ExportNewBrain();

            NeuralNetworkBrain importBrain = new NeuralNetworkBrain(firstAgent, exportString);

            Assert.IsTrue(brain.CloneEquals(importBrain));
            Assert.IsTrue(importBrain.CloneEquals(brain));
        }

        [TestMethod]
        public void TestBrainCloneWorks()
        {
            Planet.CreateWorld(123, new NeuralNetScenario());

            Agent firstAgent = Planet.World.AllActiveObjects.OfType<Agent>().First();
            NeuralNetworkBrain brain = (NeuralNetworkBrain) firstAgent.MyBrain;
            NeuralNetworkBrain cloneBrain = (NeuralNetworkBrain) brain.Clone(firstAgent);

            Assert.IsTrue(brain.CloneEquals(cloneBrain));
            Assert.IsTrue(cloneBrain.CloneEquals(brain));
        }
    }
}
