using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Movilway.API.Service.ExtendedApi.DataContract.Common
{
    public interface IMovilwayApiResponseWrapper<TWrapper> where TWrapper:IMovilwayApiResponse
    {
        TWrapper Response { set; get; }
    }
}
