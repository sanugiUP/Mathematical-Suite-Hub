using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistryAPIClasses
{
    public class ServiceDescription
    {
        public string name { get; set; }
        public string description { get; set; }
        public string end_point_API { get; set; }
        public int operands { get; set; }
        public string operandType { get; set; }

    }
}
