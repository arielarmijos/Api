using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public enum GetTransactionRequestParameterType
    {
        [EnumMember]
        TransactionID,
        [EnumMember]
        ExternalTransactionReference,
        [EnumMember]
        TargetAgent,
        [EnumMember]
        TransactionType,
        [EnumMember]
        LastTransaction,
        [EnumMember]
        DistributionExternalTransactionReference,
    }
}