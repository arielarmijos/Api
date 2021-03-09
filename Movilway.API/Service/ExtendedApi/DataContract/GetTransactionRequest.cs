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
    public class GetTransactionRequest : IMovilwayApiRequestWrapper<GetTransactionRequestBody>
    {
        [MessageBodyMember(Name = "GetTransactionRequest", Namespace = "http://api.movilway.net/schema/extended")]
        public GetTransactionRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "GetTransactionRequest", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetTransactionRequestBody : ASecuredApiRequest
    {
        [Loggable]
        [DataMember(Order = 3, IsRequired = true)]
        public GetTransactionRequestParameterType ParameterType { set; get; }

        [Loggable]
        [DataMember(Order = 4, IsRequired = true)]
        public String ParameterValue { set; get; }

        public override string ToString()
        {
            return Utils.logFormat(this);
            //return "GetTransactionRequest {AuthenticationData{Username=" + base.AuthenticationData.Username + ",Password=******,SessionId=" + base.AuthenticationData.SessionID + 
            //            "},DeviceType=" + base.DeviceType + ",Platform=" + base.Platform + ",ParameterType=" + this.ParameterType + ",ParameterValue=" + this.ParameterValue + "}";
        }
    }
}