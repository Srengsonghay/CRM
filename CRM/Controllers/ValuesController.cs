using CRM.Class;
using CRM.Models;
using CRM.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.WebPages;

namespace CRM.Controllers
{
    public class ValuesController : ApiController
    {
        private static CRM_DAL crmService = new CRM_DAL();
        protected APIResponse _response;
        public ValuesController()
        {
            this._response = new APIResponse();
        }
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public APIResponse Post([FromBody] CreateCaseDto newCase)
        {
            return _response;
        }

        // PUT api/values/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
