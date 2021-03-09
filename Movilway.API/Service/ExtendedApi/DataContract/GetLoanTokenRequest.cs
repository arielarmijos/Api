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
    public class GetLoanTokenRequest:IMovilwayApiRequestWrapper<GetLoanTokenRequestBody>
    {
        [MessageBodyMember(Name = "GetLoanTokenRequest", Namespace = "http://api.movilway.net/schema/extended")]
        public GetLoanTokenRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "GetLoanTokenRequest", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetLoanTokenRequestBody : ASecuredApiRequest
    {
        //[Loggable]
        //[DataMember(Order = 4, IsRequired = false, EmitDefaultValue = false)]
        //public String Agent { set; get; }

        public override string ToString()
        {
            return Utils.logFormat(this);
            //return "GetBalanceRequest {AuthenticationData{Username=" + base.AuthenticationData.Username + ",Password=******,SessionId=" + base.AuthenticationData.SessionID + "}" +
            //                            ",DeviceType=" + base.DeviceType + ",Platform=" + base.Platform + "}";
        }
    }
}