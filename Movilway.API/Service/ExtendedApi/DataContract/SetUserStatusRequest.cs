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
    public class SetUserStatusRequest : IMovilwayApiRequestWrapper<SetUserStatusRequestBody>
    {
        [MessageBodyMember(Name = "SetUserStatusRequest")]
        public SetUserStatusRequestBody Request { set; get; }

    }

    [Loggable]
    [DataContract(Name = "SetUserStatusRequest", Namespace = "http://api.movilway.net/schema/extended")]
    public class SetUserStatusRequestBody : ASecuredApiRequest
    {
        [Loggable]
        [DataMember(Order = 5, IsRequired = true)]
        public int UserId { set; get; }

        [Loggable]
        [DataMember(Order = 6, IsRequired = true)]
        public string Status { set; get; }

        [Loggable]
        [DataMember(Order = 7, IsRequired = false)]
        public int AccessType { set; get; }

        [Loggable]
        [DataMember(Order = 8, IsRequired = false)]
        public string Comment { set; get; }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }

        /*
        public override bool Equals(object obj)
        {
            TopUpRequestBody objval = obj as TopUpRequestBody;

            if (obj != null)
                return Equals(objval);
            else
                return false;
        }*/
    }
}