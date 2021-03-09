using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;

namespace Movilway.API.Service.ExtendedApi.Provider.Payment
{
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Payment, ServiceName = ApiServiceName.Payment)]
    internal partial class PaymentProvider : AGenericPlatformAuthentication
    {

        /// <summary>
        /// Variable para almacenar todos los mensajes de retorno de los diferentes llamados
        /// </summary>
        private ErrorMessagesMnemonics errorMessage = ErrorMessagesMnemonics.None;

        private string PSEUSer;
        private string PSEPassword;
        private string PSEMD5Key;

        /// <summary>
        /// Gets or sets culture
        /// </summary>
        private System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("es-CO");

        /// <summary>
        /// Initializes static members of the <see cref="PaymentProvider" /> class.
        /// </summary>
        static PaymentProvider()
        {
            PaymentProvider.logger = LoggerFactory.GetLogger(typeof(PaymentProvider));
        }

        public PaymentProvider() {

            this.PSEUSer = ConfigurationManager.AppSettings["PSEUSer"];
            this.PSEPassword = ConfigurationManager.AppSettings["PSEPassword"];
            this.PSEMD5Key = ConfigurationManager.AppSettings["PSEMD5Key"];

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        
        }
    }
}