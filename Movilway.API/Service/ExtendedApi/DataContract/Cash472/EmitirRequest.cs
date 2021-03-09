// <copyright file="EmitirRequest.cs" company="Movilway">
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
    /// una nueva emisión de un giro
    /// </summary>
    public class EmitirRequest : ASecuredApiRequest
    {
        /// <summary>
        /// Gets or sets Emisor
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 3)]
        public Cash472.Cliente Emisor { get; set; }

        /// <summary>
        /// Gets or sets Receptor
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 4)]
        public Cash472.Cliente Receptor { get; set; }

        /// <summary>
        /// Gets or sets Pdv
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 5)]
        public string Pdv { get; set; }

        /// <summary>
        /// Gets or sets CiudadDestino
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 6)]
        public string CiudadDestino { get; set; }

        /// <summary>
        /// Gets or sets ValorRecibido
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 7)]
        public long ValorRecibido { get; set; }

        /// <summary>
        /// Gets or sets ValorAEntregar
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 8)]
        public long ValorAEntregar { get; set; }

        /// <summary>
        /// Gets or sets ValorFlete
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 9)]
        public long ValorFlete { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IncluyeFlete
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 10)]
        public bool IncluyeFlete { get; set; }

        /// <summary>
        /// Gets or sets CiudadOrigen
        /// </summary>
        internal string CiudadPdv { get; set; }

        /// <summary>
        /// Valida que todos los campos obligatorias no tengan su valor por omisión
        /// </summary>
        /// <returns><c>true</c> si todos los campos obligatorios existen, <c>false</c> en caso contrario</returns>
        public bool IsValidRequest()
        {
            bool ret = true;

            ret = this.Emisor != null;
            ret = this.Receptor != null;
            ret = !string.IsNullOrEmpty(this.Pdv);
            //// ret = !string.IsNullOrEmpty(this.CiudadPdv);
            ret = !string.IsNullOrEmpty(this.CiudadDestino);
            ret = this.ValorRecibido > 0;
            ret = this.ValorAEntregar > 0;
            ret = this.ValorFlete > 0;

            return ret;
        }
    }
}
