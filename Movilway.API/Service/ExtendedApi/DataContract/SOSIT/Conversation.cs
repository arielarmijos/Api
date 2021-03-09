using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Movilway.API.Service.ExtendedApi.DataContract.SOSIT
{
    public class Conversation 
    {
        /// <summary>
        /// Gets or sets Createddate
        /// </summary>
        [DataMember(Order = 3), XmlElement]
        public DateTime Createddate { set; get; }

        /// <summary>
        /// Gets or sets notifyid
        /// </summary>
        [DataMember(Order = 4), XmlElement]
        public int Notifyid { set; get; }

        /// <summary>
        /// Gets or sets from
        /// </summary>
        [DataMember(Order = 5), XmlElement]
        public string From { set; get; }

        /// <summary>
        /// Gets or sets description
        /// </summary>
        [DataMember(Order = 6), XmlElement]
        public string Description { set; get; }

    }
}