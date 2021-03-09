﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Movilway.API.Service.ExtendedApi.DataContract.Common;

namespace Movilway.API.Service.ExtendedApi.Provider
{
    public interface IServiceProvider
    {
        IMovilwayApiResponse PerformOperation(IMovilwayApiRequest requestObject);

       // void ValidateSecurity(IMovilwayApiRequest requestObject);
    }
}
