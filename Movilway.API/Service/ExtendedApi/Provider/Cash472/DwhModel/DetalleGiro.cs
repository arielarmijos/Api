// <copyright file="Giro.cs" company="Movilway">
//     Copyright (c) Movilway. All rights reserved.
// </copyright>
namespace Movilway.API.Service.ExtendedApi.Provider.Cash472.DwhModel
{
    using Movilway.API.Service.ExtendedApi.DataContract.Cash472;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Representación del modelo de base de datos de un giro
    /// </summary>
    internal class DetalleGiro
    {
        /// <summary>
        /// Gets or sets Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets TipoTransaccionId
        /// </summary>
        public int TipoTransaccionId { get; set; }

        /// <summary>
        /// Gets or sets CodigoTransaccion
        /// </summary>
        public string CodigoTransaccion { get; set; }

        /// <summary>
        /// Gets or sets CodigoRespuesta472
        /// </summary>
        public string CodigoRespuesta472 { get; set; }

        /// <summary>
        /// Gets or sets CodigoAutorizacion472
        /// </summary>
        public string CodigoAutorizacion472 { get; set; }

        /// <summary>
        /// Gets or sets NumeroReferencia472
        /// </summary>
        public string NumeroReferencia472 { get; set; }

        /// <summary>
        /// Gets or sets FechaEnvio472
        /// </summary>
        public DateTimeOffset FechaEnvio472 { get; set; }

        /// <summary>
        /// Gets or sets FechaRespuesta472
        /// </summary>
        public DateTimeOffset FechaRespuesta472 { get; set; }

        /// <summary>
        /// Gets or sets ErrorApi
        /// </summary>
        public string ErrorApi { get; set; }

        /// <summary>
        /// Gets or sets ErrorApiDescripcion
        /// </summary>
        public string ErrorApiDescripcion { get; set; }

        /// <summary>
        /// Gets or sets ErrorProtocolo
        /// </summary>
        public string ErrorProtocolo { get; set; }

        /// <summary>
        /// Gets or sets ErrorProtocoloDescripcion
        /// </summary>
        public string ErrorProtocoloDescripcion { get; set; }

        /// <summary>
        /// Gets or sets DetalleRespuesta
        /// </summary>
        public string DetalleRespuesta { get; set; }
                              
    }
}
