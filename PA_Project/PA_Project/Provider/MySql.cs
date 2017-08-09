using System;
using PA_Project.CDE;

namespace PA_Project.Provider
{
    public class MySql : SubscriberAgent
    {
        public override string GetName() { return "database"; }
        public override void HandleEvent(object sender, EngineEventArgs e)
        {
            if (e.TargetAgentId != Identifier) return;
            switch (e.EventName)
            {
                case "init": Install(sender,e); break;
                case "start": Start(sender,e); break;
                default: throw new Exception("Event not supported.");
            }
        }
        public void Install(object sender, EngineEventArgs e)
        {
            SimulateExecution(GetName() + "_init", e.TargetAgentId);
        }
        public void Start(object sender, EngineEventArgs e)
        {
            SimulateExecution(GetName() + "_start", e.TargetAgentId);
        }
    }
}