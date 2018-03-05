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

namespace CSLERP
{
    public partial class LeaveSettings : System.Windows.Forms.Form
    {
        string selectedCatalogueID = "";
        int option = 0;
        int rowid = 0;
        string prevdesig = "";
        string prevoff = "";
        string prevleaveid = "";
        System.Windows.Forms.Button prevbtn = null;
        public LeaveSettings()
        {
            try
            {
                InitializeComponent();

            }
            catch (Exception)
            {

            }
        }
        private void LeaveSettings_Load(object sender, EventArgs e)
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
            //CreateCataloueButtons();
        }

        private void initVariables()
        {

            //249, 29
            pnlLeaveSanctionLimitOuter.Location = new Point(249, 20);
            pnlLeaveTypeOuter.Location = new Point(249, 20);
            pnlLeaveOfficeMapOuter.Location = new Point(249, 20);

            pnlLeaveSanctionLimitOuter.Parent = pnlUI;
            pnlLeaveTypeOuter.Parent = pnlUI;
            pnlLeaveOfficeMapOuter.Parent = pnlUI;

            pnlLeaveSanctionLimitInner.Parent = pnlLeaveSanctionLimitOuter;
            pnlLeaveTypeInner.Parent = pnlLeaveTypeOuter;
            pnlLeaveOfficeMapInner.Parent = pnlLeaveOfficeMapOuter;

            OfficeDB.fillOfficeComboNew(cmbLomOffice);
            CatalogueValueDB.fillCatalogValueComboNew(cmbLslDesignation, "Designation");
            CatalogueValueDB.fillCatalogValueComboNew(cmbGender, "Gender");
            LeaveSettingsdb.fillLeaveComboNew(cmbLomLeaveID);
            LeaveSettingsdb.fillLeaveComboNew(cmbLslLeaveID);
            cmbGender.Items.Add("All");
            btnNew.Visible = false;

            pnlLeaveOfficeMapOuter.Visible = false;
            pnlLeaveSanctionLimitOuter.Visible = false;
            pnlLeaveTypeOuter.Visible = false;
        }

        private void applyPrivilege()
        {
            try
            {
                if (Main.itemPriv[1])
                {
                    btnNew.Visible = true;
                }
                else
                {
                    btnNew.Visible = false;
                }
                if (Main.itemPriv[2])
                {
                    grdList.Columns["Edit"].Visible = true;
                }
                else
                {
                    grdList.Columns["Edit"].Visible = false;
                }
            }
            catch (Exception)
            {
            }
        }

        public void init(int opt)
        {
            pnlLeaveList.Visible = false;
            if (opt == 1)
            {
                grdList.Visible = false;
                pnlLeaveTypeOuter.Visible = true;
                pnlLeaveTypeInner.Visible = true;

                pnlLeaveSanctionLimitOuter.Visible = false;
                pnlLeaveSanctionLimitInner.Visible = false;

                pnlLeaveOfficeMapOuter.Visible = false;
                pnlLeaveOfficeMapInner.Visible = false;

                txtLeaveID.Enabled = true;
                txtlDescription.Enabled = true;
                txtMaxSanctionLimit.Enabled = true;
                cmbSanctionType.Enabled = true;
                cmbCarryForward.SelectedIndex = 0;
            }
            else if (opt == 2)
            {
                grdList.Visible = false;

                grdList.Visible = false;
                pnlLeaveTypeOuter.Visible = false;
                pnlLeaveTypeInner.Visible = false;

                pnlLeaveSanctionLimitOuter.Visible = true;
                pnlLeaveSanctionLimitInner.Visible = true;

                pnlLeaveOfficeMapOuter.Visible = false;
                pnlLeaveOfficeMapInner.Visible = false;

                cmbLslLeaveID.Enabled = true;
                txtLslMaxSanctionLimit.Enabled = true;
                cmbLslDesignation.Enabled = true;

            }
            else if (opt == 3)
            {
                grdList.Visible = false;
                pnlLeaveTypeOuter.Visible = false;
                pnlLeaveTypeInner.Visible = false;

                pnlLeaveSanctionLimitOuter.Visible = false;
                pnlLeaveSanctionLimitInner.Visible = false;

                pnlLeaveOfficeMapOuter.Visible = true;
                pnlLeaveOfficeMapInner.Visible = true;

                cmbLomLeaveID.Enabled = true;
                txtLomMaxDays.Enabled = true;
                cmbLomOffice.Enabled = true;
            }
        }

        private void closeAllPanels()
        {
            try
            {
                pnlLeaveTypeOuter.Visible = false;
                pnlLeaveSanctionLimitOuter.Visible = false;
                pnlLeaveOfficeMapOuter.Visible = false;
                pnlLeaveTypeInner.Visible = true;
                pnlLeaveSanctionLimitInner.Visible = false;
                pnlLeaveOfficeMapInner.Visible = false;
            }
            catch (Exception)
            {

            }
        }


        private void fillStatusCombo(System.Windows.Forms.ComboBox cmb)
        {
            try
            {
                cmb.Items.Clear();
                for (int i = 0; i < Main.statusValues.GetLength(0); i++)
                {
                    cmb.Items.Add(Main.statusValues[i, 1]);
                }
            }
            catch (Exception)
            {

            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                closeAllPanels();
                clearData();
                grdList.Visible = true;
                pnlLeaveList.Visible = true;
                btnNew.Visible = true;
                enableBottomButtons();
            }
            catch (Exception)
            {

            }
        }

        public void clearData()
        {
            try
            {
                txtLeaveID.Text = "";
                txtlDescription.Text = "";
                txtLomMaxDays.Text = "";
                txtLslMaxSanctionLimit.Text = "";
                txtMaxSanctionLimit.Text = "";
                cmbLslLeaveID.SelectedIndex = 0;
                cmbLomLeaveID.SelectedIndex = 0;
                cmbLomOffice.SelectedIndex = 0;
                cmbLslDesignation.SelectedIndex = 0;
                cmbSanctionType.SelectedIndex = 0;
                cmbGender.SelectedIndex = 0;
            }
            catch (Exception)
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

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                closeAllPanels();
                clearData();
                btnSave.Text = "Save";
                btnLslSave.Text = "Save";
                btnLomSave.Text = "Save";
                init(option);
                disableBottomButtons();
            }
            catch (Exception)
            {

            }
        }

        public static int status(string StatusString)
        {
            int StatusCode = 0;
            try
            {
                if (StatusString == "HR")
                {
                    StatusCode = 2;
                }
                if (StatusString == "Department Head")
                {
                    StatusCode = 1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(System.Reflection.MethodBase.GetCurrentMethod().Name + "() : Error");

            }
            return StatusCode;
        }

        public static string strstatus(int StatusString)
        {
            string StatusCode = "";
            try
            {
                if (StatusString == 2)
                {
                    StatusCode = "HR";
                }
                if (StatusString == 1)
                {
                    StatusCode = "Department Head";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(System.Reflection.MethodBase.GetCurrentMethod().Name + "() : Error");

            }
            return StatusCode;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Leave leave = new Leave();
                LeaveSettingsdb leaveDB = new LeaveSettingsdb();
                if (option == 1)
                {
                    leave.leaveID = txtLeaveID.Text;
                    leave.description = txtlDescription.Text;
                    leave.MaxAccrual = Convert.ToInt32(txtMaxSanctionLimit.Text);
                    leave.SanctionType = status(cmbSanctionType.SelectedItem.ToString());
                    leave.ahead = Convert.ToInt32 (txtDaysAhead.Text);
                    leave.Delay = Convert.ToInt32(txtDaysDelayed.Text);
                    leave.CarryForward = getcarryforwardint(cmbCarryForward.SelectedItem.ToString());
                    if (cmbGender.SelectedItem.ToString() == "All")
                    {
                        leave.Gender = cmbGender.SelectedItem.ToString();
                    }
                    else
                    {
                        leave.Gender = ((Structures.ComboBoxItem)cmbGender.SelectedItem).HiddenValue;
                    }
                    if (leaveDB.ValidateLeaveType(leave))
                    {
                        if (btnSave.Text.Equals("Update"))
                        {
                            if (leaveDB.UpdateLeaveType(leave))
                            {
                                MessageBox.Show("LeaveType updated");
                                closeAllPanels();
                                leavetype();
                            }
                            else
                            {
                                MessageBox.Show("Failed to update LeaveType");
                            }
                        }
                        else if (btnSave.Text.Equals("Save"))
                        {
                            if (leaveDB.validateLeaveType(leave))
                            {
                                if (leaveDB.InsertLeaveType(leave))
                                {
                                    MessageBox.Show("LeaveType Value Added");
                                    closeAllPanels();
                                    leavetype();
                                }
                                else
                                {
                                    MessageBox.Show("Failed to Insert LeaveType");
                                }
                            }
                            else
                            {
                                MessageBox.Show("Leave Type Already exists!!!");
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("LeaveType Data Validation failed");
                    }
                }
            }
            catch (Exception)
            {

                MessageBox.Show("Failed Adding / Editing User Data");
            }
        }

        private void grdList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0)
                    return;
                string col = grdList.Columns[e.ColumnIndex].HeaderText;
                if (col == "Edit")
                {
                    rowid = Convert.ToInt32(grdList.Rows[e.RowIndex].Cells["lRowID"].Value.ToString());

                    if (option == 1)
                    {
                        btnSave.Text = "Update";
                        pnlLeaveTypeInner.Visible = true;
                        pnlLeaveTypeOuter.Visible = true;
                        pnlLeaveList.Visible = false;
                        txtLeaveID.Enabled = false;
                        txtlDescription.Enabled = true;
                        cmbSanctionType.Enabled = true;
                        cmbSanctionType.SelectedIndex = cmbSanctionType.FindStringExact(grdList.Rows[e.RowIndex].Cells["lSanctionType"].Value.ToString());
                        cmbCarryForward.SelectedIndex = cmbCarryForward.FindStringExact(grdList.Rows[e.RowIndex].Cells["Carryforward"].Value.ToString());
                        if (grdList.Rows[e.RowIndex].Cells["lGender"].Value.ToString() == "All")
                        {
                            cmbGender.SelectedIndex = cmbGender.FindString(grdList.Rows[e.RowIndex].Cells["lGender"].Value.ToString());
                        }
                        else
                        {
                            cmbGender.SelectedIndex = Structures.ComboFUnctions.getComboIndex(cmbGender, grdList.Rows[e.RowIndex].Cells["lGender"].Value.ToString());
                        }
                        txtLeaveID.Text = grdList.Rows[e.RowIndex].Cells["LeaveID"].Value.ToString();
                        txtlDescription.Text = grdList.Rows[e.RowIndex].Cells["lDescription"].Value.ToString();
                        txtMaxSanctionLimit.Text = grdList.Rows[e.RowIndex].Cells["lMaxSanctionLimit"].Value.ToString();
                        txtDaysAhead.Text= grdList.Rows[e.RowIndex].Cells["DaysAhead"].Value.ToString();
                        txtDaysDelayed.Text = grdList.Rows[e.RowIndex].Cells["DaysDelayed"].Value.ToString();
                        prevleaveid = grdList.Rows[e.RowIndex].Cells["LeaveID"].Value.ToString();
                        disableBottomButtons();
                    }
                    if (option == 2)
                    {
                        btnLslSave.Text = "Update";
                        pnlLeaveSanctionLimitInner.Visible = true;
                        pnlLeaveSanctionLimitOuter.Visible = true;
                        pnlLeaveList.Visible = false;
                        cmbLslLeaveID.Enabled = false;
                        cmbLslDesignation.Enabled = true;
                        txtLslMaxSanctionLimit.Enabled = true;
                        cmbLslLeaveID.SelectedIndex = Structures.ComboFUnctions.getComboIndex(cmbLslLeaveID, grdList.Rows[e.RowIndex].Cells["LeaveID"].Value.ToString());
                        cmbLslDesignation.SelectedIndex = Structures.ComboFUnctions.getComboIndex(cmbLslDesignation, grdList.Rows[e.RowIndex].Cells["lDesignation"].Value.ToString());
                        txtLslMaxSanctionLimit.Text = grdList.Rows[e.RowIndex].Cells["lMaxSanctionLimit"].Value.ToString();
                        prevdesig = cmbLslDesignation.SelectedItem.ToString();
                        disableBottomButtons();
                    }
                    if (option == 3)
                    {
                        btnLomSave.Text = "Update";
                        pnlLeaveOfficeMapInner.Visible = true;
                        pnlLeaveOfficeMapOuter.Visible = true;
                        pnlLeaveList.Visible = false;
                        cmbLomLeaveID.Enabled = false;
                        cmbLomOffice.Enabled = true;
                        txtLomMaxDays.Enabled = true;
                        cmbLomLeaveID.SelectedIndex = Structures.ComboFUnctions.getComboIndex(cmbLomLeaveID, grdList.Rows[e.RowIndex].Cells["LeaveID"].Value.ToString());
                        cmbLomOffice.SelectedIndex = Structures.ComboFUnctions.getComboIndex(cmbLomOffice, grdList.Rows[e.RowIndex].Cells["lOfficeID"].Value.ToString());
                        txtLomMaxDays.Text = grdList.Rows[e.RowIndex].Cells["lMaxDays"].Value.ToString();
                        prevoff = cmbLomOffice.SelectedItem.ToString();
                        disableBottomButtons();

                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private void disableBottomButtons()
        {
            btnNew.Visible = false;
            btnExit.Visible = false;
        }
        private void enableBottomButtons()
        {
            btnNew.Visible = true;
            btnExit.Visible = true;
        }

        private void btnLeaveType_Click(object sender, EventArgs e)
        {
            OfficeDB.fillOfficeComboNew(cmbLomOffice);
            CatalogueValueDB.fillCatalogValueComboNew(cmbLslDesignation, "Designation");
            CatalogueValueDB.fillCatalogValueComboNew(cmbGender, "Gender");
            LeaveSettingsdb.fillLeaveComboNew(cmbLomLeaveID);
            LeaveSettingsdb.fillLeaveComboNew(cmbLslLeaveID);
            cmbGender.Items.Add("All");
            leavetype();
        }
        void leavetype()
        {
            option = 1;
            try
            {
                grdList.Rows.Clear();
                LeaveSettingsdb record = new LeaveSettingsdb();
                List<Leave> leavetype = record.getLeaveTypeList();
                foreach (Leave leave in leavetype)
                {
                    grdList.Rows.Add();
                    grdList.Rows[grdList.RowCount - 1].Cells["lSerialNo"].Value = grdList.Rows.Count;
                    grdList.Rows[grdList.RowCount - 1].Cells["LeaveID"].Value = leave.leaveID;
                    grdList.Rows[grdList.RowCount - 1].Cells["lDescription"].Value = leave.description;
                    grdList.Rows[grdList.RowCount - 1].Cells["lMaxSanctionLimit"].Value = leave.MaxAccrual;
                    grdList.Rows[grdList.RowCount - 1].Cells["lSanctionType"].Value = strstatus(leave.SanctionType);
                    grdList.Rows[grdList.RowCount - 1].Cells["lGender"].Value = leave.Gender;
                    grdList.Rows[grdList.RowCount - 1].Cells["lRowID"].Value = leave.rowid;
                    grdList.Rows[grdList.RowCount - 1].Cells["DaysAhead"].Value = leave.ahead;
                    grdList.Rows[grdList.RowCount - 1].Cells["DaysDelayed"].Value = leave.Delay;
                    grdList.Rows[grdList.RowCount - 1].Cells["Carryforward"].Value = getcarryforwardstr(leave.CarryForward);
                }
                setvisiblity(option);
                closeAllPanels();
                pnlLeaveList.Visible = true;
                enableBottomButtons();
                pnlBottomButtons.Visible = true;
            }
            catch (Exception ex)
            {

            }
        }

        public  string getcarryforwardstr(int val)
        {
            string info="";
            if (val == 0)
            {
                info = "No";
            }
            else if (val == 1)
            {
                info = "Yes";
            }
            return info;
        }

        public int getcarryforwardint(string val)
        {
            int info=2;
            if (val == "No")
            {
                info = 0;
            }
            else if (val == "Yes")
            {
                info =1;
            }
            return info;
        }


        private void btnLeaveSanctionLimit_Click(object sender, EventArgs e)
        {
            OfficeDB.fillOfficeComboNew(cmbLomOffice);
            CatalogueValueDB.fillCatalogValueComboNew(cmbLslDesignation, "Designation");
            CatalogueValueDB.fillCatalogValueComboNew(cmbGender, "Gender");
            LeaveSettingsdb.fillLeaveComboNew(cmbLomLeaveID);
            LeaveSettingsdb.fillLeaveComboNew(cmbLslLeaveID);
            cmbGender.Items.Add("All");
            leavesanctionlimit();
        }
        void leavesanctionlimit()
        {
            option = 2;
            try
            {
                grdList.Rows.Clear();
                LeaveSettingsdb record = new LeaveSettingsdb();
                List<Leave> leavetype = record.getSanctionLimitList();
                foreach (Leave leave in leavetype)
                {
                    grdList.Rows.Add();
                    grdList.Rows[grdList.RowCount - 1].Cells["lSerialNo"].Value = grdList.Rows.Count;
                    grdList.Rows[grdList.RowCount - 1].Cells["LeaveID"].Value = leave.leaveID;
                    grdList.Rows[grdList.RowCount - 1].Cells["lDesignation"].Value = leave.designation;
                    grdList.Rows[grdList.RowCount - 1].Cells["lMaxSanctionLimit"].Value = leave.MaxAccrual;
                    grdList.Rows[grdList.RowCount - 1].Cells["lRowID"].Value = leave.rowid;
                }
                setvisiblity(option);
                closeAllPanels();
                pnlLeaveList.Visible = true;
                enableBottomButtons();
                pnlBottomButtons.Visible = true;
            }
            catch (Exception ex)
            {

            }
        }

        private void btnLeaveOfficeMapping_Click(object sender, EventArgs e)
        {
            OfficeDB.fillOfficeComboNew(cmbLomOffice);
            CatalogueValueDB.fillCatalogValueComboNew(cmbLslDesignation, "Designation");
            CatalogueValueDB.fillCatalogValueComboNew(cmbGender, "Gender");
            LeaveSettingsdb.fillLeaveComboNew(cmbLomLeaveID);
            cmbGender.Items.Add("All");
            LeaveSettingsdb.fillLeaveComboNew(cmbLslLeaveID);
            leaveofficemapping();
        }
        void leaveofficemapping()
        {
            option = 3;
            try
            {
                grdList.Rows.Clear();
                LeaveSettingsdb record = new LeaveSettingsdb();
                List<Leave> leavetype = record.getleaveofficemappingList();

                foreach (Leave leave in leavetype)
                {
                    grdList.Rows.Add();
                    grdList.Rows[grdList.RowCount - 1].Cells["lSerialNo"].Value = grdList.Rows.Count;
                    grdList.Rows[grdList.RowCount - 1].Cells["LeaveID"].Value = leave.leaveID;
                    grdList.Rows[grdList.RowCount - 1].Cells["lOfficeID"].Value = leave.officeID;
                    grdList.Rows[grdList.RowCount - 1].Cells["OfficeName"].Value = leave.officeName;
                    grdList.Rows[grdList.RowCount - 1].Cells["lMaxDays"].Value = leave.MaxDays;
                    grdList.Rows[grdList.RowCount - 1].Cells["lRowID"].Value = leave.rowid;
                }
                setvisiblity(option);
                closeAllPanels();
                pnlLeaveList.Visible = true;
                enableBottomButtons();
                pnlBottomButtons.Visible = true;
            }
            catch (Exception ex)
            {

            }
        }

        public void setvisiblity(int opt)
        {
            grdList.Visible = true;
            if (opt == 1)
            {
                grdList.Columns["lSanctionType"].Visible = true;
                grdList.Columns["lMaxSanctionLimit"].Visible = true;
                grdList.Columns["lDescription"].Visible = true;
                grdList.Columns["lGender"].Visible = true;
                grdList.Columns["lOfficeID"].Visible = false;
                grdList.Columns["OfficeName"].Visible = false;
                grdList.Columns["lMaxDays"].Visible = false;
                grdList.Columns["lDesignation"].Visible = false;
                grdList.Columns["Carryforward"].Visible = true;
                grdList.Columns["DaysAhead"].Visible = true;
                grdList.Columns["DaysDelayed"].Visible = true;

            }
            else if (opt == 2)
            {
                grdList.Columns["lDesignation"].Visible = true;
                grdList.Columns["lMaxSanctionLimit"].Visible = true;
                grdList.Columns["lOfficeID"].Visible = false;
                grdList.Columns["OfficeName"].Visible = false;
                grdList.Columns["lMaxDays"].Visible = false;
                grdList.Columns["lDescription"].Visible = false;
                grdList.Columns["lSanctionType"].Visible = false; 
                grdList.Columns["lGender"].Visible = false;
                grdList.Columns["lGender"].Visible = false;
                grdList.Columns["DaysAhead"].Visible=false;
                grdList.Columns["DaysDelayed"].Visible = false;
                grdList.Columns["Carryforward"].Visible = false;

            }
            else if (opt == 3)
            {
                grdList.Columns["lMaxSanctionLimit"].Visible = false;
                grdList.Columns["lDescription"].Visible = false;
                grdList.Columns["lSanctionType"].Visible = false;
                grdList.Columns["lDesignation"].Visible = false;
                grdList.Columns["lOfficeID"].Visible = false;
                grdList.Columns["OfficeName"].Visible = true;
                grdList.Columns["lMaxDays"].Visible = true;
                grdList.Columns["lGender"].Visible = false;
                grdList.Columns["DaysAhead"].Visible = false;
                grdList.Columns["DaysDelayed"].Visible = false;
                grdList.Columns["Carryforward"].Visible = false;
            }
        }

        private void btnLomCancel_Click(object sender, EventArgs e)
        {
            try
            {
                closeAllPanels();
                clearData();
                grdList.Visible = true;
                pnlLeaveList.Visible = true;
                btnNew.Visible = true;
                enableBottomButtons();
            }
            catch (Exception)
            {

            }
        }

        private void btnLslCancel_Click(object sender, EventArgs e)
        {
            try
            {
                closeAllPanels();
                clearData();
                pnlLeaveList.Visible = true;
                btnNew.Visible = true;
                grdList.Visible = true;
                enableBottomButtons();
            }
            catch (Exception)
            {

            }
        }

        private void btnLslSave_Click(object sender, EventArgs e)
        {
            try
            {
                Leave leave = new Leave();
                LeaveSettingsdb leaveDB = new LeaveSettingsdb();
                if (option == 2)
                {

                    leave.leaveID = ((Structures.ComboBoxItem)cmbLslLeaveID.SelectedItem).HiddenValue;
                    leave.designation = ((Structures.ComboBoxItem)cmbLslDesignation.SelectedItem).HiddenValue;
                    leave.MaxAccrual = Convert.ToInt32(txtLslMaxSanctionLimit.Text.ToString());
                    if (leaveDB.ValidateLeaveSanctionLimit(leave))
                    {
                        if (btnLslSave.Text.Equals("Update"))
                        {
                            if (prevdesig != cmbLslDesignation.SelectedItem.ToString())
                            {
                                if (leaveDB.validateSanctionLimitList(leave))
                                {
                                    leave.rowid = rowid;
                                    if (leaveDB.UpdateLeaveSanctionLimit(leave))
                                    {
                                        MessageBox.Show("LeaveSanctionLimit updated");
                                        closeAllPanels();
                                        leavesanctionlimit();
                                    }
                                    else
                                    {
                                        MessageBox.Show("Failed to update LeaveSanctionLimit");
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Sanction Limit for this data Already Exists!!!");
                                }
                            }
                            else
                            {
                                leave.rowid = rowid;
                                if (leaveDB.UpdateLeaveSanctionLimit(leave))
                                {
                                    MessageBox.Show("LeaveSanctionLimit updated");
                                    closeAllPanels();
                                    leavesanctionlimit();
                                }
                                else
                                {
                                    MessageBox.Show("Failed to update LeaveSanctionLimit");
                                }
                            }

                        }
                        else if (btnLslSave.Text.Equals("Save"))
                        {
                            if (leaveDB.validateSanctionLimitList(leave))
                            {
                                if (leaveDB.InsertLeaveSanctionLimit(leave))
                                {
                                    MessageBox.Show("LeaveSanctionLimit Value Added");
                                    closeAllPanels();
                                    leavesanctionlimit();
                                }
                                else
                                {
                                    MessageBox.Show("Failed to Insert LeaveSanctionLimit");
                                }
                            }
                            else
                            {
                                MessageBox.Show("Sanction Limit for this data Already Exists!!!");
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("LeaveSanctionLimit Data Validation failed");
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Failed Adding / Editing User Data");
            }
        }

        private void btnLomSave_Click(object sender, EventArgs e)
        {
            try
            {
                Leave leave = new Leave();
                LeaveSettingsdb leaveDB = new LeaveSettingsdb();
                if (option == 3)
                {
                    leave.leaveID = ((Structures.ComboBoxItem)cmbLomLeaveID.SelectedItem).HiddenValue;
                    leave.officeID = ((Structures.ComboBoxItem)cmbLomOffice.SelectedItem).HiddenValue;
                    leave.MaxDays = Convert.ToInt32(txtLomMaxDays.Text);
                    if (leaveDB.ValidateLeaveOfficeMapping(leave))
                    {
                        if (btnLomSave.Text.Equals("Update"))
                        {
                            if (prevoff != cmbLomOffice.SelectedItem.ToString())
                            {
                                if (leaveDB.Validatemapping(leave))
                                {
                                    leave.rowid = rowid;
                                    if (leaveDB.UpdateLeaveOfficeMapping(leave))
                                    {
                                        MessageBox.Show("LeaveOfficeMapping updated");
                                        closeAllPanels();
                                        leaveofficemapping();
                                    }
                                    else
                                    {
                                        MessageBox.Show("Failed to update LeaveOfficeMapping");
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Mapping Already Exists");
                                }
                            }
                            else
                            {
                                leave.rowid = rowid;
                                if (leaveDB.UpdateLeaveOfficeMapping(leave))
                                {
                                    MessageBox.Show("LeaveOfficeMapping updated");
                                    closeAllPanels();
                                    leaveofficemapping();
                                }
                                else
                                {
                                    MessageBox.Show("Failed to update LeaveOfficeMapping");
                                }
                            }
                        }
                        else if (btnLomSave.Text.Equals("Save"))
                        {
                            if (leaveDB.Validatemapping(leave))
                            {
                                if (leaveDB.InsertLeaveofficeMapping(leave))
                                {
                                    MessageBox.Show("LeaveOfficeMapping Value Added");
                                    closeAllPanels();
                                    leaveofficemapping();
                                }
                                else
                                {
                                    MessageBox.Show("Failed to Insert LeaveOfficeMapping");
                                }
                            }
                            else
                            {
                                MessageBox.Show("Mapping already Exists!!!!");
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("LeaveOfficeMapping Data Validation failed");
                    }
                }
            }
            catch (Exception)
            {

                MessageBox.Show("Failed Adding / Editing User Data");
            }
        }

        private void cmbCarryForward_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void LeaveSettings_Enter(object sender, EventArgs e)
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

