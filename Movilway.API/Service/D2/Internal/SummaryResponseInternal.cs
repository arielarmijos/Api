﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;
using Movilway.API.Service.External;

namespace Movilway.API.Service.D2.Internal
{
    public class SummaryResponseInternal : ApiResponseInternal
    {
        public int TransactionCount { set; get; }
        public Decimal TotalAmount { set; get; }
    }
}