using System;
using System.Collections.Generic;

namespace VirtualStudentAdviser.Models
{
    public partial class Major
    {
        public Major()
        {
            Student = new HashSet<Student>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int DepartmentId { get; set; }
        public string DegreeType { get; set; }
        public int Status { get; set; }

        public virtual ICollection<Student> Student { get; set; }
        public virtual Department Department { get; set; }
    }
}
