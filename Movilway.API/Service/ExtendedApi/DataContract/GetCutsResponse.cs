using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging.Attribute;
using System.Text;

namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [MessageContract(IsWrapped = false)]
    public class GetCutsResponse : IMovilwayApiResponseWrapper<GetCutsResponseBody>
    {
        [MessageBodyMember(Name = "GetCutsResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public GetCutsResponseBody Response { get; set; }
    }

    [Loggable]
    [DataContract(Name = "GetCutsResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetCutsResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AGenericApiResponse
    {
        [Loggable]
        [DataMember(Order = 3)]
        public CutsList CutsList { get; set; }

        [Loggable]
        [DataMember(Order = 4)]
        public CutsFooter CutsFooter { get; set; }

        public GetCutsResponseBody()
        {
            CutsList = new CutsList();
            CutsFooter = new CutsFooter();
        }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }

    [CollectionDataContract(Namespace = "http://api.movilway.net/schema/extended", ItemName = "CutsList")]
    public class CutsList : List<CutInfo>
    {

        public override string ToString()
        {
            return string.Concat(this.GetType().Name, " Count ", this.Count);
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema/extended")]
    public class CutInfo
    {

        [Loggable]
        [DataMember(Order = 0)]
        //Código de transacción de corte manual
        public string TransactionCode
        {
            get;
            set;
        }

        [Loggable]
        [DataMember(Order = 1)]
       //Código de usuario (Quien realizó el corte)
        public string UserCode
        {
            get;
            set;
        }

        [Loggable]
        [DataMember(Order = 2)]
       //Nombre usuario
        public string UserName
        {
            get;
            set;
        }

        [Loggable]
        [DataMember(Order = 3)]
       //Código de Cliente (a quien se realizó la distribución)
        public string ClientCode
        {
            get;
            set;
        }
         [DataMember(Order = 4)]
       //Nombre del Cliente
        public string ClientName
        {
            get;
            set;
        }

        [Loggable]
        [DataMember(Order = 5)]
       //Fecha y Hora de transacción de corte 
        public DateTime CutDate
        {
            get;
            set;
        }

       [Loggable]
       [DataMember(Order = 6)]
       //Código de Producto
        public String ProductCode
        {
            get;
            set;
        }

       [Loggable]
       [DataMember(Order = 7)]
       //Descripción de Producto}
        public String ProductDescription
        {
            get;
            set;
        }

       [Loggable]
       [DataMember(Order = 8)]
       //Cantidad de Pines ( 1 PIN = 1 Dólar) – Total por operadora
        public decimal PinsAmount
        {
            get;
            set;
        }



       [Loggable]
       [DataMember(Order = 9)]
       //Unidad (Fracción)
        public string  UnityFraction
        {
            get;
            set;
        }

    }

    [DataContract(Namespace = "http://api.movilway.net/schema/extended")]
    public class CutsFooter
    {

        [DataMember(Order = 1, EmitDefaultValue = false)]
        public decimal TotalInvoices
        {
            get;
            set;
        }

        [DataMember(Order = 2, EmitDefaultValue = false)]
        public int NumberInvoices
        {
            get;
            set;
        }

        public override string ToString()
        {
            return string.Concat(this.GetType().Name, " TotalInvoices ", TotalInvoices);
        }


    }
}