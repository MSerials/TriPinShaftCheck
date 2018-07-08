using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThressPinShaft
{
    public partial class csv
    {
        private string file_name =  DateTime.Now.ToString("yyyy-MM-dd") + ".csv";
        public csv(string FileName = "")
        {
            if (FileName.Length > 0)
            {
                file_name = FileName;
            }
        }

       
    }
}
