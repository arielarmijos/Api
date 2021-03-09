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
    public class RegisterDebtPaymentResponse : IMovilwayApiResponseWrapper<RegisterDebtPaymentResponseBody>
    {
        [MessageBodyMember(Name = "RegisterDebtPaymentResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public RegisterDebtPaymentResponseBody Response { set; get; }

        public RegisterDebtPaymentResponse()
        {
            Response = new RegisterDebtPaymentResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "RegisterDebtPaymentResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class RegisterDebtPaymentResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AGenericApiResponse
    {
        
    }
}