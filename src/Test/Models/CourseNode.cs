using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Test 
{
    public class CourseNode
    {
        public int CourseId;
        public   List<int[]> CourseSchedule;
        public int[] Prerequisites;
        public int[] Corequisites;
    }
}
