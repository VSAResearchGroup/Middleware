using System;
using System.Collections.Generic;

namespace VirtualStudentAdviser.Models
{
    public partial class WeekDay
    {
        public WeekDay()
        {
            CourseTime = new HashSet<CourseTime>();
        }

        public int DayId { get; set; }
        public string WeekDay1 { get; set; }
        public int? Status { get; set; }

        public virtual ICollection<CourseTime> CourseTime { get; set; }
    }
}
