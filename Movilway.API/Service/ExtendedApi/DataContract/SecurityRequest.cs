using Movilway.API.Service.ExtendedApi.DataContract.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Web;
using Movilway.Logging.Attribute;

namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [MessageContract(IsWrapped=false)]
    public class SecurityRequest : IMovilwayApiRequestWrapper<SecurityRequestBody>//,IMovilwayApiRequestWrapper<IMovilwayApiRequest>,
    {

    


        public SecurityRequestBody Request
        {
            get;
            set;
        }


        //[MessageBodyMember(Name = "SecurityRequestBody")]
        //public SecurityRequestBody Request
        //{
        //    get;
        //    set;
        //}



        //public SecurityRequest()
        //{
        //    Request = new SecurityRequestBody();
        //}

    }

    [Loggable]
    [DataContract(Name = "SecurityRequestBody", Namespace = "http://api.movilway.net/schema/extended")]
    public class SecurityRequestBody : ASecuredApiRequest
    {

    }
}