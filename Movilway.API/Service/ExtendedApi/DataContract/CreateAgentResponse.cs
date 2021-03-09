using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;
using Movilway.Logging.Attribute;
using Movilway.API.Service.ExtendedApi.DataContract.Common;

namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [MessageContract(IsWrapped = false)]
    public class CreateAgentResponse : IMovilwayApiResponseWrapper<CreateAgentResponseBody>
    {
        [MessageBodyMember(Name = "CreateAgentResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public CreateAgentResponseBody Response { set; get; }

        public CreateAgentResponse()
        {
            Response = new CreateAgentResponseBody();
        }
    }




    [Loggable]
    [DataContract(Name = "CreateAgentResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class CreateAgentResponseBody : ExtendedApi.DataContract.Common.AGenericApiResponse
    {



        [Loggable]
        [DataMember(Order = 1)]
        public ErrorItems Errors { set; get; }

        [Loggable]
        [DataMember(Order = 2)]
        public Int32 AgeId { set; get; }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }


    }

    [CollectionDataContract(Name = "ErrorItems", Namespace = "http://api.movilway.net/schema/extended", ItemName = "Error")]
    public class ErrorItems : List<ErrorItem>
    {
    }

    [Loggable]
    [DataContract(Name = "Error", Namespace = "http://api.movilway.net/schema/extended")]
    public class ErrorItem
    {
        [Loggable]
        [DataMember(Order = 0)]
        public String ErrorId { set; get; }

        [Loggable]
        [DataMember(Order = 1)]
        public String ErrorDescription { set; get; }
    }
}