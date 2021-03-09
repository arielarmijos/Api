// <copyright file="ConsultaRequest.cs" company="Movilway">
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
    public class ConsultaRequest : ASecuredApiRequest
    {
        /// <summary>
        /// Gets or sets TipoCliente
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 3)]
        public Cash472.TipoCliente TipoCliente { get; set; }

        /// <summary>
        /// Gets or sets TipoIdentificacion
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 4)]
        public TipoIdentificacion TipoIdentificacion { get; set; }

        /// <summary>
        /// Gets or sets NumeroIdentificacion
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 5)]
        public string NumeroIdentificacion { get; set; }

        /// <summary>
        /// Gets or sets Pdv
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 6)]
        public string Pdv { get; set; }

        /// <summary>
        /// Gets or sets Pin
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = false, EmitDefaultValue = true, Order = 7)]
        public string Pin { get; set; }

        /// <summary>
        /// Valida que todos los campos obligatorias no tengan su valor por omisión
        /// </summary>
        /// <returns><c>true</c> si todos los campos obligatorios existen, <c>false</c> en caso contrario</returns>
        public bool IsValidRequest()
        {
            bool ret = true;

            ret = !string.IsNullOrEmpty(this.NumeroIdentificacion);
            ret = !string.IsNullOrEmpty(this.Pdv);

            return ret;
        }
    }
}
