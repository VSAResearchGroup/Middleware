using System;
using System.Collections.Generic;

namespace Test
{
    public partial class StudentEnrollment
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string CourseNumber { get; set; }
        public int QuarterId { get; set; }
        public int Elective { get; set; }
        public int Core { get; set; }
        public int CreditNo { get; set; }
        public int Status { get; set; }
        public int Year { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public DateTime LastDateModified { get; set; }

        public virtual Quarter Quarter { get; set; }
        public virtual Student Student { get; set; }
    }
}
