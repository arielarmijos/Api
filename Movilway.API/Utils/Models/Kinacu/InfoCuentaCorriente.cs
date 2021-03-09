// <copyright file="InfoCuentaCorriente.cs" company="Movilway">
//     Copyright (c) Movilway. All rights reserved.
// </copyright>
namespace Movilway.API.Utils.Models.Kinacu
{
    /// <summary>
    /// Representación de una cuenta corriente
    /// </summary>
    internal class InfoCuentaCorriente
    {
        /// <summary>
        /// Gets or sets Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets Saldo
        /// </summary>
        public decimal Saldo { get; set; }

        /// <summary>
        /// Gets or sets LimiteCredito
        /// </summary>
        public decimal LimiteCredito { get; set; }
    }
}
