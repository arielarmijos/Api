// <copyright file="Cliente.cs" company="Movilway">
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
    /// Modelo representación de una ciudad
    /// </summary>
    public class Ciudad
    {
        /// <summary>
        /// Gets or sets CodigoDANE
        /// </summary>
        [DataMember(Order = 3), XmlElement]
        public string CodigoDANE { get; set; }

        /// <summary>
        /// Gets or sets Nombre
        /// </summary>
        [DataMember(Order = 4), XmlElement]
        public string Nombre { get; set; }

        /// <summary>
        /// Gets or sets CodPostal
        /// </summary>
        [DataMember(Order = 5), XmlElement]
        public string CodPostal { get; set; }

        
    }
}
