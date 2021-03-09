using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using Movilway.API.Service.External;

namespace Movilway.API.Service.Management.Internal
{
    public class GroupInfoInternal
    {
        public long GroupID{ set; get; }
        public String Name { set; get; }
        public int Type { set; get; }
        public String Category { set; get; }
    }
}