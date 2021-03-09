using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service
{
    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class ExtendedApiRequest
    {
        [DataMember(Order=0, IsRequired = true)]
        public String Username { set; get; }

        [DataMember(Order = 1, IsRequired = true)]
        public String Password { set; get; }
    }
}