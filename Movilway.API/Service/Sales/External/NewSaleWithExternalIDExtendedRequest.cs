using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.Sales.External
{

    [MessageContract(IsWrapped = false)]
    public class NewSaleWithExternalIDExtendedRequest
    {
        [MessageBodyMember(Name = "NewSaleWithExternalIDExtendedRequest", Namespace = "http://api.movilway.net/schema")]
        public NewSaleWithExternalIDExtendedRequestBody Request { set; get; }

        public NewSaleWithExternalIDExtendedRequest()
        {
            Request = new NewSaleWithExternalIDExtendedRequestBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class NewSaleWithExternalIDExtendedRequestBody
    {
        [DataMember(Order = 0, IsRequired = true)]
        public String AccessId { set; get; }

        [DataMember(Order = 1, IsRequired = true)]
        public String Password { set; get; }

        [DataMember(Order = 2, IsRequired = true)]
        public int AccessType { set; get; }

        [DataMember(Order = 3, IsRequired = true)]
        public String IdProduct { set; get; }

        [DataMember(Order = 4, IsRequired = true)]
        public String Customer { set; get; }

        [DataMember(Order = 5, IsRequired = true)]
        public int Amount { set; get; }

        [DataMember(Order = 6, IsRequired = true)]
        public int CommitSale { set; get; }

        [DataMember(Order = 7, IsRequired = true)]
        public String ExternalId { set; get; }

        [DataMember(Order = 8, IsRequired = true)]
        public String PdvRepresented { set; get; }
    }
}