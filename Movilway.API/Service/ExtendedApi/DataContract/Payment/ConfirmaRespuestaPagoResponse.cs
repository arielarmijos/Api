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
    public class ConfirmaRespuestaPagoResponse : AGenericApiResponse
    {
        //[Loggable]
        [DataMember(Order = 3), XmlElement]
        public string respuesta;

        //[Loggable]
        [DataMember(Order = 4), XmlElement]
        public string errorsms;

        //[Loggable]
        [DataMember(Order = 5), XmlElement]
        public string int_estado_pago;

        //[Loggable]
        [DataMember(Order = 6), XmlElement]
        public string str_nombre_banco;

        //[Loggable]
        [DataMember(Order = 7), XmlElement]
        public string str_codigo_transaccion;

        //[Loggable]
        [DataMember(Order = 8), XmlElement]
        public string int_ciclo_transaccion;

        //[Loggable]
        [DataMember(Order = 9), XmlElement]
        public string dat_fecha;

        //[Loggable]
        [DataMember(Order = 10), XmlElement]
        public string fechatransaccion;

        //[Loggable]
        [DataMember(Order = 11), XmlElement]
        public string horatransaccion;

        //[Loggable]
        [DataMember(Order = 12), XmlElement]
        public string descripcion_pago;

        //[Loggable]
        [DataMember(Order = 13), XmlElement]
        public string email;

        //[Loggable]
        [DataMember(Order = 14), XmlElement]
        public string nombre_cliente;

        //[Loggable]
        [DataMember(Order = 15), XmlElement]
        public string apellido_cliente;

        //[Loggable]
        [DataMember(Order = 16), XmlElement]
        public string telefono_cliente;

        //[Loggable]
        [DataMember(Order = 17), XmlElement]
        public string direccion;

        //[Loggable]
        [DataMember(Order = 18), XmlElement]
        public string pais;

        //[Loggable]
        [DataMember(Order = 19), XmlElement]
        public string ciudad;

        //[Loggable]
        [DataMember(Order = 20), XmlElement]
        public string info_opcional1;

        //[Loggable]
        [DataMember(Order = 21), XmlElement]
        public string info_opcional2;

        //[Loggable]
        [DataMember(Order = 22), XmlElement]
        public string info_opcional3;

        //[Loggable]
        [DataMember(Order = 23), XmlElement]
        public string firma;

        //[Loggable]
        [DataMember(Order = 24), XmlElement]
        public string ip;

        //[Loggable]
        [DataMember(Order = 25), XmlElement]
        public string total_con_iva;

        //[Loggable]
        [DataMember(Order = 26), XmlElement]
        public string valor_iva;

        public ConfirmaRespuestaPagoResponse() { }
    }
}