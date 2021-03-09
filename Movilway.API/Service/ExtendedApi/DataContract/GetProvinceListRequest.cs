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
    public class GetProvinceListRequest:IMovilwayApiRequestWrapper<GetProvinceListRequestBody>
    {
        [MessageBodyMember(Name = "GetProvinceListRequest", Namespace = "http://api.movilway.net/schema/extended")]
        public GetProvinceListRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "GetProvinceListRequest", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetProvinceListRequestBody : AUnsecuredApiRequest
    {
    }
}