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
    public class CrearTransaccionResponse : AGenericApiResponse
    {
        /// <summary>
        /// Codigo de transaccion genereado de 
        /// </summary>
        //[Loggable]
        //[DataMember(Order = 3), XmlElement]
        //public int TransactionCode { get; set; }

    }
}