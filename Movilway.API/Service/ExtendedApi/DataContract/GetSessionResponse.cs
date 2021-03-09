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
    public class GetSessionResponse : IMovilwayApiResponseWrapper<GetSessionResponseBody>
    {
        [MessageBodyMember(Name = "GetSessionResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public GetSessionResponseBody Response { set; get; }

        public GetSessionResponse()
        {
            Response = new GetSessionResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "GetSessionResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetSessionResponseBody : ExtendedApi.DataContract.Common.AGenericApiResponse
    {
        [Loggable]
        [DataMember(Order=0)]
        public String SessionID { set; get; }

        public override string ToString()
        {
            return Utils.logFormat(this);
            //return "GetSessionResponse {ResponseCode=" + base.ResponseCode + ",TransactionId=" + base.TransactionID + ",SessionId=" + this.SessionID + "}";
        }
    }
}