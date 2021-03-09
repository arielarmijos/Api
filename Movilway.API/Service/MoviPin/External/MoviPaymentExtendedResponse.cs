using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.MoviPin.External
{
    [MessageContract(IsWrapped = false)]
    public class MoviPaymentExtendedResponse
    {
        [MessageBodyMember(Name = "MoviPaymentExtendedResponse", Namespace = "http://api.movilway.net/schema")]
        public MoviPaymentExtendedResponseBody Response { set; get; }

        public MoviPaymentExtendedResponse()
        {
            Response = new MoviPaymentExtendedResponseBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class MoviPaymentExtendedResponseBody : ApiResponse
    {
        [DataMember(Order = 1)]
        public Decimal Fee { set; get; }

        [DataMember(Order = 2)]
        public String ResultNameSpace{ set; get; }

        [DataMember(Order = 3)]
        public long ScheduleID { set; get; }

        [DataMember(Order = 4)]
        public String TransExtReference { set; get; }
    }
}