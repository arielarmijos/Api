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
    public class CashOutRequest : IMovilwayApiRequestWrapper<CashOutRequestBody>
    {
        [MessageBodyMember(Name = "CashOutRequest")]
        public CashOutRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "CashOutRequest", Namespace="http://api.movilway.net/schema/extended")]
    public class CashOutRequestBody : ASecuredFinancialApiRequest
    {

        [Loggable]
        [DataMember(Order = 3, IsRequired = true)]
        public String Agent { set; get; }
    }
}