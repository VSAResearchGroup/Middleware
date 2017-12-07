using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VirtualStudentAdviser.Models
{
    public class StudentStudyPlan
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int PlanId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastDateModified { get; set; }
        public int Status { get; set; }
        public string PlanName { get; set; }
        public StudentStudyPlan(int StudentId, int PlanId, DateTime CreationDate, DateTime LastDateModified, int Status, string PlanName)
        {
            this.StudentId = StudentId;
            this.PlanId = PlanId;
            this.CreationDate = CreationDate;
            this.LastDateModified = LastDateModified;
            this.Status = Status;
            this.PlanName = PlanName;
        }


        public StudentStudyPlan(int StudentId, DateTime CreationDate, DateTime LastDateModified, int Status, string planName)
        {
            this.StudentId = StudentId;
            this.CreationDate = CreationDate;
            this.LastDateModified = LastDateModified;
            this.Status = Status;
            this.PlanName = planName;
        }


    }




}
