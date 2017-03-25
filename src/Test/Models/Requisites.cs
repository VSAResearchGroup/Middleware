using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Test
{
    public class Requisites
    {

        public int Id { get; set; }
        public int CourseId { get; set; }
        public int RequisiteId { get; set; }
        public int Relationship { get; set; }
        public int DepartmentId { get; set; }
        public int Status { get; set; }
        public int UserID { get; set; }
        public DateTime LastDateModified { get; set; } 
        public int PrerequisiteId { get; set; }


        public virtual Course Course { get; set; }
         
}
}
