using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [DataContract(Namespace = "http://api.movilway.net/schema/extended")]
    public enum TransactionType
    {
        NotSpecified=0,
        buy,
        buystock,
        cashin,
        cashout,
        createcoupon,
        paystock,
        topup,
        transfercoupon,
        transfer,
        transferstock,
        sell
    }


}