using System.Collections.Generic;
using ProtoBuf;

namespace Models
{
    [ProtoContract]
    public class User
    {
        [ProtoMember(1)]
        public int Id { get; private set; }

        [ProtoMember(2)]
        public string Name { get; set; }

        [ProtoMember(3)]
        public bool Active { get; set; }

        [ProtoMember(4)]
        public IDictionary<string, string> Metadata { get; private set; }

        private User() { }

        public User(int id, string name, bool active, IDictionary<string, string> meta)
        {
            this.Id = id;
            this.Name = name;
            this.Active = active;
            this.Metadata = meta;
        }
    }
}
