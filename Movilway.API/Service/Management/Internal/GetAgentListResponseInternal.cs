using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;
using Movilway.API.Service.Internal;

namespace Movilway.API.Service.Management.Internal
{
    public class GetAgentListResponseInternal : ApiResponseInternal
    {
        public Dictionary<String, String> KeyValuePair { set; get; }
    }
}