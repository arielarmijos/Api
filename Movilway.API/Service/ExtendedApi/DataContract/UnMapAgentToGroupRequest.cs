using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.ServiceModel;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging.Attribute;

namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [MessageContract(IsWrapped=false)]
    public class UnMapAgentToGroupRequest : IMovilwayApiRequestWrapper<UnMapAgentToGroupRequestBody>
    {
        [MessageBodyMember(Name = "UnMapAgentToGroupRequest")]
        public UnMapAgentToGroupRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "UnMapAgentToGroupRequest", Namespace="http://api.movilway.net/schema/extended")]
    public class UnMapAgentToGroupRequestBody : ASecuredApiRequest
    {
        [Loggable]
        [DataMember(Order = 2, IsRequired = true)]
        public int GroupID { set; get; }

        [Loggable]
        [DataMember(Order = 3, IsRequired = true)]
        public String Agent { set; get; }


    }
}