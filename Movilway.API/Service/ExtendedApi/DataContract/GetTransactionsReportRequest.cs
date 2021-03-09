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
    public class GetTransactionsReportRequest : IMovilwayApiRequestWrapper<GetTransactionsReportRequestBody>
    {
        [MessageBodyMember(Name = "GetTransactionsReportRequest", Namespace = "http://api.movilway.net/schema/extended")]
        public GetTransactionsReportRequestBody Request { get; set; }

        public GetTransactionsReportRequest()
        {
            Request = new GetTransactionsReportRequestBody();
        }
    }

    [Loggable]
    [DataContract(Name = "GetTransactionsReportRequest", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetTransactionsReportRequestBody : ASecuredApiRequest
    {
        [Loggable]
        [DataMember(Order = 1,IsRequired= true)]
        public DateTime InitialDate { get; set; }

        [Loggable]
        [DataMember(Order = 2, IsRequired = true)]
        public DateTime FinalDate { get; set; }

        [Loggable]
        [DataMember(Order = 3, IsRequired = true)]
        public String TransactionType { set; get; }

        [Loggable]
        [DataMember(Order = 4, IsRequired = true)]
        public String Agent { set; get; }

        //last param
        /// <summary>
        /// 
        /// </summary>
        [Loggable]
        [DataMember(Order = 5, IsRequired = true)]
        public Int32 Top { set; get; }
    }
}