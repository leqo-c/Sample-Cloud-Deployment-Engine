using System;
using System.Data;
using System.Linq;
using PA_Project.Constructs;
using PA_Project.Provider;

namespace PA_Project.CDE {
    public class ExtendedCloudDeploymentEngine : CloudDeploymentEngine {
        public ExtendedCloudDeploymentEngine(Specification spec) : base(spec) { }

        public void UpdateSpecification(Specification newSpec, CloudProvider cp) {
            foreach (var agent in newSpec.Agents) {
                int oldUnitsNumber;
                var ok = Spec.Services.Units.TryGetValue(agent.Name, out oldUnitsNumber);
                if (!ok) throw new Exception("Specification is not valid.");
                var newUnitsNumber = newSpec.Services.Units[agent.Name];
                var difference = newUnitsNumber - oldUnitsNumber;
                if (difference > 0)
                    for (var i = 0; i < difference; i++)
                        CreatedAgents.Add(cp.CreateAgent(agent.Name, this));
                else if (difference < 0)
                    for (var i = 0; i < Math.Abs(difference); i++)
                        cp.KillAgent(agent.Name, this);
            }
            MergeSpec(newSpec);
        }

        private void MergeSpec(Specification newSpec) {
            foreach (var newSpecAgent in newSpec.Agents) {
                var agent = (from a in Spec.Agents where a.Name == newSpecAgent.Name select a).First();
                var eventsOfAgent = (from e in agent.Events select e.Name).ToList();
                foreach (var evnt in newSpecAgent.Events)
                    if (!eventsOfAgent.Contains(evnt.Name))
                        agent.Events.Add(evnt);
            }
            foreach (var newSpecRelation in newSpec.Relations) {
                if (!Spec.Relations.Contains(newSpecRelation))
                    Spec.Relations.Add(newSpecRelation);
            }
        }
    }
}