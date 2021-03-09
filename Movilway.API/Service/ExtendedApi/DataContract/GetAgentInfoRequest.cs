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
    public class GetAgentInfoRequest : IMovilwayApiRequestWrapper<GetAgentInfoRequestBody>
    {
        [MessageBodyMember(Name = "GetAgentInfoRequest")]
        public GetAgentInfoRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "GetAgentInfoRequest", Namespace="http://api.movilway.net/schema/extended")]
    public class GetAgentInfoRequestBody : ASecuredApiRequest
    {
        [Loggable]
        [DataMember(Order = 3, IsRequired = true)]
        public String Agent { set; get; }

        [Loggable]
        [DataMember(Order = 4, IsRequired = false, EmitDefaultValue = false)]
        public bool? SearchById { set; get; }

        [Loggable]
        [DataMember(Order = 5, IsRequired = false, EmitDefaultValue = false)]
        public String AgentId { set; get; }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }
}