using System;

namespace PA_Project.CDE {
    public class EngineEventArgs : EventArgs {
        public int TargetAgentId { get; }
        public string EventName { get; }
        public EngineEventArgs(int target, string eventName) {
            TargetAgentId = target;
            EventName = eventName;
        }
    }
}