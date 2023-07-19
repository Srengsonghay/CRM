using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRM.Models.Dto
{
    public class CaseDto
    {
        public string CaseNo { get; set; }
        public Guid Id { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsSuccess { get; set; }

    }
}