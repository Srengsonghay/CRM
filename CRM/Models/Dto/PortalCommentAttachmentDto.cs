using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRM.Models.Dto
{
    public class PortalCommentAttachmentDto
    {
        //CommentId
        public Guid Id { get; set; }
        public int AdxPortalComment { get; set; }
        public string FileName { get; set; }
        public string MimeType { get; set; }
        public string FileBody { get; set; }
    }
}