using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Movilway.API.Service.ExtendedApi.DataContract.Payment
{
    public class ActualizarTransaccionRequest :  ASecuredApiRequest
    {
        /// <summary>
        /// Gets or sets Referencia
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 4)]
        public int CodigoTransaccion { get; set; }

        /// <summary>
        /// Gets or sets Referencia
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 5)]
        public int Estado { get; set; }


        /// <summary>
        /// Gets or sets Referencia
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 6)]
        public string Mensaje { get; set; }


        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 4)]
        public string CodigoTransaccionExterno { get; set; }

    }
}