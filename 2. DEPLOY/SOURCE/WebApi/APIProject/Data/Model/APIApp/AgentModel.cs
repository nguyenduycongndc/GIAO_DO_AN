using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class AgentModel
    {
        public string deviceID { get; set; }
        public double distance { get; set; }
        public int ID { get; set; }
    }
    public class AgentOutputModel {
        public int ID { get; set; }
        public string url { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public double rate { get; set; }
        public string Phone { get; set; }
    }
}
