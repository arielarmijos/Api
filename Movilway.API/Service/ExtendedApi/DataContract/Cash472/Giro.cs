// <copyright file="Giro.cs" company="Movilway">
//     Copyright (c) Movilway. All rights reserved.
// </copyright>
namespace Movilway.API.Service.ExtendedApi.DataContract.Cash472
{
    using System;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    using Movilway.API.Service.ExtendedApi.DataContract.Common;
    using Movilway.Logging.Attribute;

    /// <summary>
    /// Modelo representación de un giro
    /// </summary>
    public class Giro
    {
        /// <summary>
        /// Gets or sets Emisor
        /// </summary>
        [DataMember(Order = 3), XmlElement]
        public Cash472.Cliente Emisor { get; set; }

        /// <summary>
        /// Gets or sets Receptor
        /// </summary>
        [DataMember(Order = 4), XmlElement]
        public Cash472.Cliente Receptor { get; set; }

        /// <summary>
        /// Gets or sets Id
        /// </summary>
        [DataMember(Order = 5), XmlElement]
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets TotalRecibido
        /// </summary>
        [DataMember(Order = 6), XmlElement]
        public long TotalRecibido { get; set; }

        /// <summary>
        /// Gets or sets TotalAEntregar
        /// </summary>
        [DataMember(Order = 7), XmlElement]
        public long TotalAEntregar { get; set; }

        /// <summary>
        /// Gets or sets Flete
        /// </summary>
        [DataMember(Order = 8), XmlElement]
        public long Flete { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IncluyeFlete
        /// </summary>
        [DataMember(Order = 9), XmlElement]
        public bool IncluyeFlete { get; set; }

        /// <summary>
        /// Gets or sets Fecha
        /// </summary>
        [DataMember(Order = 10), XmlElement]
        public DateTime Fecha { get; set; }

        /// <summary>
        /// Gets or sets CodigoTransaccion
        /// </summary>
        [DataMember(Order = 11), XmlElement]
        public string CodigoTransaccion { get; set; }
    }
}
