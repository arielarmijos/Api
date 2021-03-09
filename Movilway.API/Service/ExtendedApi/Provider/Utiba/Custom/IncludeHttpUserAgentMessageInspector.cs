using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.Net;

namespace Movilway.API.Service.ExtendedApi.Provider.Utiba.Custom
{
    public class IncludeHttpUserAgentMessageInspector:IClientMessageInspector
    {
        private static String UserAgentString { get { return "Movilway.API"; } }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            HttpRequestMessageProperty httpRequestMessage;
            object httpRequestMessageObject;

            if (request.Properties.TryGetValue(HttpRequestMessageProperty.Name, out httpRequestMessageObject))
            {
                httpRequestMessage = httpRequestMessageObject as HttpRequestMessageProperty;
                if (string.IsNullOrEmpty(httpRequestMessage.Headers.Get(HttpRequestHeader.UserAgent.ToString())))
                {
                    httpRequestMessage.Headers.Set(HttpRequestHeader.UserAgent,  UserAgentString);
                }
            }
            else
            {
                httpRequestMessage = new HttpRequestMessageProperty();
                httpRequestMessage.Headers.Add(HttpRequestHeader.UserAgent, UserAgentString);
                request.Properties.Add(HttpRequestMessageProperty.Name, httpRequestMessage);
            }

            return null;
        }



        public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
           
        }
    }
}