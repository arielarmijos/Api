using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;
using Movilway.Logging.Attribute;
using Movilway.API.Service.ExtendedApi.DataContract.Common;

namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [MessageContract(IsWrapped = false)]
    public class CashOutResponse : IMovilwayApiResponseWrapper<CashOutResponseBody>
    {
        [MessageBodyMember(Name = "CashOutResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public CashOutResponseBody Response { set; get; }

        public CashOutResponse()
        {
            Response = new CashOutResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "CashOutResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class CashOutResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AFinancialApiResponse
    {

    }
}