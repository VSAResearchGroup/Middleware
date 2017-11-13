using System;
using System.Collections.Generic;

namespace VirtualStudentAdviser.Models
{
    public partial class Quarter
    {
        public Quarter()
        {
            CourseTime = new HashSet<CourseTime>();
        }

        public int QuarterId { get; set; }
        public string QuarterName { get; set; }
        public int Status { get; set; }

        public virtual ICollection<CourseTime> CourseTime { get; set; }
    }
}
