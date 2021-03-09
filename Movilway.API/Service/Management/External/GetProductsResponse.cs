using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;
using Movilway.API.Service.External;

namespace Movilway.API.Service.Management.External
{
    [MessageContract(IsWrapped = false)]
    public class GetProductsResponse
    {
        [MessageBodyMember(Name = "GetProductsResponse", Namespace = "http://api.movilway.net/schema")]
        public GetProductsResponseBody Response { set; get; }

        public GetProductsResponse()
        {
            Response = new GetProductsResponseBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class GetProductsResponseBody : ApiResponse
    {
        [DataMember]
        public ApiKeyValuePair KeyValuePair { set; get; }
    }
}