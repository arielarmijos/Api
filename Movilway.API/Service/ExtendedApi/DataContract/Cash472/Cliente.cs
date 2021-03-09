// <copyright file="Cliente.cs" company="Movilway">
//     Copyright (c) Movilway. All rights reserved.
// </copyright>
namespace Movilway.API.Service.ExtendedApi.DataContract.Cash472
{
    using System;
    using System.Runtime.Serialization;

    using Movilway.API.Service.ExtendedApi.DataContract.Common;
    using Movilway.Logging.Attribute;

    /// <summary>
    /// Modelo representación de un cliente
    /// </summary>
    public class Cliente
    {
        /// <summary>
        /// Gets or sets TipoIdentificacion
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 1)]
        public TipoIdentificacion TipoIdentificacion { get; set; }

        /// <summary>
        /// Gets or sets NumeroIdentificacion
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 2)]
        public string NumeroIdentificacion { get; set; }

        /// <summary>
        /// Gets or sets PrimerNombre
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 3)]
        public string PrimerNombre { get; set; }

        /// <summary>
        /// Gets or sets SegundoNombre
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = false, EmitDefaultValue = true, Order = 4)]
        public string SegundoNombre { get; set; }

        /// <summary>
        /// Gets or sets PrimerApellido
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 5)]
        public string PrimerApellido { get; set; }

        /// <summary>
        /// Gets or sets SegundoApellido
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = false, EmitDefaultValue = true, Order = 6)]
        public string SegundoApellido { get; set; }

        /// <summary>
        /// Gets or sets FechaExpedicion
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = false, EmitDefaultValue = true, Order = 7)]
        public DateTime? FechaExpedicion { get; set; }

        /// <summary>
        /// Gets or sets CiudadDomicilio
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 8)]
        public string CiudadDomicilio { get; set; }

        /// <summary>
        /// Gets or sets Direccion
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = false, EmitDefaultValue = true, Order = 9)]
        public string Direccion { get; set; }

        /// <summary>
        /// Gets or sets Telefono
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 10)]
        public long Telefono { get; set; }

        /// <summary>
        /// Gets or sets Celular
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = false, EmitDefaultValue = false, Order = 11)]
        public long? Celular { get; set; }

        /// <summary>
        /// Valida que todos los campos obligatorias no tengan su valor por omisión
        /// </summary>
        /// <returns><c>true</c> si todos los campos obligatorios existen, <c>false</c> en caso contrario</returns>
        public bool IsValidRequest()
        {
            bool ret = true;

            ret = !string.IsNullOrEmpty(this.NumeroIdentificacion);
            ret = !string.IsNullOrEmpty(this.PrimerNombre);
            ret = !string.IsNullOrEmpty(this.PrimerApellido);
            ret = !string.IsNullOrEmpty(this.CiudadDomicilio);

            return ret;
        }
    }
}
