using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Movilway.API.Utiba;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.API.Data;
using System.Configuration;
using Movilway.Logging;

namespace Movilway.API.Service.ExtendedApi.Provider.Utiba
{
    [ServiceProviderImpl(Platform = ApiTargetPlatform.UtibaRegistration, ServiceName = ApiServiceName.GetProvinceList)]
    public class GetProvinceListProvider : AUtibaProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GetProvinceListProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformUtibaOperation(IMovilwayApiRequest requestObject, UMarketSCClient utibaClientProxy, String sessionID)
        {
            GetProvinceListRequestBody request = requestObject as GetProvinceListRequestBody;
            GetProvinceListResponseBody response = null;

            UtibaRegistrationDataContext utibaRegistration = new UtibaRegistrationDataContext();
            List<Province> provinces = utibaRegistration.Provinces.Where(p => p.CountryId == _countryID).ToList();
            if (provinces != null && provinces.Count > 0)
            {
                response = new GetProvinceListResponseBody()
                {
                    ResponseCode = 0,
                    ProvinceList = new ProvinceList()
                };
                foreach (Province province in provinces)
                {
                    response.ProvinceList.Add(province.ProvinceId, province.ProvinceName);
                }
            }
            else
            {
                response = new GetProvinceListResponseBody()
                {
                    ResponseCode = 1,
                    ResponseMessage = "No se encuentran registradas provincias para el pais donde se encuentra"
                };
            }
            return (response);
        }

        private static int _countryID;

        static GetProvinceListProvider()
        {
            _countryID = int.Parse(ConfigurationManager.AppSettings["CountryID"]);
        }
    }
}