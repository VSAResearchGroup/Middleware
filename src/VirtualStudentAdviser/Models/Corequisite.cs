using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VirtualStudentAdviser.Models
{ 
        public partial class Corequisite
        {
            public int Id { get; set; }
            public int CourseId { get; set; }
            public int GroupId { get; set; }
            public int CorequisiteId { get; set; }

            public virtual Course Course { get; set; }
         
    }
}
