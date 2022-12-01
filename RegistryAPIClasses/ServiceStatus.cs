using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistryAPIClasses
{
    public class ServiceStatus
    {
        public string status { get; set; }
        public string reason { get; set; }
        public List<ServiceDescription> serviceDescriptions { get; set; }
    }
}
