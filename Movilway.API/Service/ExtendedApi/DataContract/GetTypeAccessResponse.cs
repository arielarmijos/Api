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
    public class GetTypeAccessResponse : IMovilwayApiResponseWrapper<GetTypeAccessResponseBody>
    {
        [MessageBodyMember(Name = "GetValuesResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public GetTypeAccessResponseBody Response{  get; set; }
    }

    [Loggable]
    [DataContract(Name = "GetValuesResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetTypeAccessResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AGenericApiResponse
   {
       

        [Loggable]
        [DataMember(Order = 3)]
        public ValuesList Values { set; get; }


        public GetTypeAccessResponseBody()
        {
            Values = new ValuesList();
        }
   }


    [CollectionDataContract(Namespace = "http://api.movilway.net/schema/extended", ItemName = "Value")]
    public class ValuesList : List<TipoAcceso>
    {
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
        
            sb.Append(String.Concat(this.GetType().Name, " Count ", this.Count));
            return sb.ToString();
          //  return Utils.logFormat(this);
        }
    }

  
}