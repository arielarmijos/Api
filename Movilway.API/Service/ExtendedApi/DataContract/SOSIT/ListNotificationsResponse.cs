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
    public class ListNotificationsResponse : AGenericApiResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListNotificationsResponse" /> class.
        /// </summary>
        public ListNotificationsResponse()
            : base()
        {
            this.ResponseMessage = string.Empty;
        }

        /// <summary>
        /// Elemento que contiene los ciudades
        /// </summary>
        [DataMember(Order = 3), XmlArray("Notifications"), XmlArrayItem("Notification")]
        public List<DataContract.SOSIT.Notification> Notifications { set; get; }

    }
}