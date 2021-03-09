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
    public class TransferCommissionRequest : IMovilwayApiRequestWrapper<TransferCommissionRequestBody>
    {
        [MessageBodyMember(Name = "TransferCommissionRequest")]
        public TransferCommissionRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "TransferCommissionRequest", Namespace="http://api.movilway.net/schema/extended")]
    public class TransferCommissionRequestBody : ASecuredFinancialApiRequest
    {
        [Loggable]
        [DataMember(Order = 5, IsRequired = true)]
        public WalletType WalletType { set; get; }

        /// <summary>
        /// Id de la agencia ala que se le asigna la comision
        /// </summary>
        [Loggable]
        [DataMember(Order = 6, IsRequired = true)]
        public String Recipient { set; get; }

        /// <summary>
        /// Pdv de la agencia a la que se le asgina la comison
        /// </summary>
        [Loggable]
        [DataMember(Order = 8, IsRequired = false)]
        public String RecipientPdv { set; get; }

        /// <summary>
        /// Fecha de la transaferencia
        /// </summary>
        [Loggable]
        [DataMember(Order = 9, IsRequired = false)]
        public String RequestDate { set; get; }

        /// <summary>
        /// Comentario corresponidente a la comision a realizar
        /// </summary>        
        [Loggable]
        [DataMember(Order = 10, IsRequired = false)]
        public String Comment { set; get; }

        /// <summary>
        /// Campo manejado por proesa para evitar procesar la trasnaccion por base de datos
        /// </summary>
        [Loggable]
        [DataMember(Order = 11, IsRequired = false)]
        public bool  DBTransferIfChild{ set; get; }

        [Loggable]
        [DataMember(Order = 12, IsRequired = false)]
        public TransferRequestType TransferRequestType { set; get; }


        /// <summary>
        /// Comentario corresponidente a la comision a realizar
        /// </summary>        
        ////[Loggable]
        ////[DataMember(Order = 13, IsRequired = false)]
        public int Code { set; get; }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }
        
    }

    [DataContract(Name = "TransferRequestType")]
    public enum TransferRequestType
    {
        [EnumMember]
        FromParentToBranch,
        [EnumMember]
        FromRootToBranch,
       
    }
}