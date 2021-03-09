using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging.Attribute;

namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [MessageContract(IsWrapped = false)]
    public class MoviPaymentResponse : IMovilwayApiResponseWrapper<MoviPaymentResponseBody>
    {
        [MessageBodyMember(Name = "MoviPaymentResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public MoviPaymentResponseBody Response { set; get; }

        public MoviPaymentResponse()
        {
            Response = new MoviPaymentResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "MoviPaymentResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class MoviPaymentResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AFinancialApiResponse
    {

    }
}