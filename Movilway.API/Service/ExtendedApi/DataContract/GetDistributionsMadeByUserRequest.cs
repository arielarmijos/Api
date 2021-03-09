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
    [MessageContract(IsWrapped=false)]
    public class GetDistributionsMadeByUserRequest : IMovilwayApiRequestWrapper<GetDistributionsMadeByUserRequestBody>
    {
        [MessageBodyMember(Name = "GetDistributionsMadeByUserRequest")]
        public GetDistributionsMadeByUserRequestBody Request { set; get; }

    }

    [Loggable]
    [DataContract(Name = "GetDistributionsMadeByUserRequest", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetDistributionsMadeByUserRequestBody : ASecuredApiRequest
    {

        [Loggable]
        [DataMember(Order = 3, IsRequired = true)]
        public DateTime Date { set; get; }

        //[Loggable]
        //[DataMember(Order = 4, IsRequired = true)]
        //public String UserLogin { set; get; }
        //
        //[Loggable]
        //[DataMember(Order = 5, IsRequired = true)]
        //public String UserId { set; get; }

        [Loggable]
        [DataMember(Order = 4, IsRequired = false, EmitDefaultValue= false)]
        public DateTime? DateEnd { set; get; }


        [Loggable]
        [DataMember(Order = 5, IsRequired = true)]
        public bool FindByAgency { set; get; }


        [Loggable]
        [DataMember(Order = 6, IsRequired = true)]
        public int Page { set; get; }

        [Loggable]
        [DataMember(Order = 7, IsRequired = true)]
        public int PageSize { set; get; }


        //[Loggable]
        //[DataMember(Order = 6, IsRequired = true)]


      



        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }
}