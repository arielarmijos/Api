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
    public class GetTransactionsReportResponse : IMovilwayApiResponseWrapper<GetTransactionsReportResponseBody>
    {
        [MessageBodyMember(Name = "GetTransactionsReportResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public GetTransactionsReportResponseBody Response
        {
            get;
            set;
        }
    }


    [Loggable]
    [DataContract(Name = "GetTransactionsReportResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetTransactionsReportResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AGenericApiResponse
    {
        [DataMember]
        public ReportDataList reportData { get; set; }

        public GetTransactionsReportResponseBody()
        {
            reportData = new ReportDataList();
        }
    }


    [Loggable]
    [CollectionDataContract(Namespace = "http://api.movilway.net/schema/extended", ItemName = "ItemReport")]
    public class ReportDataList : List<ReportTransactionData>
    {

        public ReportDataList()
            : base()
        {

        }


        public ReportDataList(IEnumerable<ReportTransactionData> collection)
            : base(collection)
        {
            
        }
    }

    [Loggable]
    [DataContract(Namespace = "http://api.movilway.net/schema/extended")]
    public class ReportTransactionData
    {

        [DataMember(Order = 1, EmitDefaultValue = false)]
        public long OriginalTransactionID
        {
            get;
            set;
        }

        [Loggable]
        [DataMember(Order = 2, EmitDefaultValue = false)]
        public String Product
        {
            get;
            set;
        }

         [Loggable]
        [DataMember(Order = 3, EmitDefaultValue = false)]
        public String Origin
        {
            get;
            set;
        }
         [Loggable]
        [DataMember(Order = 4, EmitDefaultValue = false)]
        public String Destination
        {
            get;
            set;
        }
         [Loggable]
         [DataMember(Order = 5, EmitDefaultValue = false)]
         public DateTime LastTimeModified {
            get;
            set;
        }

         [DataMember(Order = 6, EmitDefaultValue = false)]
         public Decimal Amount
         {
             get;
             set;
         }

         [Loggable]
        [DataMember(Order = 7, EmitDefaultValue = false)]
         public string TransactionType
         {
             get;
             set;
         }

         [Loggable]
         [DataMember(Order = 8, EmitDefaultValue = false)]
         public string RefOperadora
         {
             get;
             set;
         }

        [Loggable]
        [DataMember(Order = 9, EmitDefaultValue = false)]
        public String ProductCode
        {
            get;
            set;
        }
    }
}