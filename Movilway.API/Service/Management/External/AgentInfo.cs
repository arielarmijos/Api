using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using Movilway.API.Service.External;

namespace Movilway.API.Service.Management.External
{
    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class AgentInfo
    {
        [DataMember(Order = 1)]
        public String Agent { set; get; }

        [DataMember(Order = 2)]
        public long AgentID { set; get; }

        [DataMember(Order = 3)]
        public long ReferenceID { set; get; }

        [DataMember(Order = 4)]
        public long OwnerID { set; get; }

        [DataMember(Order = 5)]
        public String Name { set; get; }

        [DataMember(Order = 6)]
        public String Address { set; get; }

        [DataMember(Order = 7)]
        public String Email { set; get; }

        [DataMember(Order = 8)]
        public int? Depth { set; get; }

        [DataMember(Order = 9)]
        public ApiKeyValuePair AdditionalDataOld { set; get; }
    }
}