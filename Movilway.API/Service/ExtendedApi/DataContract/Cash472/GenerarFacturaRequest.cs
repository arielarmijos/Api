// <copyright file="GenerarFacturaRequest.cs" company="Movilway">
//     Copyright (c) Movilway. All rights reserved.
// </copyright>
namespace Movilway.API.Service.ExtendedApi.DataContract.Cash472
{
    using System.Runtime.Serialization;

    using Movilway.API.Service.ExtendedApi.DataContract.Common;
    using Movilway.Logging.Attribute;

    /// <summary>
    /// Clase que determina la estructura para una petición de 
    /// generación de factura
    /// </summary>
    public class GenerarFacturaRequest : ASecuredApiRequest
    {
        /// <summary>
        /// Gets or sets TipoPos
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 3)]
        public TipoPos TipoPos { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the response must include all the data
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = true, Order = 4)]
        public bool IncludeData { get; set; }

        /// <summary>
        /// Gets or sets Id
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = false, EmitDefaultValue = true, Order = 5)]
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets ExternalId
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = false, EmitDefaultValue = true, Order = 6)]
        public long ExternalId { get; set; }

        /// <summary>
        /// Valida que todos los campos obligatorias no tengan su valor por omisión
        /// </summary>
        /// <returns><c>true</c> si todos los campos obligatorios existen, <c>false</c> en caso contrario</returns>
        public bool IsValidRequest()
        {
            bool ret = false;

            ret = this.Id != 0 || this.ExternalId != 0;

            return ret;
        }
    }
}
