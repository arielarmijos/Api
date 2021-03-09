using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Xml.Serialization;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging.Attribute;

namespace Movilway.API.Service.ExtendedApi.DataContract.Notiway
{
    /// <summary>
    /// Clase que determina la estructura de retorno ante la peticion de 
    /// lista de mensajes de Notiway
    /// </summary>
    public class GetNotiwayNewsListResponse : AGenericApiResponse
    {
        /// <summary>
        /// Cantidad de mensajes que contiene el elemento <c>News</c>
        /// </summary>
        [DataMember(Order = 1), XmlElement]
        public int Quantity { set; get; }

        /// <summary>
        /// Elemento que contiene los mensajes que cumplen con los criterios establecidos para
        /// ser retornados
        /// </summary>
        [DataMember(Order = 2), XmlArray("NewsList"), XmlArrayItem("News")]
        public List<NotiwayNews> NewsList { set; get; }

        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public GetNotiwayNewsListResponse() : base()
        {
            Quantity = 0;
            NewsList = new List<NotiwayNews>();
        }
    }
}
