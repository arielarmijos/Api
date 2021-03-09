// <copyright file="GenerarFactura.cs" company="Movilway">
//     Copyright (c) Movilway. All rights reserved.
// </copyright>
namespace Movilway.API.Service.ExtendedApi.Provider.Cash472
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.SqlClient;

    using DataContract.Cash472;
    using DataContract.Common;
    using Movilway.Logging;

    /// <summary>
    /// Implementación método NotificacionOficialCumplimiento
    /// </summary>
    internal partial class CashProvider : AGenericPlatformAuthentication
    {
        /// <summary>
        /// Realiza el proceso de generación de factura
        /// </summary>
        /// <param name="request">Objeto que contiene todos los datos de autenticacion del usuario e información del giro</param>
        /// <returns>Factura generada</returns>
        public GenerarFacturaResponse GenerarFactura(GenerarFacturaRequest request)
        {
            string methodName = string.Format("{0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
            this.LogRequest(request);

            GenerarFacturaResponse response = new GenerarFacturaResponse();
            string sessionId = this.GetSessionId(request, response, out this.errorMessage);
            if (this.errorMessage != ErrorMessagesMnemonics.None)
            {
                this.LogResponse(response);
                return response;
            }

            if (!request.IsValidRequest())
            {
                this.SetResponseErrorCode(response, ErrorMessagesMnemonics.InvalidRequiredFields);
                this.LogResponse(response);
                return response;
            }

            Movilway.API.Service.ExtendedApi.DataContract.Cash472.Factura infoFactura = this.GetInfoFactura(sessionId, request.Id, request.ExternalId, out this.errorMessage);
            if (this.errorMessage != ErrorMessagesMnemonics.None)
            {
                this.SetResponseErrorCode(response, this.errorMessage);
                this.LogResponse(response);
                return response;
            }

            response.ResponseCode = 0;
            this.LogResponse(response);

            infoFactura.Pin = Core.Security.Multipay472TripleDes.Decrypt(this.multipayTripleDesKey, infoFactura.Pin);
            response.ResponseMessage = this.ConstruirFactura(request.TipoPos, infoFactura);
            if (request.IncludeData)
            {
                response.Factura = infoFactura;
            }

            return response;
        }

        /// <summary>
        /// Obtiene la información de un cliente dado
        /// </summary>
        /// <param name="sessionId">Session ID que será escrito en los logs</param>
        /// <param name="id">ID interno del giro</param>
        /// <param name="externalId">ID externo del giro</param>
        /// <param name="returnCode">Codigo de error en caso de que algo falle (-1 = OK, >-1 = Error)</param>
        /// <param name="connection">Objeto de conexión a base de datos</param>
        /// <returns>Un objeto <c>DwhModel.Cliente</c> que contiene la información del cliente</returns>
        private Movilway.API.Service.ExtendedApi.DataContract.Cash472.Factura GetInfoFactura(string sessionId, long id, long externalId, out ErrorMessagesMnemonics returnCode, SqlConnection connection = null)
        {
            string methodName = string.Format("{0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
            returnCode = ErrorMessagesMnemonics.None;
            Movilway.API.Service.ExtendedApi.DataContract.Cash472.Factura ret = null;

            try
            {
                this.ProviderLogger.InfoLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Message("[" + sessionId + "] " + "Ejecutando query ..."));

                string query = id != 0 ? Queries.Cash.GetInfoFacturaById : Queries.Cash.GetInfoFacturaByExternalId;
                Dictionary<string, object> queryParams = new Dictionary<string, object>()
                {
                    { "@Id", id != 0 ? id : externalId }
                };

                if (connection == null)
                {
                    using (connection = Utils.Database.GetCash472DbConnection())
                    {
                        connection.Open();
                        ret = Utils.Dwh<Movilway.API.Service.ExtendedApi.DataContract.Cash472.Factura>.ExecuteSingle(
                        connection,
                        query,
                        queryParams,
                        null);
                    }
                }
                else
                {
                    ret = Utils.Dwh<Movilway.API.Service.ExtendedApi.DataContract.Cash472.Factura>.ExecuteSingle(
                    connection,
                    query,
                    queryParams,
                    null);
                }

                if (ret == null || ret.Id == 0)
                {
                    returnCode = ErrorMessagesMnemonics.UnableToFindOrderRecordInLocalDatabase;
                    ret = null;
                }

                this.ProviderLogger.InfoLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Message("[" + sessionId + "] " + "Query ejecutado"));
            }
            catch (Exception ex)
            {
                this.ProviderLogger.ExceptionLow(() => TagValue.New()
                    .MethodName(methodName)
                    .Message("[" + sessionId + "] " + "Error ejecutando query")
                    .Exception(ex));
                returnCode = ErrorMessagesMnemonics.InternalDatabaseError;
                ret = null;
            }

            return ret;
        }

        /// <summary>
        /// Construye la factura para ser impresa por el POS
        /// </summary>
        /// <param name="pos">Tipo POS</param>
        /// <param name="factura">Información de la factura</param>
        /// <returns>Factura a ser impresa por el POS</returns>
        private string ConstruirFactura(TipoPos pos, Factura factura)
        {
            string ret = string.Empty;

            if (factura == null)
            {
                return ret;
            }

            switch (pos)
            {
                case TipoPos.App:
                case TipoPos.PosWeb:
                    ret = string.Empty;
                    break;
                case TipoPos.Mobiprint3:
                    ret = this.ConstruirFacturaMobiprint3(factura);
                    break;
            }

            return ret;
        }

        /// <summary>
        /// Construye la factura a ser impresa por un Mobiprint3
        /// </summary>
        /// <param name="factura">Datos de la factura</param>
        /// <returns>Factura a ser impresa</returns>
        private string ConstruirFacturaMobiprint3(Factura factura)
        {
            Utils.PrinterUtils printer = new Utils.PrinterUtils(32, '\n');

            printer.AddLineCentered(":: M O V I L W A Y ::");
            printer.AddLineCentered("NIT: 900351928-1");
            printer.AddLineCentered("Carrera Av.19 #108-45");
            printer.AddLineCentered("Edificio Otúa Piso 2 Oficina 204");
            printer.AddLineCentered("BOGOTÁ");
            printer.AddLineCentered("SOMOS GRANDES CONTRIBUYENTES");
            printer.AddLineCentered("RESOLUCIÓN No 000076 DE");
            printer.AddLineCentered("DIC 01/2016");
            printer.AddLineEmpty();

            printer.AddLineFieldValue("Factura de venta:", factura.NumeroFactura);
            printer.AddLineFieldValue("PIN:", factura.Pin);
            printer.AddLineFieldValue("Cajero:", factura.Acceso);
            printer.AddLineFieldValue("Fecha:", factura.Fecha.ToString("dd/MM/yyyy hh:mm:ss tt"));

            printer.AddLineFieldValue("Valor Giro:", this.FormatMoney(factura.TotalAEntregar));
            printer.AddLineFieldValue("Valor Flete:", this.FormatMoney(factura.Flete));
            printer.AddLineFieldValue("Valor Total:", this.FormatMoney(factura.TotalRecibido));
            printer.AddLineFieldValue("Efectivo:", this.FormatMoney(factura.TotalRecibido));
            printer.AddLineFieldValue("Cambio:", this.FormatMoney((long)0));

            printer.AddLineEmpty();

            printer.AddLine("PAP Origen:");
            printer.AddLine(factura.AgenciaNombre);
            printer.AddLine(factura.AgenciaDireccion);
            printer.AddLine(string.Concat(factura.CiudadOrigen, " - ", factura.DepartamentoOrigen));
            printer.AddLineEmpty();

            printer.AddLine("Remitente:");
            printer.AddLine(factura.OrigenNombre);
            printer.AddLine(factura.OrigenDni);
            printer.AddLine(factura.OrigenTel);
            printer.AddLineEmpty();

            printer.AddLine("Destinatario:");
            printer.AddLine(factura.DestinoNombre);
            printer.AddLine(factura.DestinoDni);
            printer.AddLine(factura.DestinoTel);
            printer.AddLineEmpty();

            printer.AddLine("Entregué conforme:");
            printer.AddLine("________________________________");
            printer.AddLine("C.C.: __________________________");
            printer.AddLineEmpty();

            printer.AddLine("Este documento se asimila a la");
            printer.AddLine("letra de cambio y le son");
            printer.AddLine("aplicables los artículos 772 y");
            printer.AddLine("siguientes del código de");
            printer.AddLine("comercio. La entrega se");
            printer.AddLine("considera cumplida si al momento");
            printer.AddLine("del recibo del giro por el");
            printer.AddLine("destinatario no hay reclamación");
            printer.AddLine("alguna. Con la solicitud y");
            printer.AddLine("aceptación de mi parte, de la");
            printer.AddLine("prestación de este servicio,");
            printer.AddLine("entiendase que manifiesto");
            printer.AddLine("verbalmente mi autorización para");
            printer.AddLine("el tratamiento de los datos");
            printer.AddLine("personales que voluntariamente");
            printer.AddLine("he entregado a Movilway Colombia");

            printer.AddLine("S.A.S. Estos datos pueden ser");
            printer.AddLine("utilizados única y");
            printer.AddLine("exclusivamente para la");
            printer.AddLine("prestación del servicio");
            printer.AddLine("convenido. Forma de envio o pago");
            printer.AddLine("únicamente en efectivo y moneda");
            printer.AddLine("local, se paga únicamente a");
            printer.AddLine("quien va dirigido el giro. Si un");
            printer.AddLine("giro no es reclamado en 30 días");
            printer.AddLine("calendario, el dinero será");
            printer.AddLine("devuelto a origen y el dinero");
            printer.AddLine("solo podrá ser cobrado por la");
            printer.AddLine("persona que envió el giro.");

            printer.AddLineEmpty();

            printer.AddLineCentered("AUTORIZACIÓN FACTURACIÓN No");
            printer.AddLineCentered(string.Format("{0} DE {1}", factura.FacturaResolucion, factura.FacturaFecha.ToString("MMM d/yyyy", this.culture).ToUpperInvariant()));
            printer.AddLineCentered(string.Format("Prefijo {0} DEL No {1} AL No {2}", factura.FacturaPrefijo, factura.FacturaDesde, factura.FacturaHasta));
            printer.AddLineCentered("Linea de servicio al cliente:");
            printer.AddLineCentered("(1)4048616 - 3187826928");

            printer.AddLineCentered("scgiros-co@movilway.com");
            printer.AddLineCentered("www.movilway.com.co");

            return printer.GetTicket();
        }
    }
}
