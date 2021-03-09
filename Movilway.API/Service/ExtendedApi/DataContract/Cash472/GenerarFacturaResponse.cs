// <copyright file="GenerarFacturaResponse.cs" company="Movilway">
//     Copyright (c) Movilway. All rights reserved.
// </copyright>
namespace Movilway.API.Service.ExtendedApi.DataContract.Cash472
{
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    using Movilway.API.Service.ExtendedApi.DataContract.Common;
    using Movilway.Logging.Attribute;

    /// <summary>
    /// Clase que determina la estructura de retorno ante la peticion de 
    /// generación de factura
    /// </summary>
    public class GenerarFacturaResponse : AGenericApiResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenerarFacturaResponse" /> class.
        /// </summary>
        public GenerarFacturaResponse() : base()
        {
            this.ResponseMessage = string.Empty;
        }

        /// <summary>
        /// Gets or sets Factura
        /// </summary>
        [Loggable]
        [DataMember(Order = 3), XmlElement]
        public Movilway.API.Service.ExtendedApi.DataContract.Cash472.Factura Factura { get; set; }

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
