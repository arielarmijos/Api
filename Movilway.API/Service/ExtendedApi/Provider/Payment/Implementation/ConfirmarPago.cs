using Movilway.API.Service.ExtendedApi.DataContract.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Movilway.Logging;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using System.ServiceModel.Channels;
using Movilway.API.Utils.CustomTextMessageEncoder;
using Movilway.API.Service.ExtendedApi.Provider.Payment.Model;

namespace Movilway.API.Service.ExtendedApi.Provider.Payment
{
    internal partial class PaymentProvider
    {
        public ConfirmaRespuestaPagoResponse ConfirmPayment(ConfirmaRespuestaPagoRequest request)
        {
            string methodName = string.Format("{0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
            this.ProviderLogger.InfoLow(() => TagValue.New().MethodName(methodName)
                   .Message("Started"));

            ConfirmaRespuestaPagoResponse response = new ConfirmaRespuestaPagoResponse();

            string sessionId = this.GetSessionId(request, response, out this.errorMessage);

            this.ProviderLogger.InfoLow(() => TagValue.New().MethodName(methodName)
                   .Message("[" + sessionId + "] " + " Confirmando respuesta."));
            if (this.errorMessage != ErrorMessagesMnemonics.None)
            {
                this.LogResponse(response);
                return response;
            }

            try
            {

                PSE_WS.PSEPortTypeClient client = new PSE_WS.PSEPortTypeClient();


                
                CustomBinding binding = new CustomBinding(
   new CustomTextMessageBindingElement("iso-8859-1", "text/xml", MessageVersion.Soap11),
   new HttpsTransportBindingElement());


                client.Endpoint.Binding = binding;




                string message;

                // string respuesta=pse.Listar_Bancos(this.PSEUSer, this.PSEPassword, this.PSEMD5Key, out message, out bankList);

                string respuesta = client.Confirma_Respuesta_Pago(this.PSEUSer, this.PSEPassword, this.PSEMD5Key, request.NumeroTransaccion, out message, out response.int_estado_pago, out response.str_nombre_banco, out response.str_codigo_transaccion, out response.int_ciclo_transaccion, out response.dat_fecha, out response.fechatransaccion,
                    out response.horatransaccion, out response.descripcion_pago, out response.email, out response.nombre_cliente, out response.apellido_cliente, out response.telefono_cliente, out response.direccion, out response.pais, out response.ciudad, out response.info_opcional1, out response.info_opcional2, out response.info_opcional3, out response.firma, out response.ip, out response.total_con_iva, out response.valor_iva);
                response.respuesta = respuesta;
                response.ResponseMessage = message;
                response.errorsms = message;

                if (respuesta.Equals("OK"))
                    response.ResponseCode = Convert.ToInt32(TransaccionEstado.Exitoso);
                else if (respuesta.Equals("PENDING"))
                    response.ResponseCode = Convert.ToInt32(TransaccionEstado.Pendiente);
                else if (respuesta.Equals("NOT_AUTHORIZED"))
                    response.ResponseCode = Convert.ToInt32(TransaccionEstado.Rechazado);
                else if (respuesta.Equals("error"))
                    response.ResponseCode = response.ResponseCode = Convert.ToInt32(TransaccionEstado.Error);
                else
                    response.ResponseCode = Convert.ToInt32(TransaccionEstado.Desconocido);

                this.ProviderLogger.InfoLow(() => TagValue.New().MethodName(methodName).Message("[" + sessionId + "]").Tag("respuesta").Value(respuesta));
                 
                
                //Si es error resuelto por PSE se consulta el segundo WS de TDC 
                if (respuesta.Equals("error")) //o si es algún otro tipo de error.
                {
                    this.ProviderLogger.InfoLow(() => TagValue.New().MethodName(methodName).Message("[" + sessionId + "] Se inicia consulta WS TDC"));
                    
                    TDC_WS.ProcesarTarjetaCreditoPortTypeClient tdcClient = new TDC_WS.ProcesarTarjetaCreditoPortTypeClient();
                    tdcClient.Endpoint.Binding = binding;
                    string codigoderespuesta,mensajesistema,fechatransaccion,numerodeautorizacion,numerodetransaccion,respuestadelared,logdelared,IPproveedordeinternet,IPlatitud,IPlongitud,IPciudad,IPpais,IPcontinente,IPzonahoraria,IPcodigoisopais,TARJETAbanco,TARJETApais,TARJETAtipo,extra1,extra2,extra3;
                    numerodetransaccion=request.NumeroTransaccion;
                    
                    string respuestaTDC=tdcClient.Confirmar_Transaccion(this.PSEUSer, this.PSEPassword, this.PSEMD5Key, ref numerodetransaccion,
                        out codigoderespuesta, out mensajesistema, out  fechatransaccion, out  numerodeautorizacion,
                        out  respuestadelared, out logdelared, out IPproveedordeinternet, out IPlatitud, out  IPlongitud, out  IPciudad, out  IPpais, out  IPcontinente, out  IPzonahoraria,
                        out IPcodigoisopais, out  TARJETAbanco, out  TARJETApais, out  TARJETAtipo, out  extra1, out  extra2, out  extra3);

                    this.ProviderLogger.InfoLow(() => TagValue.New().MethodName(methodName).Message("[" + sessionId + "]").Tag("respuesta TDC").Value(respuestaTDC).Tag("codigoderespuesta").Value(codigoderespuesta).Tag("mensajesistema").Value(mensajesistema));

                    this.ProviderLogger.InfoLow(() => TagValue.New().MethodName(methodName).Message("[" + sessionId + "] Se mapea resultado TDC con PSE WS TDC"));
                    response = SetResponseFromTDCWS(response, respuestaTDC, numerodetransaccion,
                         codigoderespuesta, mensajesistema, fechatransaccion, numerodeautorizacion,
                          respuestadelared, logdelared, IPproveedordeinternet, IPlatitud, IPlongitud, IPciudad, IPpais, IPcontinente, IPzonahoraria,
                         IPcodigoisopais, TARJETAbanco, TARJETApais, TARJETAtipo, extra1, extra2, extra3);
                    this.ProviderLogger.InfoLow(() => TagValue.New().MethodName(methodName).Message("[" + sessionId + "] fin de Mapeo TDC con PSE WS TDC"));
                   
                        
                }


            }
            catch (Exception e)
            {
                this.ProviderLogger.InfoLow(() => TagValue.New().MethodName(methodName).Message("[" + sessionId + "] Exception"));
                this.ProviderLogger.ExceptionHigh(() => TagValue.New().MethodName(methodName).Message("[" + sessionId + "] Exception").Exception(e));

            }
            finally {
                this.ProviderLogger.InfoLow(() => TagValue.New().MethodName(methodName)
                      .Message("[" + sessionId + "] End"));
            }
            return response;
        }

        /// <summary>
        /// Se Mapea los valores correspondientes al WS de TDC (No todos son los mismos que retorna el WS PSE)
        /// </summary>
        /// <param name="response"></param>
        /// <param name="respuestaTDC"></param>
        /// <param name="codigoderespuesta"></param>
        /// <param name="mensajesistema"></param>
        /// <param name="fechatransaccion"></param>
        /// <param name="numerodeautorizacion"></param>
        /// <param name="respuestadelared"></param>
        /// <param name="logdelared"></param>
        /// <param name="IPproveedordeinternet"></param>
        /// <param name="IPlatitud"></param>
        /// <param name="IPlongitud"></param>
        /// <param name="IPciudad"></param>
        /// <param name="IPpais"></param>
        /// <param name="IPcontinente"></param>
        /// <param name="IPzonahoraria"></param>
        /// <param name="IPcodigoisopais"></param>
        /// <param name="TARJETAbanco"></param>
        /// <param name="TARJETApais"></param>
        /// <param name="TARJETAtipo"></param>
        /// <param name="extra1"></param>
        /// <param name="extra2"></param>
        /// <param name="extra3"></param>
        /// <returns></returns>
        private ConfirmaRespuestaPagoResponse SetResponseFromTDCWS(ConfirmaRespuestaPagoResponse response, string respuestaTDC,string numerodetransaccion, string codigoderespuesta, string mensajesistema, string fechatransaccion, string numerodeautorizacion,
                        string  respuestadelared, string logdelared, string IPproveedordeinternet, string IPlatitud, string  IPlongitud, string  IPciudad, string  IPpais, string  IPcontinente, string  IPzonahoraria,
                        string IPcodigoisopais, string  TARJETAbanco, string  TARJETApais, string  TARJETAtipo, string  extra1, string  extra2, string  extra3) {

                            string noData = null;

                            string respuesta = "error", message = "SUCCESS";
                            string ciclo = noData, date = noData;
                            

                            if (respuestaTDC.ToLower().Equals("aprobada"))
                            {
                                response.ResponseCode = Convert.ToInt32(TransaccionEstado.Exitoso);
                                respuesta = "OK";

                                try
                                {
                                    string[] splitData = mensajesistema.Split(new string[] { "Ciclo:", "Fecha Servidor PSE:" }, StringSplitOptions.None);
                                    ciclo = splitData[1].Trim();
                                    date = splitData[2].Trim();
                                }
                                catch (Exception)
                                {
                                    
                                    
                                }

                            }
                            else if (respuestaTDC.ToLower().Equals("pendiente"))
                            {
                                response.ResponseCode = Convert.ToInt32(TransaccionEstado.Pendiente);
                                respuesta = "PENDING";
                            }
                            else if (respuestaTDC.ToLower().Equals("rechazada"))
                            {
                                response.ResponseCode = Convert.ToInt32(TransaccionEstado.Rechazado);
                                respuesta = "NOT_AUTHORIZED";
                            }
                            else {
                                response.ResponseCode = Convert.ToInt32(TransaccionEstado.Desconocido);
                                respuesta = mensajesistema;
                            }

                            response.respuesta = respuesta;
                            response.ResponseMessage = message;
                            response.errorsms = message;

                            //response.int_estado_pago = TARJETAbanco;
                            response.str_nombre_banco = TARJETAbanco;
                            response.str_codigo_transaccion = numerodeautorizacion;
                            response.fechatransaccion = fechatransaccion;
                            response.horatransaccion = noData;
                            response.descripcion_pago = noData;
                            response.email = noData;
                            response.nombre_cliente = noData;
                            response.apellido_cliente = noData;
                            response.telefono_cliente = noData;
                            response.direccion = noData;
                            response.pais = IPcodigoisopais;
                            response.ciudad = noData;
                            response.info_opcional1 = extra1;
                            response.info_opcional2 = extra2;
                            response.info_opcional3 = extra3;
                            response.ip = noData;
                            response.total_con_iva = noData;
                            response.valor_iva = noData;
            


            

                            response.int_ciclo_transaccion = ciclo;
                            response.dat_fecha = date;
                            response.TransactionID = 2;// Para identificar del lado de PosWeb que la respuesta es del WS TDC


                            return response;
        }
    }
}