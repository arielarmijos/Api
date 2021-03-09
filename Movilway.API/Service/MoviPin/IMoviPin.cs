using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Movilway.API.Service.MoviPin.External;

namespace Movilway.API.Service.MoviPin
{
    [ServiceContract(Namespace = "http://api.movilway.net")]
    public interface IMoviPin : ILoginServiceContract, ISessionServiceContract
    {
        [OperationContract]
        MoviPaymentResponse MoviPayment(MoviPaymentRequest externalRequest);
        [OperationContract]
        MoviPaymentExtendedResponse MoviPaymentExtended(MoviPaymentExtendedRequest externalRequest);
    }
}
