using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Movilway.API.Service.External
{
    [CollectionDataContract(Namespace = "http://api.movilway.net/schema", KeyName="Key", ItemName="Element", ValueName="Value")]
    public class ApiKeyValuePair:Dictionary<String, String>
    {

    }
}