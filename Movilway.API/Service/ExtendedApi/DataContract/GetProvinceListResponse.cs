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
    public class GetProvinceListResponse : IMovilwayApiResponseWrapper<GetProvinceListResponseBody>
    {
        [MessageBodyMember(Name = "GetProvinceListResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public GetProvinceListResponseBody Response { set; get; }

        public GetProvinceListResponse()
        {
            Response = new GetProvinceListResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "GetProvinceListResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetProvinceListResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AGenericApiResponse
    {
        [DataMember(Order = 1)]
        public ProvinceList ProvinceList { set; get; }
    }

    [CollectionDataContract(Namespace = "http://api.movilway.net/schema/extended", 
        KeyName="ID", ValueName="Description", ItemName="Province")]
    public class ProvinceList : Dictionary<int, String> { }
}