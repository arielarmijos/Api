using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging.Attribute;

namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [MessageContract(IsWrapped = false)]
    public class QueryPaymentResponse : IMovilwayApiResponseWrapper<QueryPaymentResponseBody>
    {
         [MessageBodyMember(Name = "QueryPaymentResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public QueryPaymentResponseBody Response { set; get; }

         public QueryPaymentResponse()
        {
            Response = new QueryPaymentResponseBody();
        }
    }

     [Loggable]
     [DataContract(Name = "QueryPaymentResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class QueryPaymentResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AFinancialApiResponse
    {
        //[Loggable]
        //[DataMember(Order = 4, IsRequired = false, EmitDefaultValue = false)]
        //public String ExternalTransactionReference { set; get; }

         [Loggable]
         [DataMember(Order = 4, IsRequired = false, EmitDefaultValue = false)]
         public string ResponseCodeOpetator { get; set; }

         [Loggable]
         [DataMember(Order = 5, IsRequired = false, EmitDefaultValue = false)]
         public string DetailsOperator { get; set; }

        [Loggable]
        [DataMember(Order = 6, IsRequired = false, EmitDefaultValue = false)]
        public string Data { get; set; }

        [Loggable]
        [DataMember(Order = 7, IsRequired = false, EmitDefaultValue = false)]
        public string DataDescriptor { get; set; }

        [Loggable]
        [DataMember(Order = 8, IsRequired = false, EmitDefaultValue = false)]
        public bool Result { get; set; }

        [Loggable]
        [DataMember(Order = 9, IsRequired = false, EmitDefaultValue = false)]
        public String QueryResultType { get; set; }

        [Loggable]
        [DataMember(Order = 10, IsRequired = false, EmitDefaultValue = false)]
        public String IdAutorization { get; internal set; }

        [Loggable]
        [DataMember(Order = 11, IsRequired = false, EmitDefaultValue = false)]
        public Decimal Amount { get; set; }


        public QueryPaymentResponseBody()
            :base()
        {
            ResponseCodeOpetator = "";

            DetailsOperator = "";

            Data = "";

            DataDescriptor = "";

            Result = false;

            QueryResultType = "";

            //[Loggable]
            //[DataMember(Order = 9, IsRequired = false, EmitDefaultValue = false)]
            //public String IdAutorization { get; set; }
            TransactionID = 0;

            IdAutorization = "";

            Amount = 0.0m;

        }



        public override string ToString()
        {
            return Utils.logFormat(this);

            //return 
            //    String.Join("|",
            //ResponseCodeOpetator,

            //DetailsOperator ,

            //Data ,

            //DataDescriptor,

            //Result ,

            //QueryResultType ,

            ////[Loggable]
            ////[DataMember(Order = 9, IsRequired = false, EmitDefaultValue = false)]
            ////public String IdAutorization { get; set; }
            //TransactionID ,

            //IdAutorization ,

            //Amount.ToString(System.Globalization.CultureInfo.InvariantCulture));

        }
    
    }

}