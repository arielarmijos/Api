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
    public class GetAuditoryUsersReportRequest:IMovilwayApiRequestWrapper<GetAuditoryUsersReportRequestBody>
    {
        [MessageBodyMember(Name = "GetAuditoryUsersReportRequest")]
        public GetAuditoryUsersReportRequestBody Request { get; set; }
    }

    [Loggable]
    [DataContract(Name = "GetAuditoryUsersReportRequest", Namespace = "http://api.movilway.net/schema/extended")]

    public class GetAuditoryUsersReportRequestBody : ASecuredApiRequest
    {
        [Loggable]
        [DataMember(Order = 3, IsRequired = true)]
        public DateTime InitialDate { set; get; }

        [Loggable]
        [DataMember(Order = 4, IsRequired = true)]
        public DateTime EndDate { set; get; }

        [Loggable]
        [DataMember(Order = 5, IsRequired = false)]
        public FilterRequest FilterRequest { set; get; }

    }

    [DataContract(Name = "FilterRequest")]
    public enum FilterRequest
    {
        [EnumMember]
        ByDateCreation,
        [EnumMember]
        ByLastAccess,
        [EnumMember]
        ByValidityDate,
    }
}