using System;

namespace Movilway.API.Service.ExtendedApi.DataContract.Common
{
    public interface IMovilwayApiResponse
    {
        int? ResponseCode { get; set; }
        string ResponseMessage { get; set; }
        int? TransactionID { get; set; }
    }
}
