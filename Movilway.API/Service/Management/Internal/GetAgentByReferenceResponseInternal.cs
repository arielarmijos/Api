using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;
using Movilway.API.Service.Internal;

namespace Movilway.API.Service.Management.Internal
{
    public class GetAgentByReferenceResponseInternal:ApiResponseInternal
    {
        public String Agent { set; get; }
        public long AgentID { set; get; }
        public long ReferenceID { set; get; }
        public long OwnerID { set; get; }
        public String Name { set; get; }
        public String Address { set; get; }
        public String Email { set; get; }
        public int? Depth { set; get; }
        public Dictionary<String, String> AdditionalData { set; get; }
    }
}