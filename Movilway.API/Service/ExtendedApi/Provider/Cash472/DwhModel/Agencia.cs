// <copyright file="agencia.cs" company="Movilway">
//     Copyright (c) Movilway. All rights reserved.
// </copyright>
namespace Movilway.API.Service.ExtendedApi.Provider.Cash472.DwhModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// Representación del modelo de base de datos de una agencia.
    /// </summary>
    internal class Agencia : Utils.Models.Kinacu.InfoAgente
    {
        /// <summary>
        /// Gets or sets Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets Ciudad
        /// </summary>
        public string Ciudad { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is enabled or not
        /// </summary>
        public bool Habilitado { get; set; }
    }
}
