using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using Movilway.API.Service.ExtendedApi.Provider.Macro;
using Movilway.Logging;

namespace Movilway.API.Service.ExtendedApi.Macro
{
    /// <summary>Clase para interceptar los mensajes SOAP que llegan y salen del servicio.</summary>
    public class MyMessageInspector : IDispatchMessageInspector, IParameterInspector
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(MacroProductProvider));
        protected ILogger Logger { get { return logger; } }

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            var requestEnvelope = request.ToString();

            var trace = Convert.ToBoolean(ConfigurationManager.AppSettings["TraceEnvelopeRequest"] ?? "false");

            if (trace)
            {
                Logger.InfoLow("[MacroProduct Incoming Request]: \r\n" + requestEnvelope);
            }

            return null;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            var replyEnvelope = reply.ToString();

            var trace = Convert.ToBoolean(ConfigurationManager.AppSettings["TraceEnvelopeReply"] ?? "false");

            if (!trace) return;

            Logger.InfoLow("[MacroProduct Outgoing Reply]: \r\n" + replyEnvelope);
        }

        public void AfterCall(string operationName, object[] outputs, object returnValue, object correlationState)
        {
            return;
        }

        public object BeforeCall(string operationName, object[] inputs)
        {
            return null;
        }

        public Message RemoveProperty(Message oldMessage)
        {
            var ms = new MemoryStream();
            var xw = XmlWriter.Create(ms);
            oldMessage.WriteMessage(xw);
            xw.Flush();
            var body = Encoding.UTF8.GetString(ms.ToArray());
            xw.Close();

            //Se hace la modificacion necesaria.
            body = Regex.Replace(body, "", "");

            ms = new MemoryStream(Encoding.UTF8.GetBytes(body));
            var xdr = XmlDictionaryReader.CreateTextReader(ms, new XmlDictionaryReaderQuotas());
            var newMessage = Message.CreateMessage(xdr, int.MaxValue, oldMessage.Version);
            newMessage.Properties.CopyProperties(oldMessage.Properties);
            return newMessage;
        }
    }

    /// <summary>Inspector para ser aplicado al Servicio</summary>
    public class InspectorBehavior : Attribute, IServiceBehavior
    {
        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
            return;
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (var epDisp in serviceHostBase.ChannelDispatchers
                                   .Cast<ChannelDispatcher>()
                                   .SelectMany(chDisp => chDisp.Endpoints))
            {
                epDisp.DispatchRuntime.MessageInspectors.Add(new MyMessageInspector());

                foreach (var op in epDisp.DispatchRuntime.Operations)
                    op.ParameterInspectors.Add(new MyMessageInspector());
            }
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            return;
        }
    }
}