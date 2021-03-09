using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.ServiceModel;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging.Attribute;

namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [MessageContract(IsWrapped=false)]
    public class SellRequest: IMovilwayApiRequestWrapper<SellRequestBody>
    {
        [MessageBodyMember(Name = "SellRequest")]
        public SellRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "SellRequest", Namespace="http://api.movilway.net/schema/extended")]
    public class SellRequestBody : ASecuredFinancialApiRequest
    {
        [Loggable]
        [DataMember(Order = 3, IsRequired = true)]
        public String Agent { set; get; }

        // Este parametro existia en el API anterior pero se estaba enviando quemado el valor 1 en el codigo
        // asi que no tiene sentido pedirlo.
        //[DataMember(Order = 4, IsRequired = true)]
        //public int Type { set; get; }
    }
}