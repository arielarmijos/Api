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
    public class TopUpRequest:IMovilwayApiRequestWrapper<TopUpRequestBody>
    {
        [MessageBodyMember(Name = "TopUpRequest")]
        public TopUpRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "TopUpRequest", Namespace="http://api.movilway.net/schema/extended")]
    public class TopUpRequestBody : ASecuredFinancialApiRequest,IEquatable<TopUpRequestBody>
    {
        [Loggable]
        [DataMember(Order = 5, IsRequired = true)]
        public String MNO { set; get; }

        [Loggable]
        [DataMember(Order = 6, IsRequired = true)]
        public String Recipient { set; get; }

        [Loggable]
        [DataMember(Order = 7, IsRequired = false, EmitDefaultValue = false)]
        public WalletType WalletType { set; get; }

        [Loggable]
        [DataMember(Order = 8, IsRequired = false)]
        public String TerminalID { set; get; }

        public override string ToString()
        {
            return Utils.logFormat(this);
            //return "TopUpRequest {AuthenticationData{Username=" + base.AuthenticationData.Username + ",Password=******,SessionId=" + base.AuthenticationData.SessionID + "}" +
            //        ",DeviceType=" + base.DeviceType + ",Platform=" + base.Platform + ",Amount=" + base.Amount + ",ExternalTransactionReference=" + base.ExternalTransactionReference + 
            //        ",MNO=" + this.MNO + ",Recipient=" + this.Recipient + ",WalletType=" + this.WalletType + ",TerminalID=" + this.TerminalID + "}";
        }


        public override bool Equals(object obj)
        {
           TopUpRequestBody objval = obj as TopUpRequestBody;

           if (obj != null)
               return Equals(objval);
           else
               return false;
        }

        public bool Equals(TopUpRequestBody other)
        {
            if (other == null)
                return false;

            return  AuthenticationData.Equals(other.AuthenticationData) &&
                      DeviceType.Equals(other.DeviceType) &&
                      Amount == other.Amount &&
                      String.Equals(ExternalTransactionReference, other.ExternalTransactionReference) &&
                      String.Equals(MNO, other.MNO) &&
                      String.Equals(Recipient, other.Recipient);
        }

        public override int GetHashCode()
        {

            int hash = 100003;
            int secondPrime = 70001;
            //NO SE HACE VALIDACION DE VALORES NULOS DADO QUE LOS ATRIBUTOS SON OBLIGATORIOS
            hash = hash * secondPrime + AuthenticationData.GetHashCode();
            hash = hash * secondPrime + DeviceType.GetHashCode();
            hash = hash * secondPrime + Amount.GetHashCode();
            hash = hash * secondPrime + ExternalTransactionReference.GetHashCode();
            hash = hash * secondPrime + MNO.GetHashCode();
            hash = hash * secondPrime + Recipient.GetHashCode();
            return hash;
        }
    }
}