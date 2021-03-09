using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using System.Collections.Generic;

namespace Movilway.API.Service.ExtendedApi.DataContract.SOSIT
{
    public class GetSolicitudeResponse : AGenericApiResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetSolicitudeResponse" /> class.
        /// </summary>
        public GetSolicitudeResponse()
            : base()
        {
            this.ResponseMessage = string.Empty;
        }

        /// <summary>
        /// Gets or sets Workorderid
        /// </summary>
        [DataMember(Order = 3), XmlElement]
        public string Workorderid { set; get; }

        /// <summary>
        /// Gets or sets Createdtime
        /// </summary>
        [DataMember(Order = 4), XmlElement]
        public DateTime Createdtime { set; get; }

        /// <summary>
        /// Gets or sets subject
        /// </summary>
        [DataMember(Order = 5), XmlElement]
        public string Subject { set; get; }

        /// <summary>
        /// Gets or sets description
        /// </summary>
        [DataMember(Order = 6), XmlElement]
        public string Description { set; get; }

        /// <summary>
        /// Gets or sets status
        /// </summary>
        [DataMember(Order = 7), XmlElement]
        public string Status { set; get; }

        /// <summary>
        /// Gets or sets category
        /// </summary>
        [DataMember(Order = 8), XmlElement]
        public string Category { set; get; }

        /// <summary>
        /// Gets or sets subcategory
        /// </summary>
        [DataMember(Order = 9), XmlElement]
        public string Subcategory { set; get; }


        /// <summary>
        /// Gets or sets item
        /// </summary>
        [DataMember(Order = 10), XmlElement]
        public string Item { set; get; }


        /// <summary>
        /// Gets or sets priority
        /// </summary>
        [DataMember(Order = 11), XmlElement]
        public string Priority { set; get; }


        /// <summary>
        /// Gets or sets group
        /// </summary>
        [DataMember(Order = 12), XmlElement]
        public string Group { set; get; }


        /// <summary>
        /// Elemento que contiene los ciudades
        /// </summary>
        [DataMember(Order = 13), XmlArray("Conversations"), XmlArrayItem("Conversation")]
        public List<DataContract.SOSIT.Conversation> Conversations { set; get; }

            
            
            




    }
}