using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;
using Movilway.Logging.Attribute;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using System.Text;

namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [MessageContract(IsWrapped = false)]
    public class GetParametersResponse : IMovilwayApiResponseWrapper<GetParametersResponseBody>
    {
        [MessageBodyMember(Name = "GetParametersResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public GetParametersResponseBody Response { set; get; }

        public GetParametersResponse()
        {
            Response = new GetParametersResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "GetParametersResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetParametersResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AGenericApiResponse
    {
        [Loggable]
        [DataMember(Order = 3)]
        public ParameterList ParametersInfo { set; get; }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }

    [CollectionDataContract(Namespace = "http://api.movilway.net/schema/extended", ItemName = "Parameters")]
    public class ParameterList : List<ParametersInfo>
    {
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            //foreach (var item in this)
            //    sb.Append(Utils.logFormat(item) + ",");
            //if (sb.Length > 0) sb.Remove(sb.Length - 1, 1);
            sb.Append(String.Concat(this.GetType().Name, " Count ", this.Count));
            return sb.ToString();
        }
    }

    [Loggable]
    [DataContract(Namespace = "http://api.movilway.net/schema/extended")]
    public class ParametersInfo
    {
        [Loggable]
        [DataMember(Order = 0, EmitDefaultValue=false)]
        public int AccessTypeId { set; get; }

        [Loggable]
        [DataMember(Order = 1, EmitDefaultValue = false)]
        public String AccessTypeName { set; get; }

        [Loggable]
        [DataMember(Order = 2, EmitDefaultValue = false)]
        public int MinimumPasswordLength { set; get; }

        [Loggable]
        [DataMember(Order = 3, EmitDefaultValue = false)]
        public int MaximumPasswordLength { set; get; }

        [Loggable]
        [DataMember(Order = 4, EmitDefaultValue = false)]
        public int MinimumAccessLength { set; get; }

        [Loggable]
        [DataMember(Order = 5, EmitDefaultValue = false)]
        public int MaximumAccessLength { set; get; }
    }
}