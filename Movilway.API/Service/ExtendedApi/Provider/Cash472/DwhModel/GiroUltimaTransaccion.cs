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
    internal class GiroUltimaTransaccion
    {
        /// <summary>
        /// Gets or sets Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets EmisorId
        /// </summary>
        public int EmisorId { get; set; }

        /// <summary>
        /// Gets or sets ReceptorId
        /// </summary>
        public int ReceptorId { get; set; }
        
        /// <summary>
        /// Gets or sets EstadoId
        /// </summary>
        public int  EstadoId { get; set; }

        /// <summary>
        /// Gets or sets Pdv
        /// </summary>
        public string Pdv { get; set; }

        /// <summary>
        /// Gets or sets Fecha
        /// </summary>
        public DateTimeOffset Fecha { get; set; }

        /// <summary>
        /// Gets or sets FechaUltimaTransaccion
        /// </summary>
        public DateTimeOffset FechaUltimaTransaccion { get; set; }

        /// <summary>
        /// Gets or sets TotalRecibido
        /// </summary>
        public decimal TotalRecibido { get; set; }

        /// <summary>
        /// Gets or sets TotalAEntregar
        /// </summary>
        public decimal? TotalAEntregar { get; set; }

        /// <summary>
        /// Gets or sets Flete
        /// </summary>
        public decimal? Flete { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IncluyeFlete
        /// </summary>
        public bool IncluyeFlete { get; set; }
                
        /// <summary>
        /// Gets or sets CiudadOrigenDANE
        /// </summary>
        public string CiudadOrigenDANE { get; set; }

        /// <summary>
        /// Gets or sets DepartamentoOrigen
        /// </summary>
        public string DepartamentoOrigen { get; set; }

        /// <summary>
        /// Gets or sets CiudadOrigen
        /// </summary>
        public string CiudadOrigen { get; set; }

        /// <summary>
        /// Gets or sets CiudadDestinoDANE
        /// </summary>
        public string CiudadDestinoDANE { get; set; }

        /// <summary>
        /// Gets or sets DepartamentoDestino
        /// </summary>
        public string DepartamentoDestino { get; set; }

        /// <summary>
        /// Gets or sets CiudadDestino
        /// </summary>
        public string CiudadDestino { get; set; }
        
        /// <summary>
        /// Gets or sets Token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets Pin
        /// </summary>
        public string Pin { get; set; }
                
        /// <summary>
        /// Gets or sets NumeroFactura asignado por el sistema
        /// </summary>
        public string NumeroFactura { get; set; }

        // <summary>
        /// Gets or sets NumeroFactura asignado por 472
        /// </summary>
        public string NumeroFactura472 { get; set; }
               
        /// <summary>
        /// Gets or sets ExternalId es el identificador unico que le asigna 472 al Giro
        /// </summary>
        public int ExternalId { get; set; }

        /// <summary>
        /// Gets or sets NumeroComprobantePago472 
        /// </summary>
        public string NumeroComprobantePago472 { get; set; }

        /// <summary>
        /// Gets or sets Id
        /// </summary>
        public int DetalleId { get; set; }

        /// <summary>
        /// Gets or sets TipoTransaccionId
        /// </summary>
        public int TipoTransaccionId { get; set; }

        /// <summary>
        /// Gets or sets CodigoTransaccion
        /// </summary>
        public string  CodigoTransaccion { get; set; }

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
        public DateTimeOffset? FechaEnvio472 { get; set; }

        /// <summary>
        /// Gets or sets FechaRespuesta472
        /// </summary>
        public DateTimeOffset? FechaRespuesta472 { get; set; }

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
