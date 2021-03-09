//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Movilway.API.Data.Notiway
{
    using System;
    using System.Collections.Generic;
    
    public partial class MessageSchedule
    {
        public MessageSchedule()
        {
            this.AuditMessageScheduleEntries = new HashSet<AuditMessageScheduleEntry>();
        }
    
        public int MessageScheduleId { get; set; }
        public int MessageId { get; set; }
        public string Frecuency { get; set; }
        public string Days { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public System.TimeSpan StartTime { get; set; }
        public System.TimeSpan EndTime { get; set; }
        public System.DateTimeOffset CreationDateTime { get; set; }
        public int CreationACLUserId { get; set; }
        public string CreationACLUserLogin { get; set; }
        public string CreationHostAddress { get; set; }
        public string CreationHostName { get; set; }
        public string StatusId { get; set; }
    
        public virtual ICollection<AuditMessageScheduleEntry> AuditMessageScheduleEntries { get; set; }
        public virtual Message Message { get; set; }
        public virtual Status Status { get; set; }
    }
}
