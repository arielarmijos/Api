// <copyright file="EmitirResponse.cs" company="Movilway">
//     Copyright (c) Movilway. All rights reserved.
// </copyright>
namespace Movilway.API.Service.ExtendedApi.DataContract.Cash472
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Web;
    using System.Xml.Serialization;

    using Movilway.API.Service.ExtendedApi.DataContract.Common;
    using Movilway.Logging.Attribute;

    /// <summary>
    /// Clase que determina la estructura de retorno ante la peticion de 
    /// una nueva emisión de un giro
    /// </summary>
    public class EmitirResponse : AGenericApiResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmitirResponse" /> class.
        /// </summary>
        public EmitirResponse() : base()
        {
            this.ResponseMessage = string.Empty;
        }

        /// <summary>
        /// Gets or sets Pin
        /// </summary>
        [DataMember(Order = 3), XmlElement]
        public string Pin { get; set; }

        /// <summary>
        /// Gets or sets NumeroFactura
        /// </summary>
        [Loggable]
        [DataMember(Order = 4), XmlElement]
        public string NumeroFactura { get; set; }

        /// <summary>
        /// Gets or sets CodigoTransaccion
        /// </summary>
        [Loggable]
        [DataMember(Order = 5), XmlElement]
        public string CodigoTransaccion { get; set; }

        /// <summary>
        /// Gets or sets CodigoTransaccionConstitucion
        /// </summary>
        [Loggable]
        [DataMember(Order = 6), XmlElement]
        public string CodigoTransaccionConstitucion { get; set; }

        /// <summary>
        /// Gets or sets CodigoAutorizacion
        /// </summary>
        [Loggable]
        [DataMember(Order = 7), XmlElement]
        public string CodigoAutorizacion { get; set; }

        /// <summary>
        /// Gets or sets NumeroTransaccion472
        /// </summary>
        [Loggable]
        [DataMember(Order = 8), XmlElement]
        public string NumeroTransaccion472 { get; set; }

        /// <summary>
        /// Gets or sets Fecha
        /// </summary>
        [Loggable]
        [DataMember(Order = 9), XmlElement]
        public DateTime Fecha { get; set; }

        /// <summary>
        /// Gets or sets Id
        /// </summary>
        [Loggable]
        [DataMember(Order = 10), XmlElement]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets ExternalId es el identificador unico que le asigna 472 al Giro
        /// </summary>
        [Loggable]
        [DataMember(Order = 11), XmlElement]
        public int ExternalId { get; set; }

        /// <summary>
        /// Personalización ToString para logs
        /// </summary>
        /// <returns><c>string</c> personalizado</returns>
        public override string ToString()
        {
            return Movilway.API.Service.ExtendedApi.DataContract.Utils.logFormat(this);
        }
    }
}
