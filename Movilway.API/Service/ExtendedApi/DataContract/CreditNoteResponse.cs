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
    public class CreditNoteResponse : IMovilwayApiResponseWrapper<CreditNoteResponseBody>
    {
        [MessageBodyMember(Name = "CreditNoteResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public CreditNoteResponseBody Response { set; get; }

        public CreditNoteResponse()
        {
            Response = new CreditNoteResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "CreditNoteResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class CreditNoteResponseBody : Common.AGenericApiResponse
    {
        decimal NewStock { get; set; }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }
}
