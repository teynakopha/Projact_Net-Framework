using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using Npgsql;
using System.Data.SQLite;
using System.ServiceProcess;

namespace TAPB
{
    public partial class Form1 : Form
    {
        NpgsqlCommand deleteCMD;
        NpgsqlCommand deleteCMD2;
        string vpx_delete;
        string vpxarg_delete;
        List<int> eventid = new List<int>();
        public Form1()
        {
            InitializeComponent();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitiaForm();
        }
        public void InitiaForm()
        {
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "HH:mm";
            dateTimePicker1.ShowUpDown = true;
            setDefault_vCenterDB();
            checkBox_DBType.Checked = true;

           
        }
        public void setDefault_vCenterDB()
        {
           SQLiteOperation setDF = new SQLiteOperation();
           SQLiteDataReader reader =  setDF.query("select * from vCenterDB");
            while(reader.Read())
            {
                textBox_ServerIP.Text = reader[1].ToString();
                textBox_User.Text = reader[2].ToString();
                textBox_Password.Text = reader[3].ToString();
                textBox_dbname.Text = reader[4].ToString();
            }
            DateTime time = DateTime.Now;

            textBox_ServerIP.ReadOnly = true;
            textBox_User.ReadOnly = true;
            textBox_Password.ReadOnly = true;
            textBox_dbname.ReadOnly = true;
            setDF.getLogDB(richTextBox_log);

        }
        public void connectDB()
        {
            try
            {
                string string_conn = string.Format("Server={0};User Id={1}; Password={2};Database=VCDB;",textBox_ServerIP.Text,
                    textBox_User.Text,textBox_Password.Text);
                MessageBox.Show(string_conn);
                NpgsqlConnection conn = new NpgsqlConnection(string_conn);
                richTextBox_log.Text = "success";
                conn.Open();

                // Define a query returning a single row result set
                NpgsqlCommand command = new NpgsqlCommand("select distinct username from vpx_event where username is not null;", conn);
                MessageBox.Show(richTextBox2.Text);
                NpgsqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    comboBox_user.Items.Add(dr[0]);
                }
                conn.Close();
            }
            catch (Exception e)
            {
                richTextBox_log.Text = e.ToString();
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            connectDB();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            TaskScheduler.Instance.ScheduleTask(16, 12, 0.00417,
                    () =>
                            {
                            InvokeUI(() => {
                                richTextBox_log.Text += "Task 1 : " + DateTime.Now + "\n";
                            });
                 });
        }
        private void InvokeUI(Action a)
        {
            this.BeginInvoke(new MethodInvoker(a));
        }
        private void retire_select()
        {
            string string_conn = string.Format("Server={0};User Id={1}; Password={2};Database=VCDB;", textBox_ServerIP.Text,
    textBox_User.Text, textBox_Password.Text);
            NpgsqlConnection conn = new NpgsqlConnection(string_conn);
            string sql = string.Format("select event_id from vpx_event where  (NOW() AT TIME ZONE 'Asia/bangkok')::date - (create_time AT TIME ZONE 'Asia/bangkok')::date as diff " +
                "and username ='{0}'",comboBox_user.Text);
            NpgsqlCommand command = new NpgsqlCommand("select distinct username from vpx_event;", conn);
            MessageBox.Show(richTextBox2.Text);
            NpgsqlDataReader dr = command.ExecuteReader();
            while (dr.Read())
            {
                eventid.Add(Convert.ToInt16(dr[0]));

            }
            conn.Close();
        }
        private void delete()
        {
            string string_conn = string.Format("Server={0};User Id={1}; Password={2};Database=VCDB;", textBox_ServerIP.Text,
                textBox_User.Text, textBox_Password.Text);
            MessageBox.Show(string_conn);
            NpgsqlConnection conn = new NpgsqlConnection(string_conn);
            conn.Open();
            foreach (int a in eventid)
            {
                richTextBox_log.Text += " delete id :" + a + "\n";
                vpx_delete = string.Format("delete from vpx_event where event_id = {0}", a);
                NpgsqlCommand command = new NpgsqlCommand(vpx_delete, conn);
                command.ExecuteNonQuery();
                command.CommandText = string.Format("delete from vpx_event_arg where event_id = {0}", a);
                command.ExecuteNonQuery();
            }

            conn.Close();

        }

        private void label4_Click(object sender, EventArgs e)
        {
            Console.Write("df");
        }

        // db config sqlite 
        public void saveconfig()
        {
            SQLiteConnection.CreateFile("Config.db");
            SQLiteConnection m_dbConnection;
            m_dbConnection = new SQLiteConnection("Data Source=Config.sqlite;Version=3;");
            m_dbConnection.Open();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            

        }
        private void createService(object data)
        {
#if DEBUG
            RapidClean_database myservice = new RapidClean_database();
            myservice.deleteEvent();
            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
#else
            ServiceBase[] ServiceToRun;
            ServiceToRun = new ServiceBase[] {
                new RapidClean_database()
            };
            ServiceBase.Run(ServiceToRun);
#endif
        }

        private void button_testconnect_Click(object sender, EventArgs e)
        {
            if(checkBox_DBType.Checked == true)
            {
                VC_DB conn = new VC_DB();
                try
                {
                    conn.connect();
                    var reader = conn.getUser();
                    while (reader.Read())
                    {
                        comboBox_user.Items.Add(reader[0]);
                    }
                    MessageBox.Show("Connect to Database is Complated");
                    conn.getDBlog(richTextBox_log);
                }
                catch (Exception g)
                {
                    MessageBox.Show(g.ToString());
                }
            }
            //update database
            //if (checkBox1.Checked = true)
            //{
            //    string vcenter_ip = textBox_ServerIP.Text;
            //    string vcenter_user = textBox_User.Text;
            //    string vcenter_password = textBox_Password.Text;
            //    string vcenter_port = textBox_port.Text;
            //    string sql = string.Format("update vCenterDB set vcenter_ip='{0}',vcenter_user='{1}'," +
            //        "vcenter_password='{2}',vcenter_port='{3}' where id =1", vcenter_ip, vcenter_user, vcenter_password, vcenter_port);
            //    SQLiteOperation update = new SQLiteOperation();
            //    update.query(sql);
            //    SQLiteOperation setDF = new SQLiteOperation();
            //    SQLiteDataReader reader = setDF.query("select * from vCenterDB");

            //    while (reader.Read())
            //    {
            //        textBox_ServerIP.Text = reader[1].ToString();
            //        textBox_User.Text = reader[2].ToString();
            //        textBox_Password.Text = reader[3].ToString();
            //        textBox_port.Text = reader[4].ToString();
            //    }
            //    textBox_ServerIP.ReadOnly = true;
            //    textBox_User.ReadOnly = true;
            //    textBox_Password.ReadOnly = true;
            //    textBox_port.ReadOnly = true;
            //    checkBox1.Checked = false;
            //    connectDB();
            //}
            //else
            //{
            //    connectDB();
            //}
        }
        private void MethodeTest()
        {
            TaskScheduler.Instance.ScheduleTask(16, 23, 1.0,
        () =>
        {
            InvokeUI(() => {
                richTextBox_log.Text += "Task 1 : " + DateTime.Now + "\n";
            });
        });
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                textBox_ServerIP.ReadOnly = false;
                textBox_User.ReadOnly = false;
                textBox_Password.ReadOnly = false;
                textBox_dbname.ReadOnly = false;

            }
            else
            {
                textBox_ServerIP.ReadOnly = true;
                textBox_User.ReadOnly = true;
                textBox_Password.ReadOnly = true;
                textBox_dbname.ReadOnly = true;
            }
        }
        public void Test()
        {
            MessageBox.Show("f");
        }

        private void button_save_Click(object sender, EventArgs e)
        {
            SQLiteOperation conn = new SQLiteOperation();
            var check = true;
            string user = comboBox_user.Text;
            
            string time = dateTimePicker1.Text;
            int autorun_check;
            if (checkBox_autorun.Checked)
            {
                autorun_check = 1;
            }
            else
                autorun_check = 0;
            
            int day = Convert.ToInt16(comboBox_Day.Text);
            if (check)
            {
                
                string sql = string.Format("update Scheduler set sc_user = '{0}',sc_time='{1}',sc_day = {2},sc_autorun={3} where sc_id =1 ;",
                    user,time,day,autorun_check);
                conn.query(sql);
                richTextBox_log.Text += DateTime.Now.ToString() + " : update job scheduler is complated \n";
                MessageBox.Show("update is complated");
            }
            else
            {
                MessageBox.Show("Task insert db");
                string sql = string.Format("insert into Scheduler(sc_user,sc_time,sc_day,sc_autorun) values('{0}','{1}',{2},{3});", user, time, day,autorun_check);
                conn.query(sql);
                
            }

        }

        private void button_SaveIP_Click(object sender, EventArgs e)
        {
           string server_ip =  textBox_ServerIP.Text;
            string user = textBox_User.Text;
            string password = textBox_Password.Text;
            string dbname = textBox_dbname.Text;
            SQLiteOperation db = new SQLiteOperation();
            db.updateIP_DB(server_ip,user,password,dbname);
            checkBox1.Checked = false;
            db.getLogDB(richTextBox_log);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Thread newThread = new Thread(createService);
            newThread.Start();
        }
    }
    public class SQLiteOperation
    {
        WriteLog log = new WriteLog();
        string locationDatabase = String.Format("{0}\\Config.db", System.Environment.CurrentDirectory);
        public SQLiteOperation()
        {
            if(checkFilesExit())
            {
                try
                {
                    connectDB();
                }
                catch(SQLiteException e)
                {
                    MessageBox.Show(e.ToString());
                } 
            }
            else
            {
                try
                {
                    createDB();
                    connectDB();
                }
                catch (SQLiteException e)
                {
                    MessageBox.Show(e.ToString());
                }
            }
        }
        // check db is exit if
        public bool checkFilesExit()
        {
            if(System.IO.File.Exists(locationDatabase))
            {
                return true;
            }
            else
            {
                return false; 
            }
        }
        private SQLiteConnection connectDB()
        {
            SQLiteConnection m_dbConnection;
            m_dbConnection = new SQLiteConnection("Data Source=Config.db;Version=3;");
            m_dbConnection.Open();
            return m_dbConnection;

        }
        private void createDB()
        {
            SQLiteConnection.CreateFile(locationDatabase);
            createTable();
            insertDefault();
        }

        private void createTable()
        {
            string sql = "create table vCenterDB (id integer primary key autoincrement,vcenter_ip varchar(20),vcenter_user varchar(20),vcenter_password " +
                " varchar(20),vcenter_dbname varchar(20))";
            string sqlScheduler = "create table Scheduler(sc_id integer primary key autoincrement,sc_user vachar(40),sc_time varchar(20),sc_day integer,sc_autorun bool)";
            try
            {
                SQLiteConnection connection = connectDB();
                SQLiteCommand command = new SQLiteCommand(sql, connection);
                command.ExecuteNonQuery();
                command.CommandText = sqlScheduler;
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (SQLiteException e)
            {
                MessageBox.Show(e.ToString());
            }

        }
        private void insertDefault()
        {
            try
            {
                string sql = "insert into vCenterDB(vcenter_ip," +
        "vcenter_user,vcenter_password,vcenter_dbname) values('10.10.10.176','sa','P@ssw0rd','vcenterdb'); ";
                SQLiteConnection connection = connectDB();
                SQLiteCommand cmd = new SQLiteCommand(sql, connection);
                cmd.ExecuteNonQuery();
                connection.Close();
            }
            catch (SQLiteException e)
            {
                MessageBox.Show(e.ToString());
            }

        }
        public void updateIP_DB(string _ip,string user,string password,string dbname)
        {
            SQLiteConnection conn = connectDB();
            string sql = string.Format("update vCenterDB set vcenter_ip = '{0}',vcenter_user = '{1}'," +
                "vcenter_password = '{2}',vcenter_dbname='{3}' where id = 1;", _ip, user, password, dbname);
            SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            cmd.ExecuteNonQuery();
            log.writeLog("update vcenter db complated");
            conn.Close();
        }
        public SQLiteDataReader query(string sql)
        {
            SQLiteConnection connection = connectDB();

            SQLiteCommand cmd = new SQLiteCommand(sql, connection);
            SQLiteDataReader reader = cmd.ExecuteReader();
            return reader;
            connection.Close();

        }
        public string getLocationDB()
        {
            return locationDatabase;
        }

        public void connect_getuserToForm()
        {

        }
        public void getLogDB(RichTextBox msg)
        {
            msg.Text = log.getLog();
        }

    }
    public class TaskScheduler
    {
        private static TaskScheduler _instance;
        private List<System.Threading.Timer> timers = new List<System.Threading.Timer>();

        private TaskScheduler() { }

        public static TaskScheduler Instance => _instance ?? (_instance = new TaskScheduler());

        public void ScheduleTask(int hour, int min, double intervalInHour, Action task)
        {
            DateTime now = DateTime.Now;
            DateTime firstRun = new DateTime(now.Year, now.Month, now.Day, hour, min, 0, 0);
            if (now > firstRun)
            {
                firstRun = firstRun.AddDays(1);
            }

            TimeSpan timeToGo = firstRun - now;
            if (timeToGo <= TimeSpan.Zero)
            {
                timeToGo = TimeSpan.Zero;
            }

            var timer = new System.Threading.Timer(x =>{task.Invoke();}, null, timeToGo, TimeSpan.FromHours(intervalInHour));

            timers.Add(timer);
        }
    }

}
