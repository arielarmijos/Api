using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Xml.Serialization;

namespace Movilway.API.Service.ExtendedApi.DataContract.Payment
{
    public class GetBanklistResponse: AGenericApiResponse
    {
         /// <summary>
        /// Initializes a new instance of the <see cref="GetBanklistResponse" /> class.
        /// </summary>
        public GetBanklistResponse()
            : base()
        {
            this.ResponseMessage = string.Empty;
            BankList = new List<Bank>();
        }

        /// <summary>
        /// Gets or sets Banks
        /// </summary>
        [Loggable]
        [DataMember(Order = 3), XmlElement]
        public List<DataContract.Payment.Bank> BankList { get; set; }

        /// <summary>
        /// Personalización ToString para logs
        /// </summary>
        /// <returns><c>string</c> personalizado</returns>
        public override string ToString()
        {
            return Movilway.API.Service.ExtendedApi.DataContract.Utils.logFormat(this);
        }
    }
}