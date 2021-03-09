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
    public class GetNotificationResponse : AGenericApiResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetSolicitudeResponse" /> class.
        /// </summary>
        public GetNotificationResponse()
            : base()
        {
            this.ResponseMessage = string.Empty;
        }

        /// <summary>
        /// Gets or sets Notificationid
        /// </summary>
        [DataMember(Order = 3), XmlElement]
        public int Notificationid { set; get; }

        /// <summary>
        /// Gets or sets from
        /// </summary>
        [DataMember(Order = 4), XmlElement]
        public string From { set; get; }

        /// <summary>
        /// Gets or sets Createdate
        /// </summary>
        [DataMember(Order = 5), XmlElement]
        public DateTime Createdate { set; get; }

        /// <summary>
        /// Gets or sets Title
        /// </summary>
        [DataMember(Order = 6), XmlElement]
        public string Title { set; get; }

        /// <summary>
        /// Gets or sets Description
        /// </summary>
        [DataMember(Order = 7), XmlElement]
        public string Description { set; get; }

        /// <summary>
        /// Gets or sets Description
        /// </summary>
        [DataMember(Order = 8), XmlElement]
        public string Toaddress { set; get; }  

    }
}