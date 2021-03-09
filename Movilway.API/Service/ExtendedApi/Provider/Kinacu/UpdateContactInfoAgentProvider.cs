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
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.UpdateContactInfoAgent)]
    public class UpdateContactInfoAgentProvider : AKinacuProvider
    {
        private String _GenericError = "NO SE PUDO ACTUALIZAR LA INFORMACION DE CONTACTO DEL AGENTE";

        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(UpdateAgentProvider));
        protected override ILogger ProviderLogger { get { return logger; } }


        /// <summary>
        /// Actualiza la informacion de contacto de un agente en el sistema
        /// </summary>
        /// <param name="requestObject"></param>
        /// <param name="kinacuWS"></param>
        /// <param name="sessionID"></param>
        /// <returns></returns>
        /// <returns>Retorna el tipo IMovilwayApiResponse con el respectivo codigo de error y mensaje</returns>
        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, KinacuWebService.SaleInterface kinacuWS, string sessionID)
        {

            UpdateContactInfoAgentResponseBody response = new UpdateContactInfoAgentResponseBody();
            try
            {
                if (sessionID.Equals("0"))
                    return new UpdateContactInfoAgentResponseBody()
                    {
                        ResponseCode = 90,
                        ResponseMessage = "error session",
                        TransactionID = 0
                    };

                UpdateContactInfoAgentRequestBody request = requestObject as UpdateContactInfoAgentRequestBody;

                logger.InfoLow(() => TagValue.New().Message("[API] " + base.LOG_PREFIX + "[UpdateContactInfoAgentProvider]").Tag("[RCV-DATA] UpdateContactInfoAgentProviderParameters ").Value(request));

                var result = Utils.ChangeBranchContactInfo(request.AgeId.ToString(), request.Email, request.Phone);

                if (result)
                    response = new UpdateContactInfoAgentResponseBody
                    {
                        ResponseCode = 0,
                        ResponseMessage = "Exito",
                        TransactionID = new Random().Next(10000000, 99999999)
                    };
                else
                    response = new UpdateContactInfoAgentResponseBody
                    {
                        ResponseCode = 99,
                        ResponseMessage = _GenericError
                    };
            }
            catch (Exception ex)
            {
                //cambiar error general erro inesperado
                response.ResponseCode = 500;
                response.ResponseMessage = _GenericError;
                response.TransactionID = 0;
                string mensaje = String.Concat("[API] " + base.LOG_PREFIX + "[UpdateContactInfoAgentProvider] ", ". Exception: ", ex.Message, ". ", ex.StackTrace);
                logger.ErrorLow(mensaje);
            }

            logger.InfoLow(() => TagValue.New().Message("[API] " + base.LOG_PREFIX).Tag("[UpdateContactInfoAgentProviderResult]").Value(response));
            return (response);
        }


    }
}