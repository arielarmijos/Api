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
    public class GetCityListRequest:IMovilwayApiRequestWrapper<GetCityListRequestBody>
    {
        [MessageBodyMember(Name = "GetCityListRequest", Namespace = "http://api.movilway.net/schema/extended")]
        public GetCityListRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "GetCityListRequest", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetCityListRequestBody : AUnsecuredApiRequest
    {
        [Loggable]
        [DataMember(Order = 2, IsRequired = true)]
        public int ProvinceID { set; get; }
    }
}