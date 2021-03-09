using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.ServiceModel;
using Movilway.Logging.Attribute;

namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [DataContract(Namespace = "http://api.movilway.net/schema/extended")]
    public class TipoAcceso
    {
        [Loggable]
        [DataMember(EmitDefaultValue = false, Order = 0)]
        public Int32 ID { get; set; }
        [Loggable]
        [DataMember(EmitDefaultValue = false, Order = 1)]
        public String DESCRIPCION { get; set; }
        [Loggable]
        [DataMember(EmitDefaultValue = false, Order = 2)]
        public int USR_MINIMO { get; set; }
        [Loggable]
        [DataMember(EmitDefaultValue = false, Order = 3)]
        public int USR_MAXIMO { get; set; }
        [Loggable]
        [DataMember(EmitDefaultValue = false, Order = 4)]
        public int PERIODOVALIDEZ { get; set; }
        [Loggable]
        [DataMember(EmitDefaultValue = false, Order = 5)]
        public char CHEQUEARVALIDEZ { get; set; }
        [Loggable]
        [DataMember(EmitDefaultValue = false, Order = 6)]
        public int MAXREINTENTO { get; set; }
        [Loggable]
        [DataMember(EmitDefaultValue = false, Order = 7)]
        public int PWD_MINIMO { get; set; }
        [Loggable]
        [DataMember(EmitDefaultValue = false, Order = 8)]
        public int PWD_MAXIMO { get; set; }
        [Loggable]
        [DataMember(EmitDefaultValue = false, Order = 9)]
        public int DDA_ID_USUARIO { get; set; }
        [Loggable]
        [DataMember(EmitDefaultValue = false, Order = 10)]
        public int DDA_ID_PASSWORD { get; set; }
        [Loggable]
        [DataMember(EmitDefaultValue = false, Order = 11)]
        public int VALORESDUPLICADOS { get; set; }

    }
}