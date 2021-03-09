// <copyright file="CreditNoteRequest.cs" company="Movilway">
//     Copyright (c) Movilway. All rights reserved.
// </copyright>
namespace Movilway.API.Service.ExtendedApi.DataContract
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using Movilway.API.Service.ExtendedApi.DataContract.Common;
    using Movilway.Logging.Attribute;

    /// <summary>
    /// Implementación del WS para una nota crédito, petición
    /// </summary>
    [MessageContract(IsWrapped=false)]
    public class CreditNoteRequest : IMovilwayApiRequestWrapper<CreditNoteRequestBody>
    {
        [MessageBodyMember(Name = "CreditNoteRequest")]
        public CreditNoteRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "CreditNoteRequest", Namespace = "http://api.movilway.net/schema/extended")]
    public class CreditNoteRequestBody : ASecuredApiRequest
    {
        [Loggable]
        [DataMember(Order = 3, IsRequired = true)]
        public int AgentId { get; set; }

        [Loggable]
        [DataMember(Order = 4, IsRequired = true)]
        public decimal Amount { get; set; }

        [Loggable]
        [DataMember(Order = 5, IsRequired = true)]
        public string Comment { get; set; }

        [Loggable]
        [DataMember(Order = 6, IsRequired = false, EmitDefaultValue = true)]
        public string SupportDocument { get; set; }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }
}
