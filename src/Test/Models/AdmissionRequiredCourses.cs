using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Test 
{
    public class AdmissionRequiredCourses
    {

        public int Id { get; set; }
        public int MajorId { get; set; }
        public int SchoolId { get; set; }
        public int CourseId { get; set; }
        public int DepartmentId { get; set; } 

        public virtual Course Course { get; set; }
    }
} 
