using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VirtualStudentAdviser.Models
{
    public class PlanVerificationInfo
    {
        public int planId;
        public List<string> unfulfilledDegreeCourses { get; set; }
        public List<string> unfulfilledPrereqs { get; set; }
        public List<string> incorrectScheduling { get; set; }

   

        public PlanVerificationInfo()
        {
            unfulfilledDegreeCourses = new List<string>();
            unfulfilledPrereqs = new List<string>();
            incorrectScheduling = new List<string>();

        }


    }
}
