using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.API.Service.ExtendedApi.DataContract.Payment;

using Movilway.API.Utils;
using Movilway.API.Utils.CustomTextMessageEncoder;
using Movilway.API.Utils.Web;
using Movilway.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Web;
using System.Xml;

namespace Movilway.API.Service.ExtendedApi.Provider.Payment
{
    internal partial class PaymentProvider
    {
        public IniciarPagoResponse InitPayment(IniciarPagoRequest request)
        {
            string methodName = string.Format("{0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
            this.ProviderLogger.InfoLow(() => TagValue.New().MethodName(methodName).Message("Started"));
            IniciarPagoResponse response = new IniciarPagoResponse();

            string sessionId = this.GetSessionId(request, response, out this.errorMessage);

            this.ProviderLogger.InfoLow(() => TagValue.New().MethodName(methodName)
                   .Message("[" + sessionId + "] " + "Iniciando Pago."));
            if (this.errorMessage != ErrorMessagesMnemonics.None)
            {
                this.LogResponse(response);
                return response;
            }

            /* Iniciar_PagoRequest pr = new Iniciar_PagoRequest(this.PSEUSer, this.PSEPassword, this.PSEMD5Key, request.Referencia, request.TotalConIva, request.ValorIva, request.DescripcionPago, request.Email, request.IdCliente, request.NombreCliente, request.ApellidoCliente, request.TelefonoCliente, request.Direccion, request.Pais, request.Ciudad,
                     request.Ip, null, request.Opcional1, request.Opcional2, request.Opcional3,
                     null, null, null, null, null, null, null, null, null,
                     request.UrlRetorno, request.CodigoDelBanco, request.TipoDeUsuario, null, null, null);

             pr.Id_cliente = "123";
             System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(pr.GetType());
             String s;
             var encoding = Encoding.GetEncoding("iso-8859-1");

             using (StringWriter textWriter = new StringWriter())
             {

                 x.Serialize(textWriter, pr);

                 s = textWriter.ToString();
             }*/



            try
            {
                string BaseService = ConfigurationManager.AppSettings["PSEBaseHost"];
                string BaseSoapAction = ConfigurationManager.AppSettings["PSESoapActionBase"];

                Request rq = new Request();
                rq.Url = BaseService;
                rq.ExtraHeaders.Add("SOAPAction", BaseSoapAction + "/secure/webservices/WS_PSE.do/Iniciar_Pago");
                rq.Method = HttpUtils.Post;
                rq.ContentType = HttpUtils.ContentTypeTEXT_XML_UTF8;
                rq.PostData = string.Format(SoapRequest.INICIAR_PAGO, this.PSEUSer, this.PSEPassword, this.PSEMD5Key, request.Referencia, request.TotalConIva, request.ValorIva, request.DescripcionPago, request.Email, request.IdCliente, request.NombreCliente, request.ApellidoCliente, request.TelefonoCliente, request.Direccion, request.Pais, request.Ciudad, request.Ip, request.Opcional1, request.Opcional2, request.Opcional3, request.UrlRetorno, request.CodigoDelBanco, request.TipoDeUsuario);

                this.ProviderLogger.InfoHigh(() => TagValue.New().MethodName(methodName).Message("[" + sessionId + "] starting HttpRequest"));
                String ResponsText = HttpUtils.GetResponse(rq);
                this.ProviderLogger.InfoHigh(() => TagValue.New().MethodName(methodName).Message("[" + sessionId + "] HttpRequest End"));
                
                if (string.IsNullOrEmpty(ResponsText))
                {
                    this.ProviderLogger.ErrorHigh(() => TagValue.New().MethodName(methodName).Message("[" + sessionId + "] Response Empty."));
                    response.Message = "ERROR";
                    response.ResponseMessage = "Http Response Empty";
                    response.ResponseCode = 99;
                }
                else
                {


                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(ResponsText);

                    XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(xmlDoc.NameTable);

                    xmlNamespaceManager.AddNamespace("ns1", "http://DefaultNamespace");
                    xmlNamespaceManager.AddNamespace("SOAP-ENV", "http://schemas.xmlsoap.org/soap/envelope/");

                    XmlNode respuesta = xmlDoc.SelectSingleNode("//respuesta");

                    if (respuesta != null)
                    {
                        response.Message = respuesta.InnerText;

                        if (respuesta.InnerText.ToUpper().Equals("SUCCESS"))
                        {
                            XmlNode node = xmlDoc.SelectSingleNode("//errorsms");
                            response.ResponseMessage = node == null ? string.Empty : node.InnerText;
                            node = xmlDoc.SelectSingleNode("//url");
                            response.Url = node == null ? string.Empty : node.InnerText;
                            node = xmlDoc.SelectSingleNode("//numerotransaccion");
                            response.NumeroTransaccion = node == null ? "0000" : node.InnerText;
                            response.ResponseCode = 0;
                        }
                        else
                        {
                            XmlNode node = xmlDoc.SelectSingleNode("//errorsms");
                            response.ResponseMessage = node == null ? string.Empty : node.InnerText;
                            this.ProviderLogger.ErrorHigh(() => TagValue.New().MethodName(methodName).Tag("Response Transaction ").Value(response.Message).Tag("respuesta").Value(respuesta.InnerText));
                            response.ResponseCode =99;
                            response.Message = "ERROR";
                            //response.ResponseMessage = respuesta;
                        }



                        this.ProviderLogger.InfoLow(() => TagValue.New().MethodName(methodName).Message("[" + sessionId + "]").Tag("Response Transaction ").Value(respuesta.InnerText));
                    }
                    else
                    {
                        this.ProviderLogger.InfoLow(() => TagValue.New().MethodName(methodName).Message("[" + sessionId + "]").Tag("respuesta tag not found").Value(respuesta.InnerText));
                        response.ResponseMessage = "respuesta tag not found";
                        response.ResponseCode = 99;
                        response.Message = "ERROR";
                    }



                    #region WS
                    //             PSE.PSE pse = new PSE.PSE();
                    //             PSE_WS.PSEPortTypeClient client = new PSE_WS.PSEPortTypeClient();



                    //             CustomBinding binding = new CustomBinding(
                    //new CustomTextMessageBindingElement("iso-8859-1", "text/xml", MessageVersion.Soap11),
                    //new HttpsTransportBindingElement());
                    //             client.Endpoint.Binding = binding;

                    //             string message;
                    //             string url;
                    //             string transactionNumber;
                    //             /*string respuesta=pse.Iniciar_Pago(this.PSEUSer, this.PSEPassword, this.PSEMD5Key,request.Referencia,request.TotalConIva,request.ValorIva,request.DescripcionPago,request.Email,request.IdCliente,request.NombreCliente,request.ApellidoCliente,request.TelefonoCliente,request.Direccion,request.Pais,request.Ciudad,
                    //                 request.Ip,null,request.Opcional1,request.Opcional2,request.Opcional3,
                    //                 null,null,null,null,null,null,null,null,null,
                    //                 request.UrlRetorno,request.CodigoDelBanco,request.TipoDeUsuario,null,null,null, out message,out url, out transactionNumber);*/
                    //             string respuesta = client.Iniciar_Pago(this.PSEUSer, this.PSEPassword, this.PSEMD5Key, request.Referencia, request.TotalConIva, request.ValorIva, request.DescripcionPago, request.Email, request.IdCliente, request.NombreCliente, request.ApellidoCliente, request.TelefonoCliente, request.Direccion, request.Pais, request.Ciudad,
                    //                 request.Ip, null, request.Opcional1, request.Opcional2, request.Opcional3,
                    //                 null, null, null, null, null, null, null, null, null,
                    //                 request.UrlRetorno, request.CodigoDelBanco, request.TipoDeUsuario, null, null, null, out message, out url, out transactionNumber);
                    #endregion

                }

            }
            catch (Exception e)
            {
                this.ProviderLogger.ExceptionHigh(() => TagValue.New().MethodName(methodName).Message("[" + sessionId + "] API Exception").Exception(e));
                response.Message = "Exception.";
                response.ResponseMessage = e.Message;
                response.ResponseCode = 99;

            }
            finally
            {
                this.ProviderLogger.InfoLow(() => TagValue.New().MethodName(methodName).Message("[" + sessionId + "] End"));
            }

            return response;
        }
    }
}