using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp5
{
   public class Root
    {
        public byte RootId { get; set; }
        public byte RootSize { get; set; }
        // it is no longer root, good to know which is the new root ;-)
        public byte removedBy { get; set; }

    }
}
