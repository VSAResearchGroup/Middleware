using System;
using System.Collections.Generic;

namespace VirtualStudentAdviser.Models
{
    public partial class TimeSlot
    {
        public TimeSlot()
        {
            CourseTimeEndTime = new HashSet<CourseTime>();
            CourseTimeStartTime = new HashSet<CourseTime>();
        }

        public int TimeId { get; set; }
        public TimeSpan Time { get; set; }
        public int Status { get; set; }

        public virtual ICollection<CourseTime> CourseTimeEndTime { get; set; }
        public virtual ICollection<CourseTime> CourseTimeStartTime { get; set; }
    }
}
