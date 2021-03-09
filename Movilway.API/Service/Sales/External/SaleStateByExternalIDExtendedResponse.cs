using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.Sales.External
{
    [MessageContract(IsWrapped = false)]
    public class SaleStateByExternalIDExtendedResponse
    {
        [MessageBodyMember(Name = "SaleStateByExternalIDExtendedResponse", Namespace = "http://api.movilway.net/schema")]
        public SaleStateByExternalIDExtendedResponseBody Response { set; get; }

        public SaleStateByExternalIDExtendedResponse()
        {
            Response = new SaleStateByExternalIDExtendedResponseBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class SaleStateByExternalIDExtendedResponseBody
    {
        [DataMember(Order = 0)]
        public string IdTransaccion { get; set; }

        [DataMember(Order = 1)]
        public string Customer { get; set; }

        [DataMember(Order = 2)]
        public int Amount { get; set; }

        [DataMember(Order = 3)]
        public DateTime Date { get; set; }

        [DataMember(Order = 4)]
        public string ReloadState { get; set; }

        [DataMember(Order = 5)]
        public string ReloadStateCode { get; set; } 


    }
}