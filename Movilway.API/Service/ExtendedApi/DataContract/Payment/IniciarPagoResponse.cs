using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Xml.Serialization;

namespace Movilway.API.Service.ExtendedApi.DataContract.Payment
{
    public class IniciarPagoResponse : AGenericApiResponse
    {

        public IniciarPagoResponse() { }

        /// <summary>
        /// Gets or sets Banks
        /// </summary>
        [Loggable]
        [DataMember(Order = 3), XmlElement]
        public string  Url{ get; set; }
        /// <summary>
        /// Gets or sets Banks
        /// </summary>
        [Loggable]
        [DataMember(Order = 4), XmlElement]
        public string NumeroTransaccion { get; set; }

        [Loggable]
        [DataMember(Order = 5), XmlElement]
        public string Message { get; set; }
    }
}