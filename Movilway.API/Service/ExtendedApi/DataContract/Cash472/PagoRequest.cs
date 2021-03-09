// <copyright file="PagoRequest.cs" company="Movilway">
//     Copyright (c) Movilway. All rights reserved.
// </copyright>
namespace Movilway.API.Service.ExtendedApi.DataContract.Cash472
{
    using System.Runtime.Serialization;

    using Movilway.API.Service.ExtendedApi.DataContract.Common;
    using Movilway.Logging.Attribute;

    /// <summary>
    /// Clase que determina la estructura para la petición de un pago
    /// </summary>
    public class PagoRequest : ASecuredApiRequest
    {
        /// <summary>
        /// Gets or sets TipoIdentificacion
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 3)]
        public TipoIdentificacion TipoIdentificacion { get; set; }

        /// <summary>
        /// Gets or sets NumeroIdentificacion
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 4)]
        public string NumeroIdentificacion { get; set; }

        /// <summary>
        /// Gets or sets Id
        /// </summary>
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 5)]
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets Pdv
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 6)]
        public string Pdv { get; set; }

        /// <summary>
        /// Gets or sets CiudadPdv
        /// </summary>
        //[Loggable]
        //[DataMember(IsRequired = true, EmitDefaultValue = false, Order = 7)]
        internal long CiudadPdv { get; set; }


        /// <summary>
        /// Gets or sets Valor
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 8)]
        public long Valor { get; set; }

        /// <summary>
        /// Gets or sets Pin correspondiente al ExternalId
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 9)]
        public string Pin { get; set; }

        /// <summary>
        /// Gets or sets ValorRecibidoTotal
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 10)]
        public long ValorRecibidoTotal { get; set; }

        /// <summary>
        /// Gets or sets ValorFlete
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 11)]
        public long ValorFlete { get; set; }

        // <summary>
        /// Gets or sets IncluyeFlete
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 12)]
        public bool IncluyeFlete { get; set; }


        /// <summary>
        /// Valida que todos los campos obligatorias no tengan su valor por omisión
        /// </summary>
        /// <returns><c>true</c> si todos los campos obligatorios existen, <c>false</c> en caso contrario</returns>
        public bool IsValidRequest()
        {
            bool ret = true;

            ret = !string.IsNullOrEmpty(this.NumeroIdentificacion);
            ret = !string.IsNullOrEmpty(this.Pdv);
            //ret = !string.IsNullOrEmpty(this.CiudadPdv);
            ret = this.Id > 0;
            ret = this.Valor > 0;

            return ret;
        }
    }
}
