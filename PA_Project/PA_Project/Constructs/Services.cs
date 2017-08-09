using System.Collections.Generic;

namespace PA_Project.Constructs {
    public class Services {
        public Dictionary<string, int> Units { get; set; }
        public Services() { Units = new Dictionary<string, int>(); }
    }
}