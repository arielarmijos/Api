using Movilway.API.KinacuWebService;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.API.Service.ExtendedApi.DataContract.Payment;
using Movilway.Logging;
using Movilway.API.Service.ExtendedApi.Provider.Kinacu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace Movilway.API.Service.ExtendedApi.Provider.Payment
{
    internal partial class PaymentProvider
    {

        public GetPSEBankResponse ObtenerBancoPSE(GetPSEBankRequest request) {

            string methodName = string.Format("{0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
            this.ProviderLogger.InfoLow(() => TagValue.New().MethodName(methodName).Message("Started"));

            GetPSEBankResponse response = new GetPSEBankResponse();

            string sessionId = this.GetSessionId(request, response, out this.errorMessage);

            this.ProviderLogger.InfoLow(() => TagValue.New().MethodName(methodName)
                   .Message("[" + sessionId + "] " + "Obteniendo Bancos"));
            if (this.errorMessage != ErrorMessagesMnemonics.None)
            {
                this.LogResponse(response);
                return response;
            }


            //logger.InfoLow("[KIN] " + base.LOG_PREFIX + "[GetBankListProvider] [SEND-DATA] getAccountBankParameters {UserId=" + sessionID + "}");
            this.ProviderLogger.InfoLow(() => TagValue.New().MethodName(methodName)
                   .Message("[" + sessionId + "] " + "Obteniendo Bancos"));

            BankAccount[] banks;
            string message;
            //banks = Movilway.API.Service.ExtendedApi.Provider.Kinacu.Utils.GetAccountBanks(request.AuthenticationData.Username, out message);
            SaleInterface kinacuWS = new SaleInterface();
            // CONDIGURACION INTERFAZ
            int timeOutSeconds = int.Parse(ConfigurationManager.AppSettings["DefaultTimeout"]);
            kinacuWS.Timeout = timeOutSeconds * 1000;
            banks = kinacuWS.GetAccountBank(int.Parse(sessionId), out message);

            

            if (banks == null || banks.Length == 0)
            {
                this.ProviderLogger.InfoLow(() => TagValue.New().MethodName(methodName)
                  .Message("[" + sessionId + "] " + "No posee cuentas"));
                
                return new GetPSEBankResponse()
                {
                    ResponseCode = 99,
                    ResponseMessage = "No posee cuentas",
                    TransactionID = 0
                };
            }
            else
            {
                
                this.ProviderLogger.InfoLow(() => TagValue.New().MethodName(methodName)
                  .Message("[" + sessionId + "] " + "getAccountBankResult").Tag("bankCount").Value(banks.Length));
            }

            response = new GetPSEBankResponse()
            {
                ResponseCode = 0,
                ResponseMessage = "exito",
                TransactionID = 0
            };


            String pseBankId = ConfigurationManager.AppSettings["PSEBankId"] ?? string.Empty;
            bool found = false;
            foreach (var bank in banks)
            {

                if (pseBankId.Equals(bank.BankId.ToString()))
                {
                    found = true;

                    this.ProviderLogger.InfoLow(() => TagValue.New().MethodName(methodName)
                  .Message("[" + sessionId + "] " + " PSE Account found")
                  .Tag("BankId").Value(bank.BankId)
                  .Tag("BankName").Value(bank.BankName)
                  .Tag("CBU").Value(bank.CBU)
                  .Tag("Number").Value(bank.Number)
                  .Tag("Id").Value(bank.Id)
                  );

                    response.BankId = bank.BankId;
                    response.BankName = bank.BankName;
                    response.CBU = bank.CBU;
                    response.Id = bank.Id;
                    response.Number = bank.Number;
                    break;
                }
            }

            if (found)
                return (response);
            else
            {
                this.ProviderLogger.InfoLow(() => TagValue.New().MethodName(methodName)
                  .Message("[" + sessionId + "] " + "No posee cuenta PSE"));
                return new GetPSEBankResponse()
                {
                    ResponseCode = 99,
                    ResponseMessage = "cuenta PSE no encontrada",
                    TransactionID = 0
                };
            }
        
        }
    }
}