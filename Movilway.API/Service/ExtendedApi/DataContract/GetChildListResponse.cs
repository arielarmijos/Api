using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging.Attribute;
using System.Text;

namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [MessageContract(IsWrapped = false)]
    public class GetChildListResponse : IMovilwayApiResponseWrapper<GetChildListResponseBody>
    {
        [MessageBodyMember(Name = "GetChildListResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public GetChildListResponseBody Response { set; get; }

        public GetChildListResponse()
        {
            Response = new GetChildListResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "GetChildListResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetChildListResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AGenericApiResponse
    {

     

        public GetChildListResponseBody (){

        this.ChildList = new ChildList();
        }


        [Loggable]
        [DataMember(Order = 3, EmitDefaultValue=false)]
        public ChildList ChildList { set; get; }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }

    [CollectionDataContract(Namespace = "http://api.movilway.net/schema/extended", ItemName = "Child")]
    public class ChildList : List<BasicAgentInfo> 
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
}