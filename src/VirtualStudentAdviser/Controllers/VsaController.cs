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
using System.Data.SqlClient;



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

        /// <summary>
        /// Gets plan with planId id
        /// </summary>
        /// <para name ="id" ></para>
        /// <remarks>
        ///   [
        ///          {
        ///              planId: 70,
        ///              courseNumber: "PHYS& 243",
        ///                quarter: "Fall",
        ///                year: 2016
        ///      
        ///             }
        ///    ]
        /// </remarks>
        ///
        ///
        [HttpGet("{id}"), Produces("application/json")]
        public JsonResult Get(int id)
        {
            //Completed courses  
            var result = _IVSARepostory.getStudyPlan(planId: id);

            return new JsonResult(result);
        }
       
        
        /// <summary>
        ///  Runs the rec engine using ParameterSet param that is written to the body of the request
        /// </summary>
        /// <param name="param">
        /// Input ParameterSet for the algorithm
        /// </param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns a list of course objects
        /// </summary>
        [HttpGet("getCourses")]
        public JsonResult getCourses()
        {
            return new JsonResult(_IVSARepostory.getAllCourses());
            
        }

        /// <summary>
        /// Saves study plan to database. 
        /// </summary>
        /// <remarks>
        /// studyPlan is different than the plan stored with id oldPlanId but has the same major and school
        /// </remarks>
        /// <para name ="studyPlan" > List of SelectSudyPlan. These are the studyplans with the string values of the StudyPlan ids</para>
        /// <para name ="studentId" >Id to the student the plan belongs to</para>
        /// <para name ="oldPlanId" >The plan Id for the plan before the plan was manipulated by the user</para>
        /// <para name ="planName" >Name of plan given by student</para>
        /// <returns>
        /// returns the new planId for the saved plan
        /// </returns>
        [HttpPost("savePlan/{studentId}/{oldPlanId}/{planName}")]
        public int savePlan([FromBody] List<SelectStudyPlan> studyPlan,int studentId, int oldPlanId, string planName)
        {

          List<StudyPlan> newStudyPlan = _IVSARepostory.convertStudyPlan(studyPlan);
            var msPair = _IVSARepostory.getMajorSchoolPairs(oldPlanId);
          int planId = _IVSARepostory.insertNewStudyPlan(newStudyPlan,  studentId, msPair[0], msPair[1],  planName);

            //  int newId = _IVSARepostory.insertStudyPlan(newStudyPlan);

            return planId;
        }


        /// <summary>
        /// Returns all the inactive plans associated with studentId.
        /// </summary>
        /// <remarks>Returns all of the plans except the first since there is not a active field in the StudentStudyPlan table.
        ///  Needs to be changed to return the plans with an active value of 0 when active exists </remarks>
        /// <para name ="studentId" >Id to the student the plan belongs to</para>
        
        [HttpGet("getInactivePlanInfo/{studentId}")]
        public JsonResult getInactivePlanInfo(int studentId)
        {
           return new JsonResult(_IVSARepostory.GetInactiveParameterSets( studentId));

        }

        /// <summary>
        /// Returns the active plan associated with studentId.
        /// </summary>
        /// <remarks>Returns the first plan since there is not an active field in the StudentStudyPlan table.
        ///  Needs to be changed to return the plan with an active value of 1 when active exists
        /// 
        /// {
        ///   parameterSet": {
        ///         id: 0,
        ///         majorId: 0,
        ///         major: "Mechanical Engineering",
        ///         schoolId: 0,
        ///         school: "\nUniversity of Washington",
        ///         jobTypeId: 0,
        ///         jobType: "Full Time",
        ///         budgetId: 0,
        ///         budget: "$100.00 - $599.99",
        ///         timePreferenceId: 0,
        ///         timePreference: "Morning",
        ///         quarterPreferenceId: 0,
        ///         quarterPreference: "Fall      ",
        ///         completedCourses: "[1,2]",
        ///         placementCourses: "Default placement",
        ///         dateAdded: "0001-01-01T00:00:00",
        ///         lastDateModified: "0001-01-01T00:00:00",
        ///         status: 0
        ///     },
        /// planName": null,
        /// studentId": 456,
        /// planId": 2166
        ///}
    /// 
    /// 
    ///  </remarks>
    /// <para name ="studentId" >Id to the student the plan belongs to</para>
    [HttpGet("getActivePlanInfo/{studentId}")]
        public JsonResult getActivePlanInfo(int studentId)
        {
          PlanInfo getParamerterSet = _IVSARepostory.GetActiveParameterSet(studentId);
            if (getParamerterSet == null)
            {
                return new JsonResult(new PlanInfo());
            }
            return new JsonResult(getParamerterSet);
        }

       // public HttpResponseMessage saveRating([FromBody] )
    }
}
