using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Xml.Serialization;

namespace Movilway.API.Service.ExtendedApi.DataContract.Payment
{
    public class GetPSEBankResponse : AGenericApiResponse
    {

        /// <summary>
        /// Gets or sets BankId
        /// </summary>
        [Loggable]
        [DataMember(Order = 3), XmlElement]
        public int BankId { get; set; }
        
        /// <summary>
        /// Gets or sets BankName
        /// </summary>
        [Loggable]
        [DataMember(Order = 4), XmlElement]
        public string BankName { get; set; }

        /// <summary>
        /// Gets or sets CBU
        /// </summary>
        [Loggable]
        [DataMember(Order = 5), XmlElement]
        public string CBU { get; set; }

        /// <summary>
        /// Gets or sets Id
        /// </summary>
        [Loggable]
        [DataMember(Order = 6), XmlElement]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets Number
        /// </summary>
        [Loggable]
        [DataMember(Order = 7), XmlElement]
        public string Number { get; set; }
    }
}