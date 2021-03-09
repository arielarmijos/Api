using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.Sales.External
{
    [MessageContract(IsWrapped = false)]
    public class NewSaleWithExternalIDExtendedResponse
    {
        [MessageBodyMember(Name = "NewSaleWithExternalIDExtendedResponse", Namespace = "http://api.movilway.net/schema")]
        public NewSaleWithExternalIDExtendedResponseBody Response { set; get; }

        public NewSaleWithExternalIDExtendedResponse()
        {
            Response = new NewSaleWithExternalIDExtendedResponseBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class NewSaleWithExternalIDExtendedResponseBody
    {
        [DataMember(Order = 0)]
        public String Result { set; get; }

        [DataMember(Order = 1)]
        public int IdTransaction { set; get; }

        [DataMember(Order = 0)]
        public String SaleData { set; get; }

        [DataMember(Order = 0)]
        public String Message { set; get; }
    }
}