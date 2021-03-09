using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging.Attribute;


namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [MessageContract(IsWrapped = false)]
    public class GetAgentRequest : IMovilwayApiRequestWrapper<GetAgentRequestBody>
    {
        [MessageBodyMember(Name = "GetAgentRequest", Namespace = "http://api.movilway.net/schema/extended")]
        public GetAgentRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "GetAgentRequest", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetAgentRequestBody : ASecuredApiRequest
    {
        [Loggable]
        [DataMember(Order = 1, IsRequired = true)]
        public int  AgeId { set; get; }


        [Loggable]
        [DataMember(Order = 2, IsRequired = false)]
        public string Login { set; get; }
    }
}