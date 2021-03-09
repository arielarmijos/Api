using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.D2.Internal
{
    public class AccountPaymentRequestInternal : ApiRequestInternal
    {
        public String Amount { set; get; } // Definido en String porque Utiba lo tiene asi para esta funcion especifica
        public String Agent { set; get; }
    }
}