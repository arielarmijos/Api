using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.D2.External
{
    [MessageContract(IsWrapped = false)]
    public class TransferResponse
    {
        [MessageBodyMember(Name = "TransferResponse", Namespace = "http://api.movilway.net/schema")]
        public TransferResponseBody Response { set; get; }

        public TransferResponse()
        {
            Response = new TransferResponseBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class TransferResponseBody : ApiResponse
    {
        [DataMember(Order = 1)]
        public Decimal Fee { set; get; }

        [DataMember(Order = 2)]
        public String ResultNameSpace { set; get; }

        [DataMember(Order = 3)]
        public long ScheduleID { set; get; }

        [DataMember(Order = 4)]
        public String TransExtReference { set; get; }
    }
}