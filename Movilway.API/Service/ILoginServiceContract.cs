using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using Movilway.API.Service.External;

namespace Movilway.API.Service
{
    [ServiceContract(Namespace = "http://api.movilway.net")]
    public interface ILoginServiceContract
    {
        [OperationContract]
        LoginResponse Login(LoginRequest loginRequest);
    }
}