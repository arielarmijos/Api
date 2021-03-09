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
    public class UpdateContactInfoAgentRequest : IMovilwayApiRequestWrapper<UpdateContactInfoAgentRequestBody>
    {
        [MessageBodyMember(Name = "UpdateContactInfoAgentRequest", Namespace = "http://api.movilway.net/schema/extended")]
        public UpdateContactInfoAgentRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "UpdateContactInfoAgentRequest", Namespace = "http://api.movilway.net/schema/extended")]
    public class UpdateContactInfoAgentRequestBody : ASecuredApiRequest
    {
        [Loggable]
        [DataMember(Order = 3, IsRequired = true)]
        public int AgeId { set; get; }
        [Loggable]
        [DataMember(Order = 4, IsRequired = true)]
        public String Phone { set; get; }        
        [Loggable]
        [DataMember(Order = 5, IsRequired = true)]
        public String Email { set; get; }
       
        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }
}
