using System;
using System.Threading;
using System.Threading.Tasks;
using PA_Project.CDE;

namespace PA_Project.Provider
{
    public delegate void NotificationEventHandler(object sender, NotificationEventArgs e);

    public class NotificationEventArgs : EventArgs
    {
        public EventInfo EventInfo { get; set; }
    }

    public abstract class SubscriberAgent
    {
        public event NotificationEventHandler OnOperationFinished;
        public int Identifier => GetHashCode();
        protected Random Rnd;
        private TaskCompletionSource<bool> _tsc;
        protected bool MustQuit;

        protected SubscriberAgent()
        {
            Rnd = new Random();
            MustQuit = false;
        }

        public abstract string GetName();
        
        public int GetId() { return Identifier; }

        public async void Lifecycle()
        {
            while (!MustQuit)
            {
                _tsc = new TaskCompletionSource<bool>();
                await _tsc.Task;
            }
            Console.WriteLine("A {0} agent was terminated.", GetName());
        }

        public void SetBidirectionalCommunication(CloudDeploymentEngine engine)
        {
            engine.Handler += HandleEvent;
            OnOperationFinished += engine.ReceivedNotification;
        }

        public void UnsetBidirectionalCommunication(CloudDeploymentEngine engine)
        {
            engine.Handler -= HandleEvent;
            OnOperationFinished -= engine.ReceivedNotification;
            Console.WriteLine("A {0} thread was aborted.", GetName());
        }

        public void GentleKill(CloudDeploymentEngine engine)
        {
            UnsetBidirectionalCommunication(engine);
            MustQuit = true;
        }

        public abstract void HandleEvent(object sender, EngineEventArgs e);

        protected void SimulateExecution(string handledEvent, int targetAgentId)
        {
            if (targetAgentId != Identifier) return;
            Console.WriteLine(handledEvent + " was fired!");
            Thread.Sleep(Rnd.Next(0,1000));
            Notify(OnOperationFinished, handledEvent);
            _tsc?.TrySetResult(true);
        }
        
        private void Notify(NotificationEventHandler handler, string evnt)
        {
            if (handler == null) return;
            var args = new NotificationEventArgs {EventInfo = new EventInfo{Id = Identifier, Type = evnt}};
            var delegates = handler.GetInvocationList();
            foreach (var del in delegates)
                ((NotificationEventHandler)del).BeginInvoke(this, args, null, null);
        }
        
    }
}