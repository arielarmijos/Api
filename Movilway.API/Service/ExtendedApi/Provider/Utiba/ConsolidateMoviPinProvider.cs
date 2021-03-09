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
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Utiba, ServiceName = ApiServiceName.ConsolidateMoviPin)]
    public class ConsolidateMoviPinProvider : IServiceProvider
    {
        private static readonly ILogger Logger = LoggerFactory.GetLogger(typeof(ConsolidateMoviPinProvider));
        private readonly String _consolidateUser;
        private readonly String _consolidatePassword;
        private readonly String _redeemUser;
        private readonly String _redeemPassword;
        
        public ConsolidateMoviPinProvider()
        {
            _consolidateUser = ConfigurationManager.AppSettings["ConsolidateUser"];
            _consolidatePassword = ConfigurationManager.AppSettings["ConsolidatePassword"];
            _redeemUser = ConfigurationManager.AppSettings["RedeemUser"];
            _redeemPassword = ConfigurationManager.AppSettings["RedeemPassword"];
        }

        public IMovilwayApiResponse PerformOperation(IMovilwayApiRequest requestObject)
        {
            var request = requestObject as ConsolidateMoviPinRequestBody;
            
            Logger.BeginLow(() => TagValue.New().Tag("Request").Value(request));


            var response = new ConsolidateMoviPinResponseBody
            {
                ResponseCode = 0,
                ResponseMessage = "Sus Movipins han sido consolidados.",
                TransactionID = 0,
                MoviPins = new MoviPins()
            };


            if (request != null)
            {
                decimal consolidatedAmount = 0;
                string recipient = null;
                var pines = new List<MoviPinDetails>();

                // Busco el detalle de cada pin recibido
                foreach (var movipin in request.MoviPins)
                    pines.Add(Utils.GetMoviPinDetails(movipin.Number));


                // Reviso que no existan pines repetidos
                if (pines.Select(m => m.Number).Distinct().Count() != pines.Count()) 
                {
                    response.ResponseCode = 99;
                    response.ResponseMessage = "Existen pines repetidos en la solicitud";
                    Logger.CheckPointLow(() => TagValue.New().Tag("Response").Value(response));
                    return (response);
                }

                // Reviso que exista más de un pin válido, ya que no tiene sentido validar un solo pin
                if (pines.Count(p => p.IsValid == true) <= 1)
                {
                    response.ResponseCode = 99;
                    response.ResponseMessage = "Existen menos de dos pines validos";
                    Logger.CheckPointLow(() => TagValue.New().Tag("Response").Value(response));
                    return (response);
                }

                // Reviso que todos los pines sean del mismo user
                if (pines.Select(p => p.Agent).Distinct().Count() != 1)
                {
                    response.ResponseCode = 99;
                    response.ResponseMessage = "No todos los pines son del mismo usuario";
                    Logger.CheckPointLow(() => TagValue.New().Tag("Response").Value(response));
                    return (response);
                }

                // Seteo el agente a utilizar más adelante al crear el nuevo pin consolidado
                recipient = pines.First().Agent;

                // Reviso que el pin a crear sea de un monto válido
                if (pines.Sum(p => p.RemainingAmount) == 0)
                {
                    response.ResponseCode = 99;
                    response.ResponseMessage = "No puede crearse un nuevo PIN por cero D2";
                    Logger.CheckPointLow(() => TagValue.New().Tag("Response").Value(response));
                    return (response);
                }


                // En esta parte se redimen uno a uno los pines
                foreach (var movipin in request.MoviPins)
                {
                    if (pines.Single(p => p.Number == movipin.Number).IsValid != null && pines.Single(p => p.Number == movipin.Number).IsValid.Value)
                    {
                        var movipaymentRequest = new MoviPaymentRequestBody
                                                        {
                                                            AuthenticationData = new AuthenticationData()
                                                                                    {
                                                                                        Username = _redeemUser,
                                                                                        Password = _redeemPassword
                                                                                    },
                                                            DeviceType = request.DeviceType,
                                                            Amount = pines.Single(p => p.Number == movipin.Number).RemainingAmount.Value,
                                                            ExternalTransactionReference = "",
                                                            MoviPin = movipin.Number
                                                        };
                        var redeeemResponse = new ServiceExecutionDelegator
                            <MoviPaymentResponseBody, MoviPaymentRequestBody>().
                            ResolveRequest(movipaymentRequest, ApiTargetPlatform.Utiba, ApiServiceName.MoviPayment);

                        if (redeeemResponse.ResponseCode.Value == 0)
                            consolidatedAmount += pines.Single(p => p.Number == movipin.Number).RemainingAmount.Value;
                        else
                        {
                            movipin.IsValid = false;
                            movipin.Agent = null;
                            movipin.RemainingAmount = null;
                        }
                    }
                    response.MoviPins.Add(pines.Single(p => p.Number == movipin.Number));
                }

                // Acá procedemos a crear el nuevo pin por el monto que redimimos
                if (consolidatedAmount > 0)
                {
                    var createMoviPinRequest = new CreateMoviPinRequestBody()
                    {
                        AuthenticationData = new AuthenticationData()
                        {
                            Username = _consolidateUser,
                            Password = _consolidatePassword
                        },
                        Amount = consolidatedAmount,
                        DeviceType = request.DeviceType,
                        ExternalTransactionReference = "",
                        Recipient = recipient
                    };
                    var createMoviPinResponse = new ServiceExecutionDelegator
                                <CreateMoviPinResponseBody, CreateMoviPinRequestBody>().
                                ResolveRequest(createMoviPinRequest, ApiTargetPlatform.Utiba, ApiServiceName.CreateMoviPin);

                    if (createMoviPinResponse.ResponseCode.Value == 0)
                    {
                        response.ConsolidatedMoviPin = createMoviPinResponse.MoviPin;
                        response.ConsolidatedAmount = consolidatedAmount;
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
                    response.ResponseMessage = "El pin se quería generar con monto cero";
                }
            }

            Logger.CheckPointLow(() => TagValue.New().Tag("Response").Value(response));
            return (response);
        }
    }

}