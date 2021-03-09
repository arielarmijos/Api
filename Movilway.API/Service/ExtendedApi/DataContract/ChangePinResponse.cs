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
    public class ChangePinResponse : IMovilwayApiResponseWrapper<ChangePinResponseBody>
    {
        [MessageBodyMember(Name = "ChangePinResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public ChangePinResponseBody Response { set; get; }

        public ChangePinResponse()
        {
            Response = new ChangePinResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "ChangePinResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class ChangePinResponseBody : ExtendedApi.DataContract.Common.AGenericApiResponse
    {
        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }
}