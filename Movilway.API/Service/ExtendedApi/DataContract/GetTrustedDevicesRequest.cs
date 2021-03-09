

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Web;

using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging.Attribute;

namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [MessageContract(IsWrapped=false)]
    public class GetTrustedDevicesRequest : IMovilwayApiRequestWrapper<GetTrustedDevicesRequestBody>
    {
        [MessageBodyMember(Name = "GetTrustedDevicesRequestBody", Namespace = "http://api.movilway.net/schema/extended")]
        public GetTrustedDevicesRequestBody Request
        {
            get;
            set;
        }
    }

    [DataContract(Name = "GetTrustedDevicesRequestBody", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetTrustedDevicesRequestBody : ASecuredApiRequest                                    
    {
        
       
    }


    //atributos por filtro
    //filtro por fecha 
    //por tipo 
    //navegador
    //marca
    [DataContract(Namespace="http://api.movilway.net/schema/extended")]
    public class FilterTrustedDevices{


        [Loggable]
        [DataMember(Order = 1, IsRequired=true)]
        public long ID { get; set; }

        [Loggable]
        [DataMember(Order=2, IsRequired=true)]
        public string Type { get; set; }//Device

        //[Loggable]
        //[DataMember(Order=3, IsRequired=true)]
        //public string Model { get; set; }

        //[Loggable]
        //[DataMember(Order=4, IsRequired=true)]
        //public string Navigator { get; set; }

        [Loggable]
        [DataMember(Order = 4, IsRequired = true)]
        public string Description { get; set; }//Device
        
        [Loggable]
        [DataMember(Order=5, IsRequired=true)]
        public DateTime RegisterDateIni { get; set; }

        [Loggable]
        [DataMember(Order = 6, IsRequired=true)]
        public DateTime RegisterDateFin { get; set; }

        ////incluir atributos de paginacion
        //public int From { get; set; }
     
        //public int PageSize { get; set; }
    }

}