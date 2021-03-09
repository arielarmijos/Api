using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.ServiceModel;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging.Attribute;


namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [MessageContract(IsWrapped = false)]
    public class MonthlyReportRequest : IMovilwayApiRequestWrapper<MonthlyReportRequestBody>
    {
        [MessageBodyMember(Name = "MonthlyReportRequest")]
        public MonthlyReportRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "MonthlyReportRequest", Namespace = "http://api.movilway.net/schema/extended")]
    public class MonthlyReportRequestBody : ASecuredApiRequest
    {
        [Loggable]
        [DataMember(Order = 5, IsRequired = true)]
        public ReportType ReportType { set; get; }

        [Loggable]
        [DataMember(Order = 6, IsRequired = true)]
        public DateTime ExecutionDate { get; set; }
   
        public override string ToString()
        {
            return Utils.logFormat(this);
         
        }

     
    }

    public enum ReportType
    {

        [EnumMember(Value = "DailyCloseSales")]
        DailyCloseSales,
        
        [EnumMember(Value = "Distribution")]
        Distribution,

        [EnumMember(Value = "MonthlyCloseSales")]
        MonthlyCloseSales  
    }
}