using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.Management.External
{

    [MessageContract(IsWrapped = false)]
    public class GetProductsExtendedRequest
    {
        [MessageBodyMember(Name = "GetProductsExtendedRequest", Namespace = "http://api.movilway.net/schema")]
        public GetProductsExtendedRequestBody Request { set; get; }

        public GetProductsExtendedRequest()
        {
            Request = new GetProductsExtendedRequestBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class GetProductsExtendedRequestBody : ExtendedApiRequest
    {
        [DataMember(Order = 1, IsRequired = true)]
        public int DeviceType { set; get; }

        [DataMember(Order = 2, IsRequired = true)]
        public String AgentReference { set; get; }
    }
}