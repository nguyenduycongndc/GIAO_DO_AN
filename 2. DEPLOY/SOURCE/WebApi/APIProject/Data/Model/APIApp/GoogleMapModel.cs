using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class GoogleMapModel
    {
        
    }
    public class Waypoint
    {
        public string place_id { get; set; }
    }
    public class WaypointDetail
    {
       
    }
    public class Map
    {
        public List<Waypoint> geocoded_waypoints { get; set; }
        public List<Routes> routes { get; set; }
    }
    public class GoogleMapInputModel
    {
        public string Origin { get; set; }
        public string Destination { get; set; }
    }
    public class Routes
    {
        public List<Legs> legs { get; set; }
    }
    public class Legs
    {
        public Distance distance { get; set; }
        public Duration duration { get; set; }
        public string start_address { get; set; }
        public string end_address { get; set; }


    }
    public class Distance
    {
        public string Text { get; set; }
        public int Value { get; set; }
    }
    public class Duration
    {
        public string Text { get; set; }
        public int Value { get; set; }
    }
}
