using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;
using Movilway.API.Service.Internal;

namespace Movilway.API.Service.Management.Internal
{
    public class GetAllAgentGroupsResponseInternal : ApiResponseInternal
    {
        public List<GroupInfoInternal> AllGroups { set; get; }
    }
}