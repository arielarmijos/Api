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
    public class GetAgentDistributionsRequest : IMovilwayApiRequestWrapper<GetAgentDistributionsRequestBody>
    {
        [MessageBodyMember(Name = "GetAgentDistributionsRequest")]
        public GetAgentDistributionsRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "GetAgentDistributionsRequestBody", Namespace="http://api.movilway.net/schema/extended")]
    public class GetAgentDistributionsRequestBody : ASecuredApiRequest
    {
        [Loggable]
        [DataMember(Order = 3, IsRequired = true)]
        public String Agent { set; get; }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }
}