using Movilway.API.Service.ExtendedApi.DataContract.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Web;
using Movilway.Logging.Attribute;

namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [MessageContract(IsWrapped=false)]
    public class SecurityResponse : IMovilwayApiResponseWrapper<SecurityResponseBody>
    {
        [MessageBodyMember(Name = "SecurityResponseBody")]
        public SecurityResponseBody Response
        {
            get;
            set;
        }

        public SecurityResponse ()
        {
            
        }

    }

    [DataContract(Name = "SecurityResponseBody", Namespace = "http://api.movilway.net/schema/extended")]
    public class SecurityResponseBody:AGenericApiResponse
    {
        [Loggable]
        [DataMember(Order = 0)]
        public String SessionID { set; get; }
    }
}