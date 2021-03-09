using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using Movilway.Logging.Attribute;

namespace Movilway.API.Service.ExtendedApi.DataContract.Common
{
    [Loggable]
    [DataContract(Namespace="http://api.movilway.net/schema/extended")]
    public abstract class ASecuredFinancialApiRequest : ASecuredApiRequest
    {
        public override Boolean IsFinancial { get { return true; } }

        [Loggable]
        [DataMember(Order = 3, IsRequired = true)]
        public decimal Amount { set; get; }

        [Loggable]
        [DataMember(Order = 4, IsRequired = true)]
        public String ExternalTransactionReference { set; get; }
    }
}