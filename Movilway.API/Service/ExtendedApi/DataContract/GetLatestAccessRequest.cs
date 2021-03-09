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
    public class GetLatestAccessRequest : IMovilwayApiRequestWrapper<GetLatestAccessRequestBody>
    {
        [MessageBodyMember(Name = "GetLatestAccessRequestBody", Namespace = "http://api.movilway.net/schema/extended")]
        public GetLatestAccessRequestBody Request
        {
            get;
            set;
        }

        public GetLatestAccessRequest()
        {
            Request = new GetLatestAccessRequestBody();
        }
    }

    [DataContract(Name = "GetLatestAccessRequestBody", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetLatestAccessRequestBody : ASecuredApiRequest
    {

    }

    //filtro por acceso
    [DataContract(Namespace = "http://api.movilway.net/schema/extended")]
    public class FilterLatestAccess
    {


        [Loggable]
        [DataMember(Order = 1, IsRequired = true)]
        public long ID { get; set; }

        [Loggable]
        [DataMember(Order = 2, IsRequired = true)]
        public string Device { get; set; }


        [Loggable]
        [DataMember(Order = 4, IsRequired = true)]
        public string Navigator { get; set; }

        [Loggable]
        [DataMember(Order = 5, IsRequired = true)]
        public String IP { get; set; }

        [Loggable]
        [DataMember(Order = 6, IsRequired = true)]
        public DateTime LowerDate { get; set; }

        [Loggable]
        [DataMember(Order = 7, IsRequired = true)]
        public DateTime UpperDate { get; set; }


        ////incluir atributos de paginacion
        //public int From { get; set; }

        //public int PageSize { get; set; }
    }
}