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
    public class GetCutsRequest : IMovilwayApiRequestWrapper<GetCutsRequestBody>
    {
        [MessageBodyMember(Name = "GetCutsRequest", Namespace = "http://api.movilway.net/schema/extended")]
        public GetCutsRequestBody Request { get; set; }
    }

   [Loggable]
   [DataContract(Name = "GetCutsRequest", Namespace = "http://api.movilway.net/schema/extended")]
   public class GetCutsRequestBody : ASecuredApiRequest
    {

        [Loggable]
        [DataMember(Order = 3, IsRequired = true)]
        public DateTime InitialDate { get; set; }

        [Loggable]
        [DataMember(Order = 4, IsRequired = false, EmitDefaultValue = true)]
        public DateTime? FinalDate { get; set; }

        [Loggable]
        [DataMember(Order = 5, IsRequired = true)]
        public string CutType { get; set; }

        [Loggable]
        [DataMember(Order = 6, IsRequired = true)]
        public bool FindByAgency { set; get; }

 

        [Loggable]
        [DataMember(Order = 7, IsRequired = true)]
        public int Page { set; get; }

        [Loggable]
        [DataMember(Order = 8, IsRequired = true)]
        public int PageSize { set; get; }



    }
}

