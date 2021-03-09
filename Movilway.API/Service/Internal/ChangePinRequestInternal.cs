using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.Internal
{
    public class ChangePinRequestInternal:ApiRequestInternal
    {
        public String Initiator { set; get; }
        public String CurrentPin { set; get; }
        public String NewPin { set; get; }
    }
        
}