using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRM.Models.Dto
{
    public class ContactDto
    {

        public Guid ContactId { get; set; }
        public string Salutation { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Gender { get; set; }
        public Guid ParentCustomerId { get; set; }
        public string JobTitle { get; set; }
        public string Email { get; set; }
        public DateTime? Birthday { get; set; }
        public string Department { get; set; }
        public string BusinessPhone { get; set; }
        public string MobilePhone { get; set; }
        public string Address { get; set; }
        public Guid ReportToId { get; set; }
        public bool IsPortalUser { get; set; }

    }
}