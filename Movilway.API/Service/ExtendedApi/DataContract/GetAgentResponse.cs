using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;
using Movilway.Logging.Attribute;
using Movilway.API.Service.ExtendedApi.DataContract.Common;


namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [MessageContract(IsWrapped = false)]
    public class GetAgentResponse: IMovilwayApiResponseWrapper<GetAgentResponseBody>
    {
        [MessageBodyMember(Name = "GetAgentResponseBody", Namespace = "http://api.movilway.net/schema/extended")]
        public GetAgentResponseBody Response { set; get; }

        public GetAgentResponse()
        {
            Response = new GetAgentResponseBody();
        }
    }


    [Loggable]
    [DataContract(Name = "GetAgentResponseBody", Namespace = "http://api.movilway.net/schema/extended")]
    public class  GetAgentResponseBody : ExtendedApi.DataContract.Common.AGenericApiResponse
    {
  
       [DataMember(Order = 1)]
        public AgentEdit Agente { set; get; }

    }

    [Loggable]
    [DataContract(Name = "GetAgentResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class AgentEdit
    {
        [Loggable]
        [DataMember(Order = 2, IsRequired = true)]
        public String RIF { set; get; }
        [Loggable]
        [DataMember(Order = 3, IsRequired = true)]
        public String Phone { set; get; }
        [Loggable]
        [DataMember(Order = 4, IsRequired = true)]
        public String AgentName { set; get; }
        [Loggable]
        [DataMember(Order = 5, IsRequired = true)]
        public String NumberIMEI { set; get; }
        [Loggable]
        [DataMember(Order = 6, IsRequired = true)]
        public String LegalName { set; get; }
        [Loggable]
        [DataMember(Order = 7, IsRequired = true)]
        public String Email { set; get; }
        [Loggable]
        [DataMember(Order = 8, IsRequired = true)]
        public String Address { set; get; }
        [Loggable]
        [DataMember(Order = 9, IsRequired = true)]
        public String ContactPerson { set; get; }
        [Loggable]
        [DataMember(Order = 10, IsRequired = true)]
        public String BetweenStreets { set; get; }
        [Loggable]
        [DataMember(Order = 11, IsRequired = true)]
        public String SubLevels { set; get; }
        [Loggable]
        [DataMember(Order = 12, IsRequired = true)]
        public String Country { get; set; }
        [Loggable]
        [DataMember(Order = 13, IsRequired = true)]
        public String Province { set; get; }
        [Loggable]
        [DataMember(Order = 14, IsRequired = true)]
        public String Pdv { set; get; }
        [Loggable]
        [DataMember(Order = 15, IsRequired = true)]
        public String City { set; get; }

        [Loggable]
        [DataMember(Order = 16, IsRequired = true)]
        public bool CommissionableDeposits { set; get; }
        [Loggable]
        [DataMember(Order = 17, IsRequired = true)]
        public decimal Commission { set; get; }
        [Loggable]
        [DataMember(Order = 18, IsRequired = true)]
        public decimal CheckingAccountCreditLimit { set; get; }
        [Loggable]
        [DataMember(Order = 19, IsRequired = true)]
        public bool AutomaticAuthorization { set; get; }
        [Loggable]
        [DataMember(Order = 20, IsRequired = true)]
        public bool AutomaticReverse { set; get; }
        [Loggable]
        [DataMember(Order = 21, IsRequired = true)]
        public bool AutomaticReposition { set; get; }
        [Loggable]
        [DataMember(Order = 22, IsRequired = true)]
        public decimal MinimumOrderAmount { set; get; }
        [Loggable]
        [DataMember(Order = 23, IsRequired = true)]
        public decimal MaximumOrderAmount { set; get; }
        [Loggable]
        [DataMember(Order = 24, IsRequired = true)]
        public decimal MaximumMonthlyAmount { set; get; }
        [Loggable]
        [DataMember(Order = 25, IsRequired = true)]
        public decimal MaximumAuthorizedDailyAmount { set; get; }
        [Loggable]
        [DataMember(Order = 26, IsRequired = true)]
        public bool AsynchronousTopup { set; get; }


        [Loggable]
        [DataMember(Order = 27, IsRequired = true)]
        public bool SalesCommission { set; get; }
        [Loggable]
        [DataMember(Order = 28, IsRequired = true)]
        public int Group { set; get; }
        [Loggable]
        [DataMember(Order = 29, IsRequired = true)]
        public List<ProductCommision> ProductsCommission { set; get; }

        [Loggable]
        [DataMember(Order = 30, IsRequired = true)]
        public String UserName1 { set; get; }
        [Loggable]
        [DataMember(Order = 31, IsRequired = true)]
        public String UserLastName1 { set; get; }
        [Loggable]
        [DataMember(Order = 32, IsRequired = true)]
        public String AccessLogin1 { set; get; }
        [Loggable]
        [DataMember(Order = 33, IsRequired = true)]
        public String AccessPassword1 { set; get; }

        [Loggable]
        [DataMember(Order = 34, IsRequired = true)]
        public String AccessLogin2 { set; get; }
        [Loggable]
        [DataMember(Order = 35, IsRequired = true)]
        public String AccessPassword2 { set; get; }
        [Loggable]
        [DataMember(Order = 36, IsRequired = true)]
        public String AccessType2 { set; get; }
        [Loggable]
        [DataMember(Order = 37, IsRequired = true)]
        public bool IsAdministrator2 { set; get; }

        //nuevos atributos requerimiento segundo acceso segundo usuario

        /// <summary>
        /// Indica si el segundo usuario tiene un segunfo accesso
        /// </summary>
        [Loggable]
        [DataMember(Order = 38, IsRequired = true)]
        public bool AvailableSecondAccessSecondUser { set; get; }

        //datos del segundo accesso si y solo sy AvailableSecondAccessSecondUser
        [Loggable]
        [DataMember(Order = 39, IsRequired = false)]
        public String AccessLogin3 { set; get; }
        [Loggable]
        [DataMember(Order = 40, IsRequired = false)]
        public String AccessPassword3 { set; get; }
        [Loggable]
        [DataMember(Order = 41, IsRequired = false)]
        public String AccessType3 { set; get; }


        [Loggable]
        [DataMember(Order = 42, IsRequired = false)]
        public Decimal AgeId { set; get; }

        [Loggable]
        [DataMember(Order = 43, IsRequired = false)]
        public Decimal TaxCategory { set; get; }

        [Loggable]
        [DataMember(Order = 44, IsRequired = false)]
        public Decimal SegmentId { set; get; }

        [Loggable]
        [DataMember(Order = 45, IsRequired = false)]
        public string Notes { set; get; }



        public AgentEdit()
        {
            ProductsCommission = new List<ProductCommision>();
        }
        

        public override string ToString()
        {
            return Utils.logFormat(this);
        }

       
    }
}