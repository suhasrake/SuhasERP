using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace CSLERP.DBData
{
    public class Leave
    {
        public string leaveID { get; set; }
        public string description { get; set; }
        public int MaxAccrual { get; set; }
        public int SanctionType { get; set; }
        public string designation { get; set; }
        public string officeID { get; set; }
        public string officeName { get; set; }
        public int MaxDays { get; set; }
        public string Gender { get; set; }
        public int rowid { get; set; }
        public int ahead { get; set; }
        public int Delay { get; set; }
        public int CarryForward { get; set; }

    }
    class LeaveSettingsdb
    {
        public List<Leave> getLeaveTypeList( )
        {
            Leave Alist;
            List<Leave> AccList = new List<Leave>();
            try
            {
                string query = "select LeaveID,Description,MaxAccrual,SanctionType,Gender,RowID, "+
                              " DaysAhead,DaysDelay,Carryforward from LeaveType  "; 
                SqlConnection conn = new SqlConnection(Login.connString);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Alist = new Leave();
                    Alist.leaveID = reader.GetString(0);
                    Alist.description = reader.GetString(1);
                    Alist.MaxAccrual =reader.GetInt32(2);
                    Alist.SanctionType =reader.GetInt32(3);
                    Alist.Gender = reader.GetString(4);
                    Alist.rowid = reader.GetInt32(5);
                    Alist.ahead = reader.GetInt32(6);
                    Alist.Delay = reader.GetInt32(7);
                    Alist.CarryForward = reader.GetInt32(8);
                    AccList.Add(Alist);
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(System.Reflection.MethodBase.GetCurrentMethod().Name + "() : Error");
            }
            return AccList;
        }
        public List<Leave> getSanctionLimitList()
        {
            Leave Alist;
            List<Leave> AccList = new List<Leave>();
            try
            {
                string query = "select LeaveID,Designation,MaxSanctionLimit,RowID from LeaveSanctionlimit ";
                SqlConnection conn = new SqlConnection(Login.connString);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Alist = new Leave();
                    Alist.leaveID = reader.GetString(0);
                    Alist.designation = reader.GetString(1);
                    Alist.MaxAccrual = reader.GetInt32(2);
                    Alist.rowid = reader.GetInt32(3);
                    AccList.Add(Alist);
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(System.Reflection.MethodBase.GetCurrentMethod().Name + "() : Error");
            }
            return AccList;
        }
        public List<Leave> getleaveofficemappingList()
        {
            Leave Alist;
            List<Leave> AccList = new List<Leave>();
            try
            {
                string query = "select a.LeaveID,a.OfficeID,a.MaxDays,a.RowID,b.Name from LeaveOfficeMapping a, Office b where a.OfficeID=b.officeID";
                SqlConnection conn = new SqlConnection(Login.connString);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Alist = new Leave();
                    Alist.leaveID = reader.GetString(0);
                    Alist.officeID = reader.GetString(1);
                    Alist.MaxDays = reader.GetInt32(2);
                    Alist.rowid = reader.GetInt32(3);
                    Alist.officeName = reader.GetString(4);
                    AccList.Add(Alist);
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(System.Reflection.MethodBase.GetCurrentMethod().Name + "() : Error");
            }
            return AccList;
        }
       
        public Boolean UpdateLeaveType(Leave leavesett)
        {
            Boolean status = true;
            string utString = "";
            try
            {
                string updateSQL = "Update LeaveType set Description='"+leavesett.description+"',SanctionType='"+leavesett.SanctionType+"',"+
                    " MaxAccrual='"+leavesett.MaxAccrual+"',Gender='"+leavesett.Gender+ "',DaysAhead='"+leavesett.ahead+"' , " +
                    " DaysDelay ='"+leavesett.Delay+"',Carryforward='"+leavesett.CarryForward+"' where LeaveID='" + leavesett.leaveID+"'";
                utString = utString + updateSQL + Main.QueryDelimiter;
                utString = utString +
                ActivityLogDB.PrepareActivityLogQquerString("update", "LeaveType", "", updateSQL) +
                Main.QueryDelimiter;

                if (!UpdateTable.UT(utString))
                {
                    status = false;
                }
            }
            catch (Exception)
            {
                MessageBox.Show(this.ToString() + "-" + System.Reflection.MethodBase.GetCurrentMethod().Name + "() : Error");
                status = false;
            }
            return status;
        }
        public Boolean UpdateLeaveSanctionLimit(Leave leavesett)
        {
            Boolean status = true;
            string utString = "";
            try
            {
                string updateSQL = "Update LeaveSanctionLimit set MaxSanctionLimit='"+leavesett.MaxAccrual+"',Designation='"+leavesett.designation+"' where LeaveID='" + leavesett.leaveID + "' and RowID='"+leavesett.rowid+"'";
                utString = utString + updateSQL + Main.QueryDelimiter;
                utString = utString +
                ActivityLogDB.PrepareActivityLogQquerString("update", "LeaveSanctionLimit", "", updateSQL) +
                Main.QueryDelimiter;

                if (!UpdateTable.UT(utString))
                {
                    status = false;
                }
            }
            catch (Exception)
            {
                MessageBox.Show(this.ToString() + "-" + System.Reflection.MethodBase.GetCurrentMethod().Name + "() : Error");
                status = false;
            }
            return status;
        }

        public Boolean UpdateLeaveOfficeMapping(Leave leavesett)
        {
            Boolean status = true;
            string utString = "";
            try
            {
                string updateSQL = "Update LeaveOfficeMapping set MaxDays='"+leavesett.MaxDays+"',OfficeID='"+leavesett.officeID+"' where LeaveID='" + leavesett.leaveID + "'and RowID='"+leavesett.rowid+"'";
                utString = utString + updateSQL + Main.QueryDelimiter;
                utString = utString +
                ActivityLogDB.PrepareActivityLogQquerString("update", "LeaveSanctionLimit", "", updateSQL) +
                Main.QueryDelimiter;
                if (!UpdateTable.UT(utString))
                {
                    status = false;
                }
            }
            catch (Exception)
            {
                MessageBox.Show(this.ToString() + "-" + System.Reflection.MethodBase.GetCurrentMethod().Name + "() : Error");
                status = false;
            }
            return status;
        }

        public Boolean InsertLeaveType(Leave leave)
        {
            Boolean status = true;
            string utString = "";
            try
            {

                string updateSQL = "insert into LeaveType (LeaveID,Description,MaxAccrual,Gender,SanctionType,DaysAhead,DaysDelay,Carryforward)" +
                    "values (" +
                    "'" + leave.leaveID + "'," +
                     "'" + leave.description + "','" +
                    leave.MaxAccrual + "','" +
                     leave.Gender + "','" +
                     leave.SanctionType + "',"+
                    " '"+leave.ahead+"',"+
                     " '" + leave.Delay + "',"+
                     " '"+leave.CarryForward+"')";
                utString = utString + updateSQL + Main.QueryDelimiter;
                utString = utString +
                  ActivityLogDB.PrepareActivityLogQquerString("insert", "LeaveType", "", updateSQL) +
                  Main.QueryDelimiter;
                if (!UpdateTable.UT(utString))
                {
                    status = false;
                }
            }
            catch (Exception)
            {
                MessageBox.Show(this.ToString() + "-" + System.Reflection.MethodBase.GetCurrentMethod().Name + "() : Error");
                status = false;
            }
            return status;
        }
        public Boolean InsertLeaveSanctionLimit(Leave leave)
        {
            Boolean status = true;
            string utString = "";
            try
            {

                string updateSQL = "insert into LeaveSanctionLimit (LeaveID,Designation,MaxSanctionLimit)" +
                    "values (" +
                    "'" + leave.leaveID + "'," +
                     "'" + leave.designation + "','" +
                    leave.MaxAccrual + "')";
                utString = utString + updateSQL + Main.QueryDelimiter;
                utString = utString +
                  ActivityLogDB.PrepareActivityLogQquerString("insert", "LeaveSanctionLimit", "", updateSQL) +
                  Main.QueryDelimiter;
                if (!UpdateTable.UT(utString))
                {
                    status = false;
                }
            }
            catch (Exception)
            {
                MessageBox.Show(this.ToString() + "-" + System.Reflection.MethodBase.GetCurrentMethod().Name + "() : Error");
                status = false;
            }
            return status;
        }

        public Boolean InsertLeaveofficeMapping(Leave leave)
        {
            Boolean status = true;
            string utString = "";
            try
            {

                string updateSQL = "insert into LeaveOfficeMapping (LeaveID,OfficeID,MaxDays)" +
                    "values (" +
                    "'" + leave.leaveID + "'," +
                     "'" + leave.officeID + "','" +
                    leave.MaxDays +  "')";
                utString = utString + updateSQL + Main.QueryDelimiter;
                utString = utString +
                  ActivityLogDB.PrepareActivityLogQquerString("insert", "LeaveOfficeMapping", "", updateSQL) +
                  Main.QueryDelimiter;
                if (!UpdateTable.UT(utString))
                {
                    status = false;
                }
            }
            catch (Exception)
            {
                MessageBox.Show(this.ToString() + "-" + System.Reflection.MethodBase.GetCurrentMethod().Name + "() : Error");
                status = false;
            }
            return status;
        }
        public Boolean ValidateLeaveType(Leave leave)
        {
            Regex r = new Regex("[a-zA-Z]");
            Boolean stat = true;
            try
            {
                if(leave.leaveID=="" || leave.leaveID.Trim().Length==0)
                {
                    stat = false;
                }
                if(leave.description=="" || leave.description.Trim().Length==0)
                {
                    stat = false;
                }
                if (leave.MaxAccrual <= 0 || r.IsMatch(leave.ahead.ToString()))
                {
                    stat = false;
                }
                if (leave.SanctionType == 0 )
                {
                    stat = false;
                }
                if(leave.Gender == "" || leave.Gender.Trim().Length == 0)
                {
                    stat = false;
                }
                if(leave.ahead <=0 || r.IsMatch(leave.ahead.ToString())  )
                {
                    stat = false;
                }
            }
            catch(Exception ex)
            {

            }
            return stat;  
        }

        public Boolean validateSanctionLimitList(Leave leave)
        {
            int count = 0;
            Boolean status = true;
            try
            {
                string query = "select LeaveID,Designation from LeaveSanctionlimit where LeaveID='"+leave.leaveID+"' and Designation='"+leave.designation+"'";
                SqlConnection conn = new SqlConnection(Login.connString);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if(reader.Read())
                {
                    status = false;
                }
               // count =(int) cmd.ExecuteScalar();
               //if(count > 0)
               // {
               //     status = false;
               // }
                conn.Close();
            }
            catch (Exception ex)
            {
                status = false;
            }
            return status;
        }

        public Boolean validateLeaveType(Leave leave)
        {
            int count = 0;
            Boolean status = true;
            try
            {
                string query = "select LeaveID from LeaveType where LeaveID='" + leave.leaveID + "'";
                SqlConnection conn = new SqlConnection(Login.connString);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    status = false;
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                status = false;
            }
            return status;
        }

        public Boolean Validatemapping(Leave leave)
        {
            int count = 0;
            Boolean status = true;
            try
            {
                string query = "select LeaveID,OfficeID from LeaveOfficeMapping where LeaveID='" + leave.leaveID + "' and OfficeID='" + leave.officeID + "'";
                SqlConnection conn = new SqlConnection(Login.connString);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    status = false;
                }
                // count =(int) cmd.ExecuteScalar();
                //if(count > 0)
                // {
                //     status = false;
                // }
                conn.Close();
            }
            catch (Exception ex)
            {
                status = false;
            }
            return status;
        }

        public Boolean ValidateLeaveSanctionLimit(Leave leave)
        {
            Boolean stat = true;
            try
            {
                if (leave.leaveID == "" || leave.leaveID.Trim().Length == 0)
                {
                    stat = false;
                }
                if (leave.designation == "" || leave.designation.Trim().Length == 0)
                {
                    stat = false;
                }
                if (leave.MaxAccrual <= 0)
                {
                    stat = false;
                }
               
            }
            catch (Exception ex)
            {

            }
            return stat;
        }
        public Boolean ValidateLeaveOfficeMapping(Leave leave)
        {
            Boolean stat = true;
            try
            {
                if (leave.leaveID == "" || leave.leaveID.Trim().Length == 0)
                {
                    stat = false;
                }
                if (leave.officeID == "" || leave.officeID.Trim().Length == 0)
                {
                    stat = false;
                }
                if (leave.MaxDays <= 0)
                {
                    stat = false;
                }
            }
            catch (Exception ex)
            {
                stat = false;
            }
            return stat;
        }

        public static List<Leave> fillleavecombo()
        {
            Leave Alist;
            List<Leave> AccList = new List<Leave>();
            try
            {
                string query = "select LeaveID,Description from LeaveType  ";
                SqlConnection conn = new SqlConnection(Login.connString);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Alist = new Leave();
                    Alist.leaveID = reader.GetString(0);
                    Alist.description = reader.GetString(1);
                    AccList.Add(Alist);
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(System.Reflection.MethodBase.GetCurrentMethod().Name + "() : Error");
            }
            return AccList;
        }

        public static void fillLeaveComboNew(System.Windows.Forms.ComboBox cmb)
        {
            cmb.Items.Clear();
            try
            {
                List<Leave> leaves = fillleavecombo();
                foreach (Leave le in leaves)
                {
                        ////cmb.Items.Add(off.OfficeID + "-" + off.name);
                        Structures.ComboBoxItem cbitem =
                            new Structures.ComboBoxItem(le.description,le.leaveID);
                        cmb.Items.Add(cbitem);
                    }
            }
            catch (Exception ex)
            {
                MessageBox.Show(System.Reflection.MethodBase.GetCurrentMethod().Name + "() : Error");
            }
        }
    }
}
