using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Movilway.API.Service.ExtendedApi.DataContract.Payment
{
    public class ConfirmaRespuestaPagoRequest : ASecuredApiRequest
    {
        /// <summary>
        /// Gets or sets Referencia
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 3)]
        public string NumeroTransaccion { get; set; }

        public ConfirmaRespuestaPagoRequest() { }
    }
}