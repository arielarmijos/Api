using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.Configuration;
using System.Configuration;

namespace Movilway.API.Service.ExtendedApi.Provider.Utiba.Custom
{
    public class IncludeHttpUserAgentBehaviorExtensionElement : BehaviorExtensionElement
    {
        public override Type BehaviorType
        {
            get
            {
                return typeof(IncludeHttpUserAgentEndpointBehavior);
            }
        }

        protected override object CreateBehavior()
        {
            return new IncludeHttpUserAgentEndpointBehavior();
        }
    }
}