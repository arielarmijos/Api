// <copyright file="ConsultaClienteResponse.cs" company="Movilway">
//     Copyright (c) Movilway. All rights reserved.
// </copyright>
namespace Movilway.API.Service.ExtendedApi.DataContract.Cash472
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Web;
    using System.Xml.Serialization;

    using Movilway.API.Service.ExtendedApi.DataContract.Common;
    using Movilway.Logging.Attribute;

    /// <summary>
    /// Clase que determina la estructura de retorno ante la peticion de 
    /// consulta de un cliente
    /// </summary>
    public class ConsultaCiudadesResponse : AGenericApiResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsultaClienteResponse" /> class.
        /// </summary>
        public ConsultaCiudadesResponse()
            : base()
        {
            this.ResponseMessage = string.Empty;
        }

        /// <summary>
        /// Cantidad de giros que contiene el elemento <c>Ciudades</c>
        /// </summary>
        [Loggable]
        [DataMember(Order = 3), XmlElement]
        public int Quantity { set; get; }

        /// <summary>
        /// Elemento que contiene los ciudades
        /// </summary>
        [DataMember(Order = 4), XmlArray("Ciudades"), XmlArrayItem("Ciudad")]
        public List<DataContract.Cash472.Ciudad> Ciudades { set; get; }

        /// <summary>
        /// Personalización ToString para logs
        /// </summary>
        /// <returns><c>string</c> personalizado</returns>
        public override string ToString()
        {
            return Movilway.API.Service.ExtendedApi.DataContract.Utils.logFormat(this);
        }
    }
}
