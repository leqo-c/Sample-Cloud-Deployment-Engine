namespace PA_Project.Constructs {
    public class Relation {
        public string ServiceRequesting { get; set; }
        public string Condition { get; set; }
        public string ServiceProviding { get; set; }
        public string Event { get; set; }

        public override bool Equals(object obj) {
            var r = obj as Relation;
            return r != null && r.ServiceRequesting == ServiceRequesting &&
                   r.Condition == Condition &&
                   r.ServiceProviding == ServiceProviding &&
                   r.Event == Event;
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = (ServiceRequesting != null ? ServiceRequesting.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Condition != null ? Condition.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ServiceProviding != null ? ServiceProviding.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Event != null ? Event.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}