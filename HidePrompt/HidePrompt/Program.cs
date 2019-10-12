using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace HidePrompt
{
    class Program
    {
        static void Main(string[] args)
        {
            Process proStart = new Process();
            proStart.StartInfo.UseShellExecute = false;
            proStart.StartInfo.CreateNoWindow = true;
            proStart.StartInfo.FileName = "uninstall.exe"; ;
            proStart.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proStart.Start();
          
        }
    }
}
