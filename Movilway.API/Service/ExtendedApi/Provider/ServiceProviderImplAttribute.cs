using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Movilway.API.Service.ExtendedApi.DataContract.Common;

namespace Movilway.API.Service.ExtendedApi.Provider
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class ServiceProviderImplAttribute:Attribute
	{
        public ApiTargetPlatform Platform { set; get; }
        public ApiServiceName ServiceName { set; get; }
	}
}