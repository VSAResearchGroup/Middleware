using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Test
{
    public class PlanParser
    {

        public VirtualAdvisorContext VirtualAdvisor = new VirtualAdvisorContext();   //New instance of the virtual database

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


        public class Plans
        {
            public Quarters[] quarter { get; set; }

            public Plans(Quarters[] quarter)
            {
                this.quarter = quarter;
            }


        }

        public class GeneratedPlans
        {
            public int CourseId { get; set; }
            public int PlanId { get; set; }
            public int QuarterId { get; set; }
            public int Year { get; set; }
        }

        public void ParseStudyPlan()
        {
            var json = System.IO.File.ReadAllText(@"C:\Users\CDLADMIN\Documents\Visual Studio 2015\Projects\Test\src\Test\output.json");
            var Plans = JArray.Parse(json); // parse as array  

            foreach (JObject plan in Plans) //this could be for very JArray
            {
                int planId = insertPlan("Default", 1, 1);
                insertStudentStudyPlan(456, planId, 1);

                JArray items = (JArray)plan["Quarters"];
                int count = items.Count;
                List<StudyPlan> studyPlan = new List<StudyPlan>();
                //public StudyPlan(int PlanId, int QuarterId, int YearId, int CourseId, DateTime DateAdded, DateTime LastDateModified)
                for (int i = 0; i < count; i++)
                {
                    int currentYear = (int)items[i]["Year"];
                    int currentQuarter = (int)items[i]["Quarter"];
                    var arrCourse = items[i]["Courses"];
                    int len = arrCourse.Count();
                    if (arrCourse.Count() != 0)
                    {
                        for (int j = 0; j < len; j++)
                        {
                            int currentCourse = (int)arrCourse[j];
                            studyPlan.Add(new StudyPlan(planId, currentQuarter, currentYear, currentCourse, DateTime.Now, DateTime.Now));

                        }

                    }

                }
                insertStudyPlan(studyPlan);

            }
        }


        public int insertPlan(string Name, int ParameterSetId, int Status)
        {
            GeneratedPlan gPlan = new GeneratedPlan(Name, ParameterSetId, DateTime.Now, DateTime.Now, Status);

            var vsa = new VirtualAdvisorContext();
            vsa.GeneratedPlan.Add(gPlan);
            vsa.SaveChanges();
            int PlanId = gPlan.Id;
            return PlanId;
        }

        public void insertStudyPlan(List<StudyPlan> studyPlan)
        {
            int count = studyPlan.Count;
            StudyPlan[] newStudyPlan = new StudyPlan[count];
            for (int i = 0; i < count; i++)
            {
                newStudyPlan[i] = studyPlan[i];
            }

            var vsa = new VirtualAdvisorContext();
            vsa.AddRange(newStudyPlan);
            vsa.SaveChanges();
        }

        public void insertStudentStudyPlan(int StudentId, int PlanId, int Status)
        {
            StudentStudyPlan sPlan = new StudentStudyPlan(StudentId, PlanId, DateTime.Now, DateTime.Now, Status);//(Name, ParameterSetId, DateTime.Now, DateTime.Now, Status);

            var vsa = new VirtualAdvisorContext();
            vsa.StudentStudyPlan.Add(sPlan);
            vsa.SaveChanges();

        }
        public void insertReviewedStudyPlan(int StudentId, int PlanId, string PlanName, int AdvisorId)
        {

        }
        /*Main class of program*/
        public static void Main(string[] args)
        {

            PlanParser ps = new PlanParser();
            ps.ParseStudyPlan();
            String doneStatus = "Successfully";
            Console.WriteLine(doneStatus);
        }

    }

}
