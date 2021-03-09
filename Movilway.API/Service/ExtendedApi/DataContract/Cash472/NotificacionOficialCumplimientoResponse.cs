// <copyright file="NotificacionOficialCumplimientoResponse.cs" company="Movilway">
//     Copyright (c) Movilway. All rights reserved.
// </copyright>
namespace Movilway.API.Service.ExtendedApi.DataContract.Cash472
{
    using Movilway.API.Service.ExtendedApi.DataContract.Common;

    /// <summary>
    /// Clase que determina la estructura de retorno ante la peticion de 
    /// la notificación al oficial de cumplimento
    /// </summary>
    public class NotificacionOficialCumplimientoResponse : AGenericApiResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificacionOficialCumplimientoResponse" /> class.
        /// </summary>
        public NotificacionOficialCumplimientoResponse() : base()
        {
            this.ResponseMessage = string.Empty;
        }

        /// <summary>
        /// Personalización ToString para logs
        /// </summary>
        /// <returns><c>string</c> personalizado</returns>
        public override string ToString()
        {
            return Movilway.API.Service.ExtendedApi.DataContract.Utils.logFormat(this);
        }
    }
}
