using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Web;

namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [MessageContract(IsWrapped = false)]
    public class GetBlockedAgenciesByInactivityReportResponse : IMovilwayApiResponseWrapper<GetBlockedAgenciesByInactivityResponseBody>
    {
        [MessageBodyMember(Name = "GetBlockedAgenciesByInactivityResponse")]
        public GetBlockedAgenciesByInactivityResponseBody Response { set; get; }
    }


    [Loggable]
    [DataContract(Name = "GetBlockedAgenciesByInactivityResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetBlockedAgenciesByInactivityResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AGenericApiResponse
    {

        [Loggable]
        [DataMember(Order =3)]
        public BlockedAgenciesDetails Details { get; set; }

        public GetBlockedAgenciesByInactivityResponseBody() {
            Details = new BlockedAgenciesDetails();
        }

    }


    [Loggable]
    [CollectionDataContract(Namespace = "http://api.movilway.net/schema/extended", ItemName = "ItemReport")]
    public class BlockedAgenciesDetails:List<BlockedAgencyData> {


        public BlockedAgenciesDetails()
         : base()
        {

        }

        public BlockedAgenciesDetails(IEnumerable<BlockedAgencyData> collection)
          : base(collection)
        {

        }
    }

    [Loggable]
    [DataContract(Namespace = "http://api.movilway.net/schema/extended")]
    public class BlockedAgencyData
    {
        [DataMember(Order = 1, EmitDefaultValue = false)]
        public string AgentCode { get; set; }

        [DataMember(Order = 2, EmitDefaultValue = false)]
        public string   AgentName { get; set; }

        [DataMember(Order = 3, EmitDefaultValue = false)]
        public DateTime?  LastSaleDate { get; set; }

        [DataMember(Order = 4, EmitDefaultValue = false)]
        public DateTime? LastDistributionDate { get; set; }

        [DataMember(Order = 5, EmitDefaultValue = false)]
        public DateTime? BlockedDate { get; set; }


        [DataMember(Order = 6, EmitDefaultValue = false)]
        public string   TaxCategoryValue { get; set; }

        [DataMember(Order = 7, EmitDefaultValue = false)]
        public string   AgencyLevel { get; set; }

        [DataMember(Order = 8, EmitDefaultValue = false)]
        public decimal   CurrentStock { get; set; }

        [DataMember(Order = 9, EmitDefaultValue = false)]
        public string   Note { get; set; }

        [DataMember(Order = 9, EmitDefaultValue = false)]
        public decimal  BeforeBlockedStock{ get; set; }
    }

}