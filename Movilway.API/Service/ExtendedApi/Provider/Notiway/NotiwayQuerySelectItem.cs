using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movilway.API.Service.ExtendedApi.Provider.Notiway
{
    internal class NotiwayQuerySelectItem
    {
        public int MessageId { get; set; }
        public String Title { get; set; }
        public String Abstract { get; set; }
        public int MessageTypeId { get; set; }
        public String Detail { get; set; }
        public String ImageURL { get; set; }
        public int MessageScheduleId { get; set; }
        public DateTime ExpirationDate { get; set; }
        public int FilterAccessTypes { get; set; }
        public int FilterProviders { get; set; }
        public int FilterProducts { get; set; }
        public DateTime FirstDeliveryDate { get; set; }
        public DateTime? LastReadDate { get; set; }
    }
}
