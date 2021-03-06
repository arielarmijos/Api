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
    
    public partial class Branch
    {
        public Branch()
        {
            this.Branch1 = new HashSet<Branch>();
            this.Messages = new HashSet<Message>();
            this.MessageTickets = new HashSet<MessageTicket>();
            this.Users = new HashSet<User>();
            this.AuditMessages = new HashSet<AuditMessage>();
            this.AuditMessages1 = new HashSet<AuditMessage>();
            this.AuditMessageTickets = new HashSet<AuditMessageTicket>();
            this.AuditMessageTickets1 = new HashSet<AuditMessageTicket>();
        }
    
        public int BranchId { get; set; }
        public int PlatformId { get; set; }
        public int CountryId { get; set; }
        public int ProvinceId { get; set; }
        public int CityId { get; set; }
        public Nullable<int> ParentBranchId { get; set; }
        public int UserId { get; set; }
        public string StatusId { get; set; }
        public string BranchName { get; set; }
        public string BranchReference { get; set; }
        public string Address { get; set; }
        public string LegalName { get; set; }
        public string LegalNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string CellPhone { get; set; }
        public string ContactName { get; set; }
        public string Observations { get; set; }
        public int SubLevels { get; set; }
        public string BranchType { get; set; }
        public System.DateTime DateSignOn { get; set; }
        public Nullable<System.DateTime> DateSignOff { get; set; }
        public string Pdv { get; set; }
        public string Imei { get; set; }
        public string Streets { get; set; }
        public decimal Stock { get; set; }
        public string Lineage { get; set; }
        public Nullable<System.DateTime> DateFirstPurchase { get; set; }
        public Nullable<System.DateTime> DateLastPurchase { get; set; }
        public Nullable<decimal> LastPurchaseAmount { get; set; }
        public Nullable<decimal> LastPurchaseDollarAmount { get; set; }
        public Nullable<int> LastPurchaseUserId { get; set; }
        public string LastPurchaseUserName { get; set; }
        public Nullable<System.DateTime> DateFirstTransaction { get; set; }
        public Nullable<System.DateTime> DateLastTransaction { get; set; }
        public Nullable<int> LastTransactionUserId { get; set; }
        public Nullable<int> LastTransactionAccessTypeId { get; set; }
    
        public virtual ICollection<Branch> Branch1 { get; set; }
        public virtual Branch Branch2 { get; set; }
        public virtual City City { get; set; }
        public virtual Platform Platform { get; set; }
        public virtual Status Status { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<MessageTicket> MessageTickets { get; set; }
        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<AuditMessage> AuditMessages { get; set; }
        public virtual ICollection<AuditMessage> AuditMessages1 { get; set; }
        public virtual ICollection<AuditMessageTicket> AuditMessageTickets { get; set; }
        public virtual ICollection<AuditMessageTicket> AuditMessageTickets1 { get; set; }
    }
}
