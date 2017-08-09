using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PA_Project.Constructs;
using PA_Project.Provider;

namespace PA_Project.CDE
{
    public class CloudDeploymentEngine
    {
        public Specification Spec { get; }
        public List<EventInfo> HandledEvents { get; set; }
        public List<EventInfo> FiredEvents { get; set; }
        public List<Tuple<int,string>> CreatedAgents { get; set; }
        private TaskCompletionSource<bool> _tsc;
        public delegate void MyEventHandler(object sender, EngineEventArgs e);
        public event MyEventHandler Handler = delegate {} ;

        public void CreateAgents(CloudProvider cp)
        {
            foreach (var unit in Spec.Services.Units)
                for (var i = 0; i < unit.Value; i++)
                    CreatedAgents.Add(cp.CreateAgent(unit.Key, this));
        }
    
        public async void CheckConditionsAndRaiseEvents()
        {
            var support = new EventInfo();
            while (true)
            {
                foreach (var agent in Spec.Agents)
                    foreach (var agentEvent in GetAgentEvents(agent))
                        foreach (var agntPair in CreatedAgents.Where(p=>p.Item2.Contains(agent.Name)))
                        {
                            var evts = from x in HandledEvents where x.Id == agntPair.Item1 select x.Type;
                            support.Id = agntPair.Item1;  
                            support.Type = agent.Name + "_" + agentEvent.Name;
                            if (HandledEvents.Contains(support) || FiredEvents.Contains(support)) continue;
                            if (!SubListOf(agentEvent.After, evts)) continue;
                            RaiseEvent(Handler, agntPair.Item1, agentEvent.Name);
                            FiredEvents.Add(support.ShallowCopy());
                        }
                foreach (var r in Spec.Relations)
                {
                    if (!HandledEvents.Select(h=>h.Type).Contains(r.Event)) continue;
                    foreach (var id in from x in CreatedAgents where x.Item2==r.ServiceRequesting select x.Item1)
                    {
                        support.Id = id;
                        support.Type = r.Condition;
                        if(!HandledEvents.Contains(support))
                            HandledEvents.Add(support.ShallowCopy());
                    }
                } 
                _tsc = new TaskCompletionSource<bool>();
                await _tsc.Task;
            }
        }

        public IEnumerable<MyEvent> GetAgentEvents(Agent a)
        {
            foreach (var evt in a.Events) yield return evt;
        }
        
        public CloudDeploymentEngine(Specification spec) 
        {
            Spec = spec;
            HandledEvents = new List<EventInfo>();
            FiredEvents = new List<EventInfo>();
            CreatedAgents = new List<Tuple<int, string>>();
        }
        
        public void RaiseEvent(MyEventHandler handler, int targetId, string eventName)
        {    
            if (handler == null) return;
            var args = new EngineEventArgs(targetId, eventName);
            var delegates = handler.GetInvocationList();
            foreach (var del in delegates)
                ((MyEventHandler)del).BeginInvoke(this, args, null, null);
        }
        
        public void ReceivedNotification(object sender, NotificationEventArgs e)
        {
            if(!HandledEvents.Contains(e.EventInfo))
                HandledEvents.Add(e.EventInfo);
            FiredEvents.Remove(e.EventInfo);
            _tsc?.TrySetResult(true);
        }

        public static bool SubListOf(IEnumerable<string> l1, IEnumerable<string> l2)
        {
            return !l1.Except(l2).Any();
        }
    }
}