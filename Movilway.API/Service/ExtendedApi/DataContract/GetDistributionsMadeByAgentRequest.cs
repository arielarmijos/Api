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
    public class GetDistributionsMadeByAgentRequest : IMovilwayApiRequestWrapper<GetDistributionsMadeByAgentRequestBody>
    {
        [MessageBodyMember(Name = "GetDistributionsMadeByAgentRequest")]
        public GetDistributionsMadeByAgentRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "GetDistributionsMadeByAgentRequest", Namespace="http://api.movilway.net/schema/extended")]
    public class GetDistributionsMadeByAgentRequestBody : ASecuredApiRequest
    {
        [Loggable]
        [DataMember(Order = 3, IsRequired = true)]
        public DateTime LowerLimit { set; get; }

        [Loggable]
        [DataMember(Order = 4, IsRequired = true)]
        public DateTime UpperLimit { set; get; }

        [Loggable]
        [DataMember(Order = 5, IsRequired = false)]
        public int[] DistributionTypeId { set; get; }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }
}
