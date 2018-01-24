using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;
using System.Data.SQLite;
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
            dateTimePicker1.Format = DateTimePickerFormat.Time;
            dateTimePicker1.ShowUpDown = true;
            setDefault_vCenterDB();

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
                textBox_port.Text = reader[4].ToString();
            }
            textBox_ServerIP.ReadOnly = true;
            textBox_User.ReadOnly = true;
            textBox_Password.ReadOnly = true;
            textBox_port.ReadOnly = true;

        }
        public void connectDB()
        {

            try
            {
                string string_conn = string.Format("Server={0};User Id={1}; Password={2};Database=VCDB;",textBox_ServerIP.Text,
                    textBox_User.Text,textBox_Password.Text);
                MessageBox.Show(string_conn);
                NpgsqlConnection conn = new NpgsqlConnection(string_conn);
                richTextBox1.Text = "success";
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
                richTextBox1.Text = e.ToString();
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            connectDB();
        }

        private void button2_Click(object sender, EventArgs e)
        {

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
                richTextBox1.Text += " delete id :" + a + "\n";
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
            SQLiteOperation conn = new SQLiteOperation();
        }

        private void button_testconnect_Click(object sender, EventArgs e)
        {
            //update database 
            if(checkBox1.Checked = true)
            {
                string vcenter_ip = textBox_ServerIP.Text;
                string vcenter_user = textBox_User.Text;
                string vcenter_password = textBox_Password.Text;
                string vcenter_port = textBox_port.Text;
                string sql = string.Format("update vCenterDB set vcenter_ip='{0}',vcenter_user='{1}'," +
                    "vcenter_password='{2}',vcenter_port='{3}' where id =1",vcenter_ip,vcenter_user,vcenter_password,vcenter_port);
                SQLiteOperation update = new SQLiteOperation();
                update.query(sql);
                SQLiteOperation setDF = new SQLiteOperation();
                SQLiteDataReader reader = setDF.query("select * from vCenterDB");
                
                while (reader.Read())
                {
                    textBox_ServerIP.Text = reader[1].ToString();
                    textBox_User.Text = reader[2].ToString();
                    textBox_Password.Text = reader[3].ToString();
                    textBox_port.Text = reader[4].ToString();
                }
                textBox_ServerIP.ReadOnly = true;
                textBox_User.ReadOnly = true;
                textBox_Password.ReadOnly = true;
                textBox_port.ReadOnly = true;
                checkBox1.Checked = false;
                connectDB();
            }
            else
            {
                connectDB();
            }
            
            
        }
        
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                textBox_ServerIP.ReadOnly = false;
                textBox_User.ReadOnly = false;
                textBox_Password.ReadOnly = false;
                textBox_port.ReadOnly = false;

            }
            else
            {
                textBox_ServerIP.ReadOnly = true;
                textBox_User.ReadOnly = true;
                textBox_Password.ReadOnly = true;
                textBox_port.ReadOnly = true;
            }
        }
    }
    public class SQLiteOperation
    {
        string locationDatabase = String.Format("{0}\\Config.db", System.Environment.CurrentDirectory);
        string log = "";
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
                " varchar(20),vcenter_port int)";
            try
            {
                SQLiteConnection connection = connectDB();
                SQLiteCommand command = new SQLiteCommand(sql, connection);
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
        "vcenter_user,vcenter_password,vcenter_port) values('10.10.10.222','postgres','QK5F#&2IH}dwmkjN',5432); ";
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
        public SQLiteDataReader query(string sql)
        {
            SQLiteConnection connection =  connectDB();
            SQLiteCommand cmd = new SQLiteCommand(sql, connection);
            SQLiteDataReader reader = cmd.ExecuteReader();
            return reader;
            connection.Close();
        }
        public string getLocationDB()
        {
            return locationDatabase;
        }
    }
}
