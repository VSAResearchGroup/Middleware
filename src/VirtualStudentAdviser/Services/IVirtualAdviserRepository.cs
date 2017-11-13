using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtualStudentAdviser.Models;

namespace VirtualStudentAdviser.Services
{
    public interface IVirtualAdviserRepository
    {

        string runRecommendationEngine(int MajorId, int[] CompleteCourses, int SchoolId);
        List<SelectStudyPlan> launchEngine(int MajorId, int[] CompleteCourses, int SchoolId);
        List<SelectStudyPlan> getStudyPlan(int planId);
    }
}
