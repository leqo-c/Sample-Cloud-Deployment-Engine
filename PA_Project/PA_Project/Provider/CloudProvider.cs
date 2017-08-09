using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using PA_Project.CDE;
using static PA_Project.CDE.CloudDeploymentEngine;

namespace PA_Project.Provider {
    public class CloudProvider {
        protected List<KeyValuePair<Thread, SubscriberAgent>> RunningAgents;

        public CloudProvider() {
            RunningAgents = new List<KeyValuePair<Thread, SubscriberAgent>>();
        }

        public Tuple<int, string> CreateAgent(string agentName, CloudDeploymentEngine engine) {
            var agentType = engine.Spec.Agents.First(a => a.Name == agentName);
            if (!SubListOf(agentType.Requires, engine.Spec.ProvidedServices))
                throw new Exception("Agent requires a condition that was not provided.");
            SubscriberAgent agent;
            if (agentName == "moodle" && agentType.ImplementationClass == "Moodle")
                agent = new Moodle();
            else if (agentName == "database" && agentType.ImplementationClass == "Mysql")
                agent = new MySql();
            else throw new Exception("Specified implementation class is unavailable.");
            agent.SetBidirectionalCommunication(engine);
            var t = new Thread(() => agent.Lifecycle()) {Name = agentName};
            RunningAgents.Add(new KeyValuePair<Thread, SubscriberAgent>(t, agent));
            t.Start();
            return new Tuple<int, string>(agent.GetId(), agentName);
        }

        public void KillAgent(string agentName, CloudDeploymentEngine engine) {
            try {
                var thread = RunningAgents.First(th => th.Key.Name == agentName);
                RunningAgents.Remove(thread);
                thread.Value.GentleKill(engine);
                engine.HandledEvents.RemoveAll(ev => ev.Id == thread.Value.Identifier);
                engine.CreatedAgents.RemoveAll(tuple => tuple.Item1 == thread.Value.Identifier);
            }
            catch (InvalidOperationException) {
                Console.WriteLine("No such agent.");
            }
        }
    }
}