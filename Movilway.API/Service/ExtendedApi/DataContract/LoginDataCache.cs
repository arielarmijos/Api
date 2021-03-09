using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Movilway.API.Service.ExtendedApi.DataContract
{
    public class LoginDataCache
    {
        public String UserName { get; set; }
        public String Platform { get; set; }
        public String Token { get; set; }
    }
}