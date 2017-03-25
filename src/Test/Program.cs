using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Test
{
    public class Program
    {

        List<CoursePreCorequisites> archiveCopre = new List<CoursePreCorequisites>();   //not necessary based on current implementation

        public Course courses;  // Course object containing all properties in the Course model
        public int DeptId;      //departmentID of selected major
        public int[] TargetCourses;   //list of target courses based on selected school and major
        public int CurrentCourse;       //course in the current iteration
        public int MajorID;       //course in the current iteration
        public VirtualAdvisorContext VirtualAdvisor = new VirtualAdvisorContext();   //New instance of the virtual database
        public LinkedList<CoursePreCorequisites> TargetList;

        /* This is list of prerequistes and prerequisites of courses. Each item in the 
        list contains a CoursePreCorequisites object (course ID, its prereq and coreq) */
        List<CoursePreCorequisites> copre = new List<CoursePreCorequisites>();

        Stack<int[]> globalStack = new Stack<int[]>(); //This is stack of arrays of prerequistes and arrays of corequisites to iterate through
        HashSet<int> visitedCourse = new HashSet<int>(); //Variable to track visited courses in iterations         

        /* Public class containing course ID, its prerequistes and its corequisites*/
        public class CoursePreCorequisites
        {
            public int CourseId { get; set; }
            public int[] Prerequisites { get; set; }
            public int[] Corequisites { get; set; }
            public List<int[]> CourseSchedule { get; set; }

            /*Constructor to set courseID, prerequisites and corequisites*/
            public CoursePreCorequisites(int CourseId, int[] Prerequisites, int[] Corequisites, List<int[]> CourseSchedule)
            {
                this.CourseId = CourseId;
                this.Prerequisites = Prerequisites;
                this.Corequisites = Corequisites;
                this.CourseSchedule = CourseSchedule;
            }

        }



        ///*Main class of program*/
        //public static void Main(string[] args)
        //{
        //    //var host = new WebHostBuilder()
        //    //    .UseKestrel()
        //    //    .UseContentRoot(Directory.GetCurrentDirectory())
        //    //    .UseIISIntegration()
        //    //    .UseStartup<Startup>()
        //    //    .Build();

        //    //host.Run();
        //    Program newPro = new Program();
        //    int[] arr = new int[2] { 1, 2 };
        //   String result = newPro.runRecommendationEngine(1, arr, 2 );

        //    Console.Out.WriteLine(result);
        //}

        public String runRecommendationEngine(int MajorId, int[] CompleteCourses, int SchoolId)
        {
            String result = "";
            MajorID = MajorId;
            DeptId = getDept(MajorId);
            //TargetCourses = getTargetCourses(MajorId, SchoolId);
            TargetCourses = new int[3] { 913, 926, 914 };// getTargetCourses(MajorId, SchoolId);
            int NumOfTargetCourses = TargetCourses.Length;
            Dictionary<int, List<CoursePreCorequisites>> TargetRequisites = new Dictionary<int, List<CoursePreCorequisites>>();

            for (int i = 0; i < NumOfTargetCourses; i++)
            {
                CurrentCourse = TargetCourses[i];
                getTargetRequistes(CurrentCourse);
                //TargetRequisites.Add(CurrentCourse, copre);
                // copre = new List<CoursePreCorequisites>();
            }
            // result  = toJson(TargetRequisites);
            result = JsonConvert.SerializeObject(copre);
            return result;

        }

        public int getDept(int MajorId)
        {
            int DeptId = Convert.ToInt32(VirtualAdvisor.Major.Where(m => m.Id == MajorId).Select(m => m.DepartmentId).FirstOrDefault());
            return DeptId;
        }


        public int[] getTargetCourses(int MajorId, int SchoolId)
        {
            int[] TargetCourses = VirtualAdvisor.AdmissionRequiredCourses.Where(a => a.Id == MajorId && a.SchoolId == SchoolId).Select(a => a.DepartmentId).ToArray<int>();
            return TargetCourses;
        }



        public String toJson(Dictionary<int, List<CoursePreCorequisites>> TargetRequisites)
        {


            String result = "";
            int count = 0;
            int targetLength = TargetRequisites.Count;
            foreach (var item in TargetRequisites)
            {
                count++;
                int targetCourseId = item.Key;
                List<CoursePreCorequisites> reqNetwork = item.Value.ToList<CoursePreCorequisites>();
                int i = 0;
                int length = reqNetwork.Count;
                result += "{TargetCourseId_" + count + ":{";
                result += "TargetCourseId: " + targetCourseId.ToString() + ",";
                result += "RequisitesNetwork: " + "[";

                foreach (var reqnet in reqNetwork)
                {
                    i++;
                    result += "{";
                    result += "CourseID:" + reqnet.CourseId.ToString() + ",";
                    result += "Prerequistes: [" + JsonConvert.SerializeObject(reqnet.Prerequisites) + "],";
                    result += "Corerequisites:" + JsonConvert.SerializeObject(reqnet.Corequisites) + "]";
                    if (i == length) { result += "}"; } else { result += "},"; }
                }
                result += "]";

                if (count == targetLength) { result += "}"; } else { result += "},"; }
                result += "}";

            }

            return result;

        }


        public void getTargetRequistes(int TargetCourseID)
        {
            int currentCourse = TargetCourseID;
            int[] PrerequisitesArr = getPrerequisite(currentCourse);
            int[] CorequisitesArr = getCorequisite(currentCourse);
            List<int[]> CourseSchedule = getCourseSchedule(currentCourse);

            copre.Add(new CoursePreCorequisites(currentCourse, PrerequisitesArr, CorequisitesArr, CourseSchedule));

            if (PrerequisitesArr.Length > 0) { globalStack.Push(PrerequisitesArr); }
            if (CorequisitesArr.Length > 0) { globalStack.Push(CorequisitesArr); }

            while (globalStack.Count != 0)
            {
                getDirectRequisites(globalStack.Pop());
            }

            //Final Dictionary to be Converted to a Json File. Json file will be sent to Phases algorithms 
            archiveCopre.Concat(copre);                     //add the requistes network of a target course id into an archive 

        }



        public void getDirectRequisites(int[] requisite)
        {
            int reqSize = requisite.Length;
            int currentCourse;
            int[] PrerequisitesArr;
            int[] CorequisitesArr;
            List<int[]> CourseSchedule;

            for (int i = 0; i < reqSize; i++)
            {
                currentCourse = requisite[i];
                if (!visitedCourse.Contains(currentCourse))
                {
                    visitedCourse.Add(currentCourse);

                    PrerequisitesArr = getPrerequisite(currentCourse);
                    CorequisitesArr = getCorequisite(currentCourse);

                    CourseSchedule = getCourseSchedule(currentCourse);

                    if (PrerequisitesArr.Length > 0) { globalStack.Push(PrerequisitesArr); }
                    if (CorequisitesArr.Length > 0) { globalStack.Push(CorequisitesArr); }

                    copre.Add(new CoursePreCorequisites(currentCourse, PrerequisitesArr, CorequisitesArr, CourseSchedule));
                }

                ////Still need to work on a better approach. Retrieve item from the archive
                //tempReq = archiveCopre.Find(c => c.getCourseId() == currentCourse);
                //req.Add(tempReq);

                //OR
                //var test = (archiveCopre.Where(s => s.getCourseId() == currentCourse)).ToList<CoursePreCorequisites>();
                //req.Concat(test);

            }

            // copre.Concat(req);  
        }



        public int[] getPrerequisite(int CourseId)
        {
            int[] Prerequisites = VirtualAdvisor.Requisites.Where(r => r.Relationship == 1 && r.CourseId == CourseId && r.DepartmentId == MajorID).Select(r => r.RequisiteId).ToArray<int>();
            return Prerequisites;
        }

        public int[] getCourseGroup(int CourseId)
        {
            int[] GroupIds = VirtualAdvisor.Prerequisite.Where(p => p.CourseId == CourseId).Select(pre => pre.GroupId).Distinct().ToArray<int>();
            return GroupIds;
        }

        public List<int[]> n_getPrerequisite(int CourseId)
        {
            List<int[]> Prerequisites = new List<int[]>();
            int[] GroupIds = getCourseGroup(CourseId);

            for (int i = 0; i < GroupIds.Length; i++)
            {

                int[] arr = VirtualAdvisor.Prerequisite.Where(p => p.CourseId == CourseId && p.GroupId == GroupIds[i]).Select(pre => pre.PrerequisiteId).ToArray<int>();
                Prerequisites.Add(arr);
            }

            return Prerequisites;
        }


        public int[] getPostrequisite(int CourseId)
        {
            int[] Postrequisite = VirtualAdvisor.Prerequisite.Where(p => p.PrerequisiteId == CourseId).Select(c => c.CourseId).Distinct().ToArray<int>();
            return Postrequisite;
        }

        public class CourseSchedule
        {
            public int StartTimeId { get; set; }
            public int EndTimeId { get; set; }
            public int DayId { get; set; }
            public int QuarterId { get; set; }
            public int Year { get; set; }

            /*Constructor to set courseID, prerequisites and corequisites*/
            public CourseSchedule(int StartTimeId, int EndTimeId, int DayId, int QuarterId, int Year)
            {
                this.StartTimeId = StartTimeId;
                this.EndTimeId = EndTimeId;
                this.DayId = DayId;
                this.QuarterId = QuarterId;
                this.Year = Year;
            }

        }


        public List<CourseSchedule> getCourseSchedule2(int CourseId)
        {
            List<CourseSchedule> CourseSchedule = new List<CourseSchedule>();
            var value = VirtualAdvisor.CourseTime.Where(c => c.CourseId == CourseId);

            foreach (CourseTime crs in value)
            {
                CourseSchedule.Add(new CourseSchedule(crs.StartTimeId, crs.EndTimeId, crs.DayId, crs.QuarterId, crs.Year));
            }

            return CourseSchedule;
        }

        public List<int[]> getCourseSchedule(int CourseId)
        {
            List<int[]> CourseSchedule = new List<int[]>();
            var value = VirtualAdvisor.CourseTime.Where(c => c.CourseId == CourseId);

            foreach (CourseTime crs in value)
            {
                int[] arr = new int[5];
                arr[0] = crs.StartTimeId; arr[1] = crs.EndTimeId; arr[2] = crs.DayId; arr[3] = crs.QuarterId; arr[4] = crs.Year;
                CourseSchedule.Add(arr);
            }

            return CourseSchedule;
        }


        public CourseNode getCourseDetails(int CourseId)
        {
            CourseNode courseNode = new CourseNode();
            courseNode.CourseId = CourseId;
            courseNode.CourseSchedule = getCourseSchedule(CourseId);
            courseNode.Prerequisites = getPrerequisite(CourseId);
            courseNode.Corequisites = getCorequisite(CourseId);
            return courseNode;
        }

        public int[] getCorequisite(int CourseId)
        {

            int[] getCorequisites = VirtualAdvisor.Requisites.Where(r => r.Relationship == 2 && r.CourseId == CourseId).Select(r => r.RequisiteId).ToArray<int>();
            return getCorequisites;
        }

        public void removeCompletedCourses(Dictionary<int, List<CoursePreCorequisites>> TargetRequisites)
        {
            //write code here
        }
    }


}
