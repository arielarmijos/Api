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
    public class LoginAvailableResponse : IMovilwayApiResponseWrapper<LoginAvailableResponseBody>
    {
        [MessageBodyMember(Name = "LoginAvailableResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public LoginAvailableResponseBody Response { set; get; }

        public LoginAvailableResponse()
        {
            Response = new LoginAvailableResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "LoginAvailableResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class LoginAvailableResponseBody : ExtendedApi.DataContract.Common.AGenericApiResponse
    {
        [Loggable]
        [DataMember(Order = 1)]
        public ErrorItems Errors { set; get; }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }
}