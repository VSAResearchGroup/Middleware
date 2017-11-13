using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VirtualStudentAdviser.Models
{
    public class ReviewedStudyPlan
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int PlanId { get; set; }
        public string PlanName { get; set; }
        public int AdvisorId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ReviewDate { get; set; }
        public DateTime LastDateModified { get; set; }
        public int Status { get; set; }


        public ReviewedStudyPlan(int StudentId, int PlanId, string PlanName, int AdvisorId, DateTime CreationDate, DateTime ReviewDate, DateTime LastDateModified, int Status)
        {
            this.StudentId = StudentId;
            this.PlanId = PlanId;
            this.AdvisorId = AdvisorId;
            this.CreationDate = CreationDate;
            this.ReviewDate = ReviewDate;
            this.LastDateModified = LastDateModified;
            this.Status = Status;

        }
    }


}
