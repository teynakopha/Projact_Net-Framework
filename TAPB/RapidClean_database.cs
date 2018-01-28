using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TAPB
{
    partial class RapidClean_database : ServiceBase
    {
        public RapidClean_database()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // TODO: Add code here to start your service.

        }
        public void Ondebug()
        {
            this.OnStart(null);
        }

        protected override void OnStop()
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.
            string strPath = AppDomain.CurrentDomain.BaseDirectory + "Log.txt";
            System.IO.File.AppendAllLines(strPath,
                new[] { "Stop time" + DateTime.Now.ToString() });
        }
        public void deleteEvent()
        {
            VC_DB mssql = new VC_DB();
            SQLiteOperation sqlite = new SQLiteOperation();
            int hour = 0;
            int minutes = 0; 
            string user = ""; // ลบ log จาก user
            int day = 24;  // เวลาลบลอกที่เก่าเกิน
            int interval = 24; // ชั้วโมง
           var reader =  sqlite.query("select * from Scheduler where sc_id=1");
            while (reader.Read())
            {
                user = reader[1].ToString();
                day = Convert.ToInt16(reader[3]);
                string Date1 = reader[2].ToString();
                // split time
                string[] a = Date1.Split(':');
                hour = Convert.ToInt16(a[0]);
                minutes = Convert.ToInt16(a[1]);
            }
            string strPath = AppDomain.CurrentDomain.BaseDirectory + "Log.txt";
            System.IO.File.AppendAllLines(strPath,
                new[] { "Start time" + DateTime.Now.ToString() });
            
            TaskScheduler.Instance.ScheduleTask(hour, minutes, interval,() =>
            {
            System.IO.File.AppendAllLines(strPath,
                new[] { "running now : " + DateTime.Now.ToString() });
                mssql.delete_event(day, user);
            });
        }
    }
}
