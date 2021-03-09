// <copyright file="InfoAgente.cs" company="Movilway">
//     Copyright (c) Movilway. All rights reserved.
// </copyright>
namespace Movilway.API.Utils.Models.Kinacu
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Representación de un agente
    /// </summary>
    internal class InfoAgente
    {
        /// <summary>
        /// Gets or sets BranchId
        /// </summary>
        public long BranchId { get; set; }

        /// <summary>
        /// Gets or sets OwnerId
        /// </summary>
        public long OwnerId { get; set; }

        /// <summary>
        /// Gets or sets NationalIdType
        /// </summary>
        public string NationalIdType { get; set; }

        /// <summary>
        /// Gets or sets NationalId
        /// </summary>
        public string NationalId { get; set; }

        /// <summary>
        /// Gets or sets Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets LegalName
        /// </summary>
        public string LegalName { get; set; }

        /// <summary>
        /// Gets or sets Address
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets SubLevel
        /// </summary>
        public int SubLevel { get; set; }

        /// <summary>
        /// Gets or sets Pdv
        /// </summary>
        public string Pdv { get; set; }

        /// <summary>
        /// Gets or sets TaxCategory
        /// </summary>
        public int TaxCategory { get; set; }

        /// <summary>
        /// Gets or sets SegmentId
        /// </summary>
        public int SegmentId { get; set; }

        /// <summary>
        /// Gets or sets CityId
        /// </summary>
        public int CityId { get; set; }

        /// <summary>
        /// Gets or sets City
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets ProvinceId
        /// </summary>
        public int ProvinceId { get; set; }

        /// <summary>
        /// Gets or sets Province
        /// </summary>
        public string Province { get; set; }
    }
}
