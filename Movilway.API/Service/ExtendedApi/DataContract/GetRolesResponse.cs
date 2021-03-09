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
    public class GetRolesResponse : IMovilwayApiResponseWrapper<GetRolesResponseBody>
    {
        [MessageBodyMember(Name = "GetRolesResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public GetRolesResponseBody Response { set; get; }

        public GetRolesResponse()
        {
            Response = new GetRolesResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "GetRolesResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetRolesResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AGenericApiResponse
    {
        [Loggable]
        [DataMember(Order = 3, EmitDefaultValue=false)]
        public RolList RolList { set; get; }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }

    [CollectionDataContract(Namespace = "http://api.movilway.net/schema/extended", ItemName = "Rol")]
    public class RolList : List<Rol> 
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