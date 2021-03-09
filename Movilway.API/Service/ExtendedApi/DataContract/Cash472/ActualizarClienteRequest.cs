// <copyright file="ActualizarClienteRequest.cs" company="Movilway">
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
    /// Clase que determina la estructura para una peticion de 
    /// un actualización de un cliente
    /// </summary>
    public class ActualizarClienteRequest : ASecuredApiRequest
    {
        /// <summary>
        /// Gets or sets Cliente
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 3)]
        public Cash472.Cliente Cliente { get; set; }

        /// <summary>
        /// Valida que todos los campos obligatorias no tengan su valor por omisión
        /// </summary>
        /// <returns><c>true</c> si todos los campos obligatorios existen, <c>false</c> en caso contrario</returns>
        public bool IsValidRequest()
        {
            bool ret = true;

            ret = this.Cliente != null;
            if (this.Cliente != null)
            {
                ret = this.Cliente.IsValidRequest();
            }

            return ret;
        }
    }
}
