using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Movilway.API.Service.External
{
    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public enum Gender
    {
        [EnumMember]
        Male,
        [EnumMember]
        Female
    }
}