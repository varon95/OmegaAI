using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp5
{
    public class Field : Button
    {
        public int id { get; set; }
        //public int group { get; set; }

        //0 or null - empty, 1 white, 2 black
        public int color { get; set; }
        public int row { get; set; }
        public int column { get; set; }

        // for evaluation
        public int groupID { get; set; }
        public int originalID { get; set; }




    }
}
