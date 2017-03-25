using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Test 
{
    public partial class Prerequisite
    { 
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int GroupId { get; set; }
        public int PrerequisiteId { get; set; }

        public virtual Course Course { get; set; } 
         

    }

  

}
 