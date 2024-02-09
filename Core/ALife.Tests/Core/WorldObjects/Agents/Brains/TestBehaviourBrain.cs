using ALife.Core;
using ALife.Core.Scenarios.TestScenarios;
using ALife.Core.WorldObjects.Agents;
using ALife.Core.WorldObjects.Agents.Brains;

namespace ALife.Tests.Core.WorldObjects.Agents.Brains
{
    [TestClass]
    public class TestBehaviourBrain
    {
        [TestMethod]
        public void TestBehaviourBrainClone()
        {
            Planet.CreateWorld(123, new GoalsTestScenario());

            Agent firstAgent = Planet.World.AllActiveObjects.OfType<Agent>().First();
            BehaviourBrain brain = (BehaviourBrain) firstAgent.MyBrain;
            BehaviourBrain cloneBrain = (BehaviourBrain) brain.Clone(firstAgent);

            Assert.IsTrue(brain.CloneEquals(cloneBrain));
            Assert.IsTrue(cloneBrain.CloneEquals(brain));
        }
    }
}
