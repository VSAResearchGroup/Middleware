using System;
using System.Collections.Generic;

namespace Test
{
    public partial class CourseTime
    {
       public int Id { get; set; }
        public int CourseId { get; set; }
       public string CourseNumber { get; set; }
        public int StartTimeId { get; set; }
        public int EndTimeId { get; set; }
        public int DayId { get; set; }
        public int QuarterId { get; set; }
        public int Year { get; set; }
        public int Status { get; set; }

        public virtual Course Course { get; set; }
        public virtual WeekDay Day { get; set; }
        public virtual TimeSlot EndTime { get; set; }
        public virtual Quarter Quarter { get; set; }
        public virtual TimeSlot StartTime { get; set; }



        
    }
}
