// <copyright file="Factura.cs" company="Movilway">
//     Copyright (c) Movilway. All rights reserved.
// </copyright>
namespace Movilway.API.Service.ExtendedApi.DataContract.Cash472
{
    using System;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    /// <summary>
    /// Modelo representación de una factura
    /// </summary>
    public class Factura
    {
        /// <summary>
        /// Gets or sets Id
        /// </summary>
        [DataMember(Order = 3)]
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets ExternalId
        /// </summary>
        [DataMember(Order = 4)]
        public long ExternalId { get; set; }

        /// <summary>
        /// Gets or sets Pin
        /// </summary>
        [DataMember(Order = 5)]
        public string Pin { get; set; }

        /// <summary>
        /// Gets or sets NumeroFactura
        /// </summary>
        [DataMember(Order = 6)]
        public string NumeroFactura { get; set; }

        /// <summary>
        /// Gets or sets DepartamentoOrigen
        /// </summary>
        [DataMember(Order = 7)]
        public string DepartamentoOrigen { get; set; }

        /// <summary>
        /// Gets or sets CiudadOrigen
        /// </summary>
        [DataMember(Order = 8)]
        public string CiudadOrigen { get; set; }

        /// <summary>
        /// Gets or sets AgenciaNombre
        /// </summary>
        [DataMember(Order = 9)]
        public string AgenciaNombre { get; set; }

        /// <summary>
        /// Gets or sets AgenciaDireccion
        /// </summary>
        [DataMember(Order = 10)]
        public string AgenciaDireccion { get; set; }

        /// <summary>
        /// Gets or sets AccesoTipo
        /// </summary>
        [DataMember(Order = 11)]
        public int AccesoTipo { get; set; }

        /// <summary>
        /// Gets or sets Acceso
        /// </summary>
        [DataMember(Order = 12)]
        public string Acceso { get; set; }

        /// <summary>
        /// Gets or sets TotalRecibido
        /// </summary>
        [DataMember(Order = 13)]
        public decimal TotalRecibido { get; set; }

        /// <summary>
        /// Gets or sets TotalAEntregar
        /// </summary>
        [DataMember(Order = 14)]
        public decimal TotalAEntregar { get; set; }

        /// <summary>
        /// Gets or sets Flete
        /// </summary>
        [DataMember(Order = 15)]
        public decimal Flete { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IncluyeFlete
        /// </summary>
        [DataMember(Order = 16)]
        public bool IncluyeFlete { get; set; }

        /// <summary>
        /// Gets or sets Fecha
        /// </summary>
        [DataMember(Order = 17)]
        public DateTime Fecha { get; set; }

        /// <summary>
        /// Gets or sets Fecha
        /// </summary>
        [IgnoreDataMember]
        public DateTimeOffset FechaOffset
        {
            set
            {
                if (value != null)
                {
                    this.Fecha = value.DateTime;
                }
            }
        }

        /// <summary>
        /// Gets or sets FacturaResolucion
        /// </summary>
        [DataMember(Order = 18)]
        public string FacturaResolucion { get; set; }

        /// <summary>
        /// Gets or sets FacturaPrefijo
        /// </summary>
        [DataMember(Order = 19)]
        public string FacturaPrefijo { get; set; }

        /// <summary>
        /// Gets or sets FacturaFecha
        /// </summary>
        [DataMember(Order = 20)]
        public DateTime FacturaFecha { get; set; }

        /// <summary>
        /// Gets or sets FacturaFecha
        /// </summary>
        [DataMember(Order = 21)]
        public int FacturaDesde { get; set; }

        /// <summary>
        /// Gets or sets FacturaHasta
        /// </summary>
        [DataMember(Order = 22)]
        public int FacturaHasta { get; set; }

        /// <summary>
        /// Gets or sets OrigenNombre
        /// </summary>
        [DataMember(Order = 23)]
        public string OrigenNombre { get; set; }

        /// <summary>
        /// Gets or sets OrigenDni
        /// </summary>
        [DataMember(Order = 24)]
        public string OrigenDni { get; set; }

        /// <summary>
        /// Gets or sets OrigenTel
        /// </summary>
        [DataMember(Order = 25)]
        public string OrigenTel { get; set; }

        /// <summary>
        /// Gets or sets DestinoNombre
        /// </summary>
        [DataMember(Order = 26)]
        public string DestinoNombre { get; set; }

        /// <summary>
        /// Gets or sets DestinoDni
        /// </summary>
        [DataMember(Order = 27)]
        public string DestinoDni { get; set; }

        /// <summary>
        /// Gets or sets DestinoTel
        /// </summary>
        [DataMember(Order = 28)]
        public string DestinoTel { get; set; }
    }
}
