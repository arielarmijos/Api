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
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.GetBankList)]
    public class GetBankListProvider : AKinacuProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GetBankListProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, SaleInterface kinacuWS, String sessionID)
        {
            if (sessionID.Equals("0"))
                return new GetBankListResponseBody()
                {
                    ResponseCode = 90,
                    ResponseMessage = "error session",
                    TransactionID = 0, 
                    BankList = new BankList()
                };

            GetBankListRequestBody request = requestObject as GetBankListRequestBody;
            GetBankListResponseBody response = null;
            string message;

            logger.InfoLow("[KIN] " + base.LOG_PREFIX + "[GetBankListProvider] [SEND-DATA] getAccountBankParameters {UserId=" + sessionID + "}");

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
                logger.InfoLow("[KIN] " + base.LOG_PREFIX + "[GetBankListProvider] [RECV-DATA] getAccountBankResult {No posee cuentas" + (banks == null ? " (null)" : String.Empty) + "}");
            }
            else
            {
                logger.InfoLow("[KIN] " + base.LOG_PREFIX + "[GetBankListProvider] [RECV-DATA] getAccountBankResult {response={Count=" + banks.Length + "}}");
            }

            response = new GetBankListResponseBody()
                {
                    ResponseCode = 0,
                    ResponseMessage = "exito",
                    TransactionID = 0
                };

            response.BankList = new BankList();
            string[] ignoreList = string.IsNullOrEmpty(ConfigurationManager.AppSettings["BankIgnoreList"]) ? new string[0] : ConfigurationManager.AppSettings["BankIgnoreList"].ToString().Split(',');
            foreach (var bank in banks)
            {
                if (!ignoreList.Contains(bank.BankId.ToString()))
                {
                    //if(response.BankList.Count(b => b.Key == bank.BankId) == 0)
                    response.BankList.Add(bank.Id, bank.BankName + " - " + bank.Number);
                }
            }

            return (response);
        }
    }
}