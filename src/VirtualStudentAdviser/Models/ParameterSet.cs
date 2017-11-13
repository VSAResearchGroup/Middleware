using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VirtualStudentAdviser.Models
{
    public class ParameterSet
    {
        public int Id { get; set; }
        public int MajorId { get; set; }
        public int SchoolId { get; set; }
        public int JobTypeId { get; set; }
        public int BudgetId { get; set; }
        public int TimePreferenceId { get; set; }
        public int QuarterPreferenceId { get; set; }

        public string CompletedCourses { get; set; }
        public string PlacementCourses { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime LastDateModified { get; set; }
        public int Status { get; set; }


        public ParameterSet(int MajorId, int SchoolId, int JobTypeId, int BudgetId, int TimePreferenceId, int QuarterPreferenceId,
            string CompletedCourses, string PlacementCourses)
        {
            this.MajorId = MajorId;
            this.SchoolId = SchoolId;
            this.JobTypeId = JobTypeId;
            this.BudgetId = BudgetId;
            this.TimePreferenceId = TimePreferenceId;
            this.QuarterPreferenceId = QuarterPreferenceId;
            this.CompletedCourses = CompletedCourses;
            this.PlacementCourses = PlacementCourses;
            this.DateAdded = DateTime.Now;
            this.LastDateModified = DateTime.Now;
            this.Status = 1;
        }

    }
}
