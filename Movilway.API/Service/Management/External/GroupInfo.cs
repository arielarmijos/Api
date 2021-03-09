using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using Movilway.API.Service.External;

namespace Movilway.API.Service.Management.External
{
    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class GroupInfo
    {
        [DataMember(Order = 1)]
        public long GroupID{ set; get; }

        [DataMember(Order = 2)]
        public String Name { set; get; }

        [DataMember(Order = 3)]
        public int Type { set; get; }

        [DataMember(Order = 4)]
        public String Category { set; get; }
    }

    [CollectionDataContract(Namespace = "http://api.movilway.net/schema")]
    public class GroupList : List<GroupInfo> { }
}