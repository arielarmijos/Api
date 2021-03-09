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
    public class GetChildListRequest : IMovilwayApiRequestWrapper<GetChildListRequestBody>
    {
        [MessageBodyMember(Name = "GetChildListRequest")]
        public GetChildListRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "GetChildListRequest", Namespace="http://api.movilway.net/schema/extended")]
    public class GetChildListRequestBody : ASecuredApiRequest
    {
        [Loggable]
        [DataMember(Order = 2, IsRequired = true)]
        public String Agent { set; get; }

        [Loggable]
        [DataMember(Order = 3, IsRequired = false, EmitDefaultValue = false)]
        public bool? ExtendedValues { set; get; }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }
}