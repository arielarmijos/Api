// <copyright file="Kinacu.cs" company="Movilway">
//     Copyright (c) Movilway. All rights reserved.
// </copyright>
namespace Movilway.API.Service.ExtendedApi.Provider.Cash472.Queries
{
    /// <summary>
    /// Kinacu database related queries for cash module
    /// </summary>
    internal static class Kinacu
    {
        /// <summary>
        /// Obtiene información básica de la agencia.
        /// </summary>
        public static readonly string GetAgencia = @" SELECT a.[age_id]'Id','000',1
                              FROM [dbo].[Agente] a with (NOLOCK)
                              join [dbo].[Usuario] u with (NOLOCK) on a.age_id=u.age_id
                              join [dbo].[Acceso] ac with (NOLOCK) on ac.usr_id=u.usr_id
                              where ac.acc_login=@user;";
    }
}
