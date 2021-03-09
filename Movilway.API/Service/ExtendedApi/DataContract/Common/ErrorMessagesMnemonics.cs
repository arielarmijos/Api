// <copyright file="ErrorMessagesMnemonics.cs" company="Movilway">
//     Copyright (c) Movilway. All rights reserved.
// </copyright>
namespace Movilway.API.Service.ExtendedApi.DataContract.Common
{
    /// <summary>
    /// Contiene todos los codigos de error que se pueden presentar en el API
    /// </summary>
    public enum ErrorMessagesMnemonics
    {
        [ErrorMessagesDescription("")]
        None = -1,

        [ErrorMessagesDescription("Error desconocido.")]
        Unknow = 0,

        [ErrorMessagesDescription("Falta información de autenticación.")]
        MissingAuthenticationInformation = 1000,

        [ErrorMessagesDescription("Imposible realizar el proceso de autenticación.")]
        UnableToAuthenticate = 1001,

        [ErrorMessagesDescription("Usuario inválido.")]
        InvalidUser = 1002,

        [ErrorMessagesDescription("Contraseña inválida.")]
        InvalidPassword = 1003,

        [ErrorMessagesDescription("Debe especificar todos los campos obligatorios.")]
        InvalidRequiredFields = 1004,

        [ErrorMessagesDescription("El saldo del operador es insuficiente.")]
        InsufficientBalance = 1005,

        [ErrorMessagesDescription("Error interno.")]
        InternalDatabaseError = 2000,

        [ErrorMessagesDescription("No se encontró el usuario en base de datos.")]
        UnableToFindUserInLocalDatabase = 2001,

        [ErrorMessagesDescription("El usuario ya existe en base de datos.")]
        UserAlreadyExistsInLocalDatabase = 2002,

        [ErrorMessagesDescription("Error interno al intentar crear el usuario en base de datos.")]
        InternalDatabaseErrorCreatingUser = 2003,

        [ErrorMessagesDescription("Error interno al intentar actualizar el usuario en base de datos.")]
        InternalDatabaseErrorUpdatingUser = 2004,

        [ErrorMessagesDescription("No se encontraron ni el usuario emisor ni receptor en base de datos.")]
        UnableToFindIssuingAndReceiverUserInLocalDatabase = 2005,

        [ErrorMessagesDescription("No se encontró el usuario Originador en base de datos.")]
        UnableToFindIssuingUserInLocalDatabase = 2006,

        [ErrorMessagesDescription("No se encontró el usuario receptor en base de datos.")]
        UnableToFindReceiverUserInLocalDatabase = 2007,

        [ErrorMessagesDescription("El usuario Originador no puede ser el mismo Destinatario.")]
        IssuingUserAndReceiverUserAreTheSame = 2008,

        [ErrorMessagesDescription("Error interno al intentar insertar la información del giro en base de datos.")]
        InternalDatabaseErrorInsertingOrder = 2009,

        [ErrorMessagesDescription("No se encontró la agencia asociada al usuario en base de datos.")]
        UnableToFindAgentInLocalDatabase = 2010,

        [ErrorMessagesDescription("No se encontró el giro con el ID especificado en base de datos.")]
        UnableToFindOrderRecordInLocalDatabase = 2011,

        [ErrorMessagesDescription("Error interno al intentar insertar la información del pago en base de datos.")]
        InternalDatabaseErrorInsertingPayment = 2012,

        [ErrorMessagesDescription("Cliente en listas restrictivas y enrolado.")]
        ClientInRestrictiveListsAndDatabase = 2013,

        [ErrorMessagesDescription("Cliente en listas restrictivas y no enrolado.")]
        ClientInRestrictiveListsAndNotInDatabase = 2014,

        [ErrorMessagesDescription("Cliente No esta en listas restrictivas y no esta enrolado.")]
        ClientNotInRestrictiveListsAndNotInDatabase = 2015,

        [ErrorMessagesDescription("Cliente No esta en listas restrictivas y  enrolado.")]
        ClientNotInRestrictiveListsAndInDatabase = 2016,

        [ErrorMessagesDescription("Error consultando las ciudades.")]
        ErrorGetCities = 2017,

        [ErrorMessagesDescription("Cliente No esta en listas restrictivas y  enrolado con otra fecha de expedición.")]
        ClientWithAnotherDateEspeditionNoLists = 2018,

        [ErrorMessagesDescription("Cliente en listas restrictivas y  enrolado con otra fecha de expedición.")]
        ClientWithAnotherDateEspeditionLists = 2019,

        [ErrorMessagesDescription("Cliente en listas restrictivas y  enrolado sin fecha de expedición.")]
        ClientWithoutDateEspeditionLists = 2020,

        [ErrorMessagesDescription("Cliente No esta en listas restrictivas y  enrolado sin fecha de expedición.")]
        ClientWithoutDateEspeditionNoLists = 2021,

        [ErrorMessagesDescription("Servicio Web no respondió.")]
        WebServiceDoesNotRespond = 5001,

        [ErrorMessagesDescription("Excepción en servicio Web.")]
        WebServiceException = 5002,

        [ErrorMessagesDescription("Excepción en llamado a método interno.")]
        ApiMethodException = 5003,

        [ErrorMessagesDescription("Error invocando el servicio.")]
        Cash472WsError = 8001,

        [ErrorMessagesDescription("Error invocando el servicio SOS IT.")]
        SOSITWsError = 9001,

        [ErrorMessagesDescription("Transaccion no se completo exitosamente.")]
        OperationError = 9002,
    }
}
