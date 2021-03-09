using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Movilway.API.Utiba;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging;

namespace Movilway.API.Service.ExtendedApi.Provider.Utiba
{
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Utiba, ServiceName = ApiServiceName.RegenerateMoviPin)]
    public class RegenerateMoviPinProvider : IServiceProvider
    {
        private static readonly ILogger Logger = LoggerFactory.GetLogger(typeof(RegenerateMoviPinProvider));
        private readonly String _consolidateUser;
        private readonly String _consolidatePassword;
        private readonly String _redeemUser;
        private readonly String _redeemPassword;
        
        public RegenerateMoviPinProvider()
        {
            _consolidateUser = ConfigurationManager.AppSettings["ConsolidateUser"];
            _consolidatePassword = ConfigurationManager.AppSettings["ConsolidatePassword"];
            _redeemUser = ConfigurationManager.AppSettings["RedeemUser"];
            _redeemPassword = ConfigurationManager.AppSettings["RedeemPassword"];
        }

        public IMovilwayApiResponse PerformOperation(IMovilwayApiRequest requestObject)
        {
            var request = requestObject as RegenerateMoviPinRequestBody;
            
            Logger.BeginLow(() => TagValue.New().Tag("Request").Value(request));


            var response = new RegenerateMoviPinResponseBody
            {
                ResponseCode = 0,
                ResponseMessage = "Su Movipin ha sido regenerado.",
                TransactionID = 0
            };


            if (request != null)
            {
                decimal regeneratedAmount = 0;
                string recipient = null;
                var pin = new MoviPinDetails();

                // Busco el detalle de cada pin recibido
                pin = Utils.GetMoviPinDetails(request.TransactionNumber);

                // Reviso que exista más de un pin válido, ya que no tiene sentido validar un solo pin
                if (!(pin.IsValid ?? false))
                {
                    response.ResponseCode = 99;
                    response.ResponseMessage = "Pin invalido";
                    Logger.CheckPointLow(() => TagValue.New().Tag("Response").Value(response));
                    return (response);
                }

                // Seteo el agente a utilizar más adelante al crear el nuevo pin regenerado
                recipient = pin.Agent;

                // En esta parte se redimen uno a uno los pines
                var movipaymentRequest = new MoviPaymentRequestBody
                                                {
                                                    AuthenticationData = new AuthenticationData()
                                                                            {
                                                                                Username = _redeemUser,
                                                                                Password = _redeemPassword
                                                                            },
                                                    DeviceType = request.DeviceType,
                                                    Amount = pin.RemainingAmount.Value,
                                                    ExternalTransactionReference = "",
                                                    MoviPin = pin.Number
                                                };
                var redeeemResponse = new ServiceExecutionDelegator
                    <MoviPaymentResponseBody, MoviPaymentRequestBody>().
                    ResolveRequest(movipaymentRequest, ApiTargetPlatform.Utiba, ApiServiceName.MoviPayment);

                if (redeeemResponse.ResponseCode.Value == 0)
                    regeneratedAmount = pin.RemainingAmount.Value;
                else
                {
                    pin.IsValid = false;
                    pin.Agent = null;
                    pin.RemainingAmount = null;
                }

                // Acá procedemos a crear el nuevo pin por el monto que redimimos
                if (regeneratedAmount > 0)
                {
                    var createMoviPinRequest = new CreateMoviPinRequestBody()
                    {
                        AuthenticationData = new AuthenticationData()
                        {
                            Username = _consolidateUser,
                            Password = _consolidatePassword
                        },
                        Amount = regeneratedAmount,
                        DeviceType = request.DeviceType,
                        ExternalTransactionReference = "",
                        Recipient = recipient
                    };
                    var createMoviPinResponse = new ServiceExecutionDelegator
                                <CreateMoviPinResponseBody, CreateMoviPinRequestBody>().
                                ResolveRequest(createMoviPinRequest, ApiTargetPlatform.Utiba, ApiServiceName.CreateMoviPin);

                    if (createMoviPinResponse.ResponseCode.Value == 0)
                    {
                        response.RegeneratedMoviPin = createMoviPinResponse.MoviPin;
                        response.RegeneratedAmount = regeneratedAmount;
                        response.ExpiryDate = createMoviPinResponse.ExpiryDate;
                    }
                    else
                    {
                        response.ResponseCode = 99;
                        response.ResponseMessage = "Operacion fallida";
                    }
                }
                else
                {
                    response.ResponseCode = 99;
                    response.ResponseMessage = "El pin se quería regenerar con monto cero";
                }
            }

            Logger.CheckPointLow(() => TagValue.New().Tag("Response").Value(response));
            return (response);
        }
    }

}