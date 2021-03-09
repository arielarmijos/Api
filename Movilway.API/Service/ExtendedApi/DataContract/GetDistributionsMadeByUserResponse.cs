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
    public class GetDistributionsMadeByUserResponse : IMovilwayApiResponseWrapper<GetDistributionsMadeByUserResponseBody>
    {
        [MessageBodyMember(Name = "GetDistributionsMadeByUserResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public GetDistributionsMadeByUserResponseBody Response { set; get; }

        public GetDistributionsMadeByUserResponse()
        {
            Response = new GetDistributionsMadeByUserResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "GetDistributionsMadeByUserResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetDistributionsMadeByUserResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AGenericApiResponse
    {
        [Loggable]
        [DataMember(Order = 3)]
        public DistributionMadeByUserList Distributions { set; get; }

        public GetDistributionsMadeByUserResponseBody()
        {
            Distributions = new DistributionMadeByUserList();
        }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }

        [Loggable]
    [CollectionDataContract(Namespace = "http://api.movilway.net/schema/extended", ItemName = "Distribution")]
    public class DistributionMadeByUserList : List<DistributionMadeByUser>
    {
        public override string ToString()
        {
            return string.Concat(this.GetType().Name, " Count ", this.Count);
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema/extended")]
    public class DistributionMadeByUser
    {
        [Loggable]
        [DataMember(Order = 0)]
        public int DistributionId { set; get; }

        //Código de usuario (Quien realizó la distribución -  es decir el user logueado)
        [Loggable]
        [DataMember(Order = 1)]
        public string UserCode { get; set; }

        //Nombre usuario
        [Loggable]
        [DataMember(Order = 2)]
        public string UserName { get; set; }

        //Código de Cliente (a quien se realizó la distribución)
        [Loggable]
        [DataMember(Order = 3)]
        public string ClientCode { get; set; }

        //Nombre del Cliente
        [Loggable]
        [DataMember(Order = 4)]
        public string ClientName { get; set; }

        //Valor de saldo distribuido
        [Loggable]
        [DataMember(Order = 5)]
        public decimal DistributionValue { get; set; }

        //Fecha y Hora de distribución realizada (año-mes-día hora:min:seg) 
        [Loggable]
        [DataMember(Order = 6)]
        public DateTime DistributionDate { get; set; }

        [Loggable]
        [DataMember(Order = 7)]
        public int  ClientCtId { get; set; }

        [Loggable]
        [DataMember(Order = 8)]
        public String TypeOfClient { get; set; }


        [Loggable]
        [DataMember(Order = 9)]
        //Todo a futuro colocar la agencia que realizo la solicitud
        public String PdvOriginRequest { get; set; }


        [IgnoreDataMember]
        public Decimal ClientCreditLimit { get; set; }

        [IgnoreDataMember]
        public int ClientSubLevels { get; set; }

    }

   // [Loggable]
   // [DataContract(Namespace = "http://api.movilway.net/schema/extended")]
    public class DistributionMadeByUserFooter
    {

        [DataMember(Order = 1, EmitDefaultValue = false)]
        public int TotalInvoices
        {
            get;
            set;
        }
    }

}