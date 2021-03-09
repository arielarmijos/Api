using Movilway.API.Service.ExtendedApi.DataContract.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Web;
using Movilway.Logging.Attribute;
using Movilway.API.Core;

namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [MessageContract(IsWrapped= false)]
    public class GetTrustedDevicesResponse : IMovilwayApiResponseWrapper<GetTrustedDevicesResponseBody>
    {
        [MessageBodyMember(Name = "GetTrustedDevicesResponseBody", Namespace = "http://api.movilway.net/schema/extended")]
        public GetTrustedDevicesResponseBody Response
        {
            get;
            set;
        }

        public GetTrustedDevicesResponse()
        {
            Response = new GetTrustedDevicesResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "GetTrustedDevicesResponseBody", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetTrustedDevicesResponseBody : AGenericApiResponse
    {
        [Loggable]
        [DataMember(Order = 1)]
        public TrustedDevices Devices { get; set; }

        public GetTrustedDevicesResponseBody()
        {
            Devices = new TrustedDevices();
        }

        public override string ToString()
        {
            return String.Concat("[",this.GetType().Name,"]", Devices.ToString());
        }
    }

    [Loggable]
    [CollectionDataContract(Namespace="http://api.movilway.net/schema/extended", ItemName="Device")]
    public class TrustedDevices : List<TrustedDevice>
    {


        public TrustedDevices()
            : base()
        {

        }


        public TrustedDevices(IEnumerable<TrustedDevice> list)
            : base(list)
        {
          
        }

        public override string ToString()
        {

            return String.Concat(this.GetType().Name, " Count ", this.Count);

        }

    }

    [Loggable]
    [DataContract(Namespace="http://api.movilway.net/schema/extended")]
    public class TrustedDevice
    {
        [Loggable]
        [DataMember(Order = 1,IsRequired= true)]
        public long ID { get; set; }

        /// <summary>
        /// Usuario asociado al dispositivo
        /// </summary>
        [Loggable]
        [DataMember(Order = 2, IsRequired = true)]
        public String  UserId { get; set; }


        /// <summary>
        /// Serial del dispositivo IMEI 
        /// </summary>
        [Loggable]
        [DataMember(Order = 3, IsRequired = true)]
        public String Token { get; set; }


        //Input to validate, before devcied is create on the system
        public String Hash { get; set; }

        [Loggable]
        [DataMember(Order = 4, IsRequired = true)]
        public String  Type { get; set; }//TypeDevice
    


        public int  IdType { get; set; }//TypeDevice

        //Input to validate, before devcied is create on the system
        [Loggable]
        [DataMember(Order = 5, IsRequired = true)]
        public String FriendlyName { get; set; }

        //[Loggable]
        //[DataMember(Order = 5, IsRequired = true)]
        //public string Model { get; set; }
        //SE CAMBIAN POR NAVIGATOR
        //[Loggable]
        //[DataMember(Order = 6, IsRequired = true)]
        //public string Navigator { get; set; }

        [Loggable]
        [DataMember(Order = 6, IsRequired = true)]
        public string Description { get; set; }

        [Loggable]
        [DataMember(Order = 7, IsRequired = true)]
        public DateTime DateActivated { get; set; }

        //[Loggable]
        //[DataMember(Order = 8, IsRequired = true)]
        //public byte IsActive { get; set; }

        [Loggable]
        [DataMember(Order = 8, IsRequired = true)]
        public Int16 Status { get; set; }

        [Loggable]
        [DataMember(Order = 9, IsRequired = true)]
        public DateTime? BloquedDate { get; set; }

        [Loggable]
        [DataMember(Order = 10, IsRequired = true)]
        public String Model { get; set; }

        [Loggable]
        [DataMember(Order = 11, IsRequired = true)]
        public String OS { get; set; }

        [Loggable]
        [DataMember(Order = 12, IsRequired = true)]
        public DateTime? LastAccess { get; set; }

        [DataMember(Order = 13, IsRequired = true)]
        public Boolean Secure { get; set; }

        [Loggable]
        [DataMember(Order = 14, IsRequired = true)]
        public DateTime DateCreated { get; set; }

        [Loggable]
        [DataMember(Order = 15, IsRequired = true)]
        public long Ticks { get; set; }


        public TrustedDevice()
        {
            Token = string.Empty;
            Description = string.Empty;
        }

        public override bool Equals(object obj)
        {
            bool result = false;
            TrustedDevice aux = null;
            if (obj != null  && ((aux  = obj as TrustedDevice) != null))
            {
                result = aux.ID == ID;
            }

            return result;
        }

        public override int GetHashCode()
        {
            int vaux = 23;

            return vaux + ID.GetHashCode();
        }




        internal bool IsValid()
        {
            return !string.IsNullOrEmpty(Token) &&
                IdType != 0 &&
               !string.IsNullOrEmpty(Description);
        }

        public String DeviceDescription()
        {
            String result = "DISPOSITIVO";

            switch (IdType)
            {
                case cons.DEVICE_TEMPORAL:

                    result = String.Concat(result," TEMPORAL");
                    break;


            }

            return result;
        }
    }
}