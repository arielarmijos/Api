using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.Sales.Internal
{
    public class GetTransactionRequestInternal : ApiRequestInternal
    {
        public String ParameterValue { set; get; }
        public GetTransactionRequestInternalParameterType ParameterType { set; get; }
    }
}