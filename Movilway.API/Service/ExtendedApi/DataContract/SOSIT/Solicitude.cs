using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Movilway.API.Service.ExtendedApi.DataContract.SOSIT
{
    public class Solicitude 
    {
        /// <summary>
        /// Gets or sets workorderid
        /// </summary>
        [DataMember(Order = 3), XmlElement]
        public int workorderid { set; get; }

        /// <summary>
        /// Gets or sets requester
        /// </summary>
        [DataMember(Order = 4), XmlElement]
        public string requester { set; get; }

        /// <summary>
        /// Gets or sets createdby
        /// </summary>
        [DataMember(Order = 5), XmlElement]
        public string createdby { set; get; }

        /// <summary>
        /// Gets or sets createdtime
        /// </summary>
        [DataMember(Order = 6), XmlElement]
        public DateTime createdtime { set; get; }

        /// <summary>
        /// Gets or sets subject
        /// </summary>
        [DataMember(Order = 7), XmlElement]
        public string subject { set; get; }

        /// <summary>
        /// Gets or sets technician
        /// </summary>
        [DataMember(Order = 8), XmlElement]
        public string technician { set; get; }

        /// <summary>
        /// Gets or sets priority
        /// </summary>
        [DataMember(Order = 9), XmlElement]
        public string priority { set; get; }

        /// <summary>
        /// Gets or sets status
        /// </summary>
        [DataMember(Order = 10), XmlElement]
        public string status { set; get; }

    }
}