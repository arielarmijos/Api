using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Movilway.API.Service.D2.External
{

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class BankInfo
    {
        [DataMember(Order = 1)]
        public long Key { set; get; }

        [DataMember(Order = 2)]
        public String Description { set; get; }
    }

    [CollectionDataContract(Namespace = "http://api.movilway.net/schema")]
    public class BankList : List<BankInfo> { }

}