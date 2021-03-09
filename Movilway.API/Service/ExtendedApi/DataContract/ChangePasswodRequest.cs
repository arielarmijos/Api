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
    public class ChangePasswodRequest : IMovilwayApiRequestWrapper<ChangePasswodRequestBody>
    {
        [MessageBodyMember(Name = "ChangePasswodRequest", Namespace = "http://api.movilway.net/schema/extended")]
        public ChangePasswodRequestBody Request { set; get; }
    }
    [Loggable]
    [DataContract(Name = "ChangePasswodRequest", Namespace = "http://api.movilway.net/schema/extended")]
    public class ChangePasswodRequestBody : ASecuredApiRequest
    {
        [Loggable]
        [DataMember(Order = 4, IsRequired = true)]
        public int UserId { set; get; }

        [Loggable]
        [DataMember(Order = 5, IsRequired = true)]
        public int AccessTypeId { set; get; }


        [DataMember(Order = 6, IsRequired = true)]
        public String NewPassword { set; get; }


        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }
}