using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgriManagement.tools
{
    static public class Logs
    {
        public static void WriteLine(string msg)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory.ToString() + "logs.txt";
            using (FileStream fs = new FileStream(path, FileMode.Append))
            {
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine(DateTime.Now.ToShortTimeString() + ":" + msg);
                sw.Flush();
                sw.Close();
            }
        }
    }
}
