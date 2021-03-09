// <copyright file="Pago.cs" company="Movilway">
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
    /// Representación del modelo de base de datos de un pago
    /// </summary>
    internal class Pago
    {
        /// <summary>
        /// Gets or sets Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets GiroId
        /// </summary>
        public int GiroId { get; set; }

        /// <summary>
        /// Gets or sets ExternalId
        /// </summary>
        public int ExternalId { get; set; }

        /// <summary>
        /// Gets or sets EmisorId
        /// </summary>
        public int EmisorId { get; set; }

        /// <summary>
        /// Gets or sets ReceptorId
        /// </summary>
        public int ReceptorId { get; set; }

        /// <summary>
        /// Gets or sets Pdv
        /// </summary>
        public string Pdv { get; set; }

        /// <summary>
        /// Gets or sets CiudadPdv
        /// </summary>
        public string CiudadPdv { get; set; }

        /// <summary>
        /// Gets or sets Fecha
        /// </summary>
        public DateTimeOffset Fecha { get; set; }

        /// <summary>
        /// Gets or sets TotalRecibido
        /// </summary>
        public decimal TotalRecibido { get; set; }

        /// <summary>
        /// Gets or sets TotalAEntregar
        /// </summary>
        public decimal TotalAEntregar { get; set; }

        /// <summary>
        /// Gets or sets Flete
        /// </summary>
        public decimal Flete { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IncluyeFlete
        /// </summary>
        public bool IncluyeFlete { get; set; }

        /// <summary>
        /// Gets or sets ValorPago
        /// </summary>
        public decimal ValorPago { get; set; }

        /// <summary>
        /// Gets or sets CodigoTransaccion
        /// </summary>
        public string CodigoTransaccion { get; set; }

        /// <summary>
        /// Gets or sets ValorPagoRespuesta
        /// </summary>
        public string ValorPagoRespuesta { get; set; }

        /// <summary>
        /// Gets or sets CodigoAutorizacion
        /// </summary>
        public string CodigoAutorizacion { get; set; }

        /// <summary>
        /// Gets or sets NumeroFactura
        /// </summary>
        public string NumeroFactura { get; set; }

        /// <summary>
        /// Gets or sets NumeroReferencia
        /// </summary>
        public string NumeroReferencia { get; set; }

        /// <summary>
        /// Gets or sets NumeroComprobantePago
        /// </summary>
        public string NumeroComprobantePago { get; set; }

        /// <summary>
        /// Gets or sets FechaPago
        /// </summary>
        public DateTime? FechaPago { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Anulado
        /// </summary>
        public bool Anulado { get; set; }

        /// <summary>
        /// Gets or sets FechaAnulado
        /// </summary>
        public DateTimeOffset? FechaAnulado { get; set; }

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
        /// Gets or sets FechaErrorProtocolo
        /// </summary>
        public DateTimeOffset? FechaErrorProtocolo { get; set; }
    }
}
