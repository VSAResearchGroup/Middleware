using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Server.Kestrel.Internal.Networking;

namespace VirtualStudentAdviser.Models
{
    public class ParameterSet
    {
       
        public int Id { get; set; }
        public int MajorId { get; set; }

        [NotMapped]
        public string Major { get; set; }
        public int SchoolId { get; set; }

        [NotMapped]
        public string School { get; set; }

        public int JobTypeId { get; set; }
        [NotMapped]
        public string JobType { get; set; }

        public int BudgetId { get; set; }

        [NotMapped]
        public string Budget { get; set; }

        public int TimePreferenceId { get; set; }
        [NotMapped]
        public string TimePreference { get; set; }

        public int QuarterPreferenceId { get; set; }
        [NotMapped]
        public string QuarterPreference { get; set; }


        public string CompletedCourses { get; set; }
        public string PlacementCourses { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime LastDateModified { get; set; }
        public int Status { get; set; }
      //  public int planID { get; internal set; }

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


        public ParameterSet()
        {
            
           
        }
        public ParameterSet( string Major, string School, string JobType, string Budget, string TimePreference, string QuarterPreference,
            string CompletedCourses, string PlacementCourses)
        {
            this.Major = Major;
            this.School = School;
            this.JobType = JobType;
            this.Budget = Budget;
            this.TimePreference = TimePreference;
            this.QuarterPreference = QuarterPreference;
            this.CompletedCourses = CompletedCourses;
            this.PlacementCourses = PlacementCourses;
        }
    }
}
