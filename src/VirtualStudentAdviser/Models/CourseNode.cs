using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VirtualStudentAdviser.Models
{
    public class CourseNode
    {
        public int CourseId;
        public   List<int[]> CourseSchedule;
        public int[] Prerequisites;
        public int[] Corequisites;
    }
}
