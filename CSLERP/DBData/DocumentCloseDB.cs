﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSLERP.DBData
{
    class DocClose
    {
        public int RowID { get; set; }
        public string DocumentID { get; set; }
        public string DocumentName { get; set; }
        public int TemporaryNo { get; set; }
        public string ApproveUser { get; set; }
        public string TableName { get; set; }
        public DateTime TemporaryDate { get; set; }
        public string forwarderlist { get; set; }
        public int DocumentNo { get; set; }
        public DateTime DocumentDate { get; set; }
    }

    class DocumentCloseDB
     {
        ActivityLogDB alDB = new ActivityLogDB();
        public List<DocClose> getDocCloseValues()
        {
            DocClose Docval;
            List<DocClose> DocValues = new List<DocClose>();
            try
            {
                SqlConnection conn = new SqlConnection(Login.connString);
                string empLogedin = Login.userLoggedIn;
                string query = "select distinct a.DocumentID,a.DocumentName,a.TableName  from Document a , DocEmpMapping b ,DocumentUC c" +
                     " where a.DocumentID = b.DocumentID and" +
                      " a.IsReversible = 1 and b.SeniorEmployeeID = '" + Login.empLoggedIn + "' or " +
                      "c.UserID = (select distinct UserID from ViewUserEmployeeList a where a.EmployeeID = '" + Login.empLoggedIn + "' )" +
                      "and c.Status = 1 and a.TableName != 'NULL'and a.IsReversible = 1 order by a.DocumentName";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Docval = new DocClose();
                    Docval.DocumentID = reader.GetString(0);
                    Docval.DocumentName = reader.GetString(1);
                    Docval.TableName = reader.GetString(2);
                    DocValues.Add(Docval);
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this.ToString() + "-" + System.Reflection.MethodBase.GetCurrentMethod().Name + "() : Error");

            }
            return DocValues;
        }


        public Boolean updateDocForClose(DocClose du)
        {
            Boolean status = true;
            string utString = "";
            try
            {
              string updateSQL = "update " + du.TableName + " set Status= 7 where RowID = " + du.RowID + "";
                utString = utString + updateSQL + Main.QueryDelimiter;
                utString = utString + ActivityLogDB.PrepareActivityLogQquerString("update", du.TableName, "", updateSQL) + Main.QueryDelimiter;
                if (!UpdateTable.UT(utString))
                {
                    status = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this.ToString() + "-" + System.Reflection.MethodBase.GetCurrentMethod().Name + "() : Error");
                status = false;
            }
            return status;
        }
        public List<DocClose> getDocumentInfo(string tablename, string documentID)
        {
            DocClose tb;
            DocumentUnlockDB dbrecord = new DocumentUnlockDB();
            List<DocClose> tbdata = new List<DocClose>();
            try
            {

                SqlConnection conn = new SqlConnection(Login.connString);
                string empLogedin = Login.userLoggedIn;
                string query = "select DocumentID,TemporaryNo,TemporaryDate,ApproveUser,ForwarderList,RowID from " + tablename +
                    " where DocumentID = '" + documentID + "' and DocumentStatus = 99 and Status=1 order by TemporaryNo desc, TemporaryDate desc" ;
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    tb = new DocClose();
                    tb.DocumentID = reader.IsDBNull(0) ? "" : reader.GetString(0);
                    tb.TemporaryNo = reader.GetInt32(1);
                    tb.TemporaryDate = reader.GetDateTime(2);
                    tb.TableName = tablename;
                    tb.ApproveUser = reader.IsDBNull(3) ? "" : reader.GetString(3);
                    tb.forwarderlist = reader.IsDBNull(4) ? "" : reader.GetString(4);
                    tb.RowID = reader.IsDBNull(5) ? 0 : reader.GetInt32(5);

                    tbdata.Add(tb);
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this.ToString() + "-" + System.Reflection.MethodBase.GetCurrentMethod().Name + "() : Error");
            }
            return tbdata;

        }
              
        public static string getCloseCommiteeListString()
        {
            string list = "";
            try
            {
                SqlConnection conn = new SqlConnection(Login.connString);
                string empLogedin = Login.userLoggedIn;
                string query = "select UserID from DocumentUC where Status = 1";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list = list + ";" + reader.GetString(0) + ";";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(System.Reflection.MethodBase.GetCurrentMethod().Name + "() : Error");
                list = null;
            }
            return list;
        }
      
    }
}
