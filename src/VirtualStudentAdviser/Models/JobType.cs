using System;
using System.Collections.Generic;

namespace VirtualStudentAdviser.Models
{
    public partial class JobType
    {
        public JobType()
        {
            Student = new HashSet<Student>();
        }

        public int Id { get; set; }
        public string JobType1 { get; set; }
        public int WorkHours { get; set; }
        public int Status { get; set; }

        public virtual ICollection<Student> Student { get; set; }
    }
}
