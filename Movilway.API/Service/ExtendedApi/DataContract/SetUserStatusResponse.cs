using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Web;

namespace Movilway.API.Service.ExtendedApi.DataContract
{
  

    [MessageContract(IsWrapped = false)]
    public class SetUserStatusResponse : IMovilwayApiResponseWrapper<SetUserStatusResponseBody>
    {
        [MessageBodyMember(Name = "SetUserStatusResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public SetUserStatusResponseBody Response { set; get; }

        public SetUserStatusResponse()
        {
            Response = new SetUserStatusResponseBody();
        }

    }

    [Loggable]
    [DataContract(Name = "SetUserStatusResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class SetUserStatusResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AGenericApiResponse
    {
        [Loggable]
        [DataMember(Order = 5, IsRequired = true)]
        public bool Result { set; get; }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }

    
}