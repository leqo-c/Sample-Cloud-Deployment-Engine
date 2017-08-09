using System.Collections.Generic;

namespace PA_Project.Constructs {
    public class Specification {
        public Services Services { get; set; }
        public List<Agent> Agents { get; set; }
        public List<Relation> Relations { get; set; }
        public List<string> ProvidedServices { get; set; }
        public Specification() { ProvidedServices = new List<string>(); }
    }
}