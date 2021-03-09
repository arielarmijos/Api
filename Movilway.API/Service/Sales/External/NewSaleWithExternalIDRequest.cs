using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.Sales.External
{

    [MessageContract(IsWrapped = false)]
    public class NewSaleWithExternalIDRequest
    {
        [MessageBodyMember(Name = "NewSaleWithExternalIDRequest", Namespace = "http://api.movilway.net/schema")]
        public NewSaleWithExternalIDRequestBody Request { set; get; }

        public NewSaleWithExternalIDRequest()
        {
            Request = new NewSaleWithExternalIDRequestBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class NewSaleWithExternalIDRequestBody
    {
        [DataMember(Order = 1, IsRequired = true)]
        public String UserId { set; get; }

        [DataMember(Order = 2, IsRequired = true)]
        public String IdProduct { set; get; }

        [DataMember(Order = 3, IsRequired = true)]
        public String Customer { set; get; }

        [DataMember(Order = 4, IsRequired = true)]
        public int Amount { set; get; }

        [DataMember(Order = 5, IsRequired = true)]
        public int CommitSale { set; get; }

        [DataMember(Order = 6, IsRequired = true)]
        public String ExternalId { set; get; }

        [DataMember(Order = 7, IsRequired = true)]
        public String PdvRepresented { set; get; }
    }
}