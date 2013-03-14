using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Models
{
    [ProtoContract]
    public class UserRequest
    {
        [ProtoMember(1)]
        public int UserId { get; set; }

        private UserRequest() { }

        public UserRequest(int userId)
        {
            this.UserId = userId;
        }
    }
}
