using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging;
using Movilway.API.KinacuWebService;
using Movilway.API.KinacuLogisticsWebService;
using System.Text;

namespace Movilway.API.Service.ExtendedApi.Provider.Kinacu
{
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.GetChildList)]
    public class GetChildListProvider : AKinacuProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GetChildListProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformKinacuOperation(IMovilwayApiRequest requestObject, SaleInterface kinacuWS, String sessionID)
        {
            if (sessionID.Equals("0"))
                return new GetChildListResponseBody()
                {
                    ResponseCode = 90,
                    ResponseMessage = "error session",
                    TransactionID = 0,
                    ChildList = new ChildList()
                };



            //GetChildListRequestBody request = requestObject as GetChildListRequestBody;
            GetChildListRequestBody request = requestObject as GetChildListRequestBody;
      
            GetChildListResponseBody response = new GetChildListResponseBody();

            LogisticsInterface logisticsWS = new LogisticsInterface();

            logger.InfoLow("[KIN] " + base.LOG_PREFIX + "[GetChildListProvider] [SEND-DATA] getChildRetailersParameters {UserId=" + sessionID + "}");

            KinacuLogisticsWebService.Retailer[] childList = logisticsWS.GetChildRetailers(int.Parse(sessionID));
            if (childList == null || childList.Count() <= 0)
            {
                logger.InfoLow("[KIN] " + base.LOG_PREFIX + "[GetChildListProvider] [RECV-DATA] getChildRetailersResult {No posee Hijos}");
            }
            else
            {
                StringBuilder sb = new StringBuilder();
              
            
                KinacuLogisticsWebService.Retailer retailer = childList[0];
                sb.Append("Retailer={Id=" + retailer.Id + ",Name=" + retailer.Name + "},");

                if (childList.Length > 1) { 
                 retailer = childList[1];
                  sb.Append("Retailer={Id=" + retailer.Id + ",Name=" + retailer.Name + "},");
                }

                sb.Remove(sb.Length - 1, 1);
                sb.Append("...");
                logger.InfoLow("[KIN] " + base.LOG_PREFIX + "[GetChildListProvider] [RECV-DATA] getChildRetailersResult {response="+ childList.Length+ ",[" + sb.ToString() + "]}");
            }

            response = new GetChildListResponseBody()
            {
                ResponseCode = (childList != null ? 0 : 99),
                ResponseMessage = (childList != null ? "exito" : "error"),
                TransactionID = 0
            };

            if (childList != null && childList.Length > 0)
            {
                response.ChildList = new ChildList();

                foreach (KinacuLogisticsWebService.Retailer retailer in childList)
                {
                    response.ChildList.Add(new BasicAgentInfo()
                    {
                        Agent = retailer.Id.ToString(),
                        Name = retailer.Name
                    });
                }

                if (request.ExtendedValues ?? false)
                {
                    var extendedValues = Utils.GetAgentExtendedValues(String.Join(",", response.ChildList.Select(ch => ch.Agent)));
                    var rand = new Random();
                    foreach (BasicAgentInfo item in response.ChildList)
                        if (extendedValues.Any(e => e.Agent == item.Agent))
                        {
                            var values = extendedValues.Single(e => e.Agent == item.Agent);
                            item.Email = values.Email;
                            item.Department = values.Department;
                            item.City = values.City;
                            item.CurrentBalance = values.CurrentBalance;
                            item.Status = values.Status;
                            item.ChildsCount = values.ChildsCount;
                            item.PDVId = values.PDVId;
                        }
                }
            }




            return (response);
        }

     
      
    }
}