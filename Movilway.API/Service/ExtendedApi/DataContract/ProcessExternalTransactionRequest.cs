using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.ServiceModel;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging.Attribute;
using System.Text;

namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [MessageContract(IsWrapped=false)]
    public class ProcessExternalTransactionRequest : IMovilwayApiRequestWrapper<ProcessExternalTransactionRequestBody>
    {
        [MessageBodyMember(Name = "ProcessExternalTransactionRequest")]
        public ProcessExternalTransactionRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "ProcessExternalTransactionRequest", Namespace = "http://api.movilway.net/schema/extended")]
    public class ProcessExternalTransactionRequestBody : ASecuredFinancialApiRequest
    {
        [Loggable]
        [DataMember(Order = 5, IsRequired = true, EmitDefaultValue = false)]
        public String Agent { set; get; }

        [Loggable]
        [DataMember(Order = 6, IsRequired = true)]
        public String TargetEntity { set; get; }

        [Loggable]
        [DataMember(Order = 7, IsRequired = true)]
        public String WalletType { set; get; }

        [Loggable]
        [DataMember(Name = "AdditionalExtraData", Order = 8, IsRequired = false, EmitDefaultValue = false)]
        public AdditionalExtraDataList AdditionalData { get; set; }
    }

    [CollectionDataContract(Namespace = "http://api.movilway.net/schema/extended", ItemName = "AdditionalExtraData",
        KeyName = "Key", ValueName = "Value")]
    public class AdditionalExtraDataList : Dictionary<String, String>
    {
        public override string ToString()
        {
            var sb = new StringBuilder();//"{");
            //int count = 0;
            //foreach (var item in this)
            //{
            //    sb.Append(item.Key).Append("=").Append(item.Value);
            //    if (count > 1)
            //        sb.Append("|");
            //    count++;
            //}
            //sb.Append("}");
            sb.Append(String.Concat(this.GetType().Name, " Count ", this.Count));
            return (sb.ToString());
        }
    }
}