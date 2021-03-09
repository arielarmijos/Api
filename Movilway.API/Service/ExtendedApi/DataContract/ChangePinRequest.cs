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
    public class ChangePinRequest:IMovilwayApiRequestWrapper<ChangePinRequestBody>
    {
        [MessageBodyMember(Name = "ChangePinRequest", Namespace = "http://api.movilway.net/schema/extended")]
        public ChangePinRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "ChangePinRequest", Namespace = "http://api.movilway.net/schema/extended")]
    public class ChangePinRequestBody : ASecuredApiRequest
    {
        [DataMember(Order = 3, IsRequired = true)]
        public String OldPin { set; get; }

        [DataMember(Order = 4, IsRequired = true)]
        public String NewPin { set; get; }

        [Loggable]
        [DataMember(Order = 5, IsRequired = true)]
        public String Agent { set; get; }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }
}