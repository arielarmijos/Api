using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.ServiceModel;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging.Attribute;

namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [MessageContract(IsWrapped=false)]
    public class BuyStockRequest : IMovilwayApiRequestWrapper<BuyStockRequestBody>
    {
        [MessageBodyMember(Name = "BuyStockRequest")]
        public BuyStockRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "BuyStockRequest", Namespace="http://api.movilway.net/schema/extended")]
    public class BuyStockRequestBody : ASecuredApiRequest
    {
        [Loggable]
        [DataMember(Order = 3, IsRequired = true)]
        public decimal Amount { set; get; }

        [Loggable]
        [DataMember(Order = 4, IsRequired = true)]
        public String BankName { set; get; }

        [Loggable]
        [DataMember(Order = 5, IsRequired = true)]
        public String TransactionReference { set; get; }

        //[Loggable]
        //[DataMember(Order = 4, IsRequired = false)]
        //public String Description { set; get; }

        [Loggable]
        [DataMember(Order = 6, IsRequired = true)]
        public DateTime TransactionDate { set; get; }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }
}