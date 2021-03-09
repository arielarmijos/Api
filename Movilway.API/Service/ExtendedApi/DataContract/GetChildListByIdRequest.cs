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
    public class GetChildListByIdRequest : IMovilwayApiRequestWrapper<GetChildListRequestByIdBody>
    {
        [MessageBodyMember(Name = "GetChildListByIdRequest", Namespace = "http://api.movilway.net/schema/extended")]
        public GetChildListRequestByIdBody Request { set; get; }



    }

    [Loggable]
    [DataContract(Name = "GetChildListByIdRequest", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetChildListRequestByIdBody : ASecuredApiRequest
    {
        [Loggable]
        [DataMember(Order = 2, IsRequired = true)]
        public int AgeId { set; get; }

        [Loggable]
        [DataMember(Order = 3, IsRequired = false, EmitDefaultValue = false)]
        public bool? ExtendedValues { set; get; }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }
}