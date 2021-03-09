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
    public class GetAgentDistributionListRequest : IMovilwayApiRequestWrapper<GetAgentDistributionListRequestBody>
    {
        [MessageBodyMember(Name = "GetAgentDistributionListRequest")]
        public GetAgentDistributionListRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "GetAgentDistributionListRequest", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetAgentDistributionListRequestBody : ASecuredApiRequest
    {
        [Loggable]
        [DataMember(Order = 2, IsRequired = true)]
        public String Agent { set; get; }

        [Loggable]
        [DataMember(Order = 3, IsRequired = false)]
        public String AgentChild { set; get; }

        [Loggable]
        [DataMember(Order = 4, IsRequired = false)]
        public String CutInfo { set; get; }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }
}