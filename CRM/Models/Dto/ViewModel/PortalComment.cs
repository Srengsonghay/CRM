using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRM.Models.Dto.ViewModel
{
    public class PortalComment
    {
        public Guid? CommentId { get; set; }
        public Guid? AttachmentId { get; set; }
    }
}