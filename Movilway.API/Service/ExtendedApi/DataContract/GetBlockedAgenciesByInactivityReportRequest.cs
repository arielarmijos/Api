using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Web;

namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [MessageContract(IsWrapped = false)]
    public class GetBlockedAgenciesByInactivityReportRequest : IMovilwayApiRequestWrapper<GetBlockedAgenciesByInactivityRequestBody>
    {
        [MessageBodyMember(Name = "GetBlockedAgenciesByInactivityRequest")]
        public GetBlockedAgenciesByInactivityRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "GetBlockedAgenciesByInactivityRequest", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetBlockedAgenciesByInactivityRequestBody : ASecuredApiRequest
    {

        [Loggable]
        [DataMember(Order = 3, IsRequired = true)]
        public String Agent { set; get; }

        [Loggable]
        [DataMember(Order = 4, IsRequired = true)]
        public DateTime DateMin { set; get; }

        [Loggable]
        [DataMember(Order = 5, IsRequired = true)]
        public DateTime DateMax { set; get; }

         [Loggable]
         [DataMember(Order = 6, IsRequired = true)]
         public int Page { set; get; }
        
         [Loggable]
         [DataMember(Order = 7, IsRequired = true)]
         public int PageSize { set; get; }




        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }

}