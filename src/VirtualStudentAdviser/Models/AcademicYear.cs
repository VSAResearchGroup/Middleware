using System;
using System.Collections.Generic;

namespace VirtualStudentAdviser.Models
{
    public partial class AcademicYear
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public int Status { get; set; }

       // public virtual Student Student { get; set; }
    }
}
