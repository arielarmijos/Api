// <copyright file="CreditNoteResponse.cs" company="Movilway">
//     Copyright (c) Movilway. All rights reserved.
// </copyright>
namespace Movilway.API.Service.ExtendedApi.DataContract
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.ServiceModel;
    using System.Runtime.Serialization;

    using Movilway.Logging.Attribute;
    using Movilway.API.Service.ExtendedApi.DataContract.Common;

    /// <summary>
    /// Implementación del WS para una nota crédito, respuesta
    /// </summary>
    [MessageContract(IsWrapped = false)]
    public class DebitNoteResponse : IMovilwayApiResponseWrapper<DebitNoteResponseBody>
    {
        [MessageBodyMember(Name = "DebitNoteResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public DebitNoteResponseBody Response { set; get; }

        public DebitNoteResponse()
        {
            Response = new DebitNoteResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "DebitNoteResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class DebitNoteResponseBody : Common.AGenericApiResponse
    {
        decimal NewStock { get; set; }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }
}
