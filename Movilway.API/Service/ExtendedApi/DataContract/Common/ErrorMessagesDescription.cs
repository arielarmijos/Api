using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Movilway.API.Service.ExtendedApi.DataContract.Common
{
    /// <summary>
    /// Contiene los mensajes de error que se pueden presentar en el API
    /// </summary>
    public class ErrorMessagesDescription : Attribute
    {
        public string Text;

        public ErrorMessagesDescription(string text)
        {
            Text = text;
        }
    }
}