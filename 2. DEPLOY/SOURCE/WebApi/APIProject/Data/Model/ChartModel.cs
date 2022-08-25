using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model
{
    public class ChartModel
    {
        public int Max_H_axis { get; set; }
        public int Max_V_axis { get; set; }
        public List<string> List_H_axis { get; set; }
        public List<int> List_V_axis { get; set; }
    }
    public class ChartServiceModel
    {
        public int Max_H_axis { get; set; }
        public int Max_V_axis { get; set; }
        public List<int> List_V_axis_gold { get; set; }
        public List<int> List_V_axis_sliver { get; set; }
        public List<int> List_V_axis_diamon { get; set; }
        public List<int> List_V_axis_combo { get; set; }
        public List<string> List_H_axis { get; set; }
    }
    public class ChartComboServer {
        public List<int> diamon { get; set; }
        public List<int> sliver { get; set; }
    }
    public class ChartComboModel {
        public ChartComboServer week1 { get; set; }
        public ChartComboServer week2 { get; set; }
        public ChartComboServer week3 { get; set; }
        public ChartComboServer week4 { get; set; }
    }
}
