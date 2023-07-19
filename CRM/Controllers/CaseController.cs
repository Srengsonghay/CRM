using CRM.Class;
using CRM.Models;
using CRM.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using System.Web.Mvc;
using FromBodyAttribute = System.Web.Http.FromBodyAttribute;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;
using HttpPutAttribute = System.Web.Http.HttpPutAttribute;
using RouteAttribute = System.Web.Mvc.RouteAttribute;

namespace CRM.Controllers
{
    [Route("api/Case")]
    public class CaseController : ApiController
    {
        private static CRM_DAL crmService = new CRM_DAL();
        protected APIResponse _response;
        //private IWebHostEnvironment _host;
        public CaseController()
        {
            this._response = new APIResponse();
        }
        [Route("GetCase")]
        public APIResponse Get(Guid id)
        {
            try
            {
                var result = crmService.getCaseByID(id);
                if (result != null)
                {
                    _response.StatusCode = HttpStatusCode.OK;
                    _response.IsSuccess = true;
                    _response.Result = result;
                    return _response;
                }
                else{
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("Not Found");
                    return _response;
                }
            }
            catch(Exception ex)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
                return _response;
            }
            //return null;
        }

        [HttpPost]
        [Route("/CreateCase")]
        public APIResponse CreateCase([FromBody] CreateCaseDto newCase)
        {
            var result = crmService.CreateCase(newCase);
            if (result != null && !result.Equals(""))
            {
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = new CaseDto
                {
                    Id = Guid.Parse(result),
                };
                return _response;
            }
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.IsSuccess = false;
            _response.ErrorMessages.Add("Service Down. Please Try again!");
            return _response;
        }
        [HttpPut]
        [Route("/UpdateCase")]
        public APIResponse UpdateCase(Guid Id,[FromBody] CreateCaseDto Case)
        {
            var result = crmService.UpdateCase(Case);
            if(result != null && !result.Equals(""))
            {
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = new CaseDto { CaseNo = result,};
                return _response;
            }
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.IsSuccess = false;
            _response.ErrorMessages.Add("Update failed");
            return _response;
        }
        [HttpPost]
        [Route("/AddComment")]
        public APIResponse AddComment([FromBody] PortalCommentDto portalComment)
        {
            var result = crmService.CreatePortalComment(portalComment);
            if (result is null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Failed to drop your comment!");
                return _response;
            }
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = result;
            return _response;
        }
        [HttpPost]
        [Route("/CreateCommentAttachment")]
        public APIResponse CreateCommentAttachment([FromBody] PortalCommentAttachmentDto attachment)
        {
            var result = crmService.PortalCommentAttachment(attachment);
            if (result is null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Upload Failed");
                return _response;
            }
            _response.StatusCode = HttpStatusCode.NoContent;
            _response.IsSuccess = true;
            _response.Result = "Uploaded Successfully";
            return _response;

        }
        [HttpPost]
        [Route("/CancelCase")]
        public APIResponse CancelCase(Guid Id, [FromBody] CreateCaseDto Case)
        {
            var result = crmService.CancelCase(Case);
            if (result.IsSuccess)
            {
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = result;
                return _response;
            }
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.IsSuccess = false;
            _response.ErrorMessages.Add(result.ErrorMessage);
            return _response;
        }



    }
}