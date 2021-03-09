using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Movilway.API.Service.ExtendedApi.DataContract.Common
{
    [DataContract(Namespace="http://api.movilway.net/schema/extended")]
    public abstract class AUnsecuredApiRequest : IMovilwayApiRequest
    {
        public Boolean IsFinancial { get { return false; } }

        public AuthenticationData AuthenticationData { get { return null; } }

        [DataMember(Order = 0, IsRequired = true)]
        public int DeviceType { set; get; }

        [DataMember(Order = 0, IsRequired = false)]
        public String Platform { set; get; }
    }
}