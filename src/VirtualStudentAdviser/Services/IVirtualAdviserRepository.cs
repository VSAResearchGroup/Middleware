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
        List<SelectStudyPlan> getStudyPlan(int planId);
        List<StudyPlan> getStudyPlans(int planId);
        List<StudyPlan> GetAllStudyPlans();
        int[] getMajorSchoolPairs(int planId);
        int[] getPlanIds();
        PlanVerificationInfo testPlan(List<StudyPlan> studyPlan, int majorId, int schoolId, int[] requiredCourses, List<Course> courses);
        List<int[]> getCourseSchedule(int courseId);
        Dictionary<int, int[]> getPrerequisites(int courseId);
        List<StudyPlan> convertStudyPlan(List<SelectStudyPlan> studyPlan);
        void insertStudyPlan(List<StudyPlan> studyPlan);
        int[][] getAllTargetCourses();
        int[] getTargetCourses(int MajorId, int SchoolId);
        List<Course> getAllCourses();
        PlanInfo GetActiveParameterSet(int studentId);
        int insertNewStudyPlan(List<StudyPlan> sp, int studentId, int majorId, int schoolId, string planName);
        PlanInfo[] GetInactiveParameterSets(int studentId);

    }
}
