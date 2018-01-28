using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TAPB
{
    class WriteLog
    {
        string log = "";
        DateTime time = DateTime.Now;
        public void writeLog(string message)
        {
            log += time.ToString() + ": " + message + "\n";
        }
        public void writeLog(string message,RichTextBox msg)
        {
            log += time.ToString() + ": "+ message + "\n";
            msg.Text += log;
        }
        public string getLog()
        {
            return this.log;
        }
    }
}
