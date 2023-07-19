using System;

namespace CRM.Models.Dto
{
    public class CheckContactDto
    {
        public Guid ContactId { get; set; }
        //public ContactDto Contact { get; set; }
        public string ErrorMessage { get; set; }
    }
}