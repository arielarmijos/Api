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
    public class GetBankListAppResponse : IMovilwayApiResponseWrapper<GetBankListAppResponseBody>
    {
        [MessageBodyMember(Name = "GetBankListAppResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public GetBankListAppResponseBody Response { set; get; }

        public GetBankListAppResponse()
        {
            Response = new GetBankListAppResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "GetBankListAppResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetBankListAppResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AGenericApiResponse
    {

        [DataMember(Order = 1)]
        public BankListApp BankList { set; get; }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }

    //[DataContract(Namespace = "http://api.movilway.net/schema/extended")]
    //public class BankInfo
    //{
    //    [DataMember(Order = 1)]
    //    public int Key { set; get; }

    //    [DataMember(Order = 2)]
    //    public String Description { set; get; }
    //}

    [CollectionDataContract(Namespace = "http://api.movilway.net/schema/extended", ItemName = "Bank")]
    public class BankListApp : List<BankInfo> { }

    [DataContract(Namespace = "http://api.movilway.net/schema/extended")]
    public class BankInfo
    {
        [DataMember(Order = 1)]
        public int ID { get; set; }

        [DataMember(Order = 2)]
        public String Name { get; set; }

        [DataMember(Order = 3)]
        public String Account { get; set; }
    }
}