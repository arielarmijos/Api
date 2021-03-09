using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Security.Cryptography;
using System.Text;
using Movilway.API.Utiba;
using System.ServiceModel.Channels;
using System.Net;
using System.Configuration;
using Movilway.API.Service.Internal;
using Movilway.API.Service.External;
using Movilway.API.Log;

namespace Movilway.API.Service
{
    public class AuthenticationProvider
    {
        private static String UserAgent;
        static AuthenticationProvider()
        {
            UserAgent = ConfigurationManager.AppSettings["UserAgent"];
        }

        internal static LoginResponseInternal LoginInternal(LoginRequestInternal loginRequest)
        {
            LogUtils.LogMethodInvocationStart(true, loginRequest);


            UMarketSCClient utibaClient = new UMarketSCClient();
            loginResponse loginInfo;
            createsessionResponse newSession;
            LoginResponseInternal loginResponse = null;
            try
            {
                using (OperationContextScope scope = new OperationContextScope(utibaClient.InnerChannel))
                {
                    HttpRequestMessageProperty messageProperty = new HttpRequestMessageProperty();
                    messageProperty.Headers.Add(HttpRequestHeader.UserAgent, UserAgent);
                    OperationContext.Current.OutgoingMessageProperties.Add(HttpRequestMessageProperty.Name, messageProperty);

                    newSession = utibaClient.createsession(new createsession());
                    String passwordHash = GetSHA1(loginRequest.User.ToLower() + loginRequest.Password);
                    String passwordAndSessionHash = GetSHA1(newSession.createsessionReturn.sessionid + passwordHash.ToLower()).ToUpper();
                    
                    login loginObject = new login();
                    loginObject.loginRequest = new loginRequestType()
                    {
                        sessionid = newSession.createsessionReturn.sessionid,
                        device_type = loginRequest.DeviceType,
                        initiator = loginRequest.User,
                        pin = passwordAndSessionHash
                    };
                    loginInfo = utibaClient.login(loginObject);
                }
                loginResponse = new LoginResponseInternal()
                {
                    ResponseCode = loginInfo.loginReturn.result,
                    ResponseMessage = loginInfo.loginReturn.result_message,
                    TransactionID = loginInfo.loginReturn.transid,
                    SessionID = newSession.createsessionReturn.sessionid
                };

                if (loginResponse.ResponseCode != 0)
                    loginResponse.SetResponseNamespace(ApiResponseInternal.ResponseNamespace.BAC);
            }
            catch(Exception e)
            {
                if (loginResponse == null)
                    loginResponse = new LoginResponseInternal();
                loginResponse.SetThrowedException(e);
            }
            return (loginResponse);
        }

        internal static GetSessionResponse GetSession(GetSessionRequest getSessionRequest)
        {
            LoginRequestInternal loginRequest = new LoginRequestInternal()
            {
                User=getSessionRequest.Request.User,
                Password = getSessionRequest.Request.Password,
                DeviceType = getSessionRequest.Request.DeviceType
            };
            LoginResponseInternal loginResponse = AuthenticationProvider.LoginInternal(loginRequest);
            GetSessionResponse getSessionResponse = new GetSessionResponse();
            GetSessionResponseBody getSessionResponseBody = new GetSessionResponseBody()
            { 
                ResponseCode = loginResponse.ResponseCode,
                ResponseMessage = loginResponse.ResponseMessage,
                SessionID = loginResponse.SessionID,
                TransactionID = loginResponse.TransactionID
            };
            getSessionResponse.Response = getSessionResponseBody;
            return (getSessionResponse);
        }

        internal static LoginResponse Login(LoginRequest getSessionRequest)
        {
            LoginRequestInternal loginRequest = new LoginRequestInternal()
            {
                User = getSessionRequest.Request.AccessId,
                Password = getSessionRequest.Request.Password,
                DeviceType = getSessionRequest.Request.AccessType
            };
            LoginResponseInternal loginResponseInternal = AuthenticationProvider.LoginInternal(loginRequest);
            LoginResponse loginResponse = new LoginResponse();
            LoginResponseBody loginResponseBody = new LoginResponseBody()
            {
                LoginResult=loginResponseInternal.SessionID,
                Message=loginResponseInternal.ResponseMessage
            };
            loginResponse.Response = loginResponseBody;
            return (loginResponse);
        }

        internal static ChangePinResponseInternal ChangePinInternal(ChangePinRequestInternal internalRequest)
        {
            UMarketSCClient utibaClient = new UMarketSCClient();
            ChangePinResponseInternal internalResponse = null;
            using (OperationContextScope scope = new OperationContextScope(utibaClient.InnerChannel))
            {
                HttpRequestMessageProperty messageProperty = new HttpRequestMessageProperty();
                messageProperty.Headers.Add(HttpRequestHeader.UserAgent, UserAgent);
                OperationContext.Current.OutgoingMessageProperties.Add(HttpRequestMessageProperty.Name, messageProperty);

                //throw new Exception("Exception fuerte");
                string passwordHash = GenerateHash(internalRequest.SessionID, internalRequest.Initiator, internalRequest.CurrentPin);
                pinResponse pinChangeUtiba = utibaClient.pin(new pin() { pinRequest = new pinRequestType() { sessionid = internalRequest.SessionID, device_type = internalRequest.DeviceType, new_pin = internalRequest.NewPin, pin = passwordHash, initiator = internalRequest.Initiator } });

                internalResponse = new ChangePinResponseInternal()
                {
                    ResponseCode = pinChangeUtiba.pinReturn.result,
                    ResponseMessage=pinChangeUtiba.pinReturn.result_message,
                    TransactionID=pinChangeUtiba.pinReturn.transid
                };
            }
            return (internalResponse);
        }

        internal static ChangePinResponse ChangePin(ChangePinRequest externalRequest)
        {
            ChangePinRequestInternal internalRequest = new ChangePinRequestInternal()
            {
                Initiator=externalRequest.Request.Initiator,
                DeviceType=externalRequest.Request.DeviceType,
                NewPin=externalRequest.Request.NewPin,
                SessionID=externalRequest.Request.SessionID,
                CurrentPin = externalRequest.Request.CurrentPin
            };
            ChangePinResponseInternal internalResponse = ChangePinInternal(internalRequest);
            ChangePinResponse externalResponse = new ChangePinResponse();
            ChangePinResponseBody externalResponseBody = new ChangePinResponseBody()
            {
                ResponseCode = internalResponse.ResponseCode,
                ResponseMessage = internalResponse.ResponseMessage,
                TransactionID = internalResponse.TransactionID
            };
            externalResponse.Response = externalResponseBody;
            return (externalResponse);
        }

        private static String GenerateHash(String sessionID, String user, String password)
        {
            string sResult = GetSHA1(user.ToLower() + password);
            sResult = (GetSHA1(sessionID + sResult.ToLower())).ToUpper();
            return sResult;
        }

        private static string GetSHA1(string str)
        {
            SHA1 sha1 = SHA1Managed.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder sb = new StringBuilder();
            stream = sha1.ComputeHash(encoding.GetBytes(str));
            for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
            return sb.ToString();
        }
    }
}