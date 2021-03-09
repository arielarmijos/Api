using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Movilway.API.Service.ExtendedApi.DataContract.Common
{
    public interface IMovilwayApiRequest
    {
        AuthenticationData AuthenticationData { get; }
        int DeviceType { get; }
        String Platform { get; }
        Boolean IsFinancial { get; }
    }
}
