// <copyright file="PagoResponse.cs" company="Movilway">
//     Copyright (c) Movilway. All rights reserved.
// </copyright>
namespace Movilway.API.Service.ExtendedApi.DataContract.Cash472
{
    using System;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    using Movilway.API.Service.ExtendedApi.DataContract.Common;
    using Movilway.Logging.Attribute;

    /// <summary>
    /// Clase que determina la estructura de retorno ante la peticion de 
    /// un pago
    /// </summary>
    public class PagoResponse : AGenericApiResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PagoResponse" /> class.
        /// </summary>
        public PagoResponse() : base()
        {
            this.ResponseMessage = string.Empty;
        }

        /// <summary>
        /// Gets or sets NumeroFactura
        /// </summary>
        [Loggable]
        [DataMember(Order = 3), XmlElement]
        public string NumeroFactura { get; set; }

        /// <summary>
        /// Gets or sets CodigoTransaccion
        /// </summary>
        [Loggable]
        [DataMember(Order = 4), XmlElement]
        public string CodigoTransaccion { get; set; }

        /// <summary>
        /// Gets or sets CodigoAutorizacion
        /// </summary>
        [Loggable]
        [DataMember(Order = 5), XmlElement]
        public string CodigoAutorizacion { get; set; }

        /// <summary>
        /// Gets or sets NumeroComprobantePago
        /// </summary>
        [Loggable]
        [DataMember(Order = 6), XmlElement]
        public string NumeroComprobantePago { get; set; }

        /// <summary>
        /// Gets or sets NumeroReferencia
        /// </summary>
        [Loggable]
        [DataMember(Order = 7), XmlElement]
        public string NumeroReferencia { get; set; }

        /// <summary>
        /// Gets or sets Valor
        /// </summary>
        [Loggable]
        [DataMember(Order = 8), XmlElement]
        public string Valor { get; set; }

        /// <summary>
        /// Gets or sets Fecha
        /// </summary>
        [Loggable]
        [DataMember(Order = 9), XmlElement]
        public DateTime Fecha { get; set; }

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
