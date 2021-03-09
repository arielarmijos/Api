using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging.Attribute;

namespace Movilway.API.Service.ExtendedApi.DataContract.SOSIT
{
    public class GetConversationResponse : AGenericApiResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetSolicitudeResponse" /> class.
        /// </summary>
        public GetConversationResponse()
            : base()
        {
            this.ResponseMessage = string.Empty;
        }

        /// <summary>
        /// Gets or sets Conversationid
        /// </summary>
        [DataMember(Order = 3), XmlElement]
        public int Conversationid { set; get; }

        /// <summary>
        /// Gets or sets Title
        /// </summary>
        [DataMember(Order = 4), XmlElement]
        public string Title { set; get; }

        /// <summary>
        /// Gets or sets Description
        /// </summary>
        [DataMember(Order = 5), XmlElement]
        public string Description { set; get; }

        /// <summary>
        /// Gets or sets Toaddress
        /// </summary>
        [DataMember(Order = 6), XmlElement]
        public string Toaddress { set; get; }

    }
}