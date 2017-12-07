using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VirtualStudentAdviser.Models
{
    public class PlanInfo
    {
        public ParameterSet parameterSet { get; set; }
        public string planName { get; set; }
        public int studentId { get; set; }
        public int planId {get; set; }
         

        public PlanInfo(ParameterSet ps, string planName, int studentId, int planId)
        {
            this.parameterSet = ps;
            this.planName = planName;
            this.studentId = studentId;
            this.planId = planId;
        }

        public PlanInfo()
        {
            this.parameterSet = new ParameterSet();
            
        }

      
    }
}
