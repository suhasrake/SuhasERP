using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CSLERP.DBData
{
    //public strin "Data Source = PRAKASHPC; Initial Catalog = newERP; Integrated Security = True"
    class usermenuprivilege
    {
        public string userID { get; set; }
        public string menuItemString { get; set; }

    }
    class itemprivilege
    {
        public Boolean privView { get; set; }
        public Boolean privAdd { get; set; }
        public Boolean privEdit { get; set; }
        public Boolean privDelete { get; set; }
    }
    class MenuPrivilegeDB
    {
        ActivityLogDB alDB = new ActivityLogDB();
        public string getUserMenuPrivilege(string userID)
        {
            string umpString = "";
            try
            {
                SqlConnection conn = new SqlConnection(Login.connString);
                string query = "select MenuItemString " +
                    "from MenuPrivilege where UserID='" + userID + "'";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    umpString = reader.GetString(0);
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                umpString = "";
                MessageBox.Show(this.ToString() + "-"+ System.Reflection.MethodBase.GetCurrentMethod().Name+"() : Error");
            }
            return umpString;
        }
        public Boolean deleteUserMenuPrivilege(string userID)
        {
            Boolean status = true;
            string utString = "";
            try
            {
                SqlConnection conn = new SqlConnection(Login.connString);
                string updateSQL = "delete MenuPrivilege " +
                    "where UserID='" + userID + "'";
                utString = utString + updateSQL + Main.QueryDelimiter;
                utString = utString +
                    ActivityLogDB.PrepareActivityLogQquerString("delete", "MenuPrivilege", "", updateSQL) +
                    Main.QueryDelimiter;
                if (!UpdateTable.UT(utString))
                {
                    status = false;
                }
            }
            catch (Exception)
            {
                status = false;
            }
            return status;
        }
        public Boolean updateUserMenuPrivilege(usermenuprivilege mp)
        {
            Boolean status = true;
            string utString = "";
            try
            {
                string updateSQL = "update MenuPrivilege set MenuItemString='" + mp.menuItemString +
                    "' where UserID='" + mp.userID + "'";
                utString = utString + updateSQL + Main.QueryDelimiter;
                utString = utString +
                  ActivityLogDB.PrepareActivityLogQquerString("update", "MenuPrivilege", "", updateSQL) +
                  Main.QueryDelimiter;
                if (!UpdateTable.UT(utString))
                {
                    status = false;
                }
            }
            catch (Exception)
            {

                status = false;
            }
            return status;
        }
        public Boolean insertUserMenuPrivilege(usermenuprivilege mp)
        {
            Boolean status = true;
            string utString = "";
            try
            {
                DateTime cdt = DateTime.Now;
                string updateSQL = "insert into MenuPrivilege (UserID,MenuItemString,CreateTime,CreateUser)" +
                    " values (" +
                    "'" + mp.userID + "'," +
                    "'" + mp.menuItemString + "'," +
                    "GETDATE()" + "," +
                    "'" + Login.userLoggedIn + "'" + ")";
                utString = utString + updateSQL + Main.QueryDelimiter;
                utString = utString +
                  ActivityLogDB.PrepareActivityLogQquerString("insert", "MenuPrivilege", "", updateSQL) +
                  Main.QueryDelimiter;
                if (!UpdateTable.UT(utString))
                {
                    status = false;
                }
            }
            catch (Exception ex)
            {

                status = false;
            }
            return status;
        }
        public Boolean validateUser(usermenuprivilege mp)
        {
            Boolean status = true;
            try
            {
                if (mp.userID.Trim().Length == 0 || mp.userID == null)
                {
                    return false;
                }
            }
            catch (Exception)
            {

            }
            return status;
        }
    }
}
