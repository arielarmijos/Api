using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [DataContract(Namespace = "http://api.movilway.net/schema/extended")]
    public enum WalletType
    {
        NotSpecified=0,
        [EnumMember]
        eWallet=1,
        [EnumMember]
        Stock=2,
        [EnumMember]
        Points=3,
        AirTime = 4,
        Debt=5
    }
}