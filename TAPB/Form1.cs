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
        }
        public void connectDB()
        {

            try
            {
                NpgsqlConnection conn = new NpgsqlConnection("Server=10.10.10.222;User Id=postgres; " +
                "Password=QK5F#&2IH}dwmkjN;Database=VCDB;");
                richTextBox1.Text = "success";
                conn.Open();

                // Define a query returning a single row result set
                NpgsqlCommand command = new NpgsqlCommand("select event_id from vpx_event where username = 'root';", conn);
                MessageBox.Show(richTextBox2.Text);
                NpgsqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    eventid.Add(Convert.ToInt16(dr[0]));

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
            d();
        }
        private void d()
        {
            NpgsqlConnection conn = new NpgsqlConnection("Server=10.10.10.222;User Id=postgres; " +
"Password=QK5F#&2IH}dwmkjN;Database=VCDB;");
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
            saveconfig();
        }
    }
    public class SQLiteOperation
    {
        public SQLiteOperation()
        {
            if(checkFilesExit())
            {
                connectDB();
            }
            else
            {
                createDB();
                connectDB();
            }
        }
        // check db is exit if
        public bool checkFilesExit()
        {
            return true;
        }
        public SQLiteConnection connectDB()
        {
            SQLiteConnection m_dbConnection;
            m_dbConnection = new SQLiteConnection("Data Source=Config.db;Version=3;");
            return m_dbConnection;

        }
        private void createDB()
        {
            SQLiteConnection.CreateFile("Config.db");
        }
    }
}
