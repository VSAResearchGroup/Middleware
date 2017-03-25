using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Test
{
    public partial class GeneratedPlan
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ParameterSetId { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime LastDateModified { get; set; }
        public int Status { get; set; }

        public GeneratedPlan(string Name, int ParameterSetId, DateTime DateAdded, DateTime LastDateModified, int Status)
        {
            this.Name = Name;
            this.ParameterSetId = ParameterSetId;
            this.DateAdded = DateAdded;
            this.LastDateModified = LastDateModified;
            this.Status = Status;
        }
    }
}
