// <copyright file="NotificacionOficialCumplimientoRequest.cs" company="Movilway">
//     Copyright (c) Movilway. All rights reserved.
// </copyright>
namespace Movilway.API.Service.ExtendedApi.DataContract.Cash472
{
    using System.Runtime.Serialization;

    using Movilway.API.Service.ExtendedApi.DataContract.Common;
    using Movilway.Logging.Attribute;

    /// <summary>
    /// Clase que determina la estructura para la petición de consulta
    /// </summary>
    public class NotificacionOficialCumplimientoRequest : ASecuredApiRequest
    {
        /// <summary>
        /// Gets or sets TipoCliente
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 3)]
        public Cash472.TipoCliente TipoCliente { get; set; }

        /// <summary>
        /// Gets or sets TipoIdentificacion
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 4)]
        public TipoIdentificacion TipoIdentificacion { get; set; }

        /// <summary>
        /// Gets or sets NumeroIdentificacion
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 5)]
        public string NumeroIdentificacion { get; set; }

        /// <summary>
        /// Gets or sets GiroId
        /// </summary>
        [DataMember(IsRequired = false, EmitDefaultValue = true, Order = 6)]
        public long GiroId { get; set; }

        /// <summary>
        /// Valida que todos los campos obligatorias no tengan su valor por omisión
        /// </summary>
        /// <returns><c>true</c> si todos los campos obligatorios existen, <c>false</c> en caso contrario</returns>
        public bool IsValidRequest()
        {
            bool ret = true;

            ret = !string.IsNullOrEmpty(this.NumeroIdentificacion);

            return ret;
        }
    }
}
