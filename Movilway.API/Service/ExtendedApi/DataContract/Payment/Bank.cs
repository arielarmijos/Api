// <copyright file="Factura.cs" company="Movilway">
//     Copyright (c) Movilway. All rights reserved.
// </copyright>
namespace Movilway.API.Service.ExtendedApi.DataContract.Payment
{

    using System;
    using System.Runtime.Serialization;
    using System.Web;

    public class Bank
    {
        /// <summary>
        /// Gets or sets Id
        /// </summary>
        [DataMember(Order = 3)]
        public string Id { get; set; }


        /// <summary>
        /// Gets or sets Name
        /// </summary>
        [DataMember(Order = 4)]
        public string Name { get; set; }
    }
}