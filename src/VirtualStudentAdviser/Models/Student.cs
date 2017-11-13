using System;
using System.Collections.Generic;

namespace VirtualStudentAdviser.Models
{
    public partial class Student
    { 
        public int StudentId { get; set; }
        public int MajorId { get; set; }
        public int DepartmentId { get; set; }
        public int JobTypeId { get; set; }
        public int Status { get; set; }
         
        public virtual JobType JobType { get; set; }
        public virtual Major Major { get; set; }
    }
}
