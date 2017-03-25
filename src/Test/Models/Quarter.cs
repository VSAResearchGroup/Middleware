using System;
using System.Collections.Generic;

namespace Test
{
    public partial class Quarter
    {
        public Quarter()
        {
            CourseTime = new HashSet<CourseTime>();
            StudentEnrollment = new HashSet<StudentEnrollment>();
        }

        public int QuarterId { get; set; }
        public string Quarter1 { get; set; }
        public int Status { get; set; }

        public virtual ICollection<CourseTime> CourseTime { get; set; }
        public virtual ICollection<StudentEnrollment> StudentEnrollment { get; set; }
    }
}
