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
    public class ValidateDepositResponse:IMovilwayApiResponseWrapper<ValidateDepositResponseBody>
    {
        [MessageBodyMember(Name = "ValidateDepositResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public ValidateDepositResponseBody Response { set; get; }

        public ValidateDepositResponse()
        {
            Response = new ValidateDepositResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "ValidateDepositResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class ValidateDepositResponseBody : AGenericApiResponse
    {
        [Loggable]
        [DataMember(Order = 3, EmitDefaultValue = false)]
        public string DepositResult { set; get; }
    }
}