using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using VirtualStudentAdviser.Services;
using VirtualStudentAdviser.Models;
using Newtonsoft.Json;
using System.Net.Http;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace VirtualStudentAdviser.Controllers
{
    [Route("api/RecEngine")] 
    public class RecEngineController : Controller
    {


        private readonly IVirtualAdviserRepository _IVSARepostory;

        public RecEngineController(IVirtualAdviserRepository IVSARepostory)
        {
            _IVSARepostory = IVSARepostory;
        }
       

        /// <summary>
        /// Launches the rec engine using the major majorId and school schoolId
        /// </summary>
        /// <param name="majorId"></param>
        /// <param name="schoolId"></param>
        /// <returns></returns>
        [HttpGet("LaunchEngine/{majorId}/{schoolId}"), Produces("application/json")]
        public JsonResult LaunchEngine(int majorId, int schoolId)
        {
            int[] arr = new int[2] { 1, 2 };  //Completed courses  
            var result = _IVSARepostory.launchEngine(majorId, arr, schoolId);
            return new JsonResult(result);
        }


        // GET api/values/5
        [HttpGet("{id}"), DisableCors()]
        public JsonResult Get(int id)
        {
           var result = _IVSARepostory.getStudyPlan(planId: id);
           // string result = JsonConvert.SerializeObject(plan);          
            return new JsonResult(result);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
