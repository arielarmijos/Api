// <copyright file="InfoDeposito.cs" company="Movilway">
//     Copyright (c) Movilway. All rights reserved.
// </copyright>
namespace Movilway.API.Utils.Models.Kinacu
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Representación de un depósito
    /// </summary>
    internal class InfoDeposito
    {
        /// <summary>
        /// Gets or sets Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets IdAgenciaOrigen
        /// </summary>
        public int IdAgenciaOrigen { get; set; }

        /// <summary>
        /// Gets or sets IdAgenciaPadre
        /// </summary>
        public int IdAgenciaPadre { get; set; }

        /// <summary>
        /// Gets or sets IdAgenciaAbuelo
        /// </summary>
        public int IdAgenciaAbuelo { get; set; }

        /// <summary>
        /// Gets or sets NombreAgenciaOrigen
        /// </summary>
        public string NombreAgenciaOrigen { get; set; }

        /// <summary>
        /// Gets or sets NombreAgenciaPadre
        /// </summary>
        public string NombreAgenciaPadre { get; set; }

        /// <summary>
        /// Gets or sets NombreAgenciaAbuelo
        /// </summary>
        public string NombreAgenciaAbuelo { get; set; }

        /// <summary>
        /// Gets or sets IdCub
        /// </summary>
        public int IdCub { get; set; }

        /// <summary>
        /// Gets or sets IdCubPadre
        /// </summary>
        public int IdCubPadre { get; set; }

        /// <summary>
        /// Gets or sets IdCubAbuelo
        /// </summary>
        public int IdCubAbuelo { get; set; }

        /// <summary>
        /// Gets or sets CubPadreNumero
        /// </summary>
        public string CubPadreNumero { get; set; }

        /// <summary>
        /// Gets or sets Monto
        /// </summary>
        public decimal Monto { get; set; }

        /// <summary>
        /// Gets or sets IdUsuario
        /// </summary>
        public int IdUsuario { get; set; }

        /// <summary>
        /// Gets or sets IdUsuarioPadre
        /// </summary>
        public int IdUsuarioPadre { get; set; }

        /// <summary>
        /// Gets or sets IdUsuarioAbuelo
        /// </summary>
        public int IdUsuarioAbuelo { get; set; }

        /// <summary>
        /// Gets or sets FechaDeposito
        /// </summary>
        public DateTime FechaDeposito { get; set; }

        /// <summary>
        /// Gets or sets FechaComprobante
        /// </summary>
        public DateTime FechaComprobante { get; set; }

        /// <summary>
        /// Gets or sets Estado
        /// </summary>
        public string Estado { get; set; }

        /// <summary>
        /// Gets or sets Comprobante
        /// </summary>
        public string Comprobante { get; set; }

        /// <summary>
        /// Gets or sets Comentario
        /// </summary>
        public string Comentario { get; set; }

        /// <summary>
        /// Gets or sets ComentarioCompleto
        /// </summary>
        public string ComentarioCompleto { get; set; }
    }
}
