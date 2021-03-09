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
    public class GetPendingDistributionsRequest : IMovilwayApiRequestWrapper<GetPendingDistributionsRequestBody>
    {
        [MessageBodyMember(Name = "GetPendingDistributionsRequest")]
        public GetPendingDistributionsRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "GetPendingDistributionsRequest", Namespace="http://api.movilway.net/schema/extended")]
    public class GetPendingDistributionsRequestBody : ASecuredApiRequest
    {
        [Loggable]
        [DataMember(Order = 3, IsRequired = true)]
        public String Agent { set; get; }

        [Loggable]
        [DataMember(Order = 4)]
        public int? Count { set; get; }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }
}