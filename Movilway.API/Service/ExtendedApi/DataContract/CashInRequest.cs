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
    public class CashInRequest : IMovilwayApiRequestWrapper<CashInRequestBody>
    {
        [MessageBodyMember(Name = "CashInRequest")]
        public CashInRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "CashInRequest", Namespace="http://api.movilway.net/schema/extended")]
    public class CashInRequestBody : ASecuredFinancialApiRequest
    {
        [Loggable]
        [DataMember(Order = 2, IsRequired = true)]
        public String Agent { set; get; }


    }
}