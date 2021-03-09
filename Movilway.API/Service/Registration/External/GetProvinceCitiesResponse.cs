using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.Registration.External
{

    [MessageContract(IsWrapped = false)]
    public class GetProvinceCitiesResponse
    {
        [MessageBodyMember(Name = "GetProvinceCitiesResponse", Namespace = "http://api.movilway.net/schema")]
        public GetProvinceCitiesResponseBody Response { set; get; }

        public GetProvinceCitiesResponse()
        {
            Response = new GetProvinceCitiesResponseBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class GetProvinceCitiesResponseBody : ApiResponse
    {
        [DataMember(IsRequired=true)]
        public CityList Cities { set; get; }
    }

    [CollectionDataContract(Namespace="http://api.movilway.net/schema")]
    public class CityList:List<CityInfo>{}

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class CityInfo
    {
        [DataMember(Order = 1, IsRequired = true)]
        public int CityID { set; get; }

        [DataMember(Order = 2, IsRequired = true)]
        public String CityName { set; get; }

        public CityInfo(int cityID, String cityName)
        {
            CityID = cityID;
            CityName = cityName;
        }
    }
}