using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Routing;
using VirtualStudentAdviser.Controllers;
using VirtualStudentAdviser.Models;
using VirtualStudentAdviser.Services;

namespace VirtualStudentAdviser
{


    /// <summary>
    /// Takes academic plans from database or as List of StudyPlans and enumerates the errors
    /// </summary>
    public static class PlanVerification
    {

        /// <summary>
        /// Takes study plan and enumerates errors
        /// </summary>
        /// <param name="studyPlan"> List of StudyPlan that all share the same planId </param>
        /// <param name="prereqs"> mapping of course and prerequisite groups</param>
        /// for example:
        ///  course group prereq
        ///  199	1	  191
        ///  199	1	  264
        ///  199	2	  191
        ///	 199	2	  1246
        ///	 199	2	  268  
        /// 

        ///  <param name="requiredCourses"> All required course for plan's major and school</param>
        ///  <param name="courses"> List of all courses availabvle at EvCC in db</param>
        ///  <param name="completedCourses"> the starting point of student. The classes the student has complete at time of generation</param>
        /// <param name="vac">copy of the database context.</param>

        /// <remarks>
        /// The VirtualAdviserContex is passed in to have access to course scheduling but that table could be added as a paramater as a List of CourseTime. 
        /// 
        /// </remarks>
        public static PlanVerificationInfo runTests(List<StudyPlan> studyPlan, Dictionary<int, int[][]> prereqs, int[] requiredCourses, List<Course> courses,int[] completedCourses, VirtualAdviserContext vac)
        {

        

        // planVerificationInfo is a mapping of planId to different lists of strings
        PlanVerificationInfo result = new PlanVerificationInfo();
            int planId = studyPlan[0].PlanId;

            result.planId = planId;

            // get all degree core courses that are unscheduled 
            List<int> degreeFulfilledCourses = Output_DegreeFulfilled(studyPlan, requiredCourses.ToList());

            // get all prereq courses that are unscheduled 
            Dictionary<int, List<int>> prereqsCorrect = Output_CorrectOrderOfPreReq(studyPlan, prereqs,completedCourses);

            // get all courses that are scheduled in quarters they are not offered 
            List<int> incorrectScheduling = Ouput_CoursesScheduledCorrectily(studyPlan, vac);


            // append each of the results of the validation methods to result
            foreach (var course in degreeFulfilledCourses)
            {
                result.incorrectScheduling.Add("Core Course " + getCourseNumberFromId(course, courses) + " is not scheduled");
            }

            foreach (var key in prereqsCorrect.Keys)
            {
                foreach (var value in prereqsCorrect[key])
                {
                    result.unfulfilledPrereqs.Add(getCourseNumberFromId(key, courses) + " is missing prereq " + getCourseNumberFromId(value, courses));

                }
            }

            foreach (var course in incorrectScheduling)
            {
                result.incorrectScheduling.Add("Course " + getCourseNumberFromId(course, courses) + " is scheduled in a quarter it is not offered");
            }
            return result;
        }


        /// <summary>
        /// checks that all scheduled courses are in quarters that offer that course
        /// </summary>
        /// <param name="studyPlan">The list of StudyPlans represetingh the plan to test</param>
        /// <param name="vac">Database Context to retrieve CourseTime info</param>
 
        /// <returns>list of all courseIds that are in quarters they are not offered</returns>
        private static List<int> Ouput_CoursesScheduledCorrectily(List<StudyPlan> studyPlan, VirtualAdviserContext vac)
        {

            List<int> result = new List<int>();
            foreach (var course in studyPlan)
            {
                // a course is incorrectily scheudled when there are no rows in CourseTime equal to the current course's courseId
                var value = vac.CourseTime.Where(c => c.CourseId == course.CourseId);
                if (!value.Any(c => c.QuarterId == course.QuarterId))
                {
                    result.Add(course.CourseId);
                }

            }
            return result;
        }

        // id for the first quarter, Autumn
        static int startQtr = 1;

        // id for the last quarter, Summer
        static int endQtr = 4;


        /// <summary>
        /// Gets the string of the course with courseId equal to param courseId
        /// </summary>
        /// <param name="courseId">The courseId of the course to find the title</param>
        /// <param name="courses">List of all courses</param>
        /// <returns>string of course representing courseId</returns>
        private static string getCourseNumberFromId(int courseId, List<Course> courses)
        {
            var course = courses.Where(c => c.CourseId == courseId).Select(m => m.CourseNumber).First();

            return course.Trim();
        }

        /// <summary>
        /// Uses param qtr to return the next quarters id.
        /// </summary>
        /// <param name="qtr"> The current quarterId</param>
        /// <returns>The next quarterId</returns>
        private static int getNextQuarter(int qtr)
        {

            //qtr is out of valid range
            if (qtr < startQtr || qtr > endQtr)
            {
                return -1;
            }

            if (qtr == endQtr)
            {
                return startQtr;
            }

            if (qtr >= startQtr)
            {
                return qtr + 1;
            }

            return qtr;
        }

        
        /// <summary>
        /// makes sure the quarters are in the correct order
        /// </summary>
        /// <param name="studyPlan"></param>
        /// <returns>
        /// true if the quarters are in correct order in the studyPlan
        /// </returns>
        /// <remarks>
        /// This method is currentily not in use for plan verification but could be used as an unit test
        /// </remarks>
        public static bool Output_CourseInfo_QuartersCorrectOrder(List<StudyPlan> studyPlan)
        {
            if (studyPlan.Count == 0)
            {
                return true;
            }
            int lastQtr = studyPlan[0].QuarterId;
            int lastYear = studyPlan[0].YearId;
            for (int i = 1; i < studyPlan.Count; i++)
            {

                int qtr = studyPlan[i].QuarterId;
                int year = studyPlan[i].YearId;
                if (qtr == lastQtr)
                {
                    continue;
                }

                int expectedQtr = getNextQuarter(lastQtr);


                if (!(qtr == expectedQtr && expectedQtr != -1 || year > lastYear || (lastQtr < qtr && lastYear == year)))
                {
                    return false;

                }


                lastQtr = qtr;
                lastYear = year;
            }
            return true;
        }

        /// <summary>
        /// using param year and qtr returns the next year if associated with the next qtr
        /// </summary>
        /// <param name="year">The current year</param>
        /// <param name="qtr">The current qtrId</param>
        /// <returns>Next qtrs year</returns>
        // returns the next year after the current qtr and year
        private static int getNextYear(int year, int qtr)
        {
            int nextQuarter = getNextQuarter(qtr);

            if (nextQuarter == 2)
            {
                return year + 1;
            }

            return year;

        }

        /// <summary>
        ///  makes sure the quarters are in the correct order
        /// </summary>
        /// <param name="studyPlan"></param>
        /// <returns>
        /// True if the quarters are in the correct order
        /// </returns>
        /// <remarks>
        /// This method is currentily not in use for plan verification but could be used as an unit test
        /// </remarks>
        public static bool Output_CourseInfo_YearsCorrectOrder(List<StudyPlan> studyPlan)
        {
            if (studyPlan.Count == 0)
            {
                return true;
            }
            int lastQtr = studyPlan[0].QuarterId;
            int lastYr = studyPlan[0].YearId;

            for (int i = 1; i < studyPlan.Count; i++)
            {
                int qtr = studyPlan[i].QuarterId;

                int year = studyPlan[i].YearId;

                // the quarter has more than one course scheduled
                if (qtr == lastQtr && year == lastYr)
                {
                    continue;
                }




                int expectedYear = getNextYear(lastYr, lastQtr);

                if (!(expectedYear == year || year > lastYr || (lastQtr < qtr && lastYr == year)))
                {
                    return false;
                }

                lastQtr = qtr;
                lastYr = year;

            }
            return true;
        }



        // tests of transfer degree is fulfilled
        /// <summary>
        /// Ensures that studyPlan contains all degrees required courses
        /// </summary>
        /// <param name="studyPlan">Current Study Plan</param>
        /// <param name="requiredCourses">List of degree required courses</param>
        /// <returns></returns>
        public static List<int> Output_DegreeFulfilled(List<StudyPlan> studyPlan, List<int> requiredCourses)
        {
            for (int i = 0; i < studyPlan.Count; i++)
            {

                int val = studyPlan[i].CourseId;
                requiredCourses.Remove(val);
            }
            return requiredCourses;
        }

        /// <summary>
        /// Ensures that all the courses have thier prereqs scheudled correctily
        /// </summary>
        /// <param name="studyPlan">The current Plan</param>
        /// <param name="prereq">Mapping of courseId to array of prereq groups</param>
        /// <param name="takenCourses">all the courses the student already has taken</param>
        /// <returns>mapping of course id and list of unschedued prereq</returns>
        /// <remarks>
        /// If all the prereq groups are unfulfilled, it chooses the missing courses from the last prereq group for validation messages  
       /// </remarks>
        // all the classes have their prereqs satisfied
        public static Dictionary<int, List<int>> Output_CorrectOrderOfPreReq(List<StudyPlan> studyPlan,
            Dictionary<int, int[][]> prereq, int[] takenCourses)
        {
            //  quarters is a list of courses. Each element corresponds to a new quarter    
            List<List<int>> quarters = studyPlan.GroupBy(m => new { m.QuarterId, m.YearId }).Select(grp => grp.Select(m => m.CourseId).ToList()).ToList();

            // convert quarters to a stack.    
            // the top element is the quarter that will be taken last
            Stack<List<int>> s = new Stack<List<int>>(quarters);

            //mapping of course id and list of unschedued prereq
            Dictionary<int, List<int>> result = new Dictionary<int, List<int>>();

            // go through each quarter and ensure that at least one group of prereqs is in the list composed of the remaining stack elements and the takenCourses
            while (s.Count > 0)
            {
                List<int> currQuarter = s.Pop();

                // select all the elements of the stack and add taken courses to it. flattenit
                var r = s.SelectMany(m => m).Concat(takenCourses).ToList();

                // ensure at all of the courses in current quarter have prereqs scheudled
                foreach (var c in currQuarter)
                {

                    int[][] coursePrereqs = getPrereqGroups(prereq, c);

                    // keeps track of if a course has prereqs met
                    bool satisfied = false;
                    List<int> intersect = null;
                    foreach (var v in coursePrereqs)
                    {
                       
                       // intersect is the list of values coursePrereqs and the remaining stack have in common
                       intersect = r.Intersect(v.ToList()).ToList();
                       satisfied = intersect.Count == v.Length;
                        
                        // if one of the groups match the course has prereqs scheduled
                      if (satisfied)
                        {
                           break;
                        }

                    }

                    // course has prereqs that are unmet 
                    if (!satisfied && coursePrereqs.Length != 0 )
                    {
                        // if !satified all the coursePrereq groups were visited

                        // add mapping of course and all the prereqs that were not scheduled in last group
                        var lastPrereqGroup = coursePrereqs.Last();
                            if (!result.ContainsKey(c))
                                result.Add(c, new List<int>());
                        result[c].AddRange(lastPrereqGroup.Except(intersect));

                    }

                }

            }
           
            return result;
        }


        /// <summary>
        /// returns the group associated with courseId
        /// </summary>
        /// <param name="prereqs"> Mapping of courseId to prereq groups</param>
        /// <param name="courseId"> current courseId</param>
        /// <returns>The group associated with courseId. If courseId has no groups it returns int[0][]</returns>
        private static int[][] getPrereqGroups(Dictionary<int, int[][]> prereqs, int courseId)
        {
            if(prereqs.ContainsKey(courseId))
                return prereqs[courseId];
            return new int[0][];
        }
    }

}

