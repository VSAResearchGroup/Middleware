using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json; 
 
namespace Test
{
    public class StudyPlan2
    {
        //Plans[
        //Quarters
        //Courses, Quarter, Year
        //Courses[CourseId, Schedules]
        //Schedules[ [1, 1], [1,1] ]
        List<Plans> generateStudyPlans = new List<Plans>();
        //Quarters[] quarters;

        public class Output
        {
            public Plans[] plan { get; set; }

            public Output(Plans[] plan)
            {
                this.plan = plan;
            }


        }
        public class Plans
        {
            public Quarters[] quarter { get; set; }

         public Plans(Quarters[] quarter)
            {
                this.quarter = quarter;
            }


        }
         

        public class Quarters  //arrays of list
            {
                public int[] Courses { get; set; }
                public int Quarter { get; set; }   //this is the quarterID
                public int Year { get; set; }   //this is the year

                public Quarters(int[] Courses, int Quarter, int Year)
                {
                    this.Courses = Courses;
                    this.Quarter = Quarter;
                    this.Year = Year;
                }
            }

        //public class Courses
        //{
        //    public int CourseId { get; set; }
        //    public List<int[]> Schedule { get; set; }

        //    public Courses(int CourseId, List<int[]> Schedule)
        //    {
        //        this.CourseId = CourseId;
        //        this.Schedule = Schedule;
        //    }
        //}





        public void printDetails(Plans[] studyPlans)
        {
            int length = studyPlans.Length;
            for (int i = 0; i < length; i++)
            {
                Plans cPlan = studyPlans[i];
                Quarters[] cQuatPlan = cPlan.quarter;
                for (int j = 0; j < cQuatPlan.Length; j++)
                {
                    Quarters q = cQuatPlan[j];
                    int[] course = q.Courses;
                    for (int k = 0; k < course.Length; k++)
                    {
                        int c = course[k];
                    }
                    int year = q.Year;
                    int quarter = q.Quarter;

                }
            }
        }






        //Plans[] plans = studyPlans.plan;
        //for (int i = 0; i < plans.Length; i++)
        //{
        //    Quarters[] cQuater = plans[i].quarter;

        //    //Courses[] Course = cQuater.Course;
        //    for (int j = 0; i < Course.Length; i++)
        //    {
        //        Courses currentCourse = Course[j];
        //        int CourseId = currentCourse.CourseId;
        //    }
        //    foreach (Quarters[] quaterPlan in plans)
        //    {
        //        int planID = 0;
        //        Quarters[] quarter = plan.quarter;
        //        for (int i = 0; i < quarter.Length; i++)
        //        {
        //            Quarters currentQuarter = quarter[i];
        //            Courses[] Course = currentQuarter.Course;
        //            for (int j = 0; i < Course.Length; i++)
        //            {
        //                Courses currentCourse = Course[j];
        //                int CourseId = currentCourse.CourseId; 

        //                //code to insert them in a database
        //            }

        //            int Year = currentQuarter.Year;
        //            int QuarterID = currentQuarter.Quarter;
        //        } 

        //    }


        //return test;


        ///*Main class of program*/
        //public static void Main(string[] args)
        //{

        //    //string json = System.IO.File.ReadAllText(Microsoft.AspNetCore.Server.MapPath("~/JSON.txt")).Replace(System.Environment.NewLine, "");


        //    StudyPlan sp = new StudyPlan();
        //    var json = System.IO.File.ReadAllText(@"C:\Users\CDLADMIN\Documents\Visual Studio 2015\Projects\Test\src\Test\output.json");

        //    List<Plans> result = JsonConvert.DeserializeObject<List<Plans>>(json);
        //    //sp.printDetails(result);





        //    //var Plans = JArray.Parse(json); // parse as array   

        //    Console.Out.WriteLine(result);
        //}






    }
}
