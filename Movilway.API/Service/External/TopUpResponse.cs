using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.External
{
    [MessageContract(IsWrapped = false)]
    public class TopUpResponse
    {
        [MessageBodyMember(Name = "TopUpResponse", Namespace = "http://api.movilway.net/schema")]
        public TopUpResponseBody Response { set; get; }

        public TopUpResponse()
        {
            Response = new TopUpResponseBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class TopUpResponseBody : ApiResponse
    {
        [DataMember(Order = 0)]
        public String HostTransRef { set; get; }

        [DataMember(Order = 1)]
        public Decimal Fee { set; get; }

        [DataMember(Order = 2)]
        public Decimal BalanceStock{ set; get; }
    }
}