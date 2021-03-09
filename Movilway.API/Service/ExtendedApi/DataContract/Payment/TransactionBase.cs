using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Movilway.API.Service.ExtendedApi.DataContract.Payment
{
    public class TransactionBase : ASecuredApiRequest
    {
        /// <summary>
        /// Gets or sets Referencia
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 3)]
        public string Referencia { get; set; }

        /// <summary>
        /// Gets or sets TotalConIva
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 4)]
        public string TotalConIva { get; set; }

        /// <summary>
        /// Gets or sets ValorIva
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 5)]
        public string ValorIva { get; set; }

        /// <summary>
        /// Gets or sets DescripcionPago
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 6)]
        public string DescripcionPago { get; set; }

        /// <summary>
        /// Gets or sets Email
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 7)]
        public string Email { get; set; }
        /// <summary>
        /// Gets or sets IdCliente
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 8)]
        public string IdCliente { get; set; }
        /// <summary>
        /// Gets or sets NombreCliente
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 9)]
        public string NombreCliente { get; set; }
        /// <summary>
        /// Gets or sets ApellidoCliente
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 10)]
        public string ApellidoCliente { get; set; }
        /// <summary>
        /// Gets or sets TelefonoCliente
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 11)]
        public string TelefonoCliente { get; set; }
        /// <summary>
        /// Gets or sets Direccion
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 12)]
        public string Direccion { get; set; }
        /// <summary>
        /// Gets or sets Pais
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 13)]
        public string Pais { get; set; }
        /// <summary>
        /// Gets or sets Ciudad
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 14)]
        public string Ciudad { get; set; }
        /// <summary>
        /// Gets or sets Ip
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 15)]
        public string Ip { get; set; }
        /// <summary>
        /// Gets or sets Opcional1
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 16)]
        public string Opcional1 { get; set; }
        /// <summary>
        /// Gets or sets Opcional2
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 17)]
        public string Opcional2 { get; set; }
        /// <summary>
        /// Gets or sets Opcional3
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 18)]
        public string Opcional3 { get; set; }
        /// <summary>
        /// Gets or sets UrlRetorno
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 19)]
        public string UrlRetorno { get; set; }
        /// <summary>
        /// Gets or sets CodigoDelBanco
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 20)]
        public string CodigoDelBanco { get; set; }
        /// <summary>
        /// Gets or sets TipoDeUsuario
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 21)]
        public string TipoDeUsuario { get; set; }
    }
}