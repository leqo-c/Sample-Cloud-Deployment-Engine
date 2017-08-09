using System.Collections.Generic;

namespace PA_Project.Constructs {
    public class Agent {
        public string ImplementationClass { get; set; }
        public string Name { get; set; }
        public List<MyEvent> Events { get; set; }
        public List<string> Requires { get; set; }
        public List<string> Provides { get; set; }
        public int Identity => GetHashCode();

        public Agent(string implClass, string name) {
            ImplementationClass = implClass;
            Name = name;
            Events = new List<MyEvent>();
            Requires = new List<string>();
            Provides = new List<string>();
        }
    }

    public class MyEvent {
        public string Name { get; set; }
        public List<string> After { get; set; }
        public string Handler { get; set; }
        public MyEvent() { After = new List<string>(); }
    }
}