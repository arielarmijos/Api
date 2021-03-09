using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.Management.External
{

    [MessageContract(IsWrapped = false)]
    public class GetProductsRequest
    {
        [MessageBodyMember(Name = "GetProductsRequest", Namespace = "http://api.movilway.net/schema")]
        public GetProductsRequestBody Request { set; get; }

        public GetProductsRequest()
        {
            Request = new GetProductsRequestBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class GetProductsRequestBody : SessionApiRequest
    {
        [DataMember(Order = 1, IsRequired = true)]
        public int DeviceType { set; get; }

        [DataMember(Order = 2, IsRequired = true)]
        public String AgentReference { set; get; }
    }
}