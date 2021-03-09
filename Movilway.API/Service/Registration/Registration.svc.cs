using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Movilway.API.Service.Registration;
using Movilway.API.Service.Registration.Internal;
using System.IO;
using System.Configuration;
using Movilway.API.Service.Registration.External;
using Movilway.API.Data;
using Movilway.API.Service.External;
using Movilway.API.Service.Internal;
using System.Net.Mail;
using System.Net;

namespace Movilway.API.Service.Registration
{

    [ServiceBehavior(Namespace = "http://api.movilway.net", ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
    public class Registration : ApiServiceBase, IRegistration
    {
        private String _tempWorkingFolder;
        private String _readyFilesFolder;
        private int _agentsPerFile;
        private int _countryID;

        public Registration()
        {
            _tempWorkingFolder = ConfigurationManager.AppSettings["TempWorkingFolder"];
            _readyFilesFolder = ConfigurationManager.AppSettings["ReadyFilesFolder"];
            _agentsPerFile = int.Parse(ConfigurationManager.AppSettings["AgentsPerFile"]);
            _countryID = int.Parse(ConfigurationManager.AppSettings["CountryID"]);
        }

        public LoginResponse Login(LoginRequest loginRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Registration.Login", Logger.LoggingLevelType.Medium);
            Log(Logger.LogMessageType.Info, String.Format("Llamando a Registration.Login con los parametros: User={0}, Password={1}, DeviceType={2}", loginRequest.Request.AccessId, loginRequest.Request.Password, loginRequest.Request.AccessType), Logger.LoggingLevelType.Low);
            return (AuthenticationProvider.Login(loginRequest));
            //Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Sales.Login", Logger.LoggingLevelType.Medium);
        }

        public GetSessionResponse GetSession(GetSessionRequest getSessionRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Registration.GetSession", Logger.LoggingLevelType.Medium);
            GetSessionResponse response = null;
            try
            {
                Log(Logger.LogMessageType.Info, String.Format("Llamando a AgentRegistration.Login con los parametros: User={0}, DeviceType={1}", getSessionRequest.Request.User, getSessionRequest.Request.DeviceType), Logger.LoggingLevelType.Low);
                LoginResponseInternal loginResponse = AuthenticationProvider.LoginInternal(new LoginRequestInternal()
                {
                    DeviceType = getSessionRequest.Request.DeviceType,
                    Password = getSessionRequest.Request.Password,
                    User = getSessionRequest.Request.User
                });
                response = new GetSessionResponse()
                {
                    Response = new GetSessionResponseBody()
                    {
                        ResponseCode = loginResponse.ResponseCode,
                        ResponseMessage = loginResponse.ResponseMessage,
                        SessionID = loginResponse.SessionID,
                        TransactionID = loginResponse.TransactionID
                    }
                };
                Log(Logger.LogMessageType.Info, String.Format("Parametros de respuesta de AgentRegistration.Login: LoginResult={0}, Message={1} ", response.Response.ResponseCode, response.Response.ResponseMessage), Logger.LoggingLevelType.Low);

            }
            catch(Exception e)
            {
                Log(Logger.LogMessageType.Error, "Excepcion en el metodo Registration.Login: "+e.ToString(), Logger.LoggingLevelType.Low);
            }
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Registration.Login", Logger.LoggingLevelType.Medium);
            return (response);
        }

        public ChangePinResponse ChangePin(ChangePinRequest externalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Registration.ChangePin", Logger.LoggingLevelType.Medium);
            ChangePinResponse response = null;
            try
            {
                Log(Logger.LogMessageType.Info, String.Format("Llamando a AgentRegistration.ChangePin con los parametros: SessionID={0}, DeviceType={1}, Initiator={2}",
                    externalRequest.Request.SessionID, externalRequest.Request.DeviceType, externalRequest.Request.Initiator), Logger.LoggingLevelType.Low);
                response = AuthenticationProvider.ChangePin(externalRequest);
                Log(Logger.LogMessageType.Info, String.Format("Parametros de respuesta de AgentRegistration.ChangePin: ResponseCode={0}, ResponseMessage={1}, TransactionID={2}", response.Response.ResponseCode, response.Response.ResponseMessage, response.Response.TransactionID), Logger.LoggingLevelType.Low);

            }
            catch (Exception e)
            {
                Log(Logger.LogMessageType.Error, "Excepcion en el metodo AgentRegistration.ChangePin: " + e.ToString(), Logger.LoggingLevelType.Low);
            }
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Registration.ChangePin", Logger.LoggingLevelType.Medium);
            return (response);
            
        }

        public RegisterAgentResponse RegisterAgent(RegisterAgentRequest externalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Registration.RegisterAgent", Logger.LoggingLevelType.Medium);
            RegisterAgentResponse externalResponse = null;
            try
            {
                Log(Logger.LogMessageType.Info, String.Format("Llamando a AgentRegistration.RegisterAgent con los parametros: Agent={0}, Name={1}, LegalName={2}, Address={3}, PhoneNumber={4}," +
                "ProvinceID={5}, CityID={6}, ContactName={7}, BirthDate={8}, Gender={9}, NationalIDType={10}, NationalID={11}, SMSAddress={12}, Email={13}, Referrer={14},"+
                "MNO1={15}, MNO2={16}, MNO3={17}, MNO4={18}, MNO5={19}",externalRequest.Request.Agent, externalRequest.Request.Name, externalRequest.Request.LegalName, externalRequest.Request.Address, 
                externalRequest.Request.PhoneNumber, externalRequest.Request.ProvinceID, externalRequest.Request.CityID, externalRequest.Request.ContactName, externalRequest.Request.BirthDate,
                externalRequest.Request.Gender, externalRequest.Request.NationalIDType, externalRequest.Request.NationalID, externalRequest.Request.SMSAddress, externalRequest.Request.Email,
                externalRequest.Request.Referrer, externalRequest.Request.MNO1, externalRequest.Request.MNO2, externalRequest.Request.MNO3, externalRequest.Request.MNO4, externalRequest.Request.MNO5)
                , Logger.LoggingLevelType.Low);


                RegisterAgentRequestInternal internalRequest = new RegisterAgentRequestInternal()
                {
                    Address = externalRequest.Request.Address,
                    Agent = externalRequest.Request.Agent,
                    BirthDate = externalRequest.Request.BirthDate,
                    CityID = externalRequest.Request.CityID,
                    ContactName = externalRequest.Request.ContactName,
                    CountryID = _countryID,
                    Email = externalRequest.Request.Email,
                    Gender = (API.Service.Internal.Gender)externalRequest.Request.Gender,
                    LegalName = externalRequest.Request.LegalName,
                    MNO1 = externalRequest.Request.MNO1,
                    MNO2 = externalRequest.Request.MNO2,
                    MNO3 = externalRequest.Request.MNO3,
                    MNO4 = externalRequest.Request.MNO4,
                    MNO5 = externalRequest.Request.MNO5,
                    Name = externalRequest.Request.Name,
                    NationalID = externalRequest.Request.NationalID,
                    NationalIDType = externalRequest.Request.NationalIDType,
                    PhoneNumber = externalRequest.Request.PhoneNumber,
                    ProvinceID = externalRequest.Request.ProvinceID,
                    Referrer = externalRequest.Request.Referrer,
                    SessionID = externalRequest.Request.SessionID,
                    SMSAddress = externalRequest.Request.SMSAddress
                };
                RegisterAgentResponseInternal internalResponse = RegisterAgentInternal(internalRequest, 1);
                RegisterAgentResponseBody externalResponseBody = new RegisterAgentResponseBody()
                {
                    ResponseCode = internalResponse.ResponseCode,
                    ResponseMessage = internalResponse.ResponseMessage
                };
                externalResponse = new RegisterAgentResponse();
                externalResponse.Response = externalResponseBody;

                Log(Logger.LogMessageType.Info, String.Format("Parametros de respuesta de AgentRegistration.RegisterAgent: ResponseCode={0}, ResponseMessage={1}, TransactionID={2}", externalResponse.Response.ResponseCode, externalResponse.Response.ResponseMessage, externalResponse.Response.TransactionID), Logger.LoggingLevelType.Low);
            }
            catch (Exception e)
            {
                Log(Logger.LogMessageType.Error, "Excepcion en el metodo AgentRegistration.RegisterAgent: " + e.ToString(), Logger.LoggingLevelType.Low);
            }
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Registration.RegisterAgent", Logger.LoggingLevelType.Medium);
            return (externalResponse);
        }
        
        public RegisterAgentBulkResponse RegisterAgentBulk(RegisterAgentBulkRequest externalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Registration.RegisterAgentBulk", Logger.LoggingLevelType.Medium);
            RegisterAgentBulkResponse externalResponse = null;
            try
            {
                if (externalRequest.Request.Agents != null && externalRequest.Request.Agents.Count > 0)
                {
                    Log(Logger.LogMessageType.Info, String.Format("Llamando a AgentRegistration.RegisterAgentBulk con un total de {0} Agentes, los detalles son: ", externalRequest.Request.Agents.Count), Logger.LoggingLevelType.Low);
                    int counter = 1;
                    Boolean failed = false;
                    foreach (RegisterAgentBulkItem agentInfo in externalRequest.Request.Agents)
                    {
                        Log(Logger.LogMessageType.Info, String.Format("Agente #{20}: Agent={0}, Name={1}, LegalName={2}, Address={3}, PhoneNumber={4}," +
                        "ProvinceID={5}, CityID={6}, ContactName={7}, BirthDate={8}, Gender={9}, NationalIDType={10}, NationalID={11}, SMSAddress={12}, Email={13}, Referrer={14}," +
                        "MNO1={15}, MNO2={16}, MNO3={17}, MNO4={18}, MNO5={19}", agentInfo.Agent, agentInfo.Name, agentInfo.LegalName, agentInfo.Address,
                        agentInfo.PhoneNumber, agentInfo.ProvinceID, agentInfo.CityID, agentInfo.ContactName, agentInfo.BirthDate,
                        agentInfo.Gender, agentInfo.NationalIDType, agentInfo.NationalID, agentInfo.SMSAddress, agentInfo.Email,
                        agentInfo.Referrer, agentInfo.MNO1, agentInfo.MNO2, agentInfo.MNO3, agentInfo.MNO4, agentInfo.MNO5, counter)
                        , Logger.LoggingLevelType.Low);

                        RegisterAgentRequestInternal internalRequest = new RegisterAgentRequestInternal()
                        {
                            Address = agentInfo.Address,
                            Agent = agentInfo.Agent,
                            BirthDate = agentInfo.BirthDate,
                            CityID = agentInfo.CityID,
                            ContactName = agentInfo.ContactName,
                            CountryID = _countryID,
                            Email = agentInfo.Email,
                            Gender = (API.Service.Internal.Gender)agentInfo.Gender,
                            LegalName = agentInfo.LegalName,
                            MNO1 = agentInfo.MNO1,
                            MNO2 = agentInfo.MNO2,
                            MNO3 = agentInfo.MNO3,
                            MNO4 = agentInfo.MNO4,
                            MNO5 = agentInfo.MNO5,
                            Name = agentInfo.Name,
                            NationalID = agentInfo.NationalID,
                            NationalIDType = agentInfo.NationalIDType,
                            PhoneNumber = agentInfo.PhoneNumber,
                            ProvinceID = agentInfo.ProvinceID,
                            Referrer = agentInfo.Referrer,
                            SessionID = externalRequest.Request.SessionID,
                            SMSAddress = agentInfo.SMSAddress
                        };
                        // RP 20111018 - Nuevo parámetro para saber cuántos registros hubo en la llamada al Bulk
                        RegisterAgentResponseInternal internalResponse = RegisterAgentInternal(internalRequest, externalRequest.Request.Agents.Count);
                        
                        if (internalResponse.ResponseCode != 0)
                            failed = true;
                        counter++;
                    }
                    RegisterAgentBulkResponseBody responseBody = new RegisterAgentBulkResponseBody()
                    {
                        ResponseCode = 0,
                        ResponseMessage = "Su peticion ha sido procesada"
                    };
                    externalResponse = new RegisterAgentBulkResponse();
                    externalResponse.Response = responseBody;
                    if (failed)
                    {
                        externalResponse.Response.ResponseCode = 1;
                        externalResponse.Response.ResponseMessage = "Algunos de los registros en el archivo fallaron, por favor contacte a soporte";
                    }

                }
                else
                {
                    externalResponse = new RegisterAgentBulkResponse()
                    {
                        Response = new RegisterAgentBulkResponseBody()
                        {
                            ResponseCode = 1,
                            ResponseMessage = "Su peticion falló por no contener agentes, por favor verifique los datos e intente de nuevo",
                            TransactionID = 0
                        }
                    };
                }

                Log(Logger.LogMessageType.Info, String.Format("Parametros de respuesta de AgentRegistration.RegisterAgentBulk: ResponseCode={0}, ResponseMessage={1}, TransactionID={2}", externalResponse.Response.ResponseCode, externalResponse.Response.ResponseMessage, externalResponse.Response.TransactionID), Logger.LoggingLevelType.Low);
            }
            catch (Exception e)
            {
                Log(Logger.LogMessageType.Error, "Excepcion en el metodo AgentRegistration.RegisterAgentBulk: " + e.ToString(), Logger.LoggingLevelType.Low);
            }
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Registration.RegisterAgentBulk", Logger.LoggingLevelType.Medium);
            return (externalResponse);
        }

        public GetProvincesResponse GetProvinces(GetProvincesRequest externalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Registration.GetProvinces", Logger.LoggingLevelType.Medium);
            GetProvincesResponse externalResponse = null;
            try
            {
                Log(Logger.LogMessageType.Info, "Llamando a AgentRegistration.GetProvinces", Logger.LoggingLevelType.Low);
                externalResponse = new GetProvincesResponse();
                UtibaRegistrationDataContext utibaRegistration = new UtibaRegistrationDataContext();
                List<Province> provinces = utibaRegistration.Provinces.Where(p => p.CountryId == _countryID).ToList();
                externalResponse.Response.Provinces = new ProvinceList();
                foreach (Province province in provinces)
                {
                    externalResponse.Response.Provinces.Add(new ProvinceInfo(province.ProvinceId, province.ProvinceName));
                }
                externalResponse.Response.ResponseCode = 0;
                Log(Logger.LogMessageType.Info, String.Format("Parametros de respuesta de AgentRegistration.GetProvinces: ResponseCode={0}, ResponseMessage={1}, TransactionID={2}", externalResponse.Response.ResponseCode, externalResponse.Response.ResponseMessage, externalResponse.Response.TransactionID), Logger.LoggingLevelType.Low);
            }
            catch (Exception e)
            {
                Log(Logger.LogMessageType.Error, "Excepcion en el metodo AgentRegistration.GetProvinces: " + e.ToString(), Logger.LoggingLevelType.Low);
            }
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Registration.GetProvinces", Logger.LoggingLevelType.Medium);
            return (externalResponse);
        }

        public GetProvinceCitiesResponse GetProvinceCities(GetProvinceCitiesRequest externalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Registration.GetProvinceCities", Logger.LoggingLevelType.Medium);
            GetProvinceCitiesResponse externalResponse = null;
            try
            {
                Log(Logger.LogMessageType.Info, "Llamando a AgentRegistration.GetProvinceCities", Logger.LoggingLevelType.Low);
                externalResponse = new GetProvinceCitiesResponse();
                UtibaRegistrationDataContext utibaRegistration = new UtibaRegistrationDataContext();
                List<City> cities = utibaRegistration.Cities.Where(c => c.CountryId == _countryID && c.ProvinceId == externalRequest.Request.ProvinceID).ToList();
                externalResponse.Response.Cities = new CityList();
                foreach (City city in cities)
                {
                    externalResponse.Response.Cities.Add(new CityInfo(city.CityId, city.CityName));
                }
                externalResponse.Response.ResponseCode = 0;
                Log(Logger.LogMessageType.Info, String.Format("Parametros de respuesta de AgentRegistration.GetProvinceCities: ResponseCode={0}, ResponseMessage={1}, TransactionID={2}", externalResponse.Response.ResponseCode, externalResponse.Response.ResponseMessage, externalResponse.Response.TransactionID), Logger.LoggingLevelType.Low);
            }
            catch (Exception e)
            {
                Log(Logger.LogMessageType.Error, "Excepcion en el metodo AgentRegistration.GetProvinceCities: " + e.ToString(), Logger.LoggingLevelType.Low);
            }
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Registration.GetProvinceCities", Logger.LoggingLevelType.Medium);
            return (externalResponse);
        }

        #region Internal Methods

        // RP 20111018 - Nuevo parámetro para saber cuántos registros hubo en la llamada al Bulk
        internal RegisterAgentResponseInternal RegisterAgentInternal(RegisterAgentRequestInternal internalRequest, int registersCount)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Registration.RegisterAgentInternal", Logger.LoggingLevelType.Medium);
            RegisterAgentResponseInternal internalResponse = new RegisterAgentResponseInternal();
            if (AddAgentToFile(internalRequest, registersCount))
            {
                internalResponse.ResponseCode = 0;
                internalResponse.ResponseMessage = "Agente agregado satisfactoriamente al archivo de carga";
            }
            else
            {
                internalResponse.ResponseCode = 1;
                internalResponse.ResponseMessage = "Ocurrio un error tratando de agregar el agente al archivo de carga";
            }
            Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Resultado Obtenido Registration.RegisterAgentInternal: ResponseCode={0}, ResponseMessage={1}" 
            , internalResponse.ResponseCode, internalResponse.ResponseMessage), Logger.LoggingLevelType.Low);
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Registration.RegisterAgentInternal", Logger.LoggingLevelType.Medium);
            return (internalResponse);
        }

        // RP 20111018 - Nuevo parámetro para saber cuántos registros hubo en la llamada al Bulk
        // IT 20111018 - 
        private Boolean AddAgentToFile(RegisterAgentRequestInternal agentInfo, int registersCount) 
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Registration.AddAgentToFile", Logger.LoggingLevelType.Medium);
            try
            {
                DateTime processDateTime = DateTime.Now;

                String agentsWorkingFileName = "temp_AgentsToRegister.txt";
                String groupsFileNamePath = _readyFilesFolder + "GroupMappings_"+processDateTime.ToString("yyyyMMddhhmmssfff") + ".txt";
                String agentsWorkingFilePath = _tempWorkingFolder + agentsWorkingFileName;


                Boolean isNewFile = (!File.Exists(agentsWorkingFilePath)); 
                if(!isNewFile)
                    isNewFile = File.ReadLines(agentsWorkingFilePath).Count() == 0;
                StreamWriter agentsFile = new StreamWriter(agentsWorkingFilePath, true);

                lock (agentsFile)
                {
                    // Verificar que el archivo este vacio para escribir el Header
                    if (isNewFile)
                        agentsFile.WriteLine("##HEADER");
                    String record = String.Format("{0};{1};{2};" + '"' + "{3}" + '"' + ";{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14};{15};{16};{17};{18};{19};{20};{21};{22}",
                        agentInfo.Agent,
                        agentInfo.Name,
                        agentInfo.LegalName,
                        agentInfo.Address,
                        agentInfo.PhoneNumber,
                        agentInfo.CountryID,
                        agentInfo.ProvinceID,
                        agentInfo.CityID,
                        agentInfo.ContactName,
                        agentInfo.BirthDate.ToString("yyyyMMdd"),
                        agentInfo.Gender.ToString(),
                        agentInfo.NationalIDType,
                        agentInfo.NationalID,
                        agentInfo.PhoneNumber,
                        agentInfo.SMSAddress,
                        agentInfo.Email,
                        agentInfo.Referrer,
                        agentInfo.MNO1,
                        agentInfo.MNO2,
                        agentInfo.MNO3,
                        agentInfo.MNO4,
                        agentInfo.MNO5,
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

                        StreamWriter footerWriter= new StreamWriter(agentsWorkingFilePath, true);
                        footerWriter.WriteLine("##FOOTER");
                        footerWriter.Flush();
                        footerWriter.Close();
                        String newFileName = "NewAgents_" + processDateTime.ToString("yyyyMMddhhmmssfff") + ".txt";
                        File.Move(agentsWorkingFilePath, _readyFilesFolder + newFileName);

                        SendFilesByEmail(_readyFilesFolder + newFileName, groupsFileNamePath);
                    }
                }
            }
            catch (Exception e)
            {
                Log(Logger.LogMessageType.Error, "Error ejecutando Registration.AddAgentToFile: " + e.ToString(), Logger.LoggingLevelType.Low);
                return (false);
            }
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Registration.AddAgentToFile", Logger.LoggingLevelType.Medium);
            return (true);
        }


        private void SendFilesByEmail(String agentsFilePath, String groupsFilePath)
        {
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
                Attachment agentsFile = new Attachment(agentsFilePath);
                Attachment groupsFile = new Attachment(groupsFilePath);
                message.Attachments.Add(agentsFile);
                message.Attachments.Add(groupsFile);

                SmtpClient client = new SmtpClient(ConfigurationManager.AppSettings["smtpServerHost"] ?? "localhost");
                client.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["NotificationsRemitent"], "");

                client.Port = int.Parse(ConfigurationManager.AppSettings["smtpServerPort"] ?? "25");
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Send(message);
            }
            catch (Exception e)
            {
                Log(Logger.LogMessageType.Error, "Error tratando de enviar el email con los archivos de registro, los detalles son: " + e.ToString(), Logger.LoggingLevelType.Low);
            }
        }

        #endregion
    }
}
