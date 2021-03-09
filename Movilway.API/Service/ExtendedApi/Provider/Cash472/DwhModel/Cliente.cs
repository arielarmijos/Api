// <copyright file="Cliente.cs" company="Movilway">
//     Copyright (c) Movilway. All rights reserved.
// </copyright>
namespace Movilway.API.Service.ExtendedApi.Provider.Cash472.DwhModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Representación del modelo de base de datos de un cliente
    /// </summary>
    internal class Cliente
    {
        /// <summary>
        /// Gets or sets Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets TipoIdentificacionId
        /// </summary>
        public int TipoIdentificacionId { get; set; }

        /// <summary>
        /// Gets or sets NumeroIdentificacion
        /// </summary>
        public string NumeroIdentificacion { get; set; }

        /// <summary>
        /// Gets or sets FechaExpedicion
        /// </summary>
        public DateTime? FechaExpedicion { get; set; }

        /// <summary>
        /// Gets or sets RazonSocial
        /// </summary>
        public string RazonSocial { get; set; }

        /// <summary>
        /// Gets or sets PrimerNombre
        /// </summary>
        public string PrimerNombre { get; set; }

        /// <summary>
        /// Gets or sets SegundoNombre
        /// </summary>
        public string SegundoNombre { get; set; }

        /// <summary>
        /// Gets or sets PrimerApellido
        /// </summary>
        public string PrimerApellido { get; set; }

        /// <summary>
        /// Gets or sets SegundoApellido
        /// </summary>
        public string SegundoApellido { get; set; }

        /// <summary>
        /// Gets or sets Ciudad
        /// </summary>
        public string Ciudad { get; set; }

        /// <summary>
        /// Gets or sets Direccion
        /// </summary>
        public string Direccion { get; set; }

        /// <summary>
        /// Gets or sets Telefono
        /// </summary>
        public long? Telefono { get; set; }

        /// <summary>
        /// Gets or sets Celular
        /// </summary>
        public long? Celular { get; set; }

        /// <summary>
        /// Gets or sets ExternalId
        /// </summary>
        public int? ExternalId { get; set; }
    }
}
