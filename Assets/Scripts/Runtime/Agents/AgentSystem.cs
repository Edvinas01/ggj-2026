using System.Collections.Generic;
using CHARK.GameManagement.Systems;

namespace RIEVES.GGJ2026.Runtime.Agents
{
    internal sealed class AgentSystem : MonoSystem, IUpdateListener, IFixedUpdateListener
    {
        private readonly List<IAgent> agents = new();

        public void AddAgent(IAgent agent)
        {
            agents.Add(agent);
        }

        public void RemoveAgent(IAgent agent)
        {
            agents.Remove(agent);
        }

        public void OnUpdated(float deltaTime)
        {
        }

        public void OnFixedUpdated(float deltaTime)
        {
        }
    }
}
