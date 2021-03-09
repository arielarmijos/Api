using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using Movilway.API.Data;
using Movilway.API.Utiba;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging;
//using Oracle.DataAccess.Client;

namespace Movilway.API.Service.ExtendedApi.Provider.BuiltIn
{
    [ServiceProviderImpl(Platform = ApiTargetPlatform.Utiba, ServiceName = ApiServiceName.GetTodayMoviPayments)]
    public class GetTodayMoviPaymentsProvider : IServiceProvider
    {
        private static readonly ILogger Logger = LoggerFactory.GetLogger(typeof(GetTodayMoviPaymentsProvider));

        //private static readonly List<TransactionLog> TestMoviPayments = new List<TransactionLog>();

        //static GetTodayMoviPaymentsProvider()
        //{
        //    var random = new Random();
        //    var randomData = from i in Enumerable.Range(0, 467)
        //                     select new TransactionLog()
        //                                {
        //                                    Amount = (decimal) (random.NextDouble()*50) + 10,
        //                                    DateTimeBegin = DateTime.Today.AddMinutes(480 + random.Next(0, 300)),
        //                                    ExternalTransactionReference = i.ToString(CultureInfo.InvariantCulture),
        //                                    ResponseCode = random.Next(10) != 5 ? 0 : 99
        //                                };
        //    TestMoviPayments = randomData.ToList();
        //    TestMoviPayments.Sort((mp1, mp2) => mp1.DateTimeBegin.CompareTo(mp2.DateTimeBegin));

        //}

        public IMovilwayApiResponse PerformOperation(IMovilwayApiRequest requestObject)
        {
            var request = requestObject as GetTodayMoviPaymentsRequestBody;
            var response = new GetTodayMoviPaymentsResponseBody
                               {
                                   ResponseCode = 0,
                                   MoviPayments = new MoviPaymentList(),
                                   TransactionID = 0,
                                   ResponseMessage = ""
                               };

            return response;
        }
    }
}