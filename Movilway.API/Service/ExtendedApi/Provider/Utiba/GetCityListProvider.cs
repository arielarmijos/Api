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
    [ServiceProviderImpl(Platform = ApiTargetPlatform.UtibaRegistration, ServiceName = ApiServiceName.GetCityList)]
    public class GetCityListProvider : AUtibaProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GetCityListProvider));
        protected override ILogger ProviderLogger { get { return logger; } }

        public override IMovilwayApiResponse PerformUtibaOperation(IMovilwayApiRequest requestObject, UMarketSCClient utibaClientProxy, String sessionID)
        {
            GetCityListRequestBody request = requestObject as GetCityListRequestBody;
            GetCityListResponseBody response = null;

            UtibaRegistrationDataContext utibaRegistration = new UtibaRegistrationDataContext();
            List<City> cities = utibaRegistration.Cities.Where(c => c.CountryId == _countryID && c.ProvinceId == request.ProvinceID).ToList();
            if (cities != null && cities.Count > 0)
            {
                response = new GetCityListResponseBody()
                {
                    ResponseCode = 0,
                    CityList = new CityList()
                };
                foreach (City city in cities)
                {
                    response.CityList.Add(city.CityId, city.CityName);
                }
            }
            else
            {
                response = new GetCityListResponseBody()
                    {
                        ResponseCode = 1,
                        ResponseMessage="No se encuentran registradas provincias para el pais donde se encuentra"
                };
            }
            return (response);
        }

        private static int _countryID;

        static GetCityListProvider()
        {
            _countryID = int.Parse(ConfigurationManager.AppSettings["CountryID"]);
        }
    }
}