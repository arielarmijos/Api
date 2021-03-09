using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using Movilway.Logging.Attribute;

namespace Movilway.API.Service.ExtendedApi.DataContract.Common
{

    [DataContract(Namespace="http://api.movilway.net/schema/extended")]
    public abstract class ASecuredApiRequest:IMovilwayApiRequest
    {
        public virtual Boolean IsFinancial { get { return false; } }

        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 0)]
        public AuthenticationData AuthenticationData { set; get; }

        [Loggable]
        [DataMember(Order = 1, IsRequired = true)]
        public int DeviceType { set; get; }

        [Loggable]
        [DataMember(Order = 2, IsRequired = false)]
        public String Platform { set; get; }
    }
}