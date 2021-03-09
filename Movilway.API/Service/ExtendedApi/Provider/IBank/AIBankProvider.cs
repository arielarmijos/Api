using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using Movilway.API.IBank;
using Movilway.API.Config;

namespace Movilway.API.Service.ExtendedApi.Provider.IBank
{
    public abstract class AIBankProvider
    {
        private static readonly Dictionary<String, IBankServiceClient> EntityToProviderMap;
        private static readonly Dictionary<String, String> EntityToAgentMap;
        
        static AIBankProvider()
        {
            EntityToProviderMap = new Dictionary<string,IBankServiceClient>();
            EntityToAgentMap = new Dictionary<string, string>();

            var apiConfig = ConfigurationManager.GetSection("Movilway.API.Config") as ApiConfiguration;
            if (apiConfig.IBankEntities != null && apiConfig.IBankEntities.Count > 0)
            {
                for(int i=0; i<apiConfig.IBankEntities.Count; i++)
                {
                    var entity = apiConfig.IBankEntities[i];
                    EntityToProviderMap.Add(entity.Name, new IBankServiceClient("BasicHttpBinding_IIBankService", entity.URL));
                    EntityToAgentMap.Add(entity.Name, entity.Agent);
                }
            }
        }

        protected IBankServiceClient GetProviderForEntity(String entityName)
        {
            if (EntityToProviderMap.ContainsKey(entityName))
                return EntityToProviderMap[entityName];
            else
                throw new ArgumentException(String.Format("No existe configuracion disponible para la entidad '{0}', por favor verifique", entityName), "entityName");
        }

        protected String GetAgentForEntity(String entityName)
        {
            if (EntityToAgentMap.ContainsKey(entityName))
                return EntityToAgentMap[entityName];
            else
                throw new ArgumentException(String.Format("No existe configuracion disponible para la entidad '{0}', por favor verifique", entityName), "entityName");
        }
    }
}