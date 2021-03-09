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
    public class BasicAgentInfo
    {
        [Loggable]
        [DataMember(EmitDefaultValue = false, Order = 0)]
        public String Agent { set; get; }

        [Loggable]
        [DataMember(EmitDefaultValue = false, Order = 1)]
        public String Name { set; get; }

        [Loggable]
        [DataMember(EmitDefaultValue = false, Order = 2)]
        public String Email { set; get; }

        [Loggable]
        [DataMember(EmitDefaultValue = false, Order = 3)]
        public String Department { set; get; }

        [Loggable]
        [DataMember(EmitDefaultValue = false, Order = 4)]
        public String City { set; get; }

        [Loggable]
        [DataMember(EmitDefaultValue = false, Order = 5)]
        public decimal CurrentBalance { set; get; }

        [Loggable]
        [DataMember(EmitDefaultValue = false, Order = 6)]
        public String Status { set; get; }

        [Loggable]
        [DataMember(EmitDefaultValue = false, Order = 7)]
        public int ChildsCount { set; get; }

        [Loggable]
        [DataMember(EmitDefaultValue = false, Order = 8)]
        public String PDVId { set; get; }
    }
}