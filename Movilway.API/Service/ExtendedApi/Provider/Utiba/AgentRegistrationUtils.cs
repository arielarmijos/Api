using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using System.Configuration;
using System.IO;
using Movilway.API.Service.ExtendedApi.DataContract;
using System.Net.Mail;

namespace Movilway.API.Service.ExtendedApi.Provider.Utiba
{
    public static class AgentRegistrationUtils
    {
        private static String _tempWorkingFolder;
        private static String _readyFilesFolder;
        private static int _agentsPerFile;
        private static int _countryID;

        static AgentRegistrationUtils()
        {
            _tempWorkingFolder = ConfigurationManager.AppSettings["TempWorkingFolder"];
            _readyFilesFolder = ConfigurationManager.AppSettings["ReadyFilesFolder"];
            _agentsPerFile = int.Parse(ConfigurationManager.AppSettings["AgentsPerFile"]);
            _countryID = int.Parse(ConfigurationManager.AppSettings["CountryID"]);
        }

        public static Boolean AddAgentToFile(AgentDetails agentDetails, int registersCount)
        {
            try
            {
                DateTime processDateTime = DateTime.Now;

                String agentsWorkingFileName = "temp_AgentsToRegister.txt";
                String groupsFileNamePath = _readyFilesFolder + "GroupMappings_" + processDateTime.ToString("yyyyMMddhhmmssfff") + ".txt";
                String agentsWorkingFilePath = _tempWorkingFolder + agentsWorkingFileName;


                Boolean isNewFile = (!File.Exists(agentsWorkingFilePath));
                if (!isNewFile)
                    isNewFile = File.ReadLines(agentsWorkingFilePath).Count() == 0;
                StreamWriter agentsFile = new StreamWriter(agentsWorkingFilePath, true);

                lock (agentsFile)
                {
                    // Verificar que el archivo este vacio para escribir el Header
                    if (isNewFile)
                        agentsFile.WriteLine("##HEADER");
                    String record = String.Format("{0};{1};{2};" + '"' + "{3}" + '"' + ";{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14};{15};{16};{17};{18};{19};{20};{21};{22}",
                        agentDetails.Agent,
                        agentDetails.Name,
                        agentDetails.LegalName,
                        agentDetails.Address,
                        agentDetails.PhoneNumber,
                        _countryID,
                        agentDetails.ProvinceID,
                        agentDetails.CityID,
                        agentDetails.ContactName,
                        agentDetails.BirthDate.ToString("yyyyMMdd"),
                        agentDetails.Gender.ToString(),
                        agentDetails.NationalIDType,
                        agentDetails.NationalID,
                        agentDetails.PhoneNumber,
                        agentDetails.SMSAddress,
                        agentDetails.Email,
                        agentDetails.Referrer,
                        agentDetails.MNO1,
                        agentDetails.MNO2,
                        agentDetails.MNO3,
                        agentDetails.MNO4,
                        agentDetails.MNO5,
                        "agent");

                    agentsFile.WriteLine(record);
                    agentsFile.Flush();
                    agentsFile.Close();

                    // RP 20111018 - Ahora chequeo según la cantidad de registros de la llamada al Bulk

                    String[] fileLines = File.ReadLines(agentsWorkingFilePath).ToArray();
                    if (fileLines.Length >= registersCount) //_agentsPerFile)
                    //if (File.ReadLines(workingFilePath).Count() >= (_agentsPerFile+1))
                    {
                        StreamWriter groupsFile = new StreamWriter(groupsFileNamePath, true);
                        groupsFile.WriteLine("##HEADER");
                        for (int i = 1; i < fileLines.Length; i++)
                        {
                            groupsFile.WriteLine(fileLines[i].Split(new char[] { ';' }).ElementAt(0) + "," + ConfigurationManager.AppSettings["DefaultAgentGroupID"]);
                        }
                        groupsFile.WriteLine("##FOOTER");
                        groupsFile.Flush();
                        groupsFile.Close();

                        StreamWriter footerWriter = new StreamWriter(agentsWorkingFilePath, true);
                        footerWriter.WriteLine("##FOOTER");
                        footerWriter.Flush();
                        footerWriter.Close();
                        String newFileName = "NewAgents_" + processDateTime.ToString("yyyyMMddhhmmssfff") + ".txt";
                        File.Move(agentsWorkingFilePath, _readyFilesFolder + newFileName);

                        SendFilesByEmail(_readyFilesFolder + newFileName, groupsFileNamePath);
                    }
                }
            }
            catch (Exception)
            {
                //Log(Logger.LogMessageType.Error, "Error ejecutando Registration.AddAgentToFile: " + e.ToString(), Logger.LoggingLevelType.Low);
                return (false);
            }
            //Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Registration.AddAgentToFile", Logger.LoggingLevelType.Medium);
            return (true);
        }


        private static void SendFilesByEmail(String agentsFilePath, String groupsFilePath)
        {
            Attachment agentsFile=null;
            Attachment groupsFile=null;
            try
            {
                String[] recipients = ConfigurationManager.AppSettings["FileRecipientsEmail"].Split(new char[] { ',' });

                MailMessage message = new MailMessage();
                message.From = new MailAddress(ConfigurationManager.AppSettings["NotificationsRemitent"]);
                foreach (String address in recipients)
                {
                    message.To.Add(new MailAddress(address));
                }
                message.Subject = "Archivos de Autoregistro - " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                message.Body = "Archivos de registro adjuntos para registro de agentes y mapeo de grupos";
                agentsFile = new Attachment(agentsFilePath);
                groupsFile = new Attachment(groupsFilePath);
                message.Attachments.Add(agentsFile);
                message.Attachments.Add(groupsFile);

                SmtpClient client = new SmtpClient(ConfigurationManager.AppSettings["smtpServerHost"] ?? "localhost");
                client.Port = int.Parse(ConfigurationManager.AppSettings["smtpServerPort"] ?? "25");
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Send(message);
            }
            catch (Exception)
            {
                if (agentsFile != null)
                    agentsFile.Dispose();
                if (groupsFile != null)
                    groupsFile.Dispose();
                //Log(Logger.LogMessageType.Error, "Error tratando de enviar el email con los archivos de registro, los detalles son: " + e.ToString(), Logger.LoggingLevelType.Low);
            }
        }
    }
}