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
    public class NewProgram
    {

        List<CoursePreCorequisites> archiveCopre = new List<CoursePreCorequisites>();   //not necessary based on current implementation

        public Course courses;  // Course object containing all properties in the Course model
        public int DeptId;      //departmentID of selected major
        public int[] TargetCourses;   //list of target courses based on selected school and major
        public int CurrentCourse;       //course in the current iteration
        public int MajorID;       //course in the current iteration
        public VirtualAdvisorContext VirtualAdvisor = new VirtualAdvisorContext();   //New instance of the virtual database
        public LinkedList<int[]> TargetList = new LinkedList<int[]>();


        /* This is list of prerequistes and prerequisites of courses. Each item in the 
        list contains a CoursePreCorequisites object (course ID, its prereq and coreq) */
        List<CoursePreCorequisites> copre = new List<CoursePreCorequisites>();

        Dictionary<int[], bool> visitedCourse = new Dictionary<int[], bool>(); //Variable to track visited courses in iterations   
        Dictionary<int, List<int[]>> globalCourseSchedule = new Dictionary<int, List<int[]>>();

        //New implementation
        public List<TargetLinkedList> targetPath = new List<TargetLinkedList>();
        public List<TargetLinkedList> tempTargetPath = new List<TargetLinkedList>();
        public List<TargetLinkedList> finalTargetPath = new List<TargetLinkedList>();
        public List<int> globalList = new List<int>();
        public Boolean done = false;

        Stack<TargetLinkedList> globalStack = new Stack<TargetLinkedList>(); //This is stack of arrays of prerequistes and arrays of corequisites to iterate through

        public class TargetLinkedList
        {
            //public  int CourseId { get; set; } 
            public LinkedList<int[]> TargetList { get; set; }

            /*Constructor to set courseID, prerequisites and corequisites*/
            public TargetLinkedList(LinkedList<int[]> TargetList) //int CourseId,
            {
                //  this.CourseId = CourseId; 
                this.TargetList = TargetList;

            }
        }

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
        //    NewProgram newPro = new NewProgram();
        //    int[] arr = new int[2] { 1, 2 };
        //    String result = newPro.runRecommendationEngine(1, arr, 2);

        //    Console.Out.WriteLine(result);
        //}

        public String runRecommendationEngine(int MajorId, int[] CompleteCourses, int SchoolId)
        {
            String result = "";
            MajorID = MajorId;
            DeptId = getDept(MajorId);
            //TargetCourses = getTargetCourses(MajorId, SchoolId);
            TargetCourses = new int[2] { 140, 234 };// getTargetCourses(MajorId, SchoolId);
            int NumOfTargetCourses = TargetCourses.Length;

            Dictionary<int, List<TargetLinkedList>> TargetRequisites = new Dictionary<int, List<TargetLinkedList>>();


            for (int i = 0; i < NumOfTargetCourses; i++)
            {
                CurrentCourse = TargetCourses[i];
                getTargetRequistes(CurrentCourse);
                TargetRequisites.Add(CurrentCourse, finalTargetPath);
                finalTargetPath = new List<TargetLinkedList>();
                visitedCourse = new Dictionary<int[], bool>();
            }

            getAllCourseSchedule();
            Object[] finalresponse = { TargetRequisites, globalCourseSchedule };
            result = JsonConvert.SerializeObject(finalresponse);
            // result = JsonConvert.SerializeObject(TargetRequisites);

            return result;

        }

        public void sortPath()
        {
            //check for how to get the length of a linked list that counts 
            //each integer arrays in the nodes of the linked list.
             
            foreach (TargetLinkedList tll in finalTargetPath)
            {
                
                int[] total = { };                
                LinkedList<int[]> nll = tll.TargetList;
                while (nll.Count != 0) {
                    total = total.Union(nll.First.Value).ToArray();
                    nll.RemoveFirst();
                }

                int count = total.Length;

            } 

        }

        public void getAllCourseSchedule()
        {

            List<int> courseList = new List<int>(globalList.Distinct());

            foreach (int CourseId in courseList)
            {
                List<int[]> CourseSchedule = getCourseSchedule(CourseId);
                globalCourseSchedule.Add(CourseId, CourseSchedule);
            }

        }

        public void getTargetRequistes(int TargetCourseID)
        {
            int currentCourse = TargetCourseID;
            globalList.Add(currentCourse);
            int[] tCourse = { currentCourse };

            int[] CorequisitesArr = getCorequisite(currentCourse);
            List<int[]> PrerequisitesArr = getPrerequisite(currentCourse);
            //List<int[]> CourseSchedule = getCourseSchedule(currentCourse); 


            foreach (int[] pre in PrerequisitesArr)
            {
                LinkedList<int[]> targetLinkedList = new LinkedList<int[]>();
                targetLinkedList.AddFirst(tCourse);
                targetLinkedList.AddFirst(pre);
                visitedCourse.Add(pre, false);
                targetPath.Add(new TargetLinkedList(targetLinkedList));    //List that saves the entire linked list 
                globalStack.Push(new TargetLinkedList(targetLinkedList));

            }

            while (!isDone())
            {
                foreach (TargetLinkedList currentTargetList in targetPath)
                {
                    getDirectPrerequisitesList(currentTargetList);
                }
                targetPath = new List<TargetLinkedList>(tempTargetPath);
                finalTargetPath.AddRange(targetPath);
                tempTargetPath.Clear();
            }

        }

        public bool isDone()
        {

            int length = visitedCourse.Count;
            int count = 0;


            foreach (KeyValuePair<int[], bool> entry in visitedCourse)
            {
                if (entry.Value) { count++; }
                // do something with entry.Value or entry.Key
            }

            if (count == length)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void updateTargetLinkedList(List<TargetLinkedList> targetList)
        {
            List<TargetLinkedList> tempTargetList = targetList;

            foreach (TargetLinkedList tList in targetList)
            {

                getDirectPrerequisitesList(tList);
            }

        }

        public void getDirectPrerequisitesList(TargetLinkedList targetLinkedList)
        {

            LinkedList<int[]> tLinkedList = targetLinkedList.TargetList;
            int[] currentHead = tLinkedList.First.Value;

            List<int[]> combinedGroupPrerequisites = joinPrereq(currentHead);
            visitedCourse[currentHead] = true;

            if (combinedGroupPrerequisites.Count == 0)
            {
                done = true;

            }
            else
            {
                foreach (int[] preArr in combinedGroupPrerequisites)
                {
                    LinkedList<int[]> newTLinkedList = new LinkedList<int[]>(tLinkedList);
                    newTLinkedList.AddFirst(preArr);
                    visitedCourse.Add(preArr, false);
                    tempTargetPath.Add(new TargetLinkedList(newTLinkedList));
                }

            }

        }

        public List<TargetLinkedList> getDirectPrerequisitesList2(TargetLinkedList targetLinkedList)
        {
            List<TargetLinkedList> tempPath = new List<TargetLinkedList>();
            Stack<TargetLinkedList> newStack = new Stack<TargetLinkedList>();
            newStack.Push(targetLinkedList);

            int[] currentHead = new int[] { };
            tempPath.Add(targetLinkedList);

            LinkedList<int[]> currentLinkedList = targetLinkedList.TargetList;
            List<TargetLinkedList> newPath = new List<TargetLinkedList>(tempPath);
            while (tempPath.Count != 0)
            {
                foreach (TargetLinkedList tl in newPath)

                {
                    currentLinkedList = newStack.Pop().TargetList;
                    currentHead = currentLinkedList.First.Value;

                    List<int[]> combinedGroupPrerequisites = joinPrereq(currentHead);
                    // tempPath = new List<TargetLinkedList>();

                    foreach (int[] preArr in combinedGroupPrerequisites)
                    {
                        LinkedList<int[]> newTLinkedList = new LinkedList<int[]>(currentLinkedList);
                        newTLinkedList.AddFirst(preArr);
                        //tempPath.Add(new TargetLinkedList(newTLinkedList));
                        newStack.Push(new TargetLinkedList(newTLinkedList));

                        //globalStack.Push(new TargetLinkedList(newTLinkedList));

                    }


                }



                newPath = new List<TargetLinkedList>(tempPath);
                String result = JsonConvert.SerializeObject(newPath);
                //  return result;
            }

            return newPath;

            // }

        }

        public List<int[]> joinPrereq(int[] currentPreArr)
        {
            List<int[]> CombinedPrerequisites = new List<int[]>();
            for (int i = 0; i < currentPreArr.Length; i++)
            {
                List<int[]> currentList = getCombinedPrerequisites(CombinedPrerequisites, currentPreArr[i]);
                globalList.Add(currentPreArr[i]);
                CombinedPrerequisites = new List<int[]>(currentList);
                // CombinedPrerequisites = getCombinedPrerequisites(CombinedPrerequisites, currentPreArr[i]);
            }
            String result = JsonConvert.SerializeObject(CombinedPrerequisites);

            return CombinedPrerequisites;
        }

        public List<int[]> getCombinedPrerequisites(List<int[]> currentList, int CourseId)
        {
            List<int[]> combinedList = new List<int[]>();
            List<int[]> newPrerequisiteList = getPrerequisite(CourseId);

            if (currentList.Count == 0)
            {
                return newPrerequisiteList;

            }
            else if (newPrerequisiteList.Count == 0)
            {
                return currentList;

            }
            else
            {
                for (int i = 0; i < currentList.Count; i++)
                {
                    for (int j = 0; j < newPrerequisiteList.Count; j++)
                    {
                        combinedList.Add((currentList[i].Union(newPrerequisiteList[j]).ToArray()));
                    }
                }

                String result = JsonConvert.SerializeObject(combinedList);
                return combinedList;
            }

        }


        //public List<int[]> getCombinedGroupPrerequisites(List<int[]> PrerequisitesArr)
        //    {

        //        foreach (int[] pre2 in PrerequisitesArr)
        //        {
        //            List<int[]> combinedGroupPrerequisites = new List<int[]>();

        //            for (int i = 0; i < pre2.Length; i++)
        //            {
        //                combinedGroupPrerequisites = getCombinedPrerequisites(combinedGroupPrerequisites, pre2[i]);
        //            }

        //        }

        //        //return combinedGroupPrerequisites;
        //    }


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


        public int[] getCourseGroup(int CourseId)
        {
            int[] GroupIds = VirtualAdvisor.Prerequisite.Where(p => p.CourseId == CourseId).Select(pre => pre.GroupId).Distinct().ToArray<int>();
            return GroupIds;
        }

        public List<int[]> getPrerequisite(int CourseId)
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


        public int[] getAllPrerequisite(int CourseId)
        {
            int[] Prerequisites = VirtualAdvisor.Prerequisite.Where(p => p.CourseId == CourseId).Select(r => r.PrerequisiteId).ToArray<int>();
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
            //courseNode.Prerequisites = getPrerequisite(CourseId);
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

        public void insertParameterSet(ParameterSet paramSet)
        {

        }

        public class ParameterSet
        {
            public int MajorId { get; set; }
            public int SchoolId { get; set; }
            public int[] TargetCourses { get; set; }
            public int[] CompletedCourses { get; set; }
            public int JobType { get; set; } 
            public int TimePreference { get; set; }
            public int[] PrimaryPurpose { get; set; }
            public int Budget { get; set; }
            public int Placements { get; set; }


            public ParameterSet(int MajorId, int SchoolId, int[] TargetCourses, int[] CompletedCourses, int JobType, int TimePreference, int[] PrimaryPurpose, int Budget, int Placements)
            {
                this.MajorId = MajorId;
                this.SchoolId = SchoolId;
                this.TargetCourses = TargetCourses;
                this.CompletedCourses = CompletedCourses;
                this.JobType = JobType;
                this.TimePreference = TimePreference;
                this.PrimaryPurpose = PrimaryPurpose;
                this.Budget = Budget;
                this.Placements = Placements;
            }

        }
    }



}

