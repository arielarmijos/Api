﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.Management.Internal
{
    public class GetParentListByReferenceIDResponseInternal:ApiResponseInternal
    {
        public List<AgentInfoInternal> ParentList { set; get; }
    }
}