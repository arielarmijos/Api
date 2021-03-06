using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.ServiceModel;
using Movilway.API.Service.ExtendedApi.DataContract.Common;

namespace Movilway.API.Service.ExtendedApi.DataContract.Macro
{
    [DataContract]
    public class GetFavoriteAmountsRequest : ASecuredApiRequest
    {
        [DataMember(Order = 2, IsRequired = true)]
        public String Agent { set; get; }
    }
}