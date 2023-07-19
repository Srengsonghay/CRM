using System;
namespace CRM.Models.Dto
{
    public class CreateCaseDto
    {
        public Guid Id { get; set; }
        public int CaseIssue { get; set; } //new case Type
        public int CaseTypeCode { get; set; }
        public int PriorityCode { get; set; }
        public int newHowManyUser { get; set; }
        public int StatusCode { get; set; }
        public int StateCode { get; set; }

        public Guid ContractId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid PrimaryContactId { get; set; } // ContactId
        public Guid ContractDetailId { get; set; } //productLineId
       

        public string Title { get; set; }
        public string Description { get; set; }
        public string newBranchContactName { get; set; }
        public string newBranchNumber { get; set; }

        //For attch file
        public int AdxPortalComment { get; set; }
        public string FileName { get; set; }
        public string MimeType { get; set; }
        public string FileBody { get; set; }

    }
}