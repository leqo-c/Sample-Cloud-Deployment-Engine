using System;
using System.ComponentModel;

namespace PA_Project.CDE {
    public class EventInfo {
        public string Type { get; set; }
        public int Id { get; set; }
        public EventInfo ShallowCopy() {
            return MemberwiseClone() as EventInfo;
        }
        public override bool Equals(object obj) {
            var he = obj as EventInfo;
            return he != null && he.Type == Type && he.Id == Id;
        }
        public override int GetHashCode() {
            unchecked {
                return ((Type != null ? Type.GetHashCode() : 0) * 397) ^ Id;
            }
        }
    }
}