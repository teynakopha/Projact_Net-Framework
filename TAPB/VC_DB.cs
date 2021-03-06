﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;


namespace TAPB
{
    class VC_DB
    {
        string connectionString  = "Data Source=10.10.10.176;Initial Catalog=vcenterdb;User ID=sa;Password=P@ssw0rd";
        List<Int16> event_id = new List<Int16>();
        SqlConnection connection = new SqlConnection();
        WriteLog log = new WriteLog();
        public VC_DB()
        {
            
        }
        public void setConfigDB(string ip, string user, string password, string db)
        {

        }
        public SqlConnection connect()
        {
            SqlConnection conn = new SqlConnection(connectionString);
            try
            {
                conn.Open();
                connection = conn;

            }
            catch (SqlException e)
            {
                MessageBox.Show(e.ToString());
            }
            return conn;
        }
        public void setConnectionString(string _ip,string _user,string password,string dbname)
        {
            string _sql = string.Format("Data Source={0};Initial Catalog={1};User ID={2};Password={3}",
                _ip,dbname,_user,password);
            connectionString = _sql;
        }
        public string getConnectionString()
        {
            return connectionString;
        }
        public SqlDataReader query(string sql)
        {
            SqlConnection connection = connect();
            SqlCommand cmd = new SqlCommand(sql, connection);
            SqlDataReader reader = cmd.ExecuteReader();
            return reader;
        }
        public SqlDataReader getUser()
        {
            string sql = "select distinct username from vpx_event where username is not null";
            var reader = query(sql);
            log.writeLog("Get all user from vCenter db Complated");
            return reader;
        }
        public void close()
        {
            connection.Close();
        }
        public void delete_event(int _day,string _user)
        {
           var event_id = selectIDbyUser(_day, _user);
            deleteLog(event_id);
        }
        public void deleteLog(List<Int16> _event_id)
        {
            var conn = connect();
   
            foreach (int temp_id in _event_id)
            {
                    string vpx_delete = string.Format("delete from vpx_event where event_id = {0}", temp_id);
                    SqlCommand cmd = new SqlCommand(vpx_delete,conn);
                    cmd.ExecuteNonQuery();
                    string vpx_arg_delete = string.Format("delete from vpx_event_arg where event_id = {0}", temp_id);
                    log.writeLog(vpx_arg_delete);
                    cmd.CommandText = vpx_arg_delete;
                    cmd.ExecuteNonQuery();
            }
            conn.Close();
        }
        public List<Int16> selectIDbyUser(int daylog,string user)
        {
             var conn = connect();
            string sql =string.Format("select event_id from vpx_event where DATEDIFF(DAY, create_time AT TIME ZONE 'North Asia Standard Time', GETDATE() AT TIME ZONE 'North Asia Standard Time') >= {0} and USERNAME = '{1}'",daylog,user);
            SqlCommand command = new SqlCommand(sql, conn);
            MessageBox.Show(sql);
            SqlDataReader dr = command.ExecuteReader();
            while (dr.Read())
            {
                event_id.Add(Convert.ToInt16(dr[0]));
            }
            return event_id;
            conn.Close();
        }
        public void getDBlog(RichTextBox msg)
        {
            msg.Text += log.getLog();
        }
    }
}
