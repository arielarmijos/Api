using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Movilway.API.Service.ExtendedApi.DataContract.Common
{
    public interface IMovilwayApiRequestWrapper<TWrapped> where TWrapped:IMovilwayApiRequest
    {
        TWrapped Request { set; get; }
    }
    
    public static class MovilwayApiRequestWrapperExtension
    {
        public static AuthenticationData GetWrappedAuthenticationData<T>(this IMovilwayApiRequestWrapper<T> request) where T : IMovilwayApiRequest
        {
            return (request.Request as IMovilwayApiRequest).AuthenticationData;
        }
    }
}
