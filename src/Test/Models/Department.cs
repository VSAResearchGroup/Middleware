using System;
using System.Collections.Generic;

namespace Test
{
    public partial class Department
    {
        public Department()
        {
            Major = new HashSet<Major>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime LastDateModified { get; set; }
        public int Status { get; set; }

        public virtual ICollection<Major> Major { get; set; }
    }
}
