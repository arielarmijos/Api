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
    public class GetProductListResponse : IMovilwayApiResponseWrapper<GetProductListResponseBody>
    {
        [MessageBodyMember(Name = "GetProductListResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public GetProductListResponseBody Response { set; get; }

        public GetProductListResponse()
        {
            Response = new GetProductListResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "GetProductListResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetProductListResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AGenericApiResponse
    {
        [Loggable]
        [DataMember(Order = 3)]
        public ProductList ProductList { set; get; }

        public override string ToString()
        {
            return Utils.logFormat(this);
            //return "GetProductListResponse {ReponseCode=" + base.ResponseCode + ",ResponseMessage=" + base.ResponseMessage + ",TransactionID=" + base.TransactionID + ",ProductList={" + ProductList.ToString() + "}}";
        }
    }

    [CollectionDataContract(Namespace = "http://api.movilway.net/schema/extended", KeyName="ID", ValueName="Description", ItemName="Product")]
    public class ProductList : Dictionary<String, String> 
    {
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            //foreach (var item in this)
            //    sb.Append("Product={Id=" + item.Key + ",Name=" + item.Value + "},");
            //if (sb.Length > 0) sb.Remove(sb.Length - 1, 1);
            sb.Append(String.Concat(this.GetType().Name, " Count ", this.Count));
            return sb.ToString();
        }
    }
}