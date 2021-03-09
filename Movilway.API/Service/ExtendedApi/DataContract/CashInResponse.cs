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
    public class CashInResponse : IMovilwayApiResponseWrapper<CashInResponseBody>
    {
        [MessageBodyMember(Name = "CashInResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public CashInResponseBody Response { set; get; }

        public CashInResponse()
        {
            Response = new CashInResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "CashInResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class CashInResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AFinancialApiResponse
    {
        
    }
}