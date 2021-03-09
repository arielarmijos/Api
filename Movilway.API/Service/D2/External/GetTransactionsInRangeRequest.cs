using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;
using Movilway.Logging.Attribute;

namespace Movilway.API.Service.D2.External
{
   
    [MessageContract(IsWrapped = false)]
    public class GetTransactionsInRangeRequest
    {
        [MessageBodyMember(Name = "GetTransactionsInRangeRequest", Namespace = "http://api.movilway.net/schema")]
        public GetTransactionsInRangeRequestBody Request { set; get; }

        public GetTransactionsInRangeRequest()
        {
            Request = new GetTransactionsInRangeRequestBody();
        }
    }

    [Loggable]
    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class GetTransactionsInRangeRequestBody : SessionApiRequest
    {
        [Loggable]
        [DataMember(Order = 1, IsRequired = true)]
        public int DeviceType { set; get; }

        [Loggable]
        [DataMember(Order = 2, IsRequired = true)]
        public String Agent { set; get; }

        [Loggable]
        [DataMember(Order = 3, IsRequired = true)]
        public DateTime StartDate { set; get; }

        [Loggable]
        [DataMember(Order = 4, IsRequired = false)]
        public DateTime EndDate { set; get; }
    }
}