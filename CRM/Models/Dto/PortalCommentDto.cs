using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.UI.WebControls;

namespace CRM.Models.Dto
{
    public class PortalCommentDto
    {
        public Guid Id { get; set; }
        public Guid CaseId { get; set; }
        public Guid ContactId { get; set; }
        public Guid UserId { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public int PortalCommentDirectionCode { get; set; }
        public int AdxPortalComment { get; set; }
        public string FileName { get; set; }
        public string MimeType { get; set; }
        public string FileBody { get; set; }
    }
}