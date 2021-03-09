using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Web;

namespace Movilway.API.Service.ExtendedApi.DataContract
{

    [MessageContract(IsWrapped = false)]
    public class GetUsersAgentResponse : IMovilwayApiResponseWrapper<GetUsersAgentResponseBody>
    {
        [MessageBodyMember(Name = "GetUsersAgentResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public GetUsersAgentResponseBody Response { set; get; }

        public GetUsersAgentResponse()
        {
            Response = new GetUsersAgentResponseBody();
        }

    }

    [Loggable]
    [DataContract(Name = "GetUsersAgentResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetUsersAgentResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AGenericApiResponse
    {
        [Loggable]
        [DataMember(Order = 5, IsRequired = true)]
        public IEnumerable<User> Users { set; get; }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }

    public class User
    {
        public int UserId { get; set; }

        public String UserName { set; get; }

        public String UserLastName { set; get; }

        public String UserStatus { set; get; }

        public List<GetUserAgentAccess> Access { get; set; }

        public String AgentName { get; set; }

        public User() {
            Access = new List<GetUserAgentAccess>();
        }

    }

    public class GetUserAgentAccess
    {
        public string Login { get; set; }
        public string AccessType { get; set; }
        public int AccessTypeId { get; set; }
    }
}