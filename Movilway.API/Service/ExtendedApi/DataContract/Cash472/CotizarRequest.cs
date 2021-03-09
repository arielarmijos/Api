// <copyright file="CotizarRequest.cs" company="Movilway">
//     Copyright (c) Movilway. All rights reserved.
// </copyright>
namespace Movilway.API.Service.ExtendedApi.DataContract.Cash472
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Web;

    using Movilway.API.Service.ExtendedApi.DataContract.Common;
    using Movilway.Logging.Attribute;

    /// <summary>
    /// Clase que determina la estructura para la petición de consulta
    /// </summary>
    public class CotizarRequest : ASecuredApiRequest
    {
        /// <summary>
        /// Gets or sets Pdv
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 3)]
        public string Pdv { get; set; }

        /// <summary>
        /// Gets or sets ValorRecibido
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 4)]
        public long ValorRecibido { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IncluyeFlete
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 5)]
        public bool IncluyeFlete { get; set; }
    }
}
