using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging.Attribute;

namespace Movilway.API.Service.ExtendedApi.DataContract.SOSIT
{
    public class ListSolicitudesRequest : ASecuredApiRequest
    {
        /// <summary>
        /// Gets or sets Branchid
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 3)]
        public int Branchid { get; set; }

        /// <summary>
        /// Gets or sets from
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 4)]
        public string from { get; set; }

        /// <summary>
        /// Gets or sets limit
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 5)]
        public string limit { get; set; }

        /// <summary>
        /// Gets or sets filterby
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 6)]
        public string filterby { get; set; }

        /// <summary>
        /// Gets or sets filterby
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 7)]
        public string CountryAcronym { get; set; }

        
        // <summary>
        /// Valida que todos los campos obligatorias no tengan su valor por omisión
        /// </summary>
        /// <returns><c>true</c> si todos los campos obligatorios existen, <c>false</c> en caso contrario</returns>
        public bool IsValidRequest()
        {
            bool ret = true;
            ret = Branchid  > 0 ? true : false;       
            return ret;
        }
    }
}