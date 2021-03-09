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
    public class ChangePasswordResponse : IMovilwayApiResponseWrapper<ChangePasswordResponseBody>
    {
        [MessageBodyMember(Name = "ChangePasswordResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public ChangePasswordResponseBody Response { set; get; }
    }

    [Loggable]
    [DataContract(Name = "ChangePasswordResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class ChangePasswordResponseBody : AGenericApiResponse
    {

        [Loggable]
        [DataMember(Order = 4, IsRequired = true)]
        public bool Result { set; get; }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }
}