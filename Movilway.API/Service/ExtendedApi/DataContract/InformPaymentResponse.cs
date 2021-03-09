using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging.Attribute;
using System.Text;

namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [MessageContract(IsWrapped = false)]
    public class InformPaymentResponse : IMovilwayApiResponseWrapper<InformPaymentResponseBody>
    {
        [MessageBodyMember(Name = "InformPaymentResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public InformPaymentResponseBody Response { set; get; }

        public InformPaymentResponse()
        {
            Response = new InformPaymentResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "InformPaymentResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class InformPaymentResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AGenericApiResponse
    {
        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }
}