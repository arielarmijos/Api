using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Movilway.API.Service.ExtendedApi.DataContract.Notiway
{
    /// <summary>
    /// Clase que determina la estructura que sera retornada como un mensaje de
    /// Notiway
    /// </summary>
    [DataContract]
    public class NotiwayNews
    {
        /// <summary>
        /// ID del mensaje
        /// </summary>
        [DataMember(Name = "NewsId", Order = 0)]//, XmlAttribute(AttributeName = "NewsId")]
        public int NewsId { set; get; }

        /// <summary>
        /// ID programacion
        /// </summary>
        [DataMember(Name = "ScheduleId", Order = 1)]//, XmlAttribute(AttributeName = "ScheduleId")]
        public int ScheduleId { set; get; }

        /// <summary>
        /// Fecha de expiracion
        /// </summary>
        [DataMember(Name = "ExpirationDate", Order = 2)]//, XmlAttribute(AttributeName = "ExpirationDate")]
        public String ExpirationDate { set; get; }

        /// <summary>
        /// Titulo del mensaje
        /// </summary>
        [DataMember(Name = "Title", Order = 3)]//, XmlAttribute(AttributeName = "Title")]
        public string Title { set; get; }

        /// <summary>
        /// Resumen del mensaje
        /// </summary>
        [DataMember(Name = "Abstract", Order = 4)]//, XmlAttribute(AttributeName = "Abstract")]
        public string Abstract { set; get; }

        /// <summary>
        /// Detalle del mensaje
        /// </summary>
        [DataMember(Name = "Detail", Order = 5)]//, XmlAttribute(AttributeName = "Detail")]
        public string Detail { set; get; }

        /// <summary>
        /// URL de la imagen del mensaje
        /// </summary>
        [DataMember(Name = "ImageURL", Order = 6)]//, XmlAttribute(AttributeName = "ImageURL")]
        public string ImageURL { set; get; }

        /// <summary>
        /// Tipo de mensaje (urgente, inbox, ...)
        /// </summary>
        [DataMember(Name = "Type", Order = 7)]//, XmlAttribute(AttributeName = "Type")]
        public int Type { set; get; }

        /// <summary>
        /// Cantidad de impresiones del mensaje
        /// </summary>
        [DataMember(Name = "Printing", Order = 8)]//, XmlAttribute(AttributeName = "Printing")]
        public int Printing { set; get; }

        /// <summary>
        /// Fecha en que se envio por primera vez el mensaje
        /// </summary>
        [DataMember(Name = "FirstDeliveryDate", Order = 9)]//, XmlAttribute(AttributeName = "FirstDeliveryDate")]
        public String FirstDeliveryDate { set; get; }

        /// <summary>
        /// Fecha en que el usuario leyo por ultima vez el mensaje
        /// </summary>
        [DataMember(Name = "LastReadDate", Order = 10)]//, XmlAttribute(AttributeName = "LastReadDate")]
        public String LastReadDate { set; get; }
    }
}
