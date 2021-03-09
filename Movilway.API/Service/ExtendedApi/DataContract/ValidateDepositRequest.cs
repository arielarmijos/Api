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
    public class ValidateDepositRequest : IMovilwayApiRequestWrapper<ValidateDepositRequestBody>
    {
        [MessageBodyMember(Name = "ValidateDepositRequest")]
        public ValidateDepositRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "ValidateDepositRequest", Namespace = "http://api.movilway.net/schema/extended")]
    public class ValidateDepositRequestBody : ASecuredApiRequest
    {
        [Loggable]
        [DataMember(Order = 1, IsRequired = true)]
        public decimal Amount { set; get; }

        [Loggable]
        [DataMember(Order = 2, IsRequired = true)]
        public String BankName { set; get; }

        [Loggable]
        [DataMember(Order = 3, IsRequired = true)]
        public String TransactionReference { set; get; }

        [Loggable]
        [DataMember(Order = 4, IsRequired = true)]
        public DateTime Date { set; get; }

        [Loggable]
        [DataMember(Order = 5, IsRequired = false)]
        public String BankBranchID { set; get; }

        [Loggable]
        [DataMember(Order = 6, IsRequired = false)]
        public String Description { set; get; }


    }
}