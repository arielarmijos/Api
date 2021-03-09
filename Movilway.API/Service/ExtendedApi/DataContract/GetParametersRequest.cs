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
    public class GetParametersRequest : IMovilwayApiRequestWrapper<GetParametersRequestBody>
    {
        [MessageBodyMember(Name = "GetParametersRequest")]
        public GetParametersRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "GetParametersRequest", Namespace="http://api.movilway.net/schema/extended")]
    public class GetParametersRequestBody : ASecuredApiRequest
    {
        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }
}