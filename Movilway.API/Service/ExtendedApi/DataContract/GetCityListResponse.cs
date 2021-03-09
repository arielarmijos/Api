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
    public class GetCityListResponse : IMovilwayApiResponseWrapper<GetCityListResponseBody>
    {
        [MessageBodyMember(Name = "GetCityListResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public GetCityListResponseBody Response { set; get; }

        public GetCityListResponse()
        {
            Response = new GetCityListResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "GetCityListResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetCityListResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AGenericApiResponse
    {
        [DataMember(Order = 1)]
        public CityList CityList { set; get; }
    }

    [CollectionDataContract(Namespace = "http://api.movilway.net/schema/extended", 
        KeyName="ID", ValueName="Description", ItemName="City")]
    public class CityList : Dictionary<int, String> { }
}