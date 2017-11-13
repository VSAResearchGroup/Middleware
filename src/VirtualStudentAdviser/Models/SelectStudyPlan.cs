using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VirtualStudentAdviser.Models
{
      public class SelectStudyPlan
        {
            public int PlanId { get; set; }
            public string CourseNumber { get; set; }
            public string Quarter { get; set; }
            public int Year { get; set; }

            public SelectStudyPlan(int PlanId, string CourseNumber, string Quarter, int Year)
            {
                this.PlanId = PlanId;
                this.CourseNumber = CourseNumber;
                this.Quarter = Quarter;
                this.Year = Year;
            }
        }

    
}
