using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.Registration.External
{

    [MessageContract(IsWrapped = false)]
    public class GetProvincesResponse
    {
        [MessageBodyMember(Name = "GetProvincesResponse", Namespace = "http://api.movilway.net/schema")]
        public GetProvincesResponseBody Response { set; get; }

        public GetProvincesResponse()
        {
            Response = new GetProvincesResponseBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class GetProvincesResponseBody : ApiResponse
    {
        [DataMember(IsRequired=true)]
        public ProvinceList Provinces { set; get; }
    }

    [CollectionDataContract(Namespace="http://api.movilway.net/schema")]
    public class ProvinceList:List<ProvinceInfo>{}

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class ProvinceInfo
    {
        [DataMember(Order = 1, IsRequired = true)]
        public int ProvinceID { set; get; }

        [DataMember(Order = 2, IsRequired = true)]
        public String ProvinceName { set; get; }

        public ProvinceInfo(int provinceID, String provinceName)
        {
            ProvinceID = provinceID;
            ProvinceName = provinceName;
        }
    }
}