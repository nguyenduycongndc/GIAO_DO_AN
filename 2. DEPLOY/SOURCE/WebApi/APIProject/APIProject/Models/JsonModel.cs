using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIProject.Models
{
    public class JsonModel
    {
        public int Status { get; set; }
        public int Code { get; set; }
        public object Data { get; set; }
        public string Message { get; set; }
    }
}