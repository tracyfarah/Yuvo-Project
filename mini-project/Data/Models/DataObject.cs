using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mini_project.Data.Models
{
    public class DataObject
    {
        public DateTime Time { get; set; }
        public string Link { get; set; }
        public string Slot { get; set; }
        public string NeAlias { get; set; }
        public string NeType { get; set; }
        public double Max_RX_Level { get; set; }
        public double Max_TX_Level { get; set; }
        public double RSL_Deviation { get; set; }
    }
}
