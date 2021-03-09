using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Web;

namespace Movilway.API.Utils.CustomTextMessageEncoder
{
    public class CustomBinding01: Binding
{
    private HttpTransportBindingElement transport;
    private BinaryMessageEncodingBindingElement encoding;

    public CustomBinding01(): base()
    {
      this.InitializeValue();
    }
    public CustomBinding01(CustomTextMessageBindingElement te, HttpTransportBindingElement transport)
    {
     // this.InitializeValue();
        this.transport = transport;
        this.encoding = new BinaryMessageEncodingBindingElement();
    }
        
    public override BindingElementCollection CreateBindingElements()
    {
      BindingElementCollection elements = new BindingElementCollection();
      elements.Add(this.encoding);
      elements.Add(this.transport);
      return elements;
    }
    public override string Scheme
    {
        get { return "https";/* this.transport.Scheme;*/ }
    }
    private void InitializeValue()
    {
      this.transport = new HttpTransportBindingElement();
      this.encoding = new BinaryMessageEncodingBindingElement();
    }
}
}