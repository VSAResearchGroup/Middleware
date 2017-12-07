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
    [Route("api/test")]
    public class TestController : Controller
    {

        private readonly IVirtualAdviserRepository _IVSARepostory;

        public TestController(IVirtualAdviserRepository IVSARepostory)
        {
            _IVSARepostory = IVSARepostory;
        }


        /// <summary>
        /// Tests the plan with planId
        /// </summary>
        /// <para name ="planId" ></para>
        [HttpGet("testPlan/{planId}")]
        public JsonResult testPlan(int planId)
        {
            List<StudyPlan> sp = _IVSARepostory.getStudyPlans(planId).ToList();
            // get all plan Ids

            var majorSchoolpair = _IVSARepostory.getMajorSchoolPairs(planId);
            var requiredCourses = _IVSARepostory.getTargetCourses(majorSchoolpair[0], majorSchoolpair[1]);
            List<Course> courses = _IVSARepostory.getAllCourses();
            PlanVerificationInfo testResult = _IVSARepostory.testPlan(sp, majorSchoolpair[0], majorSchoolpair[1], requiredCourses, courses);
            return new JsonResult(testResult);
        }

        /// <summary>
        /// Tests all plans in the DB.
        /// </summary>
         
        [HttpGet("testAllPlans")]
        public JsonResult testAllPlans()
        {
            var planIds = _IVSARepostory.getPlanIds();
            
            int count = 1;

            var courses = _IVSARepostory.getAllCourses();

           
            
            Dictionary<int, PlanVerificationInfo> finalResult = new Dictionary<int, PlanVerificationInfo>();

            foreach (var v in planIds)
            {

                List<StudyPlan> sp = _IVSARepostory.getStudyPlans(v).ToList();

                //majorSchoolPairs[n][0] contains planId
                var majorSchoolpair = _IVSARepostory.getMajorSchoolPairs(v);
                var requiredCourses = _IVSARepostory.getTargetCourses(majorSchoolpair[0], majorSchoolpair[1]);

                // majorSchoolpair[1] is major majorSchoolpair[2] is school
                PlanVerificationInfo currResult = _IVSARepostory.testPlan(sp, majorSchoolpair[0], majorSchoolpair[1], requiredCourses, courses);

                finalResult.Add(v, currResult);
                Console.WriteLine(count + "/" + planIds.Length);
                count++;
            }
            return new JsonResult(finalResult);
      }

        

    }
}


