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
    public class LoginAvailableRequest : IMovilwayApiRequestWrapper<LoginAvailableRequestBody>
    {

        [MessageBodyMember(Name = "LoginAvailableRequest", Namespace = "http://api.movilway.net/schema/extended")]
        public LoginAvailableRequestBody Request { set; get; }

        public LoginAvailableRequest()
        {
            Request = new  LoginAvailableRequestBody();
        }
        
    }


    [Loggable]
    [DataContract(Name = "LoginAvailableRequest", Namespace = "http://api.movilway.net/schema/extended")]
    public class LoginAvailableRequestBody : ASecuredApiRequest
    {
        [Loggable]
        [DataMember(Order =1, IsRequired = true)]
        public String Login { set; get; }

        [Loggable]
        [DataMember(Order = 2, IsRequired = false)]
        public Decimal? AgenteId { set; get; }

    }
   
}