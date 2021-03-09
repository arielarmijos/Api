// <copyright file="CotizarResponse.cs" company="Movilway">
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
    /// cotización
    /// </summary>
    public class CotizarResponse : AGenericApiResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CotizarResponse" /> class.
        /// </summary>
        public CotizarResponse() : base()
        {
            this.ResponseMessage = string.Empty;
        }

        /// <summary>
        /// Gets or sets TotalARecibir
        /// </summary>
        [Loggable]
        [DataMember(Order = 3), XmlElement]
        public long TotalARecibir { get; set; }

        /// <summary>
        /// Gets or sets TotalAEntregar
        /// </summary>
        [Loggable]
        [DataMember(Order = 4), XmlElement]
        public long TotalAEntregar { get; set; }

        /// <summary>
        /// Gets or sets Flete
        /// </summary>
        [Loggable]
        [DataMember(Order = 5), XmlElement]
        public long Flete { get; set; }

        /// <summary>
        /// Gets or sets CodigoTransaccion
        /// </summary>
        [Loggable]
        [DataMember(Order = 6), XmlElement]
        public string CodigoTransaccion { get; set; }

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
