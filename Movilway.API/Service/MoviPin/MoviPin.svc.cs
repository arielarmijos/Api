using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Movilway.API.Utiba;
using System.ServiceModel.Channels;
using System.Net;
using Movilway.API.Service.Internal;
using Movilway.API.Service.External;
using Movilway.API.Service.MoviPin.External;
using Movilway.API.Service.MoviPin.Internal;
using System.Configuration;

namespace Movilway.API.Service.MoviPin
{
    [ServiceBehavior(Namespace = "http://api.movilway.net", ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
    public class MoviPin : ApiServiceBase, IMoviPin
    {
        private int _defaultCouponTypeMoviPayment;
        
        public MoviPin()
        {
            _defaultCouponTypeMoviPayment = int.Parse(ConfigurationManager.AppSettings["DefaultCouponType"]);
        }
        public GetSessionResponse GetSession(GetSessionRequest getSessionRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Movipin.GetSession", Logger.LoggingLevelType.Medium);
            return (AuthenticationProvider.GetSession(getSessionRequest));
            //Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Sales.GetSession", Logger.LoggingLevelType.Medium);
        }
        public LoginResponse Login(LoginRequest loginRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método MoviPin.Login", Logger.LoggingLevelType.Medium);
            LoginResponse response = null;
            try
            {
                Log(Logger.LogMessageType.Info, String.Format("Llamando a IMoviPin.Login con los parametros: User={0}, Password={1}, DeviceType={2}", loginRequest.Request.AccessId, loginRequest.Request.Password, loginRequest.Request.AccessType), Logger.LoggingLevelType.Low);
                response = AuthenticationProvider.Login(loginRequest);
                Log(Logger.LogMessageType.Info, String.Format("Parametros de respuesta de IMoviPin.Login: LoginResult={0}, Message={1} ", response.Response.LoginResult, response.Response.Message), Logger.LoggingLevelType.Low);
            }
            catch (Exception e)
            {
                Log(Logger.LogMessageType.Error, "Excepcion en el metodo IMoviPin.Login: " + e.ToString(), Logger.LoggingLevelType.Low);
            }
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método MoviPin.Login", Logger.LoggingLevelType.Medium);
            return (response);
        }

        public MoviPaymentResponse MoviPayment(MoviPaymentRequest movipaymentRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método MoviPin.MoviPayment", Logger.LoggingLevelType.Medium);
            MoviPaymentRequestInternal internalObject = new MoviPaymentRequestInternal()
            {
                Amount = movipaymentRequest.Request.Amount,
                Type = movipaymentRequest.Request.Type,
                DeviceType = movipaymentRequest.Request.DeviceType,
                SessionID = movipaymentRequest.Request.SessionID,
                CouponID = movipaymentRequest.Request.CouponID

            };
            MoviPaymentResponseInternal internalResponse = MoviPaymentInternal(internalObject);
            MoviPaymentResponse response = new MoviPaymentResponse();
            MoviPaymentResponseBody responseBody = new MoviPaymentResponseBody()
            {

                ResponseCode = internalResponse.ResponseCode,
                ResponseMessage = internalResponse.ResponseMessage,
                TransactionID = internalResponse.TransactionID,
                Fee = internalResponse.Fee,
                ResultNameSpace = internalResponse.ResultNameSpace,
                ScheduleID = internalResponse.ScheduleID,
                TransExtReference = internalResponse.TransExtReference

            };
            response.Response = responseBody;
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método MoviPin.MoviPayment", Logger.LoggingLevelType.Medium);
            return (response);
        }

        public MoviPaymentExtendedResponse MoviPaymentExtended(MoviPaymentExtendedRequest movipaymentExtendedRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método MoviPin.MoviPaymentExtended", Logger.LoggingLevelType.Medium);
            LoginRequestInternal loginRequest = new LoginRequestInternal()
            {
                DeviceType = movipaymentExtendedRequest.Request.DeviceType,
                Password = movipaymentExtendedRequest.Request.Password,
                User = movipaymentExtendedRequest.Request.Username
            };
            LoginResponseInternal loginResponse = AuthenticationProvider.LoginInternal(loginRequest);

            MoviPaymentRequestInternal internalObject = new MoviPaymentRequestInternal()
            {
                Amount = movipaymentExtendedRequest.Request.Amount,
                Type = movipaymentExtendedRequest.Request.Type,
                DeviceType = movipaymentExtendedRequest.Request.DeviceType,
                SessionID = loginResponse.SessionID,
                CouponID = movipaymentExtendedRequest.Request.CouponID

            };
            MoviPaymentResponseInternal internalResponse = MoviPaymentInternal(internalObject);
            MoviPaymentExtendedResponse response = new MoviPaymentExtendedResponse();
            MoviPaymentExtendedResponseBody responseBody = new MoviPaymentExtendedResponseBody()
            {

                ResponseCode = internalResponse.ResponseCode,
                ResponseMessage = internalResponse.ResponseMessage,
                TransactionID = internalResponse.TransactionID,
                Fee = internalResponse.Fee,
                ResultNameSpace = internalResponse.ResultNameSpace,
                ScheduleID = internalResponse.ScheduleID,
                TransExtReference = internalResponse.TransExtReference

            };
            response.Response = responseBody;
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método MoviPin.MoviPaymentExtended", Logger.LoggingLevelType.Medium);
            return (response);
        }

        #region Internal Methods
        internal MoviPaymentResponseInternal MoviPaymentInternal(MoviPaymentRequestInternal request)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método MoviPin.MoviPaymentInternal", Logger.LoggingLevelType.Medium);
            UMarketSCClient utibaClient = new UMarketSCClient();
            MoviPaymentResponseInternal movipaymentResult;
            using (OperationContextScope scope = new OperationContextScope(utibaClient.InnerChannel))
            {
                HttpRequestMessageProperty messageProperty = new HttpRequestMessageProperty();
                messageProperty.Headers.Add(HttpRequestHeader.UserAgent, UserAgent);
                OperationContext.Current.OutgoingMessageProperties.Add(HttpRequestMessageProperty.Name, messageProperty);
                Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Parámetros Recibidos MoviPin.MoviPaymentInternal: SessionID={0}, DeviceType={1}, Amount={2}, " +
                "CouponID={3}, Type={4}", request.SessionID, request.DeviceType, request.Amount, request.CouponID, request.Type), Logger.LoggingLevelType.Low);
                Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Parámetros Enviados MoviPin.MoviPaymentInternal: SessionID={0}, DeviceType={1}, Amount={2}, " +
                "CouponID={3}, Type={4}", request.SessionID, request.DeviceType, request.Amount, request.CouponID, request.Type), Logger.LoggingLevelType.Low);
                coupontransferResponse myMoviPayment = utibaClient.coupontransfer(new Utiba.coupontransfer() { coupontransferRequestType = new coupontransferRequestType() { sessionid = request.SessionID, device_type = request.DeviceType, amount = request.Amount, couponid = request.CouponID, type = request.Type, typeSpecified = true } });
                movipaymentResult = new MoviPaymentResponseInternal()
                {
                    ResponseCode = myMoviPayment.coupontransferReturn.result,
                    ResponseMessage = myMoviPayment.coupontransferReturn.result_message,
                    TransactionID = myMoviPayment.coupontransferReturn.transid,
                    Fee = myMoviPayment.coupontransferReturn.fee,
                    ResultNameSpace = myMoviPayment.coupontransferReturn.result_namespace,
                    ScheduleID = myMoviPayment.coupontransferReturn.schedule_id,
                    TransExtReference = myMoviPayment.coupontransferReturn.trans_ext_reference
                };
                Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Resultado Obtenido MoviPin.MoviPaymentInternal: ResponseCode={0}, ResponseMessage={1}, TransactionID={2}, " +
                    "Fee={3}, ResultNameSpace={4}, ScheduleID={5}, TransExtReference={6}", movipaymentResult.ResponseCode, movipaymentResult.ResponseMessage, movipaymentResult.TransactionID,
                     movipaymentResult.Fee, movipaymentResult.ResultNameSpace, movipaymentResult.ScheduleID, movipaymentResult.TransExtReference), Logger.LoggingLevelType.Low);
            }
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método MoviPin.MoviPaymentInternal", Logger.LoggingLevelType.Medium);
            return movipaymentResult;
        }
    #endregion
    }
}
