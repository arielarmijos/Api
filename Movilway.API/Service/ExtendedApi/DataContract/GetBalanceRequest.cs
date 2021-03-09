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
    public class GetBalanceRequest:IMovilwayApiRequestWrapper<GetBalanceRequestBody>
    {
        [MessageBodyMember(Name = "GetBalanceRequest", Namespace = "http://api.movilway.net/schema/extended")]
        public GetBalanceRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "GetBalanceRequest", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetBalanceRequestBody : ASecuredApiRequest
    {
        [Loggable]
        [DataMember(Order = 3, IsRequired = false, EmitDefaultValue = false)]
        public bool? ExtendedValues { set; get; }

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