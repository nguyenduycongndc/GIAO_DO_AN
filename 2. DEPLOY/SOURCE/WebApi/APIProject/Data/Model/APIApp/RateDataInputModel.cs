using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class RateDataInputModel
    {
        public int OrderID { get; set; }
        public float Rate { get; set; }
        public string NoteRate { get; set; }
        public int Type { get; set; }
    }
}
