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
    public class ListConversationsResponse : AGenericApiResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListConversationsResponse" /> class.
        /// </summary>
        public ListConversationsResponse()
            : base()
        {
            this.ResponseMessage = string.Empty;
        }

        /// <summary>
        /// Elemento que contiene los ciudades
        /// </summary>
        [DataMember(Order = 3), XmlArray("Conversations"), XmlArrayItem("Conversation")]
        public List<DataContract.SOSIT.Conversation> Conversations { set; get; }

    }
}