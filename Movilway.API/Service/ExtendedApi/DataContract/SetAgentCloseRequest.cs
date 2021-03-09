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
    public class SetAgentCloseRequest : IMovilwayApiRequestWrapper<SetAgentCloseRequestBody>
    {
        [MessageBodyMember(Name = "SetAgentCloseRequest")]
        public SetAgentCloseRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "SetAgentCloseRequestBody", Namespace="http://api.movilway.net/schema/extended")]
    public class SetAgentCloseRequestBody : ASecuredApiRequest
    {
        [Loggable]
        [DataMember(Order = 3, IsRequired = true)]
        public String Agent { set; get; }

        [Loggable]
        [DataMember(Order = 4, IsRequired = true)]
        public int IdMin { set; get; }

        [Loggable]
        [DataMember(Order = 5, IsRequired = true)]
        public int IdMax { set; get; }

        [Loggable]
        [DataMember(Order = 6, IsRequired = true)]
        public String Type { set; get; }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }
}