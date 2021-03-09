// <copyright file="Mailer.cs" company="Movilway">
//     Copyright (c) Movilway. All rights reserved.
// </copyright>
namespace Movilway.API.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Net;
    using System.Net.Mail;

    using Movilway.Logging;

    /// <summary>
    /// Mailer Helper
    /// </summary>
    internal class Mailer
    {
        /// <summary>
        /// Objeto para gestionar el log de acceso a los diferentes metodos
        /// </summary>
        private static readonly ILogger Logger;

        /// <summary>
        /// Initializes static members of the <see cref="Mailer" /> class.
        /// </summary>
        static Mailer()
        {
            try
            {
                Logger = LoggerFactory.GetLogger(typeof(Mailer));
            }
            catch (Exception)
            {
                throw new Exception("No se ha podido iniciar el sistema de loggin");
            }
        }

        /// <summary>
        /// Envia un correo electrónico con los parámetros indicados
        /// </summary>
        /// <param name="to">Correo destinatario</param>
        /// <param name="cc">Correo copia</param>
        /// <param name="bcc">Correo copia oculta</param>
        /// <param name="subject">Asunto del correo</param>
        /// <param name="body">Mensaje del correo</param>
        /// <param name="attachmentAbsoluteFilesPath">Rutas absolutas de los archivos adjuntos</param>
        /// <param name="attachmentFilesName">Nombres de los archivos adjuntos</param>
        public static void SendMail(
            string to,
            string cc,
            string bcc,
            string subject,
            string body,
            List<string> attachmentAbsoluteFilesPath = null,
            List<string> attachmentFilesName = null)
        {
            if (string.IsNullOrEmpty(to) && string.IsNullOrEmpty(cc) && string.IsNullOrEmpty(bcc))
            {
                // No existen destinatarios
                return;
            }

            try
            {
                MailMessage mailMsg = new MailMessage();

                MailAddress mailAddressFrom = new MailAddress(ConfigurationManager.AppSettings["MailFrom"], ConfigurationManager.AppSettings["MailDisplayName"]);
                mailMsg.From = mailAddressFrom;

                mailMsg.IsBodyHtml = true;
                try
                {
                    mailMsg.IsBodyHtml = Convert.ToBoolean(ConfigurationManager.AppSettings["MailIsBodyHtml"]);
                }
                catch (Exception)
                {
                }

                // Destinatarios
                if (!string.IsNullOrEmpty(to) && to.Length > 0)
                {
                    if (to.Contains(",") || to.Contains(";"))
                    {
                        string[] mails;
                        if (to.Contains(","))
                        {
                            mails = to.Split(',');
                        }
                        else
                        {
                            mails = to.Split(';');
                        }

                        foreach (var item in mails)
                        {
                            mailMsg.To.Add(item);
                        }
                    }
                    else
                    {
                        mailMsg.To.Add(to);
                    }
                }

                // Copias
                if (!string.IsNullOrEmpty(cc) && cc.Length > 0)
                {
                    if (cc.Contains(",") || cc.Contains(";"))
                    {
                        string[] mails;
                        if (cc.Contains(","))
                        {
                            mails = cc.Split(',');
                        }
                        else
                        {
                            mails = cc.Split(';');
                        }

                        foreach (var item in mails)
                        {
                            mailMsg.CC.Add(item);
                        }
                    }
                    else
                    {
                        mailMsg.CC.Add(cc);
                    }
                }

                // Copia oculta
                if (!string.IsNullOrEmpty(bcc) && bcc.Length > 0)
                {
                    if (bcc.Contains(",") || bcc.Contains(";"))
                    {
                        string[] mails;
                        if (bcc.Contains(","))
                        {
                            mails = bcc.Split(',');
                        }
                        else
                        {
                            mails = bcc.Split(';');
                        }

                        foreach (var item in mails)
                        {
                            mailMsg.Bcc.Add(item);
                        }
                    }
                    else
                    {
                        mailMsg.Bcc.Add(bcc);
                    }
                }

                mailMsg.Subject = subject;
                mailMsg.Body = body;

                if (attachmentAbsoluteFilesPath != null && attachmentAbsoluteFilesPath.Count > 0)
                {
                    int counter = 0;
                    foreach (string absoluteFilePath in attachmentAbsoluteFilesPath)
                    {
                        counter++;
                        if (System.IO.File.Exists(absoluteFilePath))
                        {
                            try
                            {
                                string attachmentFileName = string.Empty;
                                if (attachmentFilesName != null && attachmentFilesName.Count >= counter)
                                {
                                    attachmentFileName = attachmentFilesName[(counter - 1)];
                                }

                                if (string.IsNullOrEmpty(attachmentFileName))
                                {
                                    attachmentFileName = absoluteFilePath.Substring(absoluteFilePath.LastIndexOf(System.IO.Path.DirectorySeparatorChar) + 1);
                                }

                                System.IO.Stream attachedFile = new System.IO.FileStream(absoluteFilePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                                Attachment mailAtt = new Attachment(attachedFile, attachmentFileName);
                                mailMsg.Attachments.Add(mailAtt);
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }

                SmtpClient smtpClient = new SmtpClient(ConfigurationManager.AppSettings["MailServer"], int.Parse(ConfigurationManager.AppSettings["MailPort"]));
                smtpClient.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["MailFrom"], string.Empty);
                smtpClient.EnableSsl = false;
                try
                {
                    smtpClient.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["MailEnableSsl"]);
                }
                catch (Exception)
                {
                }

                smtpClient.Send(mailMsg);
            }
            catch (Exception ex)
            {
                Logger.InfoLow("Error enviando correo");
                Logger.ExceptionLow("Exception: " + ex.Message);
                Logger.ExceptionLow("InnerException: " + ((ex.InnerException == null || ex.InnerException.Message == null) ? string.Empty : ex.InnerException.Message));
                Logger.ExceptionLow("StackTrace: " + ex.StackTrace + Environment.NewLine);
            }
        }

        /// <summary>
        /// Envia un correo electrónico con los parámetros indicados
        /// </summary>
        /// <param name="to">Correo destinatario</param>
        /// <param name="cc">Correo copia</param>
        /// <param name="bcc">Correo copia oculta</param>
        /// <param name="subject">Asunto del correo</param>
        /// <param name="body">Mensaje del correo</param>
        /// <param name="hasAttachment">Tiene adjuntos sí/no</param>
        /// <param name="attachmentAbsoluteFilePath">Ruta absoluta del archivo adjunto</param>
        /// <param name="attachmentFileName">Nombre del archivo adjunto</param>
        public static void SendMail(
            string to,
            string cc,
            string bcc,
            string subject,
            string body,
            bool hasAttachment = false,
            string attachmentAbsoluteFilePath = null,
            string attachmentFileName = null)
        {
            List<string> attachements = null;
            List<string> attachementsNames = null;

            if (hasAttachment)
            {
                if (attachmentAbsoluteFilePath != null && attachmentAbsoluteFilePath.Length > 0)
                {
                    attachements = new List<string>();
                    attachements.Add(attachmentAbsoluteFilePath);
                }

                if (attachmentFileName != null && attachmentFileName.Length > 0)
                {
                    attachementsNames = new List<string>();
                    attachementsNames.Add(attachmentFileName);
                }
            }

            Mailer.SendMail(to, cc, bcc, subject, body, attachements, attachementsNames);
        }

        /// <summary>
        /// Envia un correo electrónico con los parámetros indicados
        /// </summary>
        /// <param name="to">Correo destinatario</param>
        /// <param name="subject">Asunto del correo</param>
        /// <param name="body">Mensaje del correo</param>
        public static void SendMail(
            string to,
            string subject,
            string body)
        {
            Mailer.SendMail(to, null, null, subject, body, null, null);
        }
    }
}
