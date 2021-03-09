using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.ServiceModel;
using Movilway.Logging.Attribute;

namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [DataContract(Namespace = "http://api.movilway.net/schema/extended")]
    public class Rol
    {
        [Loggable]
        [DataMember(EmitDefaultValue = false, Order = 0)]
        public int RolId { set; get; }

        [Loggable]
        [DataMember(EmitDefaultValue = false, Order = 1)]
        public String RolName { set; get; }
    }
}