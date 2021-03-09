using Movilway.API.Core;
using Movilway.API.Core.Security;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Xml;


namespace Movilway.API.Service.ExtendedApi.Provider.Kinacu
{

    [ServiceProviderImpl(Platform = ApiTargetPlatform.Kinacu, ServiceName = ApiServiceName.QueryPayment)]
    public class QueryPaymentProvider : AKinacuProvider
    {

        private static Logging.ILogger logger = Logging.LoggerFactory.GetLogger(typeof(QueryPaymentProvider));

        private static Dictionary<string, ProductInfo> _productsInfo;

        private const int DEF_TIME_OUT = 40000;

        private const string agency_prefix ="age_id:";
        private const string user_prefix = "usr:";
        private static string AssemblyDirectory
        {
            get
            {
                string codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return System.IO.Path.GetDirectoryName(path);
            }
        }

        static QueryPaymentProvider()
        {
           LoadProducts();
        }


        private static void LoadProducts()
        {
            var _productsInfo = new Dictionary<string, ProductInfo>();


            try
            {


                var directorypath = !string.IsNullOrEmpty(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath) ? System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath : AssemblyDirectory;

                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();

                //var CONTENT = System.IO.File.ReadAllText(System.IO.Path.Combine(DIRECTORYPATH, "Xmls", queryfile));

                doc.Load(System.IO.Path.Combine(directorypath, "Xmls", "ProductQueryInfo.xml"));



                foreach (XmlNode node in doc.DocumentElement.ChildNodes)
                {
                    if (node.NodeType == XmlNodeType.Comment)
                        continue;

                    var idattr = node.Attributes.Cast<XmlAttribute>().FirstOrDefault(p => p.Name.Equals("id", StringComparison.InvariantCultureIgnoreCase)) ?? null;

                    if (idattr == null || string.IsNullOrEmpty(idattr.Value))
                        continue;

                    var id = idattr.Value;


                    var ndurl = node.ChildNodes.Cast<XmlNode>().FirstOrDefault(p => p.LocalName.Equals("Url", StringComparison.InvariantCultureIgnoreCase));

                    if (ndurl == null || string.IsNullOrEmpty(ndurl.InnerText))
                        continue;

                    Uri uriResult;
                    bool _val = Uri.TryCreate(ndurl.InnerText, UriKind.Absolute, out uriResult)
                        && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

                    if (!_val)
                    {
                        logger.ErrorHigh(() => Movilway.Logging.TagValue.New().Tag("ERROR-URL").Message("The url of the product " + id + " is not valid."));
                        continue;
                    }

                    var ndtimeout = node.ChildNodes.Cast<XmlNode>().FirstOrDefault(p => p.LocalName.Equals("Timeout", StringComparison.InvariantCultureIgnoreCase));

                    var _timeout = DEF_TIME_OUT;

                    if (ndtimeout != null)
                        if (!int.TryParse(ndtimeout.InnerText, out _timeout) || _timeout < 0)
                            _timeout = DEF_TIME_OUT;


                    if (!_productsInfo.Keys.Contains(id))
                        _productsInfo.Add(id, new ProductInfo(id, ndurl.InnerText, _timeout));

                }

            }
            catch (Exception ex)
            {
                logger.ExceptionLow(() => Movilway.Logging.TagValue.New().Message("Exception trying to load query payment iformation.").Exception(ex));
            }

            QueryPaymentProvider._productsInfo = _productsInfo;
        }













        protected override Logging.ILogger ProviderLogger
        {
            get
            {
                return logger;
            }

        }

        public override DataContract.Common.IMovilwayApiResponse PerformKinacuOperation(DataContract.Common.IMovilwayApiRequest requestObject, KinacuWebService.SaleInterface kinacuWS, string sessionID)
        {

            logger.InfoHigh("[KIN] " + base.LOG_PREFIX + "INICIO");
            if (!IsThereProducts())
            {
                logger.ErrorHigh("[KIN] " + base.LOG_PREFIX + "[QueryPaymentProvider] NO SE HAN CARGADO PRODUCTOS PARA REALIZAR CONSULTAS");
                return new QueryPaymentResponseBody()
                 {
                     ResponseCode = 99,
                     ResponseMessage = "El producto a comprar no esta configurado para la consulta."

                 };
            }

            var response = new QueryPaymentResponseBody()
            {
                ResponseCode = 99,
                ResponseMessage = "Ocurrio un error, realizando la consulta del pago intente mas tarde."

            };

            try
            {


                QueryPaymentRequestBody request = requestObject as QueryPaymentRequestBody;

                if (request == null)
                    throw new Exception("EL REQUEST A PROCESAR NO PUEDE SER NULO");
                
                var product = MapMNOtoProductId(request.MNO);

                ProductInfo productInfo = null;
                if (!_productsInfo.TryGetValue(product, out productInfo))
                {
                    response.ResponseMessage = "El producto a comprar no esta configurado para la consulta.";

                    return response;
                }
                
                int userid;
                int agencyid;

                string pdv = "";
                // KinacuLogisticsWebService.LogisticsInterface client = new KinacuLogisticsWebService.LogisticsInterface();
                KinacuManagementWebService.ManagementInterface client = new KinacuManagementWebService.ManagementInterface();
                
                int Id; string UserName = null; string UserLastName = null; string UserAddress = null; long SessionTimeOut = 0; string Message = null;

                client.GetUserInfo(int.Parse(sessionID), out Id, out UserName, out UserLastName, out UserAddress, out SessionTimeOut, out Message);

                userid = Id;

                logger.InfoHigh("[KIN] " + base.LOG_PREFIX + "Obteniendo usuario " + userid);

                int _RetailerId; string _RetailerName; string _RetailerAddress; string _RetailerLegalId; int _VoucherQuantityDownload; string _TicketHeader; string _CurrentTime; string _Message = "";

                client.GetRetailerInfo(int.Parse(sessionID), out  _RetailerId, out _RetailerName, out  _RetailerAddress, out _RetailerLegalId, out  _VoucherQuantityDownload, out _TicketHeader, out _CurrentTime, out  _Message);

                agencyid = _RetailerId;


                logger.InfoHigh("[KIN] " + base.LOG_PREFIX + "Obteniendo agencia " + userid);

                //var agentInfo = Utils.GetAgentInfoById(agencyid.ToString());

                GetAgentInfoResponseBody balanceResponse = new ServiceExecutionDelegator<GetAgentInfoResponseBody, GetAgentInfoRequestBody>().ResolveRequest(new GetAgentInfoRequestBody() //GetAgentInfoResponseBody
                {
                    AuthenticationData = request.AuthenticationData ,
                    DeviceType = request.DeviceType,
                    SearchById =true,
                    AgentId = agencyid.ToString()
                }, ApiTargetPlatform.Kinacu, ApiServiceName.GetAgentInfo);


                pdv = balanceResponse.AgentInfo.PDVID;//.PDVID;

                var ApiReference = (System.Threading.Thread.CurrentThread.ManagedThreadId * 10).ToString() + DateTime.Now.Ticks.ToString();




                QueryPaymentReference.Input input = new QueryPaymentReference.Input()
                {
                    Amount = "",
                    Client = request.Recipient,
                    Product = product,
                    DateTime = DateTime.UtcNow.ToString("yyyyMMdd HH:mm:ss"),
                    ReferenceNumber = ApiReference,
                    ExtendedData = new QueryPaymentReference.Input.ArrayOfString()
                    {
                        pdv,
                        request.TerminalID,
                        agency_prefix+agencyid.ToString(),
                        user_prefix+userid,
                        request.ExternalTransactionReference,
                        request.DeviceType.ToString()
                    }
                };


                QueryPaymentReference.queriesClient clientquery = new QueryPaymentReference.queriesClient();



                clientquery.Endpoint.Address = new EndpointAddress(productInfo.Url);

                clientquery.Endpoint.Binding.SendTimeout = TimeSpan.FromMilliseconds(productInfo.Timeout);

                logger.InfoHigh("[KIN] " + base.LOG_PREFIX + " [QueryPaymentProvider] SEND-DATA "+
                                  "Amount ="+  input.Amount+"|"+
                                  "Client ="+  input.Client+"|"+
                                  "Product ="+  input.Product+"|"+
                                  "DateTime ="+  input.DateTime+"|"+
                                  "ReferenceNumber ="+  input.ReferenceNumber+"|"+
                                  "ExtendedData ="+  String.Join("," ,input.ExtendedData));

            
                QueryPaymentReference.OutputQuery queryresponse = clientquery.Query(input);

         

                if (queryresponse != null)
                {

                    logger.InfoHigh("[KIN] " + base.LOG_PREFIX + "[QueryPaymentProvider] REC-DATA " 
                       +"Id = " + queryresponse.Id+"|"
                       +"IdAutorization = " + queryresponse.IdAutorization + "|");

                    response.ResponseCode = queryresponse.Result ? 0 : 99;
                    response.ResponseMessage = queryresponse.Result ? "OK" : "Error en la consulta";
                    response.TransactionID = Convert.ToInt32(queryresponse.Id);
                    response.Fee = queryresponse.Fee;
                    response.ResponseCodeOpetator = queryresponse.ResponseCode;
                    response.DetailsOperator = queryresponse.Details;
                    response.Data = queryresponse.Data;
                    response.DataDescriptor = queryresponse.DataDescriptor;
                    response.Result = queryresponse.Result;
                    response.QueryResultType = queryresponse.QueryResultType.ToString();
                    response.IdAutorization = queryresponse.IdAutorization;
                    response.Amount = queryresponse.Amount;
                }
                else
                {
                    logger.ErrorHigh("[KIN] " + base.LOG_PREFIX + "[QueryPaymentProvider] REC-DATA OutputQuery [null]");

                    return  new QueryPaymentResponseBody()
                    {
                        ResponseCode = 99,
                        ResponseMessage = "No se recibio respuesta, de la consulta intente mas tarde."

                    };
                }
              

            }
            catch (Exception ex)
            {
                logger.ErrorHigh("[KIN] " + base.LOG_PREFIX + "[QueryPaymentProvider] ERROR EN LA CONSULTA DEL PAGO. " + ex.GetType().Name + " " + ex.Message + " " + ex.StackTrace);

                response = new QueryPaymentResponseBody()
                {
                    ResponseCode = 99,

                    ResponseMessage = "Ocurrio un error, realizando la consulta del pago intente mas tarde."

                };
            }
            return response;
        }


        private bool IsThereProducts()
        {
            return _productsInfo != null && _productsInfo.Count > 0;
        }

        private string MapMNOtoProductId(string mno)
        {
            string mappings = System.Configuration.ConfigurationManager.AppSettings["ProductMappings"];

            foreach (string item in mappings.Split(';'))
                if (item.Split(',')[0] == mno.ToLower())
                    return item.Split(',')[1];

            return mno;
        }
        class ProductInfo
        {
            public String Id { get; private set; }

            public String Url { get; private set; }
            public int Timeout { get; private set; }


            public ProductInfo(String _Id, string _Url, int _Timeout)
            {
                Id = _Id;
                Url = _Url;
                Timeout = _Timeout;
            }
        }

    }

    
   
}