using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.ServiceModel;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging.Attribute;

namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [MessageContract(IsWrapped=false)]
    public class TransferRequest : IMovilwayApiRequestWrapper<TransferRequestBody>
    {
        [MessageBodyMember(Name = "TransferRequest")]
        public TransferRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "TransferRequest", Namespace="http://api.movilway.net/schema/extended")]
    public class TransferRequestBody : ASecuredFinancialApiRequest
    {
        [Loggable]
        [DataMember(Order = 5, IsRequired = true)]
        public WalletType WalletType { set; get; }

        [Loggable]
        [DataMember(Order = 6, IsRequired = true)]
        public String Recipient { set; get; }

        [Loggable]
        [DataMember(Order = 7, IsRequired = false)]
        public String RecipientAccessId { set; get; }

        [Loggable]
        [DataMember(Order = 8, IsRequired = false)]
        public String RecipientPdv { set; get; }

        [Loggable]
        [DataMember(Order = 9, IsRequired = false)]
        public String RequestDate { set; get; }

        [Loggable]
        [DataMember(Order = 10, IsRequired = false)]
        public String Comment { set; get; }

        [Loggable]
        [DataMember(Order = 11, IsRequired = false)]
        public bool  DBTransferIfChild{ set; get; }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }

        public override int GetHashCode()
        {

            int hash = 99907;
            int secondPrime = 67619;
            //NO SE HACE VALIDACION DE VALORES NULOS DADO QUE LOS ATRIBUTOS SON OBLIGATORIOS
            hash = hash * secondPrime + AuthenticationData.GetHashCode();
            hash = hash * secondPrime + DeviceType.GetHashCode();
            hash = hash * secondPrime + Amount.GetHashCode();
            hash = hash * secondPrime + ExternalTransactionReference.GetHashCode();
            hash = hash * secondPrime + Recipient.GetHashCode();
            hash = hash * secondPrime + WalletType.GetHashCode();
            hash = hash * secondPrime + (RecipientAccessId == null ? 0 : RecipientAccessId.GetHashCode());
            hash = hash * secondPrime + (RecipientPdv == null ? 0 : RecipientPdv.GetHashCode());
            hash = hash * secondPrime + (RequestDate == null ? 0 : RequestDate.GetHashCode());
            hash = hash * secondPrime + (Comment == null ? 0 : Comment.GetHashCode());
            return hash;
        }
    }
}