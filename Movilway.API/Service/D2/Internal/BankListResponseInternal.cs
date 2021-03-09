using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;
using Movilway.API.Service.External;

namespace Movilway.API.Service.D2.Internal
{
    public class BankListResponseInternal : ApiResponseInternal
    {
        public List<BankInfo> Banks { set; get; }
    }

    public class BankInfo
    {
        public int Key { set; get; }
        public String Description { set; get; }
    }
}