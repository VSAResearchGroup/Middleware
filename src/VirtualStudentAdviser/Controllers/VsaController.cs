using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; 
using Microsoft.AspNetCore.Mvc;
using VirtualStudentAdviser.Services;
using VirtualStudentAdviser.Models;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Cors;


namespace VirtualStudentAdviser.Controllers
{
    //[EnableCors(origins: "*", headers: "*", methods: "*", exposedHeaders: "X-Custom-Header")]    
    [Route("api/Vsa"), EnableCors("VsaCorsPolicy")]
    public class VSAController : Controller
    {
        private readonly IVirtualAdviserRepository _IVSARepostory;

        public VSAController(IVirtualAdviserRepository IVSARepostory)
        {
            _IVSARepostory = IVSARepostory;
        }

        // GET api/values/5
        [HttpGet]
        //   public string Get(int MajorId, int[] CompleteCourses, int SchoolId)
        public JsonResult Get()
        {
            int[] arr = new int[2] { 1, 2 };  //Completed courses  
            var result = _IVSARepostory.launchEngine(1, arr, 16);
            //va result = JsonConvert.SerializeObject(plan); 
            return new JsonResult(result);
        } 
         
        [HttpGet("{id}"), Produces("application/json")]
        public JsonResult Get(int id)
        {
            //Completed courses  
            var result = _IVSARepostory.getStudyPlan(planId: id);

            return new JsonResult(result);
        }
       
        
        // int MajorId, int SchoolId, int[] TargetCourses, int[] CompletedCourses, int JobType, int TimePreference, int[] PrimaryPurpose, int Budget, int Placements
        [HttpPost]
        public JsonResult runRecEngine([FromBody] ParameterSet param)
            {
                if (param == null)
                {
                    return null;
                }
            var result = _IVSARepostory.launchEngine(param.MajorId, new int[1] { 1 }, param.SchoolId);
            return new JsonResult(result);
        }


        //// GET api/values/5
        //[HttpGet("{id}")] 
        //public HttpResponseMessage Get(int id)
        //{
        //    var plan = _IVSARepostory.getStudyPlan(planId: id);
        //    string result = JsonConvert.SerializeObject(plan);
        //    //Completed courses  
        //    var response = new HttpResponseMessage()
        //    {
        //        Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json")
        //    };
        //    response.Headers.Add("X-Custom-Header", "*");
        //    response.Headers.Add("Access-Control-Allow-Headers", "*");
        //    response.Headers.Add("Access-Control-Allow-Origin", "*");
        //    response.Headers.Add("Access-Control-Allow-Methods", "*");
        //    return response; 
        //}
    }
}
