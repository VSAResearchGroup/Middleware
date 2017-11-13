using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VirtualStudentAdviser.Models
{
    public class StudyPlan
    {
        public int Id { get; set; }
        public int PlanId { get; set; }
        public int QuarterId { get; set; }
        public int YearId { get; set; }
        public int CourseId { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime LastDateModified { get; set; }


        public StudyPlan(int PlanId, int QuarterId, int YearId, int CourseId, DateTime DateAdded, DateTime LastDateModified)
        {
            this.PlanId = PlanId;
            this.QuarterId = QuarterId;
            this.YearId = YearId;
            this.CourseId = CourseId;
            this.DateAdded = DateAdded;
            this.LastDateModified = LastDateModified;

        }


        public StudyPlan() { }
    }




}
