using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VirtualStudentAdviser.Models
{
    /// <summary>
    /// Association of planId to plan errors
    /// </summary>
    /// <remarks>To add additional error results a new public list can be added here.
    /// A verification method needs to be added to PlanVerification and the list is populated in the runTests method
    ///  </remarks>
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
