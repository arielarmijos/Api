using System;
using System.Web;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.Logging;
using Movilway.API.Data;
using Movilway.API.KinacuWebService;
using System.Globalization;
using System.Configuration;

namespace Movilway.API.Service.ExtendedApi.Provider.Kinacu
{
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.GetTransaction)]
    public class GetTransactionProvider : AKinacuProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GetTransactionProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        private static readonly Movilway.Cache.ICache _cacheSaldo = null;

        private static bool _GetBalance;



        static GetTransactionProvider()
        {
            _cacheSaldo = Cache.CacheFactory.FactoryCache("GETTRAN_BALANCE");

            bool auxgettean = false;

            if (Boolean.TryParse(ConfigurationManager.AppSettings["GETTRANSACTION_BALANCE"] ?? "", out auxgettean))
                _GetBalance = auxgettean;
            else
                _GetBalance = false;

        }


        public GetTransactionProvider()
        {








        }

        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, SaleInterface kinacuWS, string sessionID)
        {

            if (sessionID.Equals("0"))
                return new GetTransactionResponseBody()
                {
                    ResponseCode = 90,
                    ResponseMessage = "error session",
                    TransactionID = 0,
                    Amount = 0m,
                    Creditor = "",
                    Debtor = "",
                    Initiator = "",
                    OriginalTransactionId = "",
                    Recipient = "",
                    TransactionDate = DateTime.Now,
                    TransactionType = ""
                };

            GetTransactionRequestBody request = requestObject as GetTransactionRequestBody;
            GetTransactionResponseBody response = null;


            try
            {
                SaleData saleData = null;
                string message = "", methodName = "saleState";
                switch (request.ParameterType)
                {
                    case GetTransactionRequestParameterType.ExternalTransactionReference:
                        methodName = "saleStateByExternalId";
                        logger.InfoLow("[KIN] " + base.LOG_PREFIX + "[GetTransactionProvider] [SEND-DATA] saleStateByExternalIdParameters {UserId=" + sessionID + ",ExternalId=" + request.ParameterValue + "}");
                        saleData = kinacuWS.SaleStateByExternalId(int.Parse(sessionID), request.ParameterValue, out message);
                        response = MapKinacuTransactionResponseToGetTransactionResponseBody(saleData, message);
                        break;

                    case GetTransactionRequestParameterType.TransactionID:
                        logger.InfoLow("[KIN] " + base.LOG_PREFIX + "[GetTransactionProvider] [SEND-DATA] saleStateParameters {UserId=" + sessionID + ",IdTransaction=" + request.ParameterValue + ",Target=}");
                        int param;
                        if (int.TryParse(request.ParameterValue, out param))
                        {
                            saleData = kinacuWS.SaleState(int.Parse(sessionID), param, "", out message);
                            response = MapKinacuTransactionResponseToGetTransactionResponseBody(saleData, message);
                        }
                        else
                            response = new GetTransactionResponseBody()
                            {
                                ResponseCode = 91,
                                ResponseMessage = "Transaccion no encontrada",
                                TransactionID = 0,
                                TransactionResult = 0,
                                OriginalTransactionId = "0",
                                TransactionDate = DateTime.Now,
                                TransactionType = "topup"
                            };
                        break;

                    case GetTransactionRequestParameterType.TargetAgent:
                        logger.InfoLow("[KIN] " + base.LOG_PREFIX + "[GetTransactionProvider] [SEND-DATA] saleStateParameters {UserId=" + sessionID + ",IdTransaction=0,Target=" + request.ParameterValue + "}");

                
                        saleData = Utils.GetSaleStateByTargent(request.AuthenticationData.Username, request.ParameterValue, out message); //kinacuWS.SaleState(int.Parse(sessionID), 0, request.ParameterValue, out message);
                        response = MapKinacuTransactionResponseToGetTransactionResponseBody(saleData, message);
                        break;

                    case GetTransactionRequestParameterType.TransactionType:
                        logger.InfoLow("[KIN] " + base.LOG_PREFIX + "[GetTransactionProvider] [SEND-DATA] saleStateParameters {UserId=" + sessionID + ",IdTransaction=0,Target=}");
                        saleData = kinacuWS.SaleState(int.Parse(sessionID), 0, "", out message);
                        response = MapKinacuTransactionResponseToGetTransactionResponseBody(saleData, message);
                        break;

                    case GetTransactionRequestParameterType.LastTransaction:
                        logger.InfoLow("[KIN] " + base.LOG_PREFIX + "[GetTransactionProvider] [SEND-DATA] saleStateParameters {UserId=" + sessionID + ",IdTransaction=0,Target=" + request.ParameterValue + "}");
                        saleData = kinacuWS.SaleState(int.Parse(sessionID), 0, "", out message);
                        response = MapKinacuTransactionResponseToGetTransactionResponseBody(saleData, message);
                        break;

                    case GetTransactionRequestParameterType.DistributionExternalTransactionReference:

                        logger.InfoLow("[KIN] " + base.LOG_PREFIX + "[GetTransactionProvider] [SEND-DATA] getDistributionState {Target=" + request.ParameterValue + "}");


                        response = MapDistributionToGetTransactionResponseBody(request);


                        break;
                }


                if (saleData != null)
                {

                    response.ResponseMessage = Movilway.API.Utils.ApiTicket.FormatSaledata(logger, "GetTransactionProvider", request, base.LOG_PREFIX, response.ResponseCode.Value == 0, response.TransactionID.Value, response.ResponseMessage);

                    logger.InfoLow("[KIN] " + base.LOG_PREFIX + "[GetTransactionProvider] [RECV-DATA] " + methodName + "Result {saleState={IdTransaction=" + saleData.IdTransaccion +
                                                            ",Customer=" + saleData.Customer + ",Amount=" + saleData.Amount + ",Date=" + saleData.Date + ",ReloadState=" + saleData.ReloadState +
                                                            ",ReloadStateCode=" + saleData.ReloadStateCode + "},message=" + message + "}");
                }
                else
                    logger.InfoLow("[KIN] " + base.LOG_PREFIX + "[GetTransactionProvider] [RECV-DATA] " + methodName + "Result {saleState={IdTransaction=,Customer=,Amount=,Date=,ReloadState=,ReloadStateCode=},message=" + message + "}");

                #region se incluye obtiene el saldo en caso de ser necesario


                //Se realiza la validacion afuera del metodo GetBalance 
                //con el fin de no manejar valores nulos en los metodos 
                //y asignar algun valor al triburo StockBlance de ser necesario
                if (
                   _GetBalance &&
                    request.DeviceType == Movilway.API.Core.cons.ACCESS_POS
                    )
                {
                    response.StockBalance = GetBalance(request);
                }
            }
            catch (Exception ex)
            {



                logger.ErrorHigh(() => TagValue.New().MethodName("GetTransaction").Message(base.LOG_PREFIX + " ERROR BUSCANDO TRANSACCION CON REFERENCIA [" + request.ParameterValue + "]").Exception(ex));

                if (ex.InnerException != null)
                {

                    logger.ErrorHigh(() => TagValue.New().MethodName("GetTransaction").Message(base.LOG_PREFIX + " INNER EXCEPTION " + request.ParameterValue).Exception(ex.InnerException));

                }


                response = new GetTransactionResponseBody()
                {
                    ResponseCode = 99,
                    ResponseMessage = "Error inesperado consultando la transaccion, consulte a su administrador.",
                    TransactionID = 0,
                    OriginalTransactionId = "",
                    Amount = 0,
                    Recipient = "",
                    TransactionDate = DateTime.Now,
                    TransactionResult = 99,
                    TransactionType = "",
                    Initiator = "",
                    Debtor = "",
                    Creditor = ""
                };
            }
            #endregion


            return response;
        }

        private GetTransactionResponseBody MapDistributionToGetTransactionResponseBody(GetTransactionRequestBody request)
        {

            GetTransactionResponseBody result = new GetTransactionResponseBody();
            // 
            try
            {
                if (request.AuthenticationData == null || string.IsNullOrEmpty(request.AuthenticationData.Username))
                {
                    result.ResponseCode = 92;
                    result.ResponseMessage = "EL REQUEST DEBE TENER ASIGNADO EL LOGIN DEL USUARIO.";
                    result.TransactionID = 0;
                    result.TransactionResult = 92;
                    result.TransactionType = "Distribution";
                    result.TransactionDate = DateTime.Now;
                    result.Amount = 0;
                    result.Recipient = "";
                    result.Initiator = "";
                    result.Debtor = "";
                    result.Creditor = "";

                    return result;
                }



                logger.InfoLow(base.LOG_PREFIX + "[GetTransactionProvider]  Obtener distribucion de BD Target=" + request.ParameterValue + "");

                var _dictionary = Utils.GetDistributionByExternalIdRequest(request);

                if (_dictionary != null && _dictionary.Count > 0 && _dictionary.Keys.Count > 0)
                {

                    logger.InfoLow(base.LOG_PREFIX + "[GetTransactionProvider] MAPEANDO DATOS");

                    int sprId = Convert.ToInt32(_dictionary["sprId"]);
                    var sprImporteSolicitud = Convert.ToDecimal(_dictionary["sprImporteSolicitud"]);
                    var sprEstado = _dictionary["sprEstado"];
                    var sprFechaAprobacion = (DateTime?)_dictionary["sprFechaAprobacion"];
                    var sltId = _dictionary["sltId"];
                    var envId = _dictionary["envId"];
                    var envEstado = _dictionary["envEstado"];
                    var envFechaEnvio = _dictionary["envFechaEnvio"];
                    var envFechaRecepcion = _dictionary["envFechaRecepcion"];
                    var envNumeroRemito = _dictionary["envNumeroRemito"];
                    var envObservaciones = _dictionary["envObservaciones"];
                    int ageIdBodegaOrigen = Convert.ToInt32(_dictionary["ageIdBodegaOrigen"]);
                    var ageIdBodegaDestino = Convert.ToInt32(_dictionary["ageIdBodegaDestino"]);
                    var AgenteSolicitante = (string)_dictionary["AgenteSolicitante"];
                    var AgenteDestinatario = (string)_dictionary["AgenteDestinatario"];
                    var sltDescripcion = _dictionary["sltDescripcion"];



                    result.ResponseCode = 0;

                    result.TransactionID = sprId;
                    result.OriginalTransactionId = request.ParameterValue;
                    result.TransactionResult = sprEstado.Equals("CE") ? 0 : 99;
                    result.TransactionType = "Distribution";
                    result.TransactionDate = sprFechaAprobacion.Value;
                    result.Amount = sprImporteSolicitud;
                    result.Recipient = AgenteDestinatario;
                    result.Initiator = "";
                    result.Debtor = "";
                    result.Creditor = "";


                    var message = "Distribucion por " + sprImporteSolicitud.ToString("0.##", (CultureInfo.InvariantCulture));

                    message += (result.TransactionResult == 0 ? " exitosa " : " fallida ") + ".";

                    message += "Numero de transaccion " + sprId + ".";
                    message += envObservaciones;

                    result.ResponseMessage = message;

                }
                else
                {
                    result.ResponseCode = 93;
                    result.ResponseMessage = "NO EXISTE DISTRIBUCION CON REFERENCIA " + request.ParameterValue + ".";
                    result.TransactionID = 0;
                    result.TransactionResult = 93;
                    result.TransactionType = "Distribution";
                    result.TransactionDate = DateTime.Now;
                    result.Amount = 0;
                    result.Recipient = "";
                    result.Initiator = "";
                    result.Debtor = "";
                    result.Creditor = "";
                }


            }
            catch (Exception ex)
            {

                logger.ErrorHigh(() => TagValue.New().MethodName("GetDistributionByExternalIdRequest").Message(base.LOG_PREFIX + " ERROR BUSCANDO DISTRIBUCION CON REFERENCIA " + request.ParameterValue).Exception(ex));

                if (ex.InnerException != null)
                {

                    logger.ErrorHigh(() => TagValue.New().MethodName("GetDistributionByExternalIdRequest").Message(base.LOG_PREFIX + " INNER EXCEPTION " + request.ParameterValue).Exception(ex.InnerException));

                }


                result.ResponseCode = 94;
                result.ResponseMessage = "ERROR BUSCANDO DISTRIBUCION CON REFERENCIA " + request.ParameterValue;
                result.TransactionID = 0;
                result.TransactionResult = 94;
                result.TransactionType = "Distribution";
                result.TransactionDate = DateTime.Now;
                result.Amount = 0;
                result.Recipient = "";
                result.Initiator = "";
                result.Debtor = "";
                result.Creditor = "";


            }
            return result;

        }

        private GetTransactionResponseBody MapKinacuTransactionResponseToGetTransactionResponseBody(SaleData saleData, string message)
        {
            if (saleData == null)
                return new GetTransactionResponseBody()
                {
                    ResponseCode = Utils.BuildResponseCode(false, message),
                    ResponseMessage = (String.IsNullOrEmpty(message) ? "error" : message),
                    TransactionID = 0,
                    OriginalTransactionId = "0",
                    Amount = 0,
                    Recipient = "",
                    TransactionDate = DateTime.Now,
                    TransactionResult = 0,
                    TransactionType = "topup",
                    Initiator = "",
                    Debtor = "",
                    Creditor = ""
                };

            return new GetTransactionResponseBody()
            {
                ResponseCode = (saleData.ReloadStateCode.ToLower().Equals("ok") ? 0 : 99),
                ResponseMessage = (String.IsNullOrEmpty(message) ? "Transaccion en error" : (String.IsNullOrEmpty(saleData.ReloadStateCode) ? message : (!saleData.ReloadStateCode.ToLower().Equals("ok") ? ExtractErrorMessage(message) : FormatDate(message)))),
                TransactionID = int.Parse(saleData.IdTransaccion.ToString()),
                OriginalTransactionId = saleData.IdTransaccion.ToString(),
                Amount = saleData.Amount / 100m,
                Recipient = saleData.Customer,
                TransactionDate = saleData.Date,
                TransactionResult = (saleData.ReloadStateCode.ToLower().Equals("ok") ? 0 : 99),
                TransactionType = "buy",
                Initiator = "",
                Debtor = "",
                Creditor = ""
            };
        }

        private string FormatDate(string message)
        {
            try
            {
                var text = "Fecha: ";
                var date = message.Substring(message.IndexOf(text) + text.Length);
                date = date.Substring(0, date.IndexOf(","));
                DateTime myDate = DateTime.ParseExact(date, "MM/dd/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
                message = message.Replace(date, myDate.ToString("yyyy-MM-ddTHH:mm:ss"));
            }
            catch (Exception) { }

            return message;
        }

        private string ExtractErrorMessage(string message)
        {
            if (message.Contains("TransaccionOperadora:"))
            {
                var boundLeft = "TransaccionOperadora:";
                //var boundRight = ";Pdv:";
                var indexLeft = message.IndexOf(boundLeft);
                //var indexRight = message.IndexOf(boundRight);

                var startIndex = indexLeft + boundLeft.Length;
                //var length = indexRight - indexLeft - boundLeft.Length;

                var result = message.Substring(startIndex).Trim();
                if (result.Length > 0)
                    return result.Split(';')[0];
                else
                    return message;
            }
            else
                return String.Concat("Error en recarga. Detalles: ", message);
        }

        private string BuildGetTransactionResponseMessage(SaleData saleData)
        {
            // <add key="611" value="   ::M O V I L W A Y:,Fecha,Pdv,VentaId,TransaccionOperadora,AgenciaNombre,VendedorId,ProductoNombre,Destino,Monto,;========================;  TICKET TRANSACCIONAL; POR FAVOR SOLICITE SU;FACTURA AL PUNTO VENTA;========================;http://www.movilway.com"/>
            return "   ::M O V I L W A Y:,Fecha,Pdv,VentaId,TransaccionOperadora,AgenciaNombre,VendedorId,ProductoNombre,Destino,Monto,;========================;  TICKET TRANSACCIONAL; POR FAVOR SOLICITE SU;FACTURA AL PUNTO VENTA;========================;http://www.movilway.com";
        }

        private string Clean(string p)
        {
            StringBuilder result = new StringBuilder();
            foreach (char item in p)
                result.Append(Char.IsDigit(item) ? item : ' ');
            return result.ToString().Replace(" ", "");
        }


        /// <summary>
        /// Retorna el saldo de kinacu o de inventario si las credenciales de usaurio son valida
        /// </summary>
        /// <param name="request">credenciales del usuario</param>
        /// <returns>saldo de kinacu o en cache o cero si es en caso de ocurrir un error</returns>
        private Decimal GetBalance(GetTransactionRequestBody request)
        {
            Decimal result = 0.0m;

            try
            {

                if (!string.IsNullOrEmpty(request.AuthenticationData.Username))
                {


                    string llave = request.DeviceType + "-" + request.AuthenticationData.Username;


                    //CUANDO SE RECARGA EL SALDO
                    Func<decimal> callback = delegate ()
                    {
                        try
                        {


                            logger.InfoLow(String.Concat("[KIN] ", base.LOG_PREFIX, "[GetTransactionProvider] SALDO NOT FOUND CACHE "));



                            GetBalanceResponseBody balanceResponse = new ServiceExecutionDelegator<GetBalanceResponseBody, GetBalanceRequestBody>().ResolveRequest(new GetBalanceRequestBody()
                            {
                                AuthenticationData = request.AuthenticationData,
                                DeviceType = request.DeviceType,

                            }, ApiTargetPlatform.Kinacu, ApiServiceName.GetBalance);


                            return balanceResponse.StockBalance.Value;
                            //lista.Add(balanceResponse.WalletBalance.Value);
                            //lista.Add(balanceResponse.PointsBalance.Value);



                        }
                        catch (Exception exin)
                        {
                            logger.ErrorLow(String.Concat("[KIN] ", base.LOG_PREFIX, "[GetTransactionProvider]  ERROR EN CACHE CALLBACK SALDO" + exin.Message + " " + exin.GetType().Name));

                            return 0.0m;
                        }


                    };

                    Action<Object, Object> accion = delegate (Object key2, Object value)
                    {

                        logger.InfoLow(String.Concat("[KIN] ", base.LOG_PREFIX, "[GetTransactionProvider] SALDO FOUND CACHE [", key2, "]"));
                    };


                    var aux = _cacheSaldo.GetValue<decimal>(llave, callback, accion);

                    result = aux;// != null || aux.Length > 0? aux[0]: 0.0m;
                }
            }
            catch (Exception ex)
            {
                logger.ErrorMedium("[" + base.LOG_PREFIX + "] ERROR OBTENIENDO BALANCE " + ex.Message + " " + ex.GetType().Name);
            }


            return result;
        }


    }
}