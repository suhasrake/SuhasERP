using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Drawing2D;
using CSLERP.DBData;
using System.Collections;

namespace CSLERP
{
    public partial class TermsAndCondition : System.Windows.Forms.Form
    {
        string docID = "TC";
        DocEmpMappingDB demDB = new DocEmpMappingDB();
        System.Data.DataTable TaxDetailsTable = new System.Data.DataTable();
        Panel pnllv = new Panel();
        ListView lv = new ListView();
        List<int> rowid = new List<int>();
        int delRowID = 0;
        public TermsAndCondition()
        {
            try
            {
                InitializeComponent();

            }
            catch (Exception)
            {

            }
        }
        private void TermsAndCondition_Load(object sender, EventArgs e)
        {
            //////this.FormBorderStyle = FormBorderStyle.None;
            Region = System.Drawing.Region.FromHrgn(Utilities.CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
            initVariables();
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Width -= 100;
            this.Height -= 100;

            String a = this.Size.ToString();
            grdList.ColumnHeadersDefaultCellStyle.BackColor = Color.LightBlue;
            grdList.EnableHeadersVisualStyles = false;
            ListTermsAndConditions();
            //applyPrivilege();
            btnNew.Visible = false;
        }
        private void ListTermsAndConditions()
        {
            try
            {
                grdList.Rows.Clear();
                rowid = new List<int>();
                TermsAndConditionsDB tcdb = new TermsAndConditionsDB();
                List<termsandconditions> TCond = tcdb.getTermsAndConditions();
                int i = 0;
                foreach (termsandconditions tc in TCond)
                {
                    grdList.Rows.Add();
                    rowid.Add(tc.RowID);
                    grdList.Rows[grdList.RowCount - 1].Cells["LineNo"].Value = i + 1;
                    grdList.Rows[grdList.RowCount - 1].Cells["gRowID"].Value = tc.RowID;
                    grdList.Rows[grdList.RowCount - 1].Cells["ParagraphID"].Value = tc.ParagraphID;
                    grdList.Rows[grdList.RowCount - 1].Cells["ParagraphHeading"].Value = tc.ParagraphHeading;
                    grdList.Rows[grdList.RowCount - 1].Cells["Details"].Value = tc.Details;
                    i++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in Terms and Condition Listing");
            }
            try
            {
                enableBottomButtons();
                pnlList.Visible = true;
                btnNew.Visible = false;
            }
            catch (Exception ex)
            {
            }
        }

        private void initVariables()
        {

            docID = Main.currentDocument;
            pnlUI.Controls.Add(pnlList);
            btnNew.Visible = false;
            btnCancel.Visible = false;
            //closeAllPanels();
            grdList.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            if(getuserPrivilegeStatus() == 1)
            {
                btnAddNewLine.Visible = false;
                btnSave.Visible = false;
            }
            else
            {
                btnAddNewLine.Visible = true;
                btnSave.Visible = true;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //clearData();
            closeAllPanels();
            pnlList.Visible = true;
            ListTermsAndConditions();
            //pnlBottomActions.Visible = true;
        }
        private void closeAllPanels()
        {
            try
            {
                pnlList.Visible = false;
                //pnlAddEdit.Visible = false;

            }
            catch (Exception)
            {

            }
        }

        public void clearData()
        {
            try
            {
                grdList.Rows.Clear();
            }
            catch (Exception ex)
            {

            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
                pnlUI.Visible = false;
            }
            catch (Exception)
            {

            }
        }
        private void enableBottomButtons()
        {
            btnNew.Visible = false;
            btnExit.Visible = true;
            pnlBottomButtons.Visible = true;
        }
        private void btnNew_Click(object sender, EventArgs e)
        {
        }
        private void btnAddNewLine_Click_1(object sender, EventArgs e)
        {
            try
            {
                AddTermsAndConditionsRow();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this.ToString() + "-"+ System.Reflection.MethodBase.GetCurrentMethod().Name+"() : Error");
            }
        }
        private Boolean AddTermsAndConditionsRow()
        {
            Boolean status = true;
            try
            {
                int[] arr;
                if (grdList.Rows.Count > 0)
                {
                    arr = new int[grdList.Rows.Count];
                    if (!verifyAndReworkTermsAndConditionsGridRows())
                    {
                        return false;
                    }
                    for (int i = 0; i < grdList.Rows.Count; i++)
                    {
                        arr[i] = Convert.ToInt32(grdList.Rows[i].Cells["ParagraphID"].Value);
                    }
                }
                else
                    arr = new int[1] { 0 };

                grdList.Rows.Add();
                int kount = grdList.RowCount;
                grdList.Rows[kount - 1].Cells["LineNo"].Value = kount;
                grdList.Rows[kount - 1].Cells["ParagraphID"].Value = arr.Max() + 1;
                grdList.Rows[kount - 1].Cells["ParagraphHeading"].Value = " ";
                grdList.Rows[kount - 1].Cells["Details"].Value = "";

                var BtnCell = (DataGridViewButtonCell)grdList.Rows[kount - 1].Cells["Delete"];
                BtnCell.Value = "Del";
            }
            catch (Exception ex)
            {
                MessageBox.Show("AddWODetailRow() : Error");
            }

            return status;
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            closeAllPanels();
            btnNew.Visible = true;
            btnExit.Visible = true;
            pnlList.Visible = true;
            enableBottomButtons();
            pnlBottomButtons.Visible = true;
        }

        private Boolean verifyAndReworkTermsAndConditionsGridRows()
        {
            Boolean status = true;

            try
            {
                if (grdList.Rows.Count <= 0)
                {
                    MessageBox.Show("No entries in Terms and Conditions Grid details");
                    return false;
                }
                for (int i = 0; i < grdList.Rows.Count; i++)
                {

                    grdList.Rows[i].Cells["LineNo"].Value = (i + 1);
                    if (((grdList.Rows[i].Cells["ParagraphHeading"].Value == null) ||
                        (grdList.Rows[i].Cells["ParagraphHeading"].Value.ToString().Trim().Length == 0) ||
                        (grdList.Rows[i].Cells["details"].Value == null)) ||
                        (grdList.Rows[i].Cells["details"].Value.ToString().Trim().Length == 0))
                    {
                        MessageBox.Show("Fill values in row " + (i + 1));
                        return false;
                    }
                }
                if (!validateItems())
                {
                    MessageBox.Show("Validation failed");
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return status;
        }
        private Boolean validateItems()
        {
            Boolean status = true;
            try
            {
                for (int i = 0; i < grdList.Rows.Count - 1; i++)
                {
                    for (int j = i + 1; j < grdList.Rows.Count; j++)
                    {

                        if (grdList.Rows[i].Cells[1].Value.ToString() == grdList.Rows[j].Cells["ParagraphHeading"].Value.ToString())
                        {
                            //duplicate item code
                            MessageBox.Show("Item code duplicated in OB details... please ensure correctness (" +
                                grdList.Rows[i].Cells["ParagraphHeading"].Value.ToString() + ")");
                            status = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                status = false;
            }
            return status;
        }

        private Boolean createAndUpdateTermsAndConditions()
        {
            Boolean status = true;
            try
            {
                TermsAndConditionsDB tcdb = new TermsAndConditionsDB();
                termsandconditions tc = new termsandconditions();
                List<termsandconditions> TCDetails = new List<termsandconditions>();
                for (int i = 0; i < grdList.Rows.Count; i++)
                {
                    try
                    {
                        tc = new termsandconditions();
                        tc.RowID= Convert.ToInt32(grdList.Rows[i].Cells["gRowID"].Value);
                        tc.ParagraphID = Convert.ToInt32(grdList.Rows[i].Cells["ParagraphID"].Value);
                        tc.ParagraphHeading = grdList.Rows[i].Cells["ParagraphHeading"].Value.ToString().Trim();
                        tc.Details = grdList.Rows[i].Cells["Details"].Value.ToString().Trim();
                        TCDetails.Add(tc);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("createAndUpdateTermsAndConditions() : Error creating Terms and Conditions");
                        status = false;
                    }
                }
                if (!verifyAndReworkTermsAndConditionsGridRows())
                {
                    MessageBox.Show("Terms and Conditions failed to save.");
                    status = false;

                }
                else
                {
                    if (!tcdb.UpdateTermsAndConditions(TCDetails, rowid))
                    {
                        MessageBox.Show("createAndUpdateTermsAndConditions() : Failed to update Terms and Conditions. Please check the values");
                        status = false;
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("createAndUpdateTermsAndConditions() : Error updating Terms and Conditions.");
                status = false;
            }
            return status;
        }
        



        private void btnSave_Click_1(object sender, EventArgs e)
        {
            Boolean status = true;
            try
            {

                // TermsAndConditionsDB tcdb = new TermsAndConditionsDB();
                termsandconditions tc = new termsandconditions();
                System.Windows.Forms.Button btn = sender as System.Windows.Forms.Button;
                string btnText = btnSave.Text;
                try
                {
                    if (!verifyAndReworkTermsAndConditionsGridRows())
                    {
                        MessageBox.Show("Terms and Conditions failed to save.");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Validation failed");
                    return;
                }
                if (btnText.Equals("Save"))
                {

                    if (createAndUpdateTermsAndConditions())
                    {
                        MessageBox.Show("Terms and Conditions updated");
                        closeAllPanels();
                        pnlList.Visible = true;
                        ListTermsAndConditions();
                        //pnlBottomActions.Visible = true;
                    }
                    else
                    {
                        status = false;
                    }
                }
                if (!status)
                {
                    MessageBox.Show("Failed to update Terms and condition Header");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("createAndUpdateTermsAndConditions() : Error");
            }
        }
        private void pnlUI_Paint(object sender, PaintEventArgs e)
        {

        }
        private void grdList_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0)
                    return;
                string columnName = grdList.Columns[e.ColumnIndex].Name;
                try
                {
                    if (columnName.Equals("Delete"))
                    {
                        //delete row
                        DialogResult dialog = MessageBox.Show("Are you sure to Delete the row ?", "Yes", MessageBoxButtons.YesNo);
                        if (dialog == DialogResult.Yes)
                        {
                            delRowID = Convert.ToInt32(grdList.Rows[e.RowIndex].Cells["gRowID"].Value);
                            TermsAndConditionsDB tcdb = new TermsAndConditionsDB();
                            if(tcdb.DeleteTermsAndConditionsrow(delRowID))
                            {
                                grdList.Rows.RemoveAt(e.RowIndex);
                                rowid.RemoveAt(rowid.IndexOf(delRowID));
                                MessageBox.Show("Terms and condition Row Deleted");
                            }
                            else
                            {
                                MessageBox.Show("ERROR!! Deleting Terms and condition Row, \n Please check if this Terms and Condition item is mapped with any Document!!!");
                                return;
                            }
                        }
                        verifyAndReworkTermsAndConditionsGridRows();
                    }
                }
                catch (Exception)
                {

                }
            }
            catch (Exception ex)
            {

            }
        }
        int getuserPrivilegeStatus()
        {
            try
            {
                if (Main.itemPriv[0] && !Main.itemPriv[1] && !Main.itemPriv[2]) //only view
                    return 1;
                else if (Main.itemPriv[0] && (Main.itemPriv[1] || Main.itemPriv[2])) //view add and edit
                    return 2;
                else if (!Main.itemPriv[0] && (Main.itemPriv[1] || Main.itemPriv[2])) //view add and edit
                    return 3;
                else if (!Main.itemPriv[0] && !Main.itemPriv[1] || !Main.itemPriv[2]) //no privilege
                    return 0;
                else
                    return -1;
            }
            catch (Exception ex)
            {
            }
            return 0;
        }

        private void TermsAndCondition_Enter(object sender, EventArgs e)
        {
            try
            {
                string frmname = this.Name;
                string menuid = Main.menuitems.Where(x => x.pageLink == frmname).Select(x => x.menuItemID).FirstOrDefault().ToString();
                Main.itemPriv = Utilities.fillItemPrivileges(Main.userOptionArray, menuid);
            }
            catch (Exception ex)
            {
            }
        }
    }
}


