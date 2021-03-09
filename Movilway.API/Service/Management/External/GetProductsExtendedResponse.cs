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
    public class GetProductsExtendedResponse
    {
        [MessageBodyMember(Name = "GetProductsExtendedResponse", Namespace = "http://api.movilway.net/schema")]
        public GetProductsExtendedResponseBody Response { set; get; }

        public GetProductsExtendedResponse()
        {
            Response = new GetProductsExtendedResponseBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class GetProductsExtendedResponseBody : ApiResponse
    {
        [DataMember]
        public ApiKeyValuePair KeyValuePair { set; get; }
    }
}