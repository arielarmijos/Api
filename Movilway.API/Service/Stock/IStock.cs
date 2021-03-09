using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Movilway.API.Service.Stock.External;

namespace Movilway.API.Service.Stock
{
    [ServiceContract(Namespace="http://api.movilway.net")]
    public interface IStock:ILoginServiceContract, ITopUpServiceContract
    {
        [OperationContract]
        BuyStockResponse BuyStock(BuyStockRequest externalRequest);
        [OperationContract]
        BuyStockExtendedResponse BuyStockExtended(BuyStockExtendedRequest externalRequest);

        [OperationContract]
        TransferStockResponse TransferStock(TransferStockRequest externalRequest);
        [OperationContract]
        TransferStockExtendedResponse TransferStockExtended(TransferStockExtendedRequest externalRequest);

        [OperationContract]
        PayStockResponse PayStock(PayStockRequest externalRequest);
        [OperationContract]
        PayStockExtendedResponse PayStockExtended(PayStockExtendedRequest externalRequest);
    }
}
