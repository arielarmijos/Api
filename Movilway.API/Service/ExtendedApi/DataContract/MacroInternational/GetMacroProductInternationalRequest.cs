using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.ServiceModel;
using Movilway.API.Service.ExtendedApi.DataContract.Common;

namespace Movilway.API.Service.ExtendedApi.DataContract.MacroInternational
{
    [DataContract]
    public class GetMacroProductInternationalRequest : ASecuredApiRequest
    {
        [DataMember(Order = 4, IsRequired = false)]
        public String CountryId { set; get; }

        [DataMember(Order = 5, IsRequired = true)]
        public String Agent { set; get; }
    }
}