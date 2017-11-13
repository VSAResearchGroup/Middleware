using System;
using System.Collections.Generic;
using System.Linq;

namespace VirtualStudentAdviser.Models
{
    public partial class Course
    {
        public Course()
        {
            CourseTime = new HashSet<CourseTime>();
            Prerequisite = new HashSet<Prerequisite>();
        }



        public int CourseId { get; set; }
        public string CourseNumber { get; set; }
        public string AbbreviatedTitle { get; set; }
        public string Title { get; set; }
        public int MinCredit { get; set; }
        public int MaxCredit { get; set; }
        public string Description { get; set; }
        public int DepartmentId { get; set; }
        public string PreRequisites { get; set; }
        public string CoRequisites { get; set; }
        public int UseCatalog { get; set; }
        public int Status { get; set; }



        //public Prerequisites() {


        //    //return an array of the Prerequisites
        //    //return int[]
        //}

        //public Corequisites()
        //{
        //    //return an array of the Corequisites
        //    //return int[]
        //}



        //  public void get

        public virtual ICollection<CourseTime> CourseTime { get; set; }
        public virtual ICollection<Prerequisite> Prerequisite { get; set; }
    }
}
