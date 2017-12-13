using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VirtualStudentAdviser.Models;
using VirtualStudentAdviser.Services;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860


namespace VirtualStudentAdviser.Controllers
{
    /// <summary>
    /// Exposes test validation module through calls to testPlan and testAllPlans
    /// </summary>
    [Route("api/test")]
    public class TestController : Controller
    {

        private readonly IVirtualAdviserRepository _IVSARepostory;
        /// <summary>
        /// Create a new TestController.
        /// </summary>
        /// <param name="IVSARepostory">
        /// IVirtualAdviserRepository that has methods to access database
        /// </param>
        /// <remarks>
        /// This is called by the ASP.NET framework and will only be called manually in testing
        /// </remarks>
        public TestController(IVirtualAdviserRepository IVSARepostory)
        {
            _IVSARepostory = IVSARepostory;
        }


        /// <summary>
        /// Tests the plan with planId
        /// </summary>
        /// <para name ="planId" >
        /// The planId of plan to test
        /// </para>
        /// <returns>
        /// Returns json of PlanVerificationInfo object
        /// </returns>
        [HttpGet("testPlan/{planId}")]
        public JsonResult testPlan(int planId)
        {
            // get all study plans with planId planId
            List<StudyPlan> sp = _IVSARepostory.getStudyPlans(planId).ToList();

            // majorSchoolpair[0] is majorid
            // majorSchoolpair[1] is schoolid
            var majorSchoolpair = _IVSARepostory.getMajorSchoolPairs(planId);

            // get all required courses for major and school pair
            var requiredCourses = _IVSARepostory.getTargetCourses(majorSchoolpair[0], majorSchoolpair[1]);

            // get all the scheduled courses in db
            List<Course> courses = _IVSARepostory.getAllCourses();

            // get plan verification results
            PlanVerificationInfo testResult = _IVSARepostory.testPlan(sp, majorSchoolpair[0], majorSchoolpair[1], requiredCourses, courses);
            return new JsonResult(testResult);
        }

        /// <summary>
        /// Tests all plans in the DB.
        /// </summary>
        /// <remarks>
        /// Need to store test result. No need to retest plans more than once and takes around 20 minutes to do the current set.
        /// </remarks>
        /// <returns>
        /// json object of mapping of planId to PlanVerificationInfo object
        /// </returns>
        [HttpGet("testAllPlans")]
        public JsonResult testAllPlans()
        {
            var planIds = _IVSARepostory.getPlanIds();
            
            // counter for progress of method
            int count = 1;

            var courses = _IVSARepostory.getAllCourses();

            // holds mapping of planId to plan verification results
            Dictionary<int, PlanVerificationInfo> finalResult = new Dictionary<int, PlanVerificationInfo>();
        
            foreach (var v in planIds)
            {

                List<StudyPlan> sp = _IVSARepostory.getStudyPlans(v).ToList();

                // majorSchoolpair[1] is major majorSchoolpair[2] is school
                var majorSchoolpair = _IVSARepostory.getMajorSchoolPairs(v);
                var requiredCourses = _IVSARepostory.getTargetCourses(majorSchoolpair[0], majorSchoolpair[1]);



                PlanVerificationInfo currResult = _IVSARepostory.testPlan(sp, majorSchoolpair[0], majorSchoolpair[1], requiredCourses, courses);

                // add current plan result to final results
                finalResult.Add(v, currResult);

                // display method progress
                Console.WriteLine(count + "/" + planIds.Length);
                count++;
            }
            return new JsonResult(finalResult);
      }   
    }
}


