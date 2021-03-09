// <copyright file="ReversoPagoRequest.cs" company="Movilway">
//     Copyright (c) Movilway. All rights reserved.
// </copyright>
namespace Movilway.API.Service.ExtendedApi.DataContract.Cash472
{
    using System.Runtime.Serialization;

    using Movilway.API.Service.ExtendedApi.DataContract.Common;
    using Movilway.Logging.Attribute;

    /// <summary>
    /// Clase que determina la estructura para la petición para
    /// el reverso de un pago
    /// </summary>
    public class ReversoPagoRequest : ASecuredApiRequest
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
        /// Gets or sets CiudadOrigen
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 7)]
        public long CiudadPdv { get; set; }

        /// <summary>
        /// Gets or sets ValorRecibido
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 8)]
        public long Valor { get; set; }

        /// <summary>
        /// Gets or sets NumeroComprobantePago
        /// </summary>
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 9)]
        public string NumeroComprobantePago { get; set; }

        /// <summary>
        /// Valida que todos los campos obligatorias no tengan su valor por omisión
        /// </summary>
        /// <returns><c>true</c> si todos los campos obligatorios existen, <c>false</c> en caso contrario</returns>
        public bool IsValidRequest()
        {
            bool ret = true;

            ret = !string.IsNullOrEmpty(this.NumeroIdentificacion);
            ret = !string.IsNullOrEmpty(this.Pdv);
            ret = !string.IsNullOrEmpty(this.NumeroComprobantePago);
            ret = this.CiudadPdv != 0;
            ret = this.Id != 0;

            return ret;
        }
    }
}
