using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Runtime.InteropServices;
using VirtualStudentAdviser.Models;

namespace VirtualStudentAdviser.Services
{
    public class VirtualAdviserRepository : IVirtualAdviserRepository
    {
        [DllImport("RecEngineCpp.dll", EntryPoint = "generate_plans", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern void generate_plans(string input, StringBuilder plans, int len);


        public List<SelectStudyPlan> launchEngine(int MajorId, int[] CompleteCourses, int SchoolId)
        {
            //Run phase 1
            int[] arr = new int[2] { 1, 2 };  //Completed courses
            string input = runRecommendationEngine(MajorId, CompleteCourses, SchoolId);
            File.WriteAllText("input.json", input);

            //Run phase 2 - Generate plans//
            int len = 100000;
            StringBuilder plans = new StringBuilder(len);
            plans.EnsureCapacity(len);
            generate_plans(input, plans, len);
            //return plans.ToString();

            //Phase 2b
            //Parse Json String Plan and store in the database
            ParseStudyPlan("Latest", plans.ToString());
            List<SelectStudyPlan> finalPlan =  getStudyPlan(planID);
            return finalPlan;

          

        }

        public VirtualAdviserRepository(VirtualAdviserContext VirtualAdvisor)
        {
            this.VirtualAdvisor = VirtualAdvisor;
        }

        private readonly VirtualAdviserContext VirtualAdvisor;          
        public int[] TargetCourses;   //list of target courses based on selected school and major
        public int CurrentCourse;       //course in the current iteration   
        Dictionary<int[], bool> visitedCourse = new Dictionary<int[], bool>(); //Variable to track visited courses in iterations   
        Dictionary<int, List<int[]>> globalCourseSchedule = new Dictionary<int, List<int[]>>();
        List<int> globalFloatingCourse = new List<int>();
        Dictionary<int, int[]> globalPostReq = new Dictionary<int, int[]>();
        Dictionary<int, int[]> postReq = new Dictionary<int, int[]>(); 
        public List<TargetLinkedList> targetPath = new List<TargetLinkedList>();
        public List<TargetLinkedList> tempTargetPath = new List<TargetLinkedList>();
        public List<TargetLinkedList> finalTargetPath = new List<TargetLinkedList>();
        public List<int> globalList = new List<int>();
        public Boolean done = false;
        public int planID = 0;
        public int parameterSetID; 
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

        public class LPInput
        {
            //public string CourseNumber;
            public List<int[]> CourseSchedule { get; set; }
            public int credits { get; set; }
            public int preReqConst { get; set; }
            public int taken { get; set; }
            public List<int[]> preReqs { get; set; }
            public int yearFilter { get; set; }

            public LPInput(List<int[]> CourseSchedule, int credits, int preReqConst, int taken, List<int[]> preReqs, int yearFilter)
            {
                this.CourseSchedule = CourseSchedule;
                this.credits = credits;
                this.preReqConst = preReqConst;
                this.taken = taken;
                this.preReqs = preReqs;
                this.yearFilter = yearFilter;

            }

        }

        public Dictionary<string, LPInput> LPgetAllCourseDetails()
        {

            List<int> courseList = new List<int>(globalList.Distinct());
            Dictionary<string, LPInput> courseDetails = new Dictionary<string, LPInput>();

            foreach (int CourseId in courseList)
            {
                string courseNumber = getCourseNumber(CourseId);
                List<int[]> CourseSchedule = getCourseSchedule(CourseId);
                int credits = getCourseCredit(CourseId); ;
                List<int[]> preReqs = getPrerequisite(CourseId);
                courseDetails.Add(courseNumber, new LPInput(CourseSchedule, credits, 0, 0, preReqs, 0));
            }

            return courseDetails;

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

        public string runRecommendationEngine(int MajorId, int[] CompletedCourses, int SchoolId)
        {
            String result = ""; 
            int DeptId = getDept(MajorId);
            TargetCourses = getTargetCourses(MajorId, SchoolId);
            int NumOfTargetCourses = TargetCourses.Length;

            Dictionary<int, List<TargetLinkedList>> TargetRequisites = new Dictionary<int, List<TargetLinkedList>>();

            insertParameterSet(MajorId, SchoolId, CompletedCourses);

            for (int i = 0; i < NumOfTargetCourses; i++)
            {
                CurrentCourse = TargetCourses[i];
                getTargetRequistes(CurrentCourse);
                refineTargetPath();
                TargetRequisites.Add(CurrentCourse, finalTargetPath);
                finalTargetPath = new List<TargetLinkedList>();
                visitedCourse = new Dictionary<int[], bool>();
            }

            //////// Section for the Algorithmic Approach
            getAllCourseSchedule();
            Object[] finalresponse = { TargetRequisites, globalCourseSchedule, globalFloatingCourse, postReq };
            result = JsonConvert.SerializeObject(finalresponse);
            return result;

            //////// Section for the Linear Programming Approach
            //Dictionary<string, LPInput> courseDetails = LPgetAllCourseDetails();
            //result = JsonConvert.SerializeObject(courseDetails);
            //return result;
        }

        //This removes preerquisite descripancies between two courses. Detailed explanation to be given later
        public void refineTargetPath()
        {
            foreach (TargetLinkedList tList in finalTargetPath)
            {
                List<int> newList = new List<int>();
                LinkedListNode<int[]> cNode = tList.TargetList.First;
                while (cNode != null)
                {
                    int[] cValue = cNode.Value;
                    int[] tempValue = cValue;
                    for (int i = 0; i < cValue.Length; i++)
                    {
                        //cValue.Where
                        //check if its in list
                        if (!newList.Contains(cValue[i]))
                        {
                            newList.Add(cValue[i]);
                        }
                        else
                        {
                            tempValue = tempValue.Where(val => val != cValue[i]).ToArray();
                        }

                        cNode.Value = tempValue;
                    }
                    cNode = cNode.Next;
                }

            }

        }

        public void sortPath()
        {
            //check for how to get the length of a linked list that counts 
            //each integer arrays in the nodes of the linked list.

            foreach (TargetLinkedList tll in finalTargetPath)
            {

                int[] total = { };
                LinkedList<int[]> nll = tll.TargetList;
                while (nll.Count != 0)
                {
                    total = total.Union(nll.First.Value).ToArray();
                    nll.RemoveFirst();
                }

                int count = total.Length;

            }

        }

        //public void getAllCourseSchedule()
        //{

        //    List<int> courseList = new List<int>(globalList.Distinct());

        //    foreach (int CourseId in courseList)
        //    {
        //        List<int[]> CourseSchedule = getCourseSchedule(CourseId);
        //        int[] floatingCourse = getAllPrerequisite(CourseId); 
        //        globalCourseSchedule.Add(CourseId, CourseSchedule);
        //        if (floatingCourse.Length == 0)
        //        {
        //            globalFloatingCourse.Add(CourseId);
        //        }
        //    }

        //}

        public void getAllCourseSchedule()
        {

            List<int> courseList = new List<int>(globalList.Distinct());

            foreach (int CourseId in courseList)
            {
                List<int[]> CourseSchedule = getCourseSchedule(CourseId);
                int[] floatingCourse = getAllPrerequisite(CourseId);
                globalPostReq.Add(CourseId, getPostrequisite(CourseId));
                globalCourseSchedule.Add(CourseId, CourseSchedule);
                if (floatingCourse.Length == 0)
                {
                    globalFloatingCourse.Add(CourseId);
                }
            }

            foreach (int CourseId in courseList)
            {
                postReq.Add(CourseId, getGlobalPostReq(CourseId));
            }

        }


        public int[] getGlobalPostReq(int CourseID)
        {

            //Dictionary<int, int[]> globalPostReq = new Dictionary<int, int[]>();
            //Dictionary<int, int[]> postReq = new Dictionary<int, int[]>();

            //combinedList.Add((currentList[i].Union(newPrerequisiteList[j]).ToArray()));
            //List<int[]> list = new List<int[]>(); 
            Stack<int[]> courseList = new Stack<int[]>();
            int[] postRequisites = globalPostReq[CourseID].ToArray();
            courseList.Push(postRequisites);
            //list.Add(postRequisites);

            while (courseList.Count != 0)
            {
                int[] currentPostReq = courseList.Pop();
                for (int i = 0; i < currentPostReq.Length; i++)
                {
                    if (globalPostReq.ContainsKey(currentPostReq[i]))
                    {
                        postRequisites.Union(globalPostReq[currentPostReq[i]]).ToArray();
                        courseList.Push(globalPostReq[currentPostReq[i]]);
                    }
                }

            }

            return postRequisites;

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

            while (!isDone() || done != true)
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


                    //while it's not empty, keep running 
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
            //String result = JsonConvert.SerializeObject(CombinedPrerequisites);

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
            int[] TargetCourses = VirtualAdvisor.AdmissionRequiredCourses.Where(a => a.MajorId == MajorId && a.SchoolId == SchoolId).Select(a => a.CourseId).ToArray<int>();
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

        public string getCourseNumber(int CourseId)
        {
            string courseNumber = VirtualAdvisor.Course.Where(c => c.CourseId == CourseId).Select(c => c.CourseNumber).FirstOrDefault().ToString();
            return courseNumber;
        }

        public int getCourseCredit(int CourseId)
        {
            int courseCredit = Convert.ToInt32(VirtualAdvisor.Course.Where(c => c.CourseId == CourseId).Select(c => c.MaxCredit).FirstOrDefault());
            return courseCredit;
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

      

        public List<SelectStudyPlan> getStudyPlan(int planId) {

            //        select a.PlanID, b.CourseNumber, c.Quarter, a.YearID
            //from StudyPlan a, Course b, Quarter c
            //where a.CourseID = b.CourseID and a.QuarterID = c.QuarterID and a.PlanID = 200
            //order by  a.YearID, a.QuarterID
 

            List<SelectStudyPlan> plan = new List<SelectStudyPlan>();
            var value = from sp in VirtualAdvisor.StudyPlan
                        where sp.PlanId == planId
                        join crs in VirtualAdvisor.Course on sp.CourseId equals crs.CourseId
                        join qt in VirtualAdvisor.Quarter on sp.QuarterId equals qt.QuarterId
                        orderby sp.YearId, sp.QuarterId
                        select new { sp.PlanId, crs.CourseNumber, qt.QuarterName, sp.YearId };
                       
            
                        //group new { studyPlan.PlanId, course.CourseNumber, quarter.QuarterName, studyPlan.YearId } by studyPlan.Id into SelectStudyPlan
                        //orderby studyPlan.YearId
                        // select SelectStudyPlan;

                        // var val = VirtualAdvisor.StudyPlan.Where(s => s.PlanId == planId).Join(VirtualAdvisor.Course).

            foreach (var sp in value)
            {
                plan.Add(new SelectStudyPlan(sp.PlanId, sp.CourseNumber.Trim(), sp.QuarterName.Trim(), sp.YearId));
            }

            return plan;
        }




        public int[] getCorequisite(int CourseId)
        {

            int[] getCorequisites = VirtualAdvisor.Prerequisite.Where(r => r.CourseId == CourseId).Select(r => r.PrerequisiteId).ToArray<int>();
            return getCorequisites;
        }

        public void removeCompletedCourses(Dictionary<int, List<CoursePreCorequisites>> TargetRequisites)
        {
            //write code here
        }

        public void insertParameterSet(int MajorId, int SchoolId, int[] CompletedCourses)
        {
            ParameterSet param = new ParameterSet(MajorId, SchoolId, JobTypeId: 1, BudgetId: 1, TimePreferenceId: 1, QuarterPreferenceId: 1,
                CompletedCourses: JsonConvert.SerializeObject(CompletedCourses), PlacementCourses: "Default placement");
            VirtualAdvisor.ParameterSet.Add(param);
            VirtualAdvisor.SaveChanges();
            parameterSetID = param.Id;

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


        public class Plans
        {
            public Quarters[] quarter { get; set; }

            public Plans(Quarters[] quarter)
            {
                this.quarter = quarter;
            }


        }
          
        public void ParseStudyPlan(string planName, string outputString)
        {
            // var json = System.IO.File.ReadAllText(@"C:\Users\CDLADMIN\Documents\Visual Studio 2015\Projects\Test\src\Test\output.json");
            //  var json = outputString;

            //public int insertPlan(string Name, int ParameterSetId, int Status, float Score)
            var Plans = JArray.Parse(outputString); // parse as array  

            foreach (JObject plan in Plans) //this could be for very JArray
            {
                float planScore = (float)plan["PlanScore"];
                planID = insertPlan(planName, parameterSetID, 1, planScore);
                insertStudentStudyPlan(456, planID, 1);

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
                            studyPlan.Add(new StudyPlan(planID, currentQuarter, currentYear, currentCourse, DateTime.Now, DateTime.Now));

                        }

                    }

                }
                insertStudyPlan(studyPlan);

            }
        }


        public int insertPlan(string Name, int ParameterSetId, int Status, float Score)
        {

            GeneratedPlan gPlan = new GeneratedPlan(Name, ParameterSetId, DateTime.Now, DateTime.Now, Status, Score);

            // var vsa = new VirtualAdviserContext();
            VirtualAdvisor.GeneratedPlan.Add(gPlan);
            VirtualAdvisor.SaveChanges();
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

            // var vsa = new VirtualAdviserContext();
            VirtualAdvisor.AddRange(newStudyPlan);
            VirtualAdvisor.SaveChanges();
        }

        public void insertStudentStudyPlan(int StudentId, int PlanId, int Status)
        {
            StudentStudyPlan sPlan = new StudentStudyPlan(StudentId, PlanId, DateTime.Now, DateTime.Now, Status);//(Name, ParameterSetId, DateTime.Now, DateTime.Now, Status);

            // var vsa = new VirtualAdviserContext();
            VirtualAdvisor.StudentStudyPlan.Add(sPlan);
            VirtualAdvisor.SaveChanges();

        }



    }


}
