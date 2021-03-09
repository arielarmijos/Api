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
    public class GetBankListResponse : IMovilwayApiResponseWrapper<GetBankListResponseBody>
    {
        [MessageBodyMember(Name = "GetBankListResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public GetBankListResponseBody Response { set; get; }

        public GetBankListResponse()
        {
            Response = new GetBankListResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "GetBankListResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetBankListResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AGenericApiResponse
    {

        [DataMember(Order = 1)]
        public BankList BankList { set; get; }

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

    [CollectionDataContract(Namespace = "http://api.movilway.net/schema/extended", 
        KeyName="ID", ValueName="Description", ItemName="Bank")]
    public class BankList : Dictionary<int, String>
    {
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var bank in this)
                sb.Append("Bank={Id=" + bank.Key + ",Name=" + bank.Value + "},");
            sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }
    }
}