using Movilway.API.Service.ExtendedApi.DataContract.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

using Movilway.Logging;
using Movilway.API.Service.ExtendedApi.DataContract.Common;

using Movilway.API.Utils.CustomTextMessageEncoder;
using System.ServiceModel.Channels;
using System.ServiceModel;

namespace Movilway.API.Service.ExtendedApi.Provider.Payment
{
    internal partial class PaymentProvider
    {

        public GetBanklistResponse GetBankList(GetBankListRequest request)
        {
            string methodName = string.Format("{0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
            this.ProviderLogger.InfoLow(() => TagValue.New().MethodName(methodName)
                   .Message("Started"));

            GetBanklistResponse response = new GetBanklistResponse();

            string sessionId = this.GetSessionId(request, response, out this.errorMessage);

            this.ProviderLogger.InfoLow(() => TagValue.New().MethodName(methodName)
                   .Message("[" + sessionId + "] " + "Obteniendo Lista de bancos."));
            if (this.errorMessage != ErrorMessagesMnemonics.None)
            {
                this.LogResponse(response);
                return response;
            }

            try
            {

                PSE_WS.PSEPortTypeClient client = new PSE_WS.PSEPortTypeClient();


                /**/
                /**/
                CustomBinding binding = new CustomBinding(
   new CustomTextMessageBindingElement("iso-8859-1", "text/xml", MessageVersion.Soap11),
   new HttpsTransportBindingElement());

                /* CustomBinding01 binding = new CustomBinding01(
    new CustomTextMessageBindingElement("iso-8859-1", "text/xml", MessageVersion.Soap11),
    new HttpTransportBindingElement());*/




                //binding.Namespace = "https://puntodepago.plataformadepago.com/secure/webservices";
                client.Endpoint.Binding = binding;/**/


                //////////////////
                /*var ws_http_binding = new WSHttpBinding();

                ws_http_binding.Security.Mode = SecurityMode.Transport;

                ChannelFactory<IInternal> factory =
                    new ChannelFactory<IInternal>(
                        ws_http_binding,
                        new EndpointAddress("https://MyMachine:8733/IInternal"));

                var channel = factory.CreateChannel();*/
                /////////////////

                PSE.PSE pse = new PSE.PSE();

                string message;
                string bankList;
                // string respuesta=pse.Listar_Bancos(this.PSEUSer, this.PSEPassword, this.PSEMD5Key, out message, out bankList);


                string respuesta = client.Listar_Bancos(this.PSEUSer, this.PSEPassword, this.PSEMD5Key, out message, out bankList);

                response.ResponseMessage = message;
                if (!string.IsNullOrEmpty(bankList))
                {
                    string[] splt = bankList.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string bnk in splt)
                    {
                        string[] splt2 = bnk.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        if (splt2.Length > 1)
                            response.BankList.Add(new Bank { Id = splt2[0].Replace("BancoID=", string.Empty).Trim(), Name = splt2[1].Replace("BancoNombre=", string.Empty).Trim() });
                    }
                }
                if (response.BankList.Count > 0)
                    response.ResponseCode = 0;

                this.ProviderLogger.InfoLow(() => TagValue.New().MethodName(methodName).Message("[" + sessionId + "]").Tag("BankCount").Value(response.BankList.Count));


            }
            catch (Exception e)
            {
                this.ProviderLogger.ExceptionHigh(() => TagValue.New().MethodName(methodName).Message("[" + sessionId + "]").Exception(e));

            }
            finally {
                this.ProviderLogger.InfoLow(() => TagValue.New().MethodName(methodName)
                       .Message("[" + sessionId + "] End"));

            }
            return response;

        }
    }
}