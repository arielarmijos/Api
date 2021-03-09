using System.Runtime.Serialization;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging.Attribute;

namespace Movilway.API.Service.ExtendedApi.DataContract.SOSIT
{
    public class AddSolicitudeRequest : ASecuredApiRequest
    {
        /// <summary>
        /// Gets or sets Solicitante
        /// </summary>
        [Loggable]
        [DataMember(IsRequired = false, EmitDefaultValue = false, Order = 3)]
        public string BranchId { get; set; }

        /// <summary>
        /// Gets or sets Subject
        /// </summary>
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 4)]
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets description
        /// </summary>
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 5)]
        public string Description { get; set; }   

        /// <summary>
        /// Gets or sets site
        /// </summary>
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 6)]
        public string Site { get; set; }

        /// <summary>
        /// Gets or sets category
        /// </summary>
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 7)]
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets subcategory
        /// </summary>
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 8)]
        public string Subcategory { get; set; }

        /// <summary>
        /// Gets or sets RequestType
        /// </summary>
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 9)]
        public string RequestType { get; set; }

        /// <summary>
        /// Gets or sets priority
        /// </summary>
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 7)]
        public string Priority { get; set; }


        /// <summary>
        /// Gets or sets ServiceCategory
        /// </summary>
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 10)]
        public string ServiceCategory { get; set; }

       
        /// <summary>
        /// Gets or sets Item
        /// </summary>
        [DataMember(IsRequired = true, EmitDefaultValue = false, Order = 11)]
        public string Item { get; set; }

        // <summary>
        /// Valida que todos los campos obligatorias no tengan su valor por omisión
        /// </summary>
        /// <returns><c>true</c> si todos los campos obligatorios existen, <c>false</c> en caso contrario</returns>
        public bool IsValidRequest()
        {
            bool ret = true;

            
            ret = !string.IsNullOrEmpty(this.Subject);
            ret = !string.IsNullOrEmpty(this.Description);
            ret = !string.IsNullOrEmpty(this.Site);
            ret = !string.IsNullOrEmpty(this.Category);
            ret = !string.IsNullOrEmpty(this.Subcategory);
            ret = !string.IsNullOrEmpty(this.RequestType);
            ret = !string.IsNullOrEmpty(this.Priority);
            ret = !string.IsNullOrEmpty(this.Priority);
            ret = !string.IsNullOrEmpty(this.ServiceCategory);
            ret = !string.IsNullOrEmpty(this.Item);
           

            return ret;
        }
    }
}