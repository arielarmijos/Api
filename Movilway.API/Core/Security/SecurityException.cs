using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Movilway.API.Core.Security
{
    public class SecurityException:Exception
    {



         public SecurityException(string message):base(message)
        {

        }

         public SecurityException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}