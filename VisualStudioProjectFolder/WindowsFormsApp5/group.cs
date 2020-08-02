using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp5
{
    public class group
    {
       public int groupID { get; set; }
       public int groupValue { get; set; }
       public int color { get; set; }
       public List<Field> member { get; set; }
    }
}
