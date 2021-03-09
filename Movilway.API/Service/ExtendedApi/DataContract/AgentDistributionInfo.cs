using Movilway.Logging.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [DataContract(Namespace = "http://api.movilway.net/schema/extended")]
    public class AgentDistributionInfo
    {
        [Loggable]
        [DataMember(EmitDefaultValue = false, Order = 0)]
        public int AgentId { set; get; }

        [Loggable]
        [DataMember(EmitDefaultValue = false, Order = 1)]
        public String AgentName { set; get; }

        [Loggable]
        [DataMember(EmitDefaultValue = false, Order = 2)]
        public String LegalNumber { set; get; }

        [Loggable]
        [DataMember(EmitDefaultValue = true, Order = 3)]
        public decimal Stock { set; get; }

        [Loggable]
        [DataMember(EmitDefaultValue = true, Order = 4)]
        public decimal Account { set; get; }

        [Loggable]
        [DataMember(EmitDefaultValue = false, Order = 5)]
        public String LastDistribution { set; get; }

        [Loggable]
        [DataMember(EmitDefaultValue = false, Order = 6)]
        public String LastTopUp { set; get; }

        [Loggable]
        [DataMember(EmitDefaultValue = false, Order = 7)]
        public String Status { set; get; }

        [Loggable]
        [DataMember(EmitDefaultValue = false, Order = 8)]
        public decimal CreditLimit { set; get; }

        [Loggable]
        [DataMember(EmitDefaultValue = false, Order = 9)]
        public String PDV { set; get; }

        [Loggable]
        [DataMember(EmitDefaultValue = false, Order = 10)]
        public decimal LastCutAmount { set; get; }

        [Loggable]
        [DataMember(EmitDefaultValue = false, Order = 11)]
        public decimal AvailableAmount { set; get; }

        [Loggable]
        [DataMember(EmitDefaultValue = false, Order = 12)]
        public decimal ChildStock { set; get; }
    }
}