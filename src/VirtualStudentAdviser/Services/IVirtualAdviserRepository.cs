using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VirtualStudentAdviser.Models;

namespace VirtualStudentAdviser.Services
{
    public interface IVirtualAdviserRepository
    {

        string runRecommendationEngine(int MajorId, int[] CompleteCourses, int SchoolId);
        List<SelectStudyPlan> launchEngine(int MajorId, int[] CompleteCourses, int SchoolId);


        /// <summary>
        /// Returns list of SelectStudyPlan associated with planId
        /// </summary>
        /// <param name="planId">
        /// plan
        /// </param>
        /// <returns>
        /// Returns list of SelectStudyPlan associated with planId
        /// </returns>
        List<SelectStudyPlan> getStudyPlan(int planId);

        /// <summary>
        /// Returns list of StudyPlan associated with planId
        /// </summary>
        /// <param name="planId">
        /// plan
        /// </param>
        /// <returns>
        /// Returns list of StudyPlan associated with planId
        /// </returns>
        List<StudyPlan> getStudyPlans(int planId);



        /// <summary>
        /// Gets the plan's assocaited major and school information
        /// </summary>
        /// <param name="planId">
        /// plan
        /// </param>
        /// <returns>
        /// Returns array with major and school ids {planId, majorId, schoolId}
        ///</returns>
        int[] getMajorSchoolPairs(int planId);

        /// <summary>
        /// Gets all of the plan ids
        /// </summary>
        /// <returns> 
        /// array of plan ids
        /// </returns>
        int[] getPlanIds();


        /// <summary>
        /// Runs verification on studyPlan
        /// </summary>
        /// <param name="studyPlan">The plan to be tested</param>
        /// <param name="majorId">Plan's majorId</param>
        /// <param name="schoolId">Plan's schoolId</param>
        /// <param name="requiredCourses">Array of all the degree's required courses</param>
        /// <param name="courses">List of all the courses offered at EvCC from the db</param>
        /// <returns>
        /// PlanVerificationInfo, arrays of error messages 
        /// </returns>
        PlanVerificationInfo testPlan(List<StudyPlan> studyPlan, int majorId, int schoolId, int[] requiredCourses, List<Course> courses);

        /// <summary>
        /// Gets all of the time scheduings for the course represented by courseId
        /// </summary>
        /// <param name="courseId">The selected course</param>
        /// <returns>List of arrays that represent the scheduling</returns>
        List<int[]> getCourseSchedule(int courseId);


        /// <summary>
        /// Takes SelectStudyPlan (string representation) and parses it into StudyPlan (id representation) 
        /// </summary>
        /// <param name="studyPlan">The studyPlan with string values</param>
        /// <returns>StudyPlan with the associated id keys</returns>
        /// <remarks>SelectStudyPlan is the format the webpage sends to the API in the body of the message.</remarks>
        List<StudyPlan> convertStudyPlan(List<SelectStudyPlan> studyPlan);

        /// <summary>
        /// Writes new plan to DB
        /// </summary>
        /// <param name="studyPlan">Plan to be written</param>
        void insertStudyPlan(List<StudyPlan> studyPlan);

        /// <summary>
        /// Returns an array of all the major and school required courses 
        /// </summary>
        /// <remarks>
        /// The inner array is of format: {CourseId, MajorId, SchoolId}
        /// </remarks>
        /// <returns>Array of required course arrays</returns>
        int[][] getAllTargetCourses();

        /// <summary>
        /// Returns the degree core courses associated with the major and school
        /// </summary>
        /// <param name="MajorId">The id of the Major</param>
        /// <param name="SchoolId">The id of the School</param>
        /// <returns>list of required course ids</returns>
        int[] getTargetCourses(int MajorId, int SchoolId);

        /// <summary>
        /// Gets all available courses in the database
        /// </summary>
        /// <returns>
        /// List of Course objects
        /// </returns>
        List<Course> getAllCourses();

        /// <summary>
        /// Returns active PlanInfo associated with the student with id studentId
        /// </summary>
        /// <param name="studentId">studentId of current student</param>
        /// <returns>Returns all the inactive PlanInfo associated with the student associated with studentId</returns>
        /// <remarks> There is not a active plan column in StudentStudyPlan db table. When that is added, this will need to be rewritten
        /// since it assumes the first plan is the active one</remarks>
        PlanInfo GetActiveParameterSet(int studentId);

        /// <summary>
        /// Writes sp to datbase
        /// </summary>
        /// <param name="sp">New plan</param>
        /// <param name="studentId">Current student's id</param>
        /// <param name="majorId">Plan's majorID</param>
        /// <param name="schoolId">Plan's schoolID</param>
        /// <param name="planName">Name of plan given by student</param>
        /// <returns></returns>
        int insertNewStudyPlan(List<StudyPlan> sp, int studentId, int majorId, int schoolId, string planName);

        /// <summary>
        /// Returns all the inactive PlanInfo associated with the student associated with studentId
        /// </summary>
        /// <param name="studentId">studentId of current student</param>
        /// <returns>Returns all the inactive PlanInfo associated with the student with id studentId</returns>
        /// <remarks> There is not a active plan column in StudentStudyPlan db table. When that is added, this will need to be rewritten
        /// since it assumes the first plan is the active one</remarks>
        PlanInfo[] GetInactiveParameterSets(int studentId);

    }
}
