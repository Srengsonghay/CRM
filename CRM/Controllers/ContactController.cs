using CRM.Class;
using CRM.Models;
using CRM.Models.Dto;
using System.Net;
using System.Web.Http;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;
using RouteAttribute = System.Web.Mvc.RouteAttribute;

namespace CRM.Controllers
{
    //api/Contact
    public class ContactController : ApiController
    {
        private static CRM_DAL crmService = new CRM_DAL();
        protected APIResponse _response;
        public ContactController()
        {
            this._response = new APIResponse();
        }
        [Route("/ListContact")]
        public APIResponse Get()
        {
            var result = crmService.ContactList();
            if (result != null)
            {
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = result;
                return _response;
            }
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.IsSuccess = false;
            _response.ErrorMessages.Add("Error");
            return _response;
        }
        [HttpPost]
        [Route("/CreateContact")]
        public APIResponse CreateContact(ContactDto contact)
        {
            var result = crmService.CreateContact(contact);
            if (result != null && result.ErrorMessage is null)
            {
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = result;
                return _response;
            }
            else
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add(result.ErrorMessage);
                return _response;
            }
        }
        [HttpPost]
        [Route("/UpdateContact")]
        public APIResponse UpdateContact(ContactDto contact)
        {
            var result = crmService.UpdateContact(contact);
            if(result != null && result.ErrorMessage is null)
            {
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = new CheckContactDto { ContactId = result.ContactId,};
                return _response;
            }
            else
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add(result.ErrorMessage);
                return _response;
            }
        }
        public APIResponse CreateContactUsNote(ContactUsDto model)
        {
            var result = crmService.ContactUsNote(model);
            if (result)
            {
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = "Success";
                return _response;
            }
            else
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Service down. Please try again!");
                return _response;
            }
        }
    }
}