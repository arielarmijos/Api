using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging;
using Movilway.API.KinacuWebService;
using System.Text;
using System.Configuration;

namespace Movilway.API.Service.ExtendedApi.Provider.Kinacu
{
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.GetBankListApp)]
    public class GetBankListAppProvider : AKinacuProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GetBankListAppProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, SaleInterface kinacuWS, String sessionID)
        {
            if (sessionID.Equals("0"))
                return new GetBankListAppResponseBody()
                {
                    ResponseCode = 90,
                    ResponseMessage = "error session",
                    TransactionID = 0, 
                    BankList = new BankListApp()
                };

            GetBankListAppRequestBody request = requestObject as GetBankListAppRequestBody;
            GetBankListAppResponseBody response = null;
            string message;

            logger.InfoLow("[KIN] " + base.LOG_PREFIX + "[GetBankListAppProvider] [SEND-DATA] getAccountBankParameters {UserId=" + sessionID + "}");

            BankAccount[] banks;
            if (request.ParentValues ?? false)
            {
                banks = Utils.GetAccountBanks(request.Agent, out message);
            }
            else
            {
                banks = kinacuWS.GetAccountBank(int.Parse(sessionID), out message);
            }

            if (banks == null || banks.Length == 0)
            {
                logger.InfoLow("[KIN] " + base.LOG_PREFIX + "[GetBankListAppProvider] [RECV-DATA] getAccountBankResult {No posee cuentas" + (banks == null ? " (null)" : String.Empty) + "}");
            }
            else
            {
                logger.InfoLow("[KIN] " + base.LOG_PREFIX + "[GetBankListAppProvider] [RECV-DATA] getAccountBankResult {response={Count=" + banks.Length + "}}");
            }

            response = new GetBankListAppResponseBody()
                {
                    ResponseCode = 0,
                    ResponseMessage = "exito",
                    TransactionID = 0
                };

            response.BankList = new BankListApp();
            string[] ignoreList = string.IsNullOrEmpty(ConfigurationManager.AppSettings["BankIgnoreList"]) ? new string[0] : ConfigurationManager.AppSettings["BankIgnoreList"].ToString().Split(',');
            if (banks != null)
            {
                foreach (var bank in banks)
                {
                    if (!ignoreList.Contains(bank.BankId.ToString()))
                    {
                        response.BankList.Add(new BankInfo() { ID = bank.Id, Name = bank.BankName, Account = bank.Number });
                    }
                }
            }

            return (response);
        }
    }
}