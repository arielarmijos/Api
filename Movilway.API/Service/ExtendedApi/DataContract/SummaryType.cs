using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [DataContract(Namespace = "http://api.movilway.net/schema/extended")]
    public enum SummaryType
    {
        NotSpecified=0,
        [EnumMember]
        ByUser=1,
        [EnumMember]
        ByAgent=2
    }
}