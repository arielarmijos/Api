using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.Management.Internal
{
    public class GetChildListByReferenceResponseInternal: ApiResponseInternal
    {
        public List<AgentInfoInternal> ChildList { set; get; }
    }
}