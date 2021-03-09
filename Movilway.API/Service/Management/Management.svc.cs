using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Movilway.API.Service.Management.External;
using Movilway.API.Service.Management.Internal;
using Movilway.API.Utiba;
using System.ServiceModel.Channels;
using System.Net;
using Movilway.API.Service.External;
using Movilway.API.Service.Internal;

namespace Movilway.API.Service.Management
{
    [ServiceBehavior(Namespace = "http://api.movilway.net", ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
    public class Management : ApiServiceBase, IManagement
    {

        public LoginResponse Login(LoginRequest loginRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Management.Login", Logger.LoggingLevelType.Medium);
            LoginResponse response = null;
            try
            {
                Log(Logger.LogMessageType.Info, String.Format("Llamando a Management.Login con los parametros: User={0}, Password={1}, DeviceType={2}", loginRequest.Request.AccessId, loginRequest.Request.Password, loginRequest.Request.AccessType), Logger.LoggingLevelType.Low);
                response = AuthenticationProvider.Login(loginRequest);
                Log(Logger.LogMessageType.Info, String.Format("Parametros de respuesta de Management.Login: LoginResult={0}, Message={1}", response.Response.LoginResult, response.Response.Message), Logger.LoggingLevelType.Low);

            }
            catch (Exception e)
            {
                Log(Logger.LogMessageType.Error, "Excepcion en el metodo Management.Login: " + e.ToString(), Logger.LoggingLevelType.Low);
            }
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Management.Login", Logger.LoggingLevelType.Medium);
            return (response);
        }
        public GetSessionResponse GetSession(GetSessionRequest getSessionRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Management.GetSession", Logger.LoggingLevelType.Medium);
            GetSessionResponse response = null;
            try
            {
                Log(Logger.LogMessageType.Info, String.Format("Llamando a AgentManagement.Login con los parametros: User={0}, DeviceType={1}", getSessionRequest.Request.User, getSessionRequest.Request.DeviceType), Logger.LoggingLevelType.Low);
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
                Log(Logger.LogMessageType.Info, String.Format("Parametros de respuesta de AgentManagement.Login: LoginResult={0}, Message={1} ", response.Response.ResponseCode, response.Response.ResponseMessage), Logger.LoggingLevelType.Low);

            }
            catch (Exception e)
            {
                Log(Logger.LogMessageType.Error, "Excepcion en el metodo Management.Login: " + e.ToString(), Logger.LoggingLevelType.Low);
            }
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Management.Login", Logger.LoggingLevelType.Medium);
            return (response);
        }

        public GetAgentByReferenceResponse GetAgentByReference(GetAgentByReferenceRequest externalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Management.GetAgentByReference", Logger.LoggingLevelType.Medium);
            GetAgentByReferenceRequestInternal internalRequest = new GetAgentByReferenceRequestInternal()
            {
                SessionID = externalRequest.Request.SessionID,
                DeviceType = externalRequest.Request.DeviceType,
                Reference = externalRequest.Request.Reference
            };
            GetAgentByReferenceResponseInternal internalResponse = GetAgentByReferenceInternal(internalRequest);
            GetAgentByReferenceResponse externalResponse = new GetAgentByReferenceResponse()
            {
                Response = new GetAgentByReferenceResponseBody()
                {
                    ResponseCode = internalResponse.ResponseCode,
                    ResponseMessage = internalResponse.ResponseMessage,
                    TransactionID = internalResponse.TransactionID,
                    AgentInfo = new External.AgentInfo()
                    {
                        Address=internalResponse.Address,
                        Agent=internalResponse.Agent,
                        AgentID=internalResponse.AgentID,
                        Email=internalResponse.Email,
                        Name=internalResponse.Name,
                        OwnerID=internalResponse.OwnerID,
                        ReferenceID=internalResponse.ReferenceID,
                        Depth = internalResponse.Depth
                    }
                }
            };
            if (internalResponse.AdditionalData != null)
            {
                externalResponse.Response.AgentInfo.AdditionalDataOld = new ApiKeyValuePair();
                foreach(KeyValuePair<String, String> additionalDataValue in internalResponse.AdditionalData)
                {
                    externalResponse.Response.AgentInfo.AdditionalDataOld.Add(additionalDataValue.Key, additionalDataValue.Value);
                }
            }
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Management.GetAgentByReference", Logger.LoggingLevelType.Medium);
            return (externalResponse);
        }
        public GetAgentByReferenceExtendedResponse GetAgentByReferenceExtended(GetAgentByReferenceExtendedRequest externalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Management.GetAgentByReference", Logger.LoggingLevelType.Medium);
            LoginRequestInternal loginRequest = new LoginRequestInternal()
            {
                DeviceType = externalRequest.Request.DeviceType,
                Password = externalRequest.Request.Password,
                User = externalRequest.Request.Username
            };
            LoginResponseInternal loginResponse = AuthenticationProvider.LoginInternal(loginRequest);
            GetAgentByReferenceRequestInternal internalRequest = new GetAgentByReferenceRequestInternal()
            {
                SessionID = loginResponse.SessionID,
                DeviceType = externalRequest.Request.DeviceType,
                Reference = externalRequest.Request.Reference
            };
            GetAgentByReferenceResponseInternal internalResponse = GetAgentByReferenceInternal(internalRequest);
            GetAgentByReferenceExtendedResponse externalResponse = new GetAgentByReferenceExtendedResponse()
            {
                Response = new GetAgentByReferenceExtendedResponseBody()
                {
                    ResponseCode = internalResponse.ResponseCode,
                    ResponseMessage = internalResponse.ResponseMessage,
                    TransactionID = internalResponse.TransactionID,
                    AgentInfo = new External.AgentInfo()
                    {
                        Address = internalResponse.Address,
                        Agent = internalResponse.Agent,
                        AgentID = internalResponse.AgentID,
                        Email = internalResponse.Email,
                        Name = internalResponse.Name,
                        OwnerID = internalResponse.OwnerID,
                        ReferenceID = internalResponse.ReferenceID,
                        Depth = internalResponse.Depth
                    }
                }
            };
            if (internalResponse.AdditionalData != null)
            {
                externalResponse.Response.AgentInfo.AdditionalDataOld = new ApiKeyValuePair();
                foreach (KeyValuePair<String, String> additionalDataValue in internalResponse.AdditionalData)
                {
                    externalResponse.Response.AgentInfo.AdditionalDataOld.Add(additionalDataValue.Key, additionalDataValue.Value);
                }
            }
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Management.GetAgentByReference", Logger.LoggingLevelType.Medium);
            return (externalResponse);
        }

        public GetParentListByReferenceIDResponse GetParentListByReferenceID(GetParentListByReferenceIDRequest externalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Management.GetParentListByReferenceID", Logger.LoggingLevelType.Medium);
            GetParentListByReferenceIDRequestInternal internalRequest = new GetParentListByReferenceIDRequestInternal()
            {
                AgentID=externalRequest.Request.AgentID,
                DeviceType=externalRequest.Request.DeviceType,
                SessionID=externalRequest.Request.SessionID
            };
            GetParentListByReferenceIDResponseInternal internalResponse = GetParentListByReferenceIDInternal(internalRequest);
            GetParentListByReferenceIDResponse externalResponse = new GetParentListByReferenceIDResponse()
            {
                Response = new GetParentListByReferenceIDResponseBody()
                {
                    ResponseCode = internalResponse.ResponseCode,
                    ResponseMessage = internalResponse.ResponseMessage,
                    TransactionID = internalResponse.TransactionID
                }
            };
            if (internalResponse.ParentList != null && internalResponse.ParentList.Count > 0)
            {
                externalResponse.Response.ParentList = new ParentList();
                foreach (Internal.AgentInfoInternal internalParent in internalResponse.ParentList)
                {
                    External.AgentInfo externalParent = new External.AgentInfo()
                    {
                        Address = internalParent.Address,
                        Agent = internalParent.Agent,
                        AgentID = internalParent.AgentID,
                        Email = internalParent.Email,
                        Name = internalParent.Name,
                        OwnerID = internalParent.OwnerID,
                        ReferenceID = internalParent.ReferenceID
                    };
                    if (internalParent.AdditionalData != null && internalParent.AdditionalData.Count > 0)
                    {
                        externalParent.AdditionalDataOld = new ApiKeyValuePair();
                        foreach (KeyValuePair<String, String> keyValue in internalParent.AdditionalData)
                        {
                            externalParent.AdditionalDataOld.Add(keyValue.Key, keyValue.Value);
                        }
                    }
                    externalResponse.Response.ParentList.Add(externalParent);
                }
            }
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Management.GetParentListByReferenceID", Logger.LoggingLevelType.Medium);
            return (externalResponse);
        }
        public GetParentListByReferenceIDExtendedResponse GetParentListByReferenceIDExtended(GetParentListByReferenceIDExtendedRequest externalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Management.GetParentListByReferenceID", Logger.LoggingLevelType.Medium);
            LoginRequestInternal loginRequest = new LoginRequestInternal()
            {
                DeviceType = externalRequest.Request.DeviceType,
                Password = externalRequest.Request.Password,
                User = externalRequest.Request.Username
            };
            LoginResponseInternal loginResponse = AuthenticationProvider.LoginInternal(loginRequest);
            GetParentListByReferenceIDRequestInternal internalRequest = new GetParentListByReferenceIDRequestInternal()
            {
                AgentID = externalRequest.Request.AgentID,
                DeviceType = externalRequest.Request.DeviceType,
                SessionID = loginResponse.SessionID
            };
            GetParentListByReferenceIDResponseInternal internalResponse = GetParentListByReferenceIDInternal(internalRequest);
            GetParentListByReferenceIDExtendedResponse externalResponse = new GetParentListByReferenceIDExtendedResponse()
            {
                Response = new GetParentListByReferenceIDExtendedResponseBody()
                {
                    ResponseCode = internalResponse.ResponseCode,
                    ResponseMessage = internalResponse.ResponseMessage,
                    TransactionID = internalResponse.TransactionID
                }
            };
            if (internalResponse.ParentList != null && internalResponse.ParentList.Count > 0)
            {
                externalResponse.Response.ParentList = new ParentList();
                foreach (Internal.AgentInfoInternal internalParent in internalResponse.ParentList)
                {
                    External.AgentInfo externalParent = new External.AgentInfo()
                    {
                        Address = internalParent.Address,
                        Agent = internalParent.Agent,
                        AgentID = internalParent.AgentID,
                        Email = internalParent.Email,
                        Name = internalParent.Name,
                        OwnerID = internalParent.OwnerID,
                        ReferenceID = internalParent.ReferenceID
                    };
                    if (internalParent.AdditionalData != null && internalParent.AdditionalData.Count > 0)
                    {
                        externalParent.AdditionalDataOld = new ApiKeyValuePair();
                        foreach (KeyValuePair<String, String> keyValue in internalParent.AdditionalData)
                        {
                            externalParent.AdditionalDataOld.Add(keyValue.Key, keyValue.Value);
                        }
                    }
                    externalResponse.Response.ParentList.Add(externalParent);
                }
            }
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Management.GetParentListByReferenceID", Logger.LoggingLevelType.Medium);
            return (externalResponse);
        }

        public GetChildListByReferenceResponse GetChildListByReference(GetChildListByReferenceRequest externalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Management.GetChildListByReference", Logger.LoggingLevelType.Medium);
            GetChildListByReferenceRequestInternal internalRequest = new GetChildListByReferenceRequestInternal()
            {
                Reference=externalRequest.Request.Reference,
                DeviceType = externalRequest.Request.DeviceType,
                SessionID = externalRequest.Request.SessionID
            };
            GetChildListByReferenceResponseInternal internalResponse = GetChildListByReferenceInternal(internalRequest);
            GetChildListByReferenceResponse externalResponse = new GetChildListByReferenceResponse()
            {
                Response=new GetChildListByReferenceResponseBody()
                {
                    ResponseCode = internalResponse.ResponseCode,
                    ResponseMessage = internalResponse.ResponseMessage,
                    TransactionID = internalResponse.TransactionID
                }
            };
            if (internalResponse.ChildList != null && internalResponse.ChildList.Count > 0)
            {
                externalResponse.Response.ParentList = new ChildList();
                foreach (Internal.AgentInfoInternal internalChild in internalResponse.ChildList)
                {
                    External.AgentInfo externalChild = new External.AgentInfo()
                    {
                        Address = internalChild.Address,
                        Agent = internalChild.Agent,
                        AgentID = internalChild.AgentID,
                        Email = internalChild.Email,
                        Name = internalChild.Name,
                        OwnerID = internalChild.OwnerID,
                        ReferenceID = internalChild.ReferenceID
                    };
                    if (internalChild.AdditionalData != null && internalChild.AdditionalData.Count > 0)
                    {
                        externalChild.AdditionalDataOld = new ApiKeyValuePair();
                        foreach (KeyValuePair<String, String> keyValue in internalChild.AdditionalData)
                        {
                            externalChild.AdditionalDataOld.Add(keyValue.Key, keyValue.Value);
                        }
                    }
                    externalResponse.Response.ParentList.Add(externalChild);
                }
            }
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Management.GetChildListByReference", Logger.LoggingLevelType.Medium);
            return (externalResponse);
        }
        public GetChildListByReferenceExtendedResponse GetChildListByReferenceExtended(GetChildListByReferenceExtendedRequest externalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Management.GetChildListByReference", Logger.LoggingLevelType.Medium);
            LoginRequestInternal loginRequest = new LoginRequestInternal()
            {
                DeviceType = externalRequest.Request.DeviceType,
                Password = externalRequest.Request.Password,
                User = externalRequest.Request.Username
            };
            LoginResponseInternal loginResponse = AuthenticationProvider.LoginInternal(loginRequest);
            GetChildListByReferenceRequestInternal internalRequest = new GetChildListByReferenceRequestInternal()
            {
                Reference = externalRequest.Request.Reference,
                DeviceType = externalRequest.Request.DeviceType,
                SessionID = loginResponse.SessionID
            };
            GetChildListByReferenceResponseInternal internalResponse = GetChildListByReferenceInternal(internalRequest);
            GetChildListByReferenceExtendedResponse externalResponse = new GetChildListByReferenceExtendedResponse()
            {
                Response = new GetChildListByReferenceExtendedResponseBody()
                {
                    ResponseCode = internalResponse.ResponseCode,
                    ResponseMessage = internalResponse.ResponseMessage,
                    TransactionID = internalResponse.TransactionID
                }
            };
            if (internalResponse.ChildList != null && internalResponse.ChildList.Count > 0)
            {
                externalResponse.Response.ParentList = new ChildList();
                foreach (Internal.AgentInfoInternal internalChild in internalResponse.ChildList)
                {
                    External.AgentInfo externalChild = new External.AgentInfo()
                    {
                        Address = internalChild.Address,
                        Agent = internalChild.Agent,
                        AgentID = internalChild.AgentID,
                        Email = internalChild.Email,
                        Name = internalChild.Name,
                        OwnerID = internalChild.OwnerID,
                        ReferenceID = internalChild.ReferenceID
                    };
                    if (internalChild.AdditionalData != null && internalChild.AdditionalData.Count > 0)
                    {
                        externalChild.AdditionalDataOld = new ApiKeyValuePair();
                        foreach (KeyValuePair<String, String> keyValue in internalChild.AdditionalData)
                        {
                            externalChild.AdditionalDataOld.Add(keyValue.Key, keyValue.Value);
                        }
                    }
                    externalResponse.Response.ParentList.Add(externalChild);
                }
            }
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Management.GetChildListByReference", Logger.LoggingLevelType.Medium);
            return (externalResponse);
        }

        public GetAgentGroupsResponse GetAgentGroups(GetAgentGroupsRequest externalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Management.GetAgentGroups", Logger.LoggingLevelType.Medium);
            GetAgentGroupsRequestInternal internalRequest = new GetAgentGroupsRequestInternal()
            {
                SessionID = externalRequest.Request.SessionID,
                AgentID = externalRequest.Request.AgentID,
                DeviceType = externalRequest.Request.DeviceType
            };
            GetAgentGroupsResponseInternal internalResponse = GetAgentGroupsInternal(internalRequest);
            GetAgentGroupsResponse externalResponse = new GetAgentGroupsResponse()
            {
                Response = new GetAgentGroupsResponseBody()
                {
                    ResponseCode = internalResponse.ResponseCode,
                    ResponseMessage = internalResponse.ResponseMessage,
                    TransactionID = internalResponse.TransactionID
                }
            };
            if (internalResponse.GroupList != null && internalResponse.GroupList.Count > 0)
            {
                externalResponse.Response.GroupList = new GroupList();
                foreach (GroupInfoInternal internalGroup in internalResponse.GroupList)
                {
                    GroupInfo externalGroup = new GroupInfo()
                    {
                        GroupID = internalGroup.GroupID,
                        Name = internalGroup.Name,
                        Category = internalGroup.Category,
                        Type = internalGroup.Type
                    };
                    externalResponse.Response.GroupList.Add(externalGroup);
                }
            }
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Management.GetAgentGroups", Logger.LoggingLevelType.Medium);
            return (externalResponse);
        }
        public GetAgentGroupsExtendedResponse GetAgentGroupsExtended(GetAgentGroupsExtendedRequest externalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Management.GetAgentGroups", Logger.LoggingLevelType.Medium);
            LoginRequestInternal loginRequest = new LoginRequestInternal()
            {
                DeviceType = externalRequest.Request.DeviceType,
                Password = externalRequest.Request.Password,
                User = externalRequest.Request.Username
            };
            LoginResponseInternal loginResponse = AuthenticationProvider.LoginInternal(loginRequest);
            GetAgentGroupsRequestInternal internalRequest = new GetAgentGroupsRequestInternal()
            {
                SessionID = loginResponse.SessionID,
                AgentID = externalRequest.Request.AgentID,
                DeviceType = externalRequest.Request.DeviceType
            };
            GetAgentGroupsResponseInternal internalResponse = GetAgentGroupsInternal(internalRequest);
            GetAgentGroupsExtendedResponse externalResponse = new GetAgentGroupsExtendedResponse()
            {
                Response = new GetAgentGroupsExtendedResponseBody()
                {
                    ResponseCode = internalResponse.ResponseCode,
                    ResponseMessage = internalResponse.ResponseMessage,
                    TransactionID = internalResponse.TransactionID
                }
            };
            if (internalResponse.GroupList != null && internalResponse.GroupList.Count > 0)
            {
                externalResponse.Response.GroupList = new GroupList();
                foreach (GroupInfoInternal internalGroup in internalResponse.GroupList)
                {
                    GroupInfo externalGroup = new GroupInfo()
                    {
                        GroupID = internalGroup.GroupID,
                        Name = internalGroup.Name,
                        Category = internalGroup.Category,
                        Type = internalGroup.Type
                    };
                    externalResponse.Response.GroupList.Add(externalGroup);
                }
            }
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Management.GetAgentGroups", Logger.LoggingLevelType.Medium);
            return (externalResponse);
        }

        public GetProductsResponse GetProducts(GetProductsRequest externalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Management.GetProducts", Logger.LoggingLevelType.Medium);
            GetProductsRequestInternal internalRequest = new GetProductsRequestInternal()
            {
                AgentReference = externalRequest.Request.AgentReference,
                DeviceType = externalRequest.Request.DeviceType,
                SessionID = externalRequest.Request.SessionID
            };
            GetProductsResponseInternal internalResponse = GetProductsInternal(internalRequest);
            GetProductsResponse externalResponse = new GetProductsResponse()
            {
                Response = new GetProductsResponseBody()
                {
                    ResponseCode = internalResponse.ResponseCode,
                    ResponseMessage = internalResponse.ResponseMessage,
                    TransactionID = internalResponse.TransactionID
                }
            };
            if (internalResponse.KeyValuePair != null && internalResponse.KeyValuePair.Count > 0)
            {
                externalResponse.Response.KeyValuePair = new ApiKeyValuePair();
                foreach (KeyValuePair<String,String> keyPair in internalResponse.KeyValuePair)
                {
                    externalResponse.Response.KeyValuePair.Add(keyPair.Key, keyPair.Value);
                }
            }
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Management.GetProducts", Logger.LoggingLevelType.Medium);
            return (externalResponse);
        }
        public GetProductsExtendedResponse GetProductsExtended(GetProductsExtendedRequest externalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Management.GetProducts", Logger.LoggingLevelType.Medium);
            LoginRequestInternal loginRequest = new LoginRequestInternal()
            {
                DeviceType = externalRequest.Request.DeviceType,
                Password = externalRequest.Request.Password,
                User = externalRequest.Request.Username
            };
            LoginResponseInternal loginResponse = AuthenticationProvider.LoginInternal(loginRequest);
            GetProductsRequestInternal internalRequest = new GetProductsRequestInternal()
            {
                AgentReference = externalRequest.Request.AgentReference,
                DeviceType = externalRequest.Request.DeviceType,
                SessionID = loginResponse.SessionID
            };
            GetProductsResponseInternal internalResponse = GetProductsInternal(internalRequest);
            GetProductsExtendedResponse externalResponse = new GetProductsExtendedResponse()
            {
                Response = new GetProductsExtendedResponseBody()
                {
                    ResponseCode = internalResponse.ResponseCode,
                    ResponseMessage = internalResponse.ResponseMessage,
                    TransactionID = internalResponse.TransactionID
                }
            };
            if (internalResponse.KeyValuePair != null && internalResponse.KeyValuePair.Count > 0)
            {
                externalResponse.Response.KeyValuePair = new ApiKeyValuePair();
                foreach (KeyValuePair<String, String> keyPair in internalResponse.KeyValuePair)
                {
                    externalResponse.Response.KeyValuePair.Add(keyPair.Key, keyPair.Value);
                }
            }
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Management.GetProducts", Logger.LoggingLevelType.Medium);
            return (externalResponse);
        }

        public GetAgentListResponse GetAgentList(GetAgentListRequest externalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Management.GetAgentList", Logger.LoggingLevelType.Medium);
            GetAgentListRequestInternal internalRequest = new GetAgentListRequestInternal()
            {
                DeviceType = externalRequest.Request.DeviceType,
                SessionID = externalRequest.Request.SessionID
            };
            GetAgentListResponseInternal internalResponse = GetAgentListInternal(internalRequest);
            GetAgentListResponse externalResponse = new GetAgentListResponse()
            {
                Response = new GetAgentListResponseBody()
                {
                    ResponseCode = internalResponse.ResponseCode,
                    ResponseMessage = internalResponse.ResponseMessage,
                    TransactionID = internalResponse.TransactionID
                }
            };
            if (internalResponse.KeyValuePair != null && internalResponse.KeyValuePair.Count > 0)
            {
                externalResponse.Response.AgentList = new ApiKeyValuePair();
                foreach (KeyValuePair<String, String> keyPair in internalResponse.KeyValuePair)
                {
                    externalResponse.Response.AgentList.Add(keyPair.Key, keyPair.Value);
                }
            }
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Management.GetAgentList", Logger.LoggingLevelType.Medium);
            return (externalResponse);
        }
        public GetAgentListExtendedResponse GetAgentListExtended(GetAgentListExtendedRequest externalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Management.GetAgentList", Logger.LoggingLevelType.Medium);
            LoginRequestInternal loginRequest = new LoginRequestInternal()
            {
                DeviceType = externalRequest.Request.DeviceType,
                Password = externalRequest.Request.Password,
                User = externalRequest.Request.Username
            };
            LoginResponseInternal loginResponse = AuthenticationProvider.LoginInternal(loginRequest);
            GetAgentListRequestInternal internalRequest = new GetAgentListRequestInternal()
            {
                DeviceType = externalRequest.Request.DeviceType,
                SessionID = loginResponse.SessionID
            };
            GetAgentListResponseInternal internalResponse = GetAgentListInternal(internalRequest);
            GetAgentListExtendedResponse externalResponse = new GetAgentListExtendedResponse()
            {
                Response = new GetAgentListExtendedResponseBody()
                {
                    ResponseCode = internalResponse.ResponseCode,
                    ResponseMessage = internalResponse.ResponseMessage,
                    TransactionID = internalResponse.TransactionID
                }
            };
            if (internalResponse.KeyValuePair != null && internalResponse.KeyValuePair.Count > 0)
            {
                externalResponse.Response.AgentList = new ApiKeyValuePair();
                foreach (KeyValuePair<String, String> keyPair in internalResponse.KeyValuePair)
                {
                    externalResponse.Response.AgentList.Add(keyPair.Key, keyPair.Value);
                }
            }
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Management.GetAgentList", Logger.LoggingLevelType.Medium);
            return (externalResponse);
        }

        public GetAllAgentGroupsResponse GetAllAgentGroups(GetAllAgentGroupsRequest externalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Management.GetAllAgentGroups", Logger.LoggingLevelType.Medium);
            GetAllAgentGroupsRequestInternal internalRequest = new GetAllAgentGroupsRequestInternal()
            {
                SessionID = externalRequest.Request.SessionID,
                DeviceType = externalRequest.Request.DeviceType
            };
            GetAllAgentGroupsResponseInternal internalResponse = GetAllAgentGroupsInternal(internalRequest);
            GetAllAgentGroupsResponse externalResponse = new GetAllAgentGroupsResponse()
            {
                Response = new GetAllAgentGroupsResponseBody()
                {
                    ResponseCode = internalResponse.ResponseCode,
                    ResponseMessage = internalResponse.ResponseMessage,
                    TransactionID = internalResponse.TransactionID
                }
            };
            if (internalResponse.AllGroups != null && internalResponse.AllGroups.Count > 0)
            {
                externalResponse.Response.AllGroups = new GroupList();
                foreach (GroupInfoInternal internalGroup in internalResponse.AllGroups)
                {
                    GroupInfo externalGroup = new GroupInfo()
                    {
                        GroupID = internalGroup.GroupID,
                        Name = internalGroup.Name,
                        Category = internalGroup.Category,
                        Type = internalGroup.Type
                    };
                    externalResponse.Response.AllGroups.Add(externalGroup);
                }
            }
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Management.GetAllAgentGroups", Logger.LoggingLevelType.Medium);
            return (externalResponse);
        }
        public GetAllAgentGroupsExtendedResponse GetAllAgentGroupsExtended(GetAllAgentGroupsExtendedRequest externalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Management.GetAllAgentGroups", Logger.LoggingLevelType.Medium);
            LoginRequestInternal loginRequest = new LoginRequestInternal()
            {
                DeviceType = externalRequest.Request.DeviceType,
                Password = externalRequest.Request.Password,
                User = externalRequest.Request.Username
            };
            LoginResponseInternal loginResponse = AuthenticationProvider.LoginInternal(loginRequest);
            GetAllAgentGroupsRequestInternal internalRequest = new GetAllAgentGroupsRequestInternal()
            {
                SessionID = loginResponse.SessionID,
                DeviceType = externalRequest.Request.DeviceType
            };
            GetAllAgentGroupsResponseInternal internalResponse = GetAllAgentGroupsInternal(internalRequest);
            GetAllAgentGroupsExtendedResponse externalResponse = new GetAllAgentGroupsExtendedResponse()
            {
                Response = new GetAllAgentGroupsExtendedResponseBody()
                {
                    ResponseCode = internalResponse.ResponseCode,
                    ResponseMessage = internalResponse.ResponseMessage,
                    TransactionID = internalResponse.TransactionID
                }
            };
            if (internalResponse.AllGroups != null && internalResponse.AllGroups.Count > 0)
            {
                externalResponse.Response.AllGroups = new GroupList();
                foreach (GroupInfoInternal internalGroup in internalResponse.AllGroups)
                {
                    GroupInfo externalGroup = new GroupInfo()
                    {
                        GroupID = internalGroup.GroupID,
                        Name = internalGroup.Name,
                        Category = internalGroup.Category,
                        Type = internalGroup.Type
                    };
                    externalResponse.Response.AllGroups.Add(externalGroup);
                }
            }
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Management.GetAllAgentGroups", Logger.LoggingLevelType.Medium);
            return (externalResponse);
        }

        public MapAgentToGroupResponse MapAgentToGroup(MapAgentToGroupRequest externalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Management.MapAgentToGroup", Logger.LoggingLevelType.Medium);
            MapAgentToGroupRequestInternal internalRequest = new MapAgentToGroupRequestInternal()
            {
                SessionID = externalRequest.Request.SessionID,
                Reference = externalRequest.Request.Reference,
                GroupID = externalRequest.Request.GroupID,
                DeviceType = externalRequest.Request.DeviceType
            };
            MapAgentToGroupResponseInternal internalResponse = MapAgentToGroupInternal(internalRequest);
            MapAgentToGroupResponse externalResponse = new MapAgentToGroupResponse()
            {
                Response = new MapAgentToGroupResponseBody()
                {
                    ResponseCode = internalResponse.ResponseCode,
                    ResponseMessage = internalResponse.ResponseMessage,
                    TransactionID = internalResponse.TransactionID
                }
            };
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Management.MapAgentToGroup", Logger.LoggingLevelType.Medium);
            return (externalResponse);
        }
        public MapAgentToGroupExtendedResponse MapAgentToGroupExtended(MapAgentToGroupExtendedRequest externalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Management.MapAgentToGroup", Logger.LoggingLevelType.Medium);
            LoginRequestInternal loginRequest = new LoginRequestInternal()
            {
                DeviceType = externalRequest.Request.DeviceType,
                Password = externalRequest.Request.Password,
                User = externalRequest.Request.Username
            };
            LoginResponseInternal loginResponse = AuthenticationProvider.LoginInternal(loginRequest);
            MapAgentToGroupRequestInternal internalRequest = new MapAgentToGroupRequestInternal()
            {
                SessionID = loginResponse.SessionID,
                Reference = externalRequest.Request.Reference,
                GroupID = externalRequest.Request.GroupID,
                DeviceType = externalRequest.Request.DeviceType
            };
            MapAgentToGroupResponseInternal internalResponse = MapAgentToGroupInternal(internalRequest);
            MapAgentToGroupExtendedResponse externalResponse = new MapAgentToGroupExtendedResponse()
            {
                Response = new MapAgentToGroupExtendedResponseBody()
                {
                    ResponseCode = internalResponse.ResponseCode,
                    ResponseMessage = internalResponse.ResponseMessage,
                    TransactionID = internalResponse.TransactionID
                }
            };
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Management.MapAgentToGroup", Logger.LoggingLevelType.Medium);
            return (externalResponse);
        }

        public UnMapAgentToGroupResponse UnMapAgentToGroup(UnMapAgentToGroupRequest externalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Management.UnMapAgentToGroup", Logger.LoggingLevelType.Medium);
            UnMapAgentToGroupRequestInternal internalRequest = new UnMapAgentToGroupRequestInternal()
            {
                SessionID = externalRequest.Request.SessionID,
                Reference = externalRequest.Request.Reference,
                GroupID = externalRequest.Request.GroupID,
                DeviceType = externalRequest.Request.DeviceType
            };
            UnMapAgentToGroupResponseInternal internalResponse = UnMapAgentToGroupInternal(internalRequest);
            UnMapAgentToGroupResponse externalResponse = new UnMapAgentToGroupResponse()
            {
                Response = new UnMapAgentToGroupResponseBody()
                {
                    ResponseCode = internalResponse.ResponseCode,
                    ResponseMessage = internalResponse.ResponseMessage,
                    TransactionID = internalResponse.TransactionID
                }
            };
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Management.UnMapAgentToGroup", Logger.LoggingLevelType.Medium);
            return (externalResponse);
        }
        public UnMapAgentToGroupExtendedResponse UnMapAgentToGroupExtended(UnMapAgentToGroupExtendedRequest externalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Management.UnMapAgentToGroup", Logger.LoggingLevelType.Medium);
            LoginRequestInternal loginRequest = new LoginRequestInternal()
            {
                DeviceType = externalRequest.Request.DeviceType,
                Password = externalRequest.Request.Password,
                User = externalRequest.Request.Username
            };
            LoginResponseInternal loginResponse = AuthenticationProvider.LoginInternal(loginRequest);
            UnMapAgentToGroupRequestInternal internalRequest = new UnMapAgentToGroupRequestInternal()
            {
                SessionID = loginResponse.SessionID,
                Reference = externalRequest.Request.Reference,
                GroupID = externalRequest.Request.GroupID,
                DeviceType = externalRequest.Request.DeviceType
            };
            UnMapAgentToGroupResponseInternal internalResponse = UnMapAgentToGroupInternal(internalRequest);
            UnMapAgentToGroupExtendedResponse externalResponse = new UnMapAgentToGroupExtendedResponse()
            {
                Response = new UnMapAgentToGroupExtendedResponseBody()
                {
                    ResponseCode = internalResponse.ResponseCode,
                    ResponseMessage = internalResponse.ResponseMessage,
                    TransactionID = internalResponse.TransactionID
                }
            };
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Management.UnMapAgentToGroup", Logger.LoggingLevelType.Medium);
            return (externalResponse);
        }
        
        #region Internal Methods
        private GetAgentByReferenceResponseInternal GetAgentByReferenceInternal(GetAgentByReferenceRequestInternal internalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Management.GetAgentByReferenceInternal", Logger.LoggingLevelType.Medium);
            GetAgentByReferenceResponseInternal internalResponse = null;
            try
            {
                UMarketSCClient utibaClient = new UMarketSCClient();
                AgentResponse utibaAgentResponse = null;
                using (OperationContextScope scope = new OperationContextScope(utibaClient.InnerChannel))
                {
                    HttpRequestMessageProperty messageProperty = new HttpRequestMessageProperty();
                    messageProperty.Headers.Add(HttpRequestHeader.UserAgent, UserAgent);
                    OperationContext.Current.OutgoingMessageProperties.Add(HttpRequestMessageProperty.Name, messageProperty);
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Parámetros Recibidos Management.GetAgentByReferenceInternal: SessionID={0}, DeviceType={1}, Reference={2}"
                    , internalRequest.SessionID, internalRequest.DeviceType, internalRequest.Reference), Logger.LoggingLevelType.Low);
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Parámetros Enviados Management.GetAgentByReferenceInternal: SessionID={0}, DeviceType={1}, Reference={2}, Category={3}"
                    , internalRequest.SessionID, internalRequest.DeviceType, internalRequest.Reference, "agent"), Logger.LoggingLevelType.Low);
                    utibaAgentResponse = utibaClient.getAgentByReference(new getAgentByReference()
                    {
                        getAgentByReferenceRequest = new getAgentByReferenceRequest()
                        {
                            sessionid = internalRequest.SessionID,
                            device_type = internalRequest.DeviceType,
                            reference = internalRequest.Reference,
                            category = "agent"
                        }
                    });
                    internalResponse = new GetAgentByReferenceResponseInternal()
                    {
                        Address = utibaAgentResponse.AgentReturn.agent.address,
                        Agent = internalRequest.Reference,
                        AgentID=utibaAgentResponse.AgentReturn.agent.agentID,
                        Email = utibaAgentResponse.AgentReturn.agent.emailAddress,
                        Name = utibaAgentResponse.AgentReturn.agent.name,
                        OwnerID = utibaAgentResponse.AgentReturn.agent.ownerID,
                        ReferenceID = utibaAgentResponse.AgentReturn.agent.referenceID,
                        ResponseCode = utibaAgentResponse.AgentReturn.result,
                        ResponseMessage = utibaAgentResponse.AgentReturn.result_namespace,
                        TransactionID = utibaAgentResponse.AgentReturn.transid,
                        Depth = utibaAgentResponse.AgentReturn.agent.depth
                    };

                    if (utibaAgentResponse.AgentReturn.agent != null)
                    {
                        if (utibaAgentResponse.AgentReturn.agent.agentData != null)
                        {
                            internalResponse.AdditionalData = new Dictionary<string, string>();
                            foreach (KeyValuePair keyPair in utibaAgentResponse.AgentReturn.agent.agentData)
                            {
                                internalResponse.AdditionalData.Add(keyPair.key, keyPair.value);
                            }
                        }
                    }
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Resultado Obtenido Management.GetAgentByReferenceInternal: " +
                    "ResponseCode={0}, ResponseMessage={1}, TransactionID={2}, Address={3}, Agent={4}, AgentID={5}, Email={6}, Name={7}, OwnerID={8}, ReferenceID={9}", internalResponse.ResponseCode, 
                    internalResponse.ResponseMessage, internalResponse.TransactionID, internalResponse.Address, internalResponse.Agent,
                    internalResponse.AgentID, internalResponse.Email, internalResponse.Name, internalResponse.OwnerID, internalResponse.ReferenceID), Logger.LoggingLevelType.Low);
                }
            }
            catch (Exception ex)
            {
                Log(Logger.LogMessageType.Error, "Ocurrio una exception procesando el metodo Management.GetAgentByReferenceInternal, los detalles son: " + ex.ToString(), Logger.LoggingLevelType.Low);
                return (null);
            }
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Management.GetAgentByReferenceInternal", Logger.LoggingLevelType.Medium);
            return (internalResponse);

        }

        private GetParentListByReferenceIDResponseInternal GetParentListByReferenceIDInternal(GetParentListByReferenceIDRequestInternal internalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Management.GetParentListByReferenceIDInternal", Logger.LoggingLevelType.Medium);
            GetParentListByReferenceIDResponseInternal internalResponse = null;
            try
            {
                UMarketSCClient utibaClient = new UMarketSCClient();
                getParentListByReferenceIDResponse utibaGetParentListResponse = null;
                using (OperationContextScope scope = new OperationContextScope(utibaClient.InnerChannel))
                {
                    HttpRequestMessageProperty messageProperty = new HttpRequestMessageProperty();
                    messageProperty.Headers.Add(HttpRequestHeader.UserAgent, UserAgent);
                    OperationContext.Current.OutgoingMessageProperties.Add(HttpRequestMessageProperty.Name, messageProperty);
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Parámetros Recibidos Management.GetParentListByReferenceIDInternal: SessionID={0}, DeviceType={1}, AgentReferenceID={2}"
                    , internalRequest.SessionID, internalRequest.DeviceType, internalRequest.AgentID), Logger.LoggingLevelType.Low);
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Parámetros Enviados Management.GetParentListByReferenceIDInternal: SessionID={0}, DeviceType={1}, AgentReferenceID={2}"
                    , internalRequest.SessionID, internalRequest.DeviceType, internalRequest.AgentID), Logger.LoggingLevelType.Low);
                    utibaGetParentListResponse = utibaClient.getParentListByReferenceID(new getParentListByReferenceIDRequest()
                    {
                        getParentListByReferenceIDRequestType = new getParentListByReferenceIDRequestType()
                        {
                            sessionid=internalRequest.SessionID,
                            device_type=internalRequest.DeviceType,
                            agentReferenceID=internalRequest.AgentID
                        }
                    });
                       
                    internalResponse = new GetParentListByReferenceIDResponseInternal()
                    {
                        ResponseCode = utibaGetParentListResponse.getParentListByReferenceIDResponseType.result,
                        ResponseMessage = utibaGetParentListResponse.getParentListByReferenceIDResponseType.result_message,
                        TransactionID = utibaGetParentListResponse.getParentListByReferenceIDResponseType.transid
                    };


                    if (utibaGetParentListResponse.getParentListByReferenceIDResponseType.agentList != null &&
                        utibaGetParentListResponse.getParentListByReferenceIDResponseType.agentList.Length > 0)
                    {
                        internalResponse.ParentList = new List<Internal.AgentInfoInternal>();
                        foreach (Agent parent in utibaGetParentListResponse.getParentListByReferenceIDResponseType.agentList)
                        {
                            Internal.AgentInfoInternal agentInfo = new Internal.AgentInfoInternal()
                            {
                                Address = parent.address,
                                Agent = parent.reference,
                                AgentID = parent.agentID,
                                Email = parent.emailAddress,
                                Name = parent.name,
                                OwnerID = parent.ownerID,
                                ReferenceID = parent.referenceID
                            };
                            if (parent.agentData != null && parent.agentData.Length > 0)
                            {
                                agentInfo.AdditionalData = new Dictionary<string, string>();
                                foreach (KeyValuePair keyPair in parent.agentData)
                                {
                                    agentInfo.AdditionalData.Add(keyPair.key, keyPair.value);
                                }
                            }
                            internalResponse.ParentList.Add(agentInfo);
                        }
                    }
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Resultado Obtenido Management.GetParentListByReferenceIDInternal: " +
                    "ResponseCode={0}, ResponseMessage={1}, TransactionID={2}", internalResponse.ResponseCode, internalResponse.ResponseMessage, internalResponse.TransactionID), Logger.LoggingLevelType.Low);
                }
            }
            catch (Exception ex)
            {
                Log(Logger.LogMessageType.Error, "Ocurrio una exception procesando el metodo Management.GetParentListByReferenceIDInternal, los detalles son: " + ex.ToString(), Logger.LoggingLevelType.Low);
                return (null);
            }
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Management.GetParentListByReferenceIDInternal", Logger.LoggingLevelType.Medium);
            return (internalResponse);
        }

        private GetChildListByReferenceResponseInternal GetChildListByReferenceInternal(GetChildListByReferenceRequestInternal internalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Management.GetChildListByReferenceInternal", Logger.LoggingLevelType.Medium);
            GetChildListByReferenceResponseInternal internalResponse = null;
            try
            {
                UMarketSCClient utibaClient = new UMarketSCClient();
                getChildListByReferenceResponse utibaGetChildListResponse = null;
                using (OperationContextScope scope = new OperationContextScope(utibaClient.InnerChannel))
                {
                    HttpRequestMessageProperty messageProperty = new HttpRequestMessageProperty();
                    messageProperty.Headers.Add(HttpRequestHeader.UserAgent, UserAgent);
                    OperationContext.Current.OutgoingMessageProperties.Add(HttpRequestMessageProperty.Name, messageProperty);
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Parámetros Recibidos Management.GetChildListByReferenceInternal: SessionID={0}, DeviceType={1}, AgentReference={2}"
                    , internalRequest.SessionID, internalRequest.DeviceType, internalRequest.Reference), Logger.LoggingLevelType.Low);
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Parámetros Enviados Management.GetChildListByReferenceInternal: SessionID={0}, DeviceType={1}, AgentReference={2}"
                    , internalRequest.SessionID, internalRequest.DeviceType, internalRequest.Reference), Logger.LoggingLevelType.Low);
                    utibaGetChildListResponse = utibaClient.getChildListByReference(new getChildListByReferenceRequest()
                    {
                        getChildListByReferenceRequestType = new getChildListByReferenceRequestType()
                        {
                            sessionid = internalRequest.SessionID,
                            device_type = internalRequest.DeviceType,
                            agentReference = internalRequest.Reference
                        }
                    });

                    internalResponse = new GetChildListByReferenceResponseInternal()
                    {
                        ResponseCode = utibaGetChildListResponse.getChildListByReferenceResponseType.result,
                        ResponseMessage = utibaGetChildListResponse.getChildListByReferenceResponseType.result_message,
                        TransactionID = utibaGetChildListResponse.getChildListByReferenceResponseType.transid
                    };


                    if (utibaGetChildListResponse.getChildListByReferenceResponseType.agentList != null &&
                        utibaGetChildListResponse.getChildListByReferenceResponseType.agentList.Length > 0)
                    {
                        internalResponse.ChildList = new List<Internal.AgentInfoInternal>();
                        foreach (Agent parent in utibaGetChildListResponse.getChildListByReferenceResponseType.agentList)
                        {
                            Internal.AgentInfoInternal agentInfo = new Internal.AgentInfoInternal()
                            {
                                Address = parent.address,
                                Agent = parent.reference,
                                AgentID = parent.agentID,
                                Email = parent.emailAddress,
                                Name = parent.name,
                                OwnerID = parent.ownerID,
                                ReferenceID = parent.referenceID
                            };
                            if (parent.agentData != null && parent.agentData.Length > 0)
                            {
                                agentInfo.AdditionalData = new Dictionary<string, string>();
                                foreach (KeyValuePair keyPair in parent.agentData)
                                {
                                    agentInfo.AdditionalData.Add(keyPair.key, keyPair.value);
                                }
                            }
                            internalResponse.ChildList.Add(agentInfo);
                        }
                    }
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Resultado Obtenido Management.GetChildListByReferenceInternal: " +
                    "ResponseCode={0}, ResponseMessage={1}, TransactionID={2}", internalResponse.ResponseCode, internalResponse.ResponseMessage, internalResponse.TransactionID), Logger.LoggingLevelType.Low);
                }
            }
            catch (Exception ex)
            {
                Log(Logger.LogMessageType.Error, "Ocurrio una exception procesando el metodo Management.GetChildListByReferenceInternal, los detalles son: " + ex.ToString(), Logger.LoggingLevelType.Low);
                return (null);
            }
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Management.GetChildListByReferenceInternal", Logger.LoggingLevelType.Medium);
            return (internalResponse);
        }

        private GetAgentGroupsResponseInternal GetAgentGroupsInternal(GetAgentGroupsRequestInternal internalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Management.GetAgentGroupsInternal", Logger.LoggingLevelType.Medium);
            GetAgentGroupsResponseInternal internalResponse = null;
            try
            {
                UMarketSCClient utibaClient = new UMarketSCClient();
                AgentGroupsResponse utibaAgentGroupsResponse = null;
                using (OperationContextScope scope = new OperationContextScope(utibaClient.InnerChannel))
                {
                    HttpRequestMessageProperty messageProperty = new HttpRequestMessageProperty();
                    messageProperty.Headers.Add(HttpRequestHeader.UserAgent, UserAgent);
                    OperationContext.Current.OutgoingMessageProperties.Add(HttpRequestMessageProperty.Name, messageProperty);
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Parámetros Recibidos Management.GetAgentGroupsInternal: SessionID={0}, DeviceType={1}, AgentID={2}"
                    , internalRequest.SessionID, internalRequest.DeviceType, internalRequest.AgentID), Logger.LoggingLevelType.Low);
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Parámetros Enviados Management.GetAgentGroupsInternal: SessionID={0}, DeviceType={1}, AgentID={2}"
                    , internalRequest.SessionID, internalRequest.DeviceType, internalRequest.AgentID), Logger.LoggingLevelType.Low);
                    utibaAgentGroupsResponse = utibaClient.getAgentGroupByAgentID(new getAgentGroupByAgentID()
                    {
                        getAgentGroupByAgentIDRequest = new getAgentGroupByAgentIDRequest()
                        {
                            sessionid = internalRequest.SessionID,
                            device_type = internalRequest.DeviceType,
                            agentID=internalRequest.AgentID
                        }
                    });
                    internalResponse = new GetAgentGroupsResponseInternal()
                    {
                        ResponseCode = utibaAgentGroupsResponse.AgentGroupsReturn.result,
                        ResponseMessage = utibaAgentGroupsResponse.AgentGroupsReturn.result_namespace,
                        TransactionID = utibaAgentGroupsResponse.AgentGroupsReturn.transid
                    };

                    if (utibaAgentGroupsResponse.AgentGroupsReturn.agentGroups != null &&
                        utibaAgentGroupsResponse.AgentGroupsReturn.agentGroups.Length>0)
                    {
                        internalResponse.GroupList = new List<GroupInfoInternal>();
                        foreach (AgentGroup agentGroup in utibaAgentGroupsResponse.AgentGroupsReturn.agentGroups)
                        {
                            internalResponse.GroupList.Add(new GroupInfoInternal()
                            {
                                GroupID = agentGroup.ID,
                                Name = agentGroup.name,
                                Category = agentGroup.category,
                                Type = agentGroup.type
                            });
                        }
                    }
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Resultado Obtenido Management.GetAgentGroupsInternal: " +
                    "ResponseCode={0}, ResponseMessage={1}, TransactionID={2}", internalResponse.ResponseCode, internalResponse.ResponseMessage, internalResponse.TransactionID), Logger.LoggingLevelType.Low);
                }
            }
            catch (Exception ex)
            {
                Log(Logger.LogMessageType.Error, "Ocurrio una exception procesando el metodo Management.GetAgentGroupsInternal, los detalles son: " + ex.ToString(), Logger.LoggingLevelType.Low);
                return (null);
            }
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Management.GetAgentGroupsInternal", Logger.LoggingLevelType.Medium);
            return (internalResponse);
        }

        private GetProductsResponseInternal GetProductsInternal(GetProductsRequestInternal internalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Management.GetProductsInternal", Logger.LoggingLevelType.Medium);
            GetProductsResponseInternal internalResponse = null;
            try
            {
                UMarketSCClient utibaClient = new UMarketSCClient();
                getProductListResponse utibaGetProductListResponse = null;
                using (OperationContextScope scope = new OperationContextScope(utibaClient.InnerChannel))
                {
                    HttpRequestMessageProperty messageProperty = new HttpRequestMessageProperty();
                    messageProperty.Headers.Add(HttpRequestHeader.UserAgent, UserAgent);
                    OperationContext.Current.OutgoingMessageProperties.Add(HttpRequestMessageProperty.Name, messageProperty);
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Parámetros Recibidos Management.GetProductsInternal: SessionID={0}, DeviceType={1}, AgentReference={2}"
                    , internalRequest.SessionID, internalRequest.DeviceType, internalRequest.AgentReference), Logger.LoggingLevelType.Low);
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Parámetros Enviados Management.GetProductsInternal: SessionID={0}, DeviceType={1}, AgentReference={2}"
                    , internalRequest.SessionID, internalRequest.DeviceType, internalRequest.AgentReference), Logger.LoggingLevelType.Low);
                    utibaGetProductListResponse = utibaClient.getProductList(new getProductList()
                    {
                        getProductListRequest = new getProductListRequestType()
                        {
                            sessionid = internalRequest.SessionID,
                            device_type = internalRequest.DeviceType,
                            agent_reference = internalRequest.AgentReference
                        }
                    });
                    internalResponse = new GetProductsResponseInternal()
                    {
                        ResponseCode = utibaGetProductListResponse.getProductListReturn.result,
                        ResponseMessage = utibaGetProductListResponse.getProductListReturn.result_message,
                        TransactionID = utibaGetProductListResponse.getProductListReturn.transid
                    };

                    if (utibaGetProductListResponse.getProductListReturn.products != null &&
                        utibaGetProductListResponse.getProductListReturn.products.Length > 0)
                    {
                        internalResponse.KeyValuePair = new Dictionary<string, string>();
                        foreach (KeyValuePair1 keyValuePair1 in utibaGetProductListResponse.getProductListReturn.products)
                        {
                            internalResponse.KeyValuePair.Add(keyValuePair1.key, keyValuePair1.value);
                        }
                    }
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Resultado Obtenido Management.GetProductsInternal: " +
                    "ResponseCode={0}, ResponseMessage={1}, TransactionID={2}", internalResponse.ResponseCode, internalResponse.ResponseMessage, internalResponse.TransactionID), Logger.LoggingLevelType.Low);
                }
            }
            catch (Exception ex)
            {
                Log(Logger.LogMessageType.Error, "Ocurrio una exception procesando el metodo Management.GetProductsInternal, los detalles son: " + ex.ToString(), Logger.LoggingLevelType.Low);
                return (null);
            }
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Management.GetProductsInternal", Logger.LoggingLevelType.Medium);
            return (internalResponse);
        }

        private GetAgentListResponseInternal GetAgentListInternal(GetAgentListRequestInternal internalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Management.GetAgentListInternal", Logger.LoggingLevelType.Medium);
            GetAgentListResponseInternal internalResponse = null;
            try
            {
                UMarketSCClient utibaClient = new UMarketSCClient();
                getAgentListResponse utibaGetAgentListResponse = null;
                using (OperationContextScope scope = new OperationContextScope(utibaClient.InnerChannel))
                {
                    HttpRequestMessageProperty messageProperty = new HttpRequestMessageProperty();
                    messageProperty.Headers.Add(HttpRequestHeader.UserAgent, UserAgent);
                    OperationContext.Current.OutgoingMessageProperties.Add(HttpRequestMessageProperty.Name, messageProperty);
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Parámetros Recibidos Management.GetAgentListInternal: SessionID={0}, DeviceType={1}"
                    , internalRequest.SessionID, internalRequest.DeviceType), Logger.LoggingLevelType.Low);
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Parámetros Enviados Management.GetAgentListInternal: SessionID={0}, DeviceType={1}"
                    , internalRequest.SessionID, internalRequest.DeviceType), Logger.LoggingLevelType.Low);
                    utibaGetAgentListResponse = utibaClient.getAgentList(new getAgentList()
                    {
                        getAgentListRequest = new getAgentListRequestType()
                        {
                            sessionid = internalRequest.SessionID,
                            device_type = internalRequest.DeviceType
                        }
                    });
                    internalResponse = new GetAgentListResponseInternal()
                    {
                        ResponseCode = utibaGetAgentListResponse.getAgentListReturn.result,
                        ResponseMessage = utibaGetAgentListResponse.getAgentListReturn.result_message,
                        TransactionID = utibaGetAgentListResponse.getAgentListReturn.transid
                    };

                    if (utibaGetAgentListResponse.getAgentListReturn.agents != null &&
                        utibaGetAgentListResponse.getAgentListReturn.agents.Length > 0)
                    {
                        internalResponse.KeyValuePair = new Dictionary<string, string>();
                        foreach (KeyValuePair1 keyValuePair1 in utibaGetAgentListResponse.getAgentListReturn.agents)
                        {
                            internalResponse.KeyValuePair.Add(keyValuePair1.key, keyValuePair1.value);
                        }
                    }
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Resultado Obtenido Management.GetAgentListInternal: " +
                    "ResponseCode={0}, ResponseMessage={1}, TransactionID={2}", internalResponse.ResponseCode, internalResponse.ResponseMessage, internalResponse.TransactionID), Logger.LoggingLevelType.Low);
                }
            }
            catch (Exception ex)
            {
                Log(Logger.LogMessageType.Error, "Ocurrio una exception procesando el metodo Management.GetAgentListInternal, los detalles son: " + ex.ToString(), Logger.LoggingLevelType.Low);
                return (null);
            }
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Management.GetAgentListInternal", Logger.LoggingLevelType.Medium);
            return (internalResponse);
        }

        private GetAllAgentGroupsResponseInternal GetAllAgentGroupsInternal(GetAllAgentGroupsRequestInternal internalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Management.GetAllAgentGroupsInternal", Logger.LoggingLevelType.Medium);
            GetAllAgentGroupsResponseInternal internalResponse = null;
            try
            {
                UMarketSCClient utibaClient = new UMarketSCClient();
                AgentGroupsResponse utibaGetAllAgentGroupsResponse = null;
                using (OperationContextScope scope = new OperationContextScope(utibaClient.InnerChannel))
                {
                    HttpRequestMessageProperty messageProperty = new HttpRequestMessageProperty();
                    messageProperty.Headers.Add(HttpRequestHeader.UserAgent, UserAgent);
                    OperationContext.Current.OutgoingMessageProperties.Add(HttpRequestMessageProperty.Name, messageProperty);
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Parámetros Recibidos Management.GetAllAgentGroupsInternal: SessionID={0}, DeviceType={1}"
                    , internalRequest.SessionID, internalRequest.DeviceType), Logger.LoggingLevelType.Low);
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Parámetros Enviados Management.GetAllAgentGroupsInternal: SessionID={0}, DeviceType={1}, Category={2}"
                    , internalRequest.SessionID, internalRequest.DeviceType, "agent"), Logger.LoggingLevelType.Low);
                    utibaGetAllAgentGroupsResponse = utibaClient.getAllAgentGroups(new getAllAgentGroups()
                    {
                        getAllAgentGroupsRequest = new getAllGroupsRequestType()
                        {
                            sessionid = internalRequest.SessionID,
                            device_type = internalRequest.DeviceType,
                            filter = new getAllGroupsRequestTypeFilter()
                            {
                                category="agent",
                                includeUncategorised = true
                            }
                        }
                    });
                    internalResponse = new GetAllAgentGroupsResponseInternal()
                    {
                        ResponseCode = utibaGetAllAgentGroupsResponse.AgentGroupsReturn.result,
                        ResponseMessage = utibaGetAllAgentGroupsResponse.AgentGroupsReturn.result_namespace,
                        TransactionID = utibaGetAllAgentGroupsResponse.AgentGroupsReturn.transid
                    };

                    if (utibaGetAllAgentGroupsResponse.AgentGroupsReturn.agentGroups != null &&
                        utibaGetAllAgentGroupsResponse.AgentGroupsReturn.agentGroups.Length > 0)
                    {
                        internalResponse.AllGroups = new List<GroupInfoInternal>();
                        foreach (AgentGroup agentGroup in utibaGetAllAgentGroupsResponse.AgentGroupsReturn.agentGroups)
                        {
                            internalResponse.AllGroups.Add(new GroupInfoInternal()
                            {
                                GroupID = agentGroup.ID,
                                Name = agentGroup.name,
                                Category = agentGroup.category,
                                Type = agentGroup.type
                            });
                        }
                    }
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Resultado Obtenido Management.GetAllAgentGroupsInternal: " +
                    "ResponseCode={0}, ResponseMessage={1}, TransactionID={2}", internalResponse.ResponseCode, internalResponse.ResponseMessage, internalResponse.TransactionID), Logger.LoggingLevelType.Low);
                }
            }
            catch (Exception ex)
            {
                Log(Logger.LogMessageType.Error, "Ocurrio una exception procesando el metodo Management.GetAllAgentGroupsInternal, los detalles son: " + ex.ToString(), Logger.LoggingLevelType.Low);
                return (null);
            }
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Management.GetAllAgentGroupsInternal", Logger.LoggingLevelType.Medium);
            return (internalResponse);
        }

        private MapAgentToGroupResponseInternal MapAgentToGroupInternal(MapAgentToGroupRequestInternal internalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Management.MapAgentToGroupInternal", Logger.LoggingLevelType.Medium);
            MapAgentToGroupResponseInternal internalResponse = null;
            try
            {
                UMarketSCClient utibaClient = new UMarketSCClient();
                mapAgentResponse utibaMapAgentResponse = null;
                using (OperationContextScope scope = new OperationContextScope(utibaClient.InnerChannel))
                {
                    HttpRequestMessageProperty messageProperty = new HttpRequestMessageProperty();
                    messageProperty.Headers.Add(HttpRequestHeader.UserAgent, UserAgent);
                    OperationContext.Current.OutgoingMessageProperties.Add(HttpRequestMessageProperty.Name, messageProperty);
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Parámetros Recibidos Management.MapAgentToGroupInternal: SessionID={0}, DeviceType={1}, GroupID={2}, Reference={3}"
                    , internalRequest.SessionID, internalRequest.DeviceType, internalRequest.GroupID, internalRequest.Reference), Logger.LoggingLevelType.Low);
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Parámetros Enviados Management.MapAgentToGroupInternal: SessionID={0}, DeviceType={1}, GroupID={2}, Reference={3}"
                    , internalRequest.SessionID, internalRequest.DeviceType, internalRequest.GroupID, internalRequest.Reference), Logger.LoggingLevelType.Low);
                    utibaMapAgentResponse = utibaClient.mapAgent(new mapAgentRequest()
                    {
                        mapAgentRequestType = new mapAgentRequestType()
                        {
                            sessionid = internalRequest.SessionID,
                            device_type = internalRequest.DeviceType,
                            agid=internalRequest.GroupID,
                            agent=internalRequest.Reference
                        }
                    });
                    internalResponse = new MapAgentToGroupResponseInternal()
                    {
                        ResponseCode = utibaMapAgentResponse.mapAgentReturn.result,
                        ResponseMessage = utibaMapAgentResponse.mapAgentReturn.result_message,
                        TransactionID = utibaMapAgentResponse.mapAgentReturn.transid
                    };
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Resultado Obtenido Management.MapAgentToGroupInternal: " +
                    "ResponseCode={0}, ResponseMessage={1}, TransactionID={2}", internalResponse.ResponseCode, internalResponse.ResponseMessage, internalResponse.TransactionID), Logger.LoggingLevelType.Low);
                }
            }
            catch (Exception ex)
            {
                Log(Logger.LogMessageType.Error, "Ocurrio una exception procesando el metodo Management.MapAgentToGroupInternal, los detalles son: " + ex.ToString(), Logger.LoggingLevelType.Low);
                return (null);
            }
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Management.MapAgentToGroupInternal", Logger.LoggingLevelType.Medium);
            return (internalResponse);
        }

        private UnMapAgentToGroupResponseInternal UnMapAgentToGroupInternal(UnMapAgentToGroupRequestInternal internalRequest)
        {
            Log(Logger.LogMessageType.Info, "->   -------------------- Comienza la ejecución del método Management.UnMapAgentToGroupInternal", Logger.LoggingLevelType.Medium);
            UnMapAgentToGroupResponseInternal internalResponse = null;
            try
            {
                UMarketSCClient utibaClient = new UMarketSCClient();
                unmapAgentResponse utibaUnMapAgentResponse = null;
                using (OperationContextScope scope = new OperationContextScope(utibaClient.InnerChannel))
                {
                    HttpRequestMessageProperty messageProperty = new HttpRequestMessageProperty();
                    messageProperty.Headers.Add(HttpRequestHeader.UserAgent, UserAgent);
                    OperationContext.Current.OutgoingMessageProperties.Add(HttpRequestMessageProperty.Name, messageProperty);
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Parámetros Recibidos Management.MapAgentToGroupInternal: SessionID={0}, DeviceType={1}, GroupID={2}, Reference={3}"
                    , internalRequest.SessionID, internalRequest.DeviceType, internalRequest.GroupID, internalRequest.Reference), Logger.LoggingLevelType.Low);
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Parámetros Enviados Management.MapAgentToGroupInternal: SessionID={0}, DeviceType={1}, GroupID={2}, Reference={3}"
                    , internalRequest.SessionID, internalRequest.DeviceType, internalRequest.GroupID, internalRequest.Reference), Logger.LoggingLevelType.Low);
                    utibaUnMapAgentResponse = utibaClient.unmapAgent(new unmapAgentRequest()
                    {
                        unmapAgentRequestType = new unmapAgentRequestType()
                        {
                            sessionid = internalRequest.SessionID,
                            device_type = internalRequest.DeviceType,
                            agid = internalRequest.GroupID,
                            agent = internalRequest.Reference
                        }
                    });
                    internalResponse = new UnMapAgentToGroupResponseInternal()
                    {
                        ResponseCode = utibaUnMapAgentResponse.unmapAgentReturn.result,
                        ResponseMessage = utibaUnMapAgentResponse.unmapAgentReturn.result_message,
                        TransactionID = utibaUnMapAgentResponse.unmapAgentReturn.transid
                    };
                    Log(Logger.LogMessageType.Info, "->   -------------------- " + String.Format("Resultado Obtenido Management.MapAgentToGroupInternal: " +
                    "ResponseCode={0}, ResponseMessage={1}, TransactionID={2}", internalResponse.ResponseCode, internalResponse.ResponseMessage, internalResponse.TransactionID), Logger.LoggingLevelType.Low);
                }
            }
            catch (Exception ex)
            {
                Log(Logger.LogMessageType.Error, "Ocurrio una exception procesando el metodo Management.MapAgentToGroupInternal, los detalles son: " + ex.ToString(), Logger.LoggingLevelType.Low);
                return (null);
            }
            Log(Logger.LogMessageType.Info, "->   -------------------- Termina la ejecución del método Management.UnMapAgentToGroupInternal", Logger.LoggingLevelType.Medium);
            return (internalResponse);
        }
    #endregion
    }
}
