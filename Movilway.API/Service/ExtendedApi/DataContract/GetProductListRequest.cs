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
    public class GetProductListRequest : IMovilwayApiRequestWrapper<GetProductListRequestBody>
    {
        [MessageBodyMember(Name = "GetProductListRequest")]
        public GetProductListRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "GetProductListRequest", Namespace="http://api.movilway.net/schema/extended")]
    public class GetProductListRequestBody : ASecuredApiRequest
    {
        [Loggable]
        [DataMember(Order = 3, IsRequired = true)]
        public String Agent { set; get; }

        //[Loggable]
        //[DataMember(Order = 3, IsRequired = false)]
        //public String Platform { set; get; }

        public override string ToString()
        {
            return Utils.logFormat(this);
            //return "GetProductListRequest {AuthenticationData{Username=" + base.AuthenticationData.Username + ",Password=******,SessionId=" + base.AuthenticationData.SessionID + 
            //                                    "},DeviceType=" + base.DeviceType + ",Platform=" + base.Platform + ",Agent=" + this.Agent + "}";
        }
    }
}