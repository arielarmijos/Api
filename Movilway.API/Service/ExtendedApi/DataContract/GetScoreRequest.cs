using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.ServiceModel;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging.Attribute;

namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [MessageContract(IsWrapped=false)]
    public class GetScoreRequest : IMovilwayApiRequestWrapper<GetScoreRequestBody>
    {
        [MessageBodyMember(Name = "GetScoreRequest")]
        public GetScoreRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "GetScoreRequest", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetScoreRequestBody : ASecuredApiRequest
    {
        [Loggable]
        [DataMember(Order = 3, IsRequired = false)]
        public String Agent { set; get; }

        public override string ToString()
        {
            return Utils.logFormat(this);
            //return "GetScoreRequest {AuthenticationData{Username=" + base.AuthenticationData.Username + ",Password=******,SessionId=" + base.AuthenticationData.SessionID +
            //            "},DeviceType=" + base.DeviceType + ",Platform=" + base.Platform + ",Agent=" + this.Agent + "}";
        }
    }
}