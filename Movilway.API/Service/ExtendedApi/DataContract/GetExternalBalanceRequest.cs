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
    public class GetExternalBalanceRequest : IMovilwayApiRequestWrapper<GetExternalBalanceRequestBody>
    {
        [MessageBodyMember(Name = "GetExternalBalanceRequest", Namespace = "http://api.movilway.net/schema/extended")]
        public GetExternalBalanceRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "GetExternalBalanceRequest", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetExternalBalanceRequestBody : ASecuredApiRequest
    {
        [Loggable]
        [DataMember(Name = "Agent", Order = 0, IsRequired = true, EmitDefaultValue = false)]
        public String Agent { set; get; }

        [Loggable]
        [DataMember(Name = "TargetEntity", Order=1, IsRequired = true, EmitDefaultValue = false)]
        public String TargetEntity { set; get; }
    }
}