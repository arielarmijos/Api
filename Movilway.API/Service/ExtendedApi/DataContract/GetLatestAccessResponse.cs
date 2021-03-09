using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Web;
namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [MessageContract(IsWrapped = false)]
    public class GetLatestAccessResponse : IMovilwayApiResponseWrapper<GetLatestAccessResponseBody>
    {
        [MessageBodyMember(Name = "GetLatestAccessResponseBody", Namespace = "http://api.movilway.net/schema/extended")]
        public GetLatestAccessResponseBody Response
        {
            get;
            set;
        }

        public GetLatestAccessResponse()
        {
            Response = new GetLatestAccessResponseBody();
        }

    }

    [DataContract(Name = "GetLatestAccessResponseBody", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetLatestAccessResponseBody : ApiResponse
    {
        [DataMember(Name = "AccessList")]
        public GetAccessList List { get; set; }

        public GetLatestAccessResponseBody()
        {
            List = new GetAccessList();
        }

        public override string ToString()
        {
            return String.Concat("[", this.GetType().Name, "]", List.ToString());
        }
    }
    [CollectionDataContract(Namespace = "http://api.movilway.net/schema/extended")]
    public class GetAccessList : List<Access>
    {
        public GetAccessList()
            : base()
        {

        }
        public GetAccessList(List<Access> list)
            : base(list)
        {


        }

    }

    [Loggable]
    [DataContract(Namespace = "http://api.movilway.net/schema/extended")]
    public class Access
    {
        const string DEFAULT_IP = "0.0.0.0";

        [Loggable]
        [DataMember(Order = 1)]
        public long ID { get; set; }


        [Loggable]
        [DataMember(Order = 2)]
        public long DeviceID { get; set; }

        [Loggable]
        [DataMember(Order = 4)]
        public DateTime DateTime { get; set; }

        [Loggable]
        [DataMember(Order = 5)]
        public String IP { get; set; }
        
        [Loggable]
        [DataMember(Order = 6)]
        public String Action { get; set; }

        //
        [Loggable]
        [DataMember(Order = 6)]
        public Decimal Amount { get; set; }

        public Access()
        {
   
            IP = DEFAULT_IP;
        }



    }
}