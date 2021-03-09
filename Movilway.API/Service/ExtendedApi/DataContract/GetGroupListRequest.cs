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
    public class GetGroupListRequest : IMovilwayApiRequestWrapper<GetGroupListRequestBody>
    {
        [MessageBodyMember(Name = "GetGroupListRequest")]
        public GetGroupListRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "GetGroupListRequest", Namespace="http://api.movilway.net/schema/extended")]
    public class GetGroupListRequestBody : ASecuredApiRequest
    {

    }
}