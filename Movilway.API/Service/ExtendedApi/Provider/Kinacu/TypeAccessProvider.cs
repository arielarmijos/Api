using Movilway.API.Core;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
namespace Movilway.API.Service.ExtendedApi.Provider.Kinacu
{

    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.GetTypeAccess)]
    public class TypeAccessProvider : AKinacuProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(TypeAccessProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        /// <summary>
        /// Retorna todos los accesos existentes en el sistema
        /// Precondicion:
        /// Existen agentes en el sistema
        /// Postcondicion:
        /// Se retorna la lista de accesos en el sistema
        /// </summary>
        /// <param name="requestObject"></param>
        /// <param name="kinacuWS"></param>
        /// <param name="sessionID"></param>
        /// <returns>IMovilwayApiResponse Con los accesos disponibles en el sistema</returns>
        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, KinacuWebService.SaleInterface kinacuWS, string sessionID)
        {
            GetTypeAccessResponseBody response = new GetTypeAccessResponseBody();

            if (sessionID.Equals("0"))
            {
                response.ResponseCode = 90;
                response.ResponseMessage = "ERROR DE SESSION";
                return response;
            }

            GetValuesRequestBody request = requestObject as GetValuesRequestBody;
            try
            {
                bool Extended = request.ExtendedValues ?? false;

                List<TipoAcceso> lista = Utils.GetTipoAccess(Extended);
                response.Values.AddRange(lista);
                response.ResponseCode = 0;
                response.ResponseMessage = "OK";
                response.TransactionID = 0;
            }
            catch (Exception ex)
            {
                response.ResponseCode = 500;
                response.ResponseMessage = "ERROR INESPERADO " + ex.Message;
                response.TransactionID = 0;

                
            }

            return response;
        }
    }
}