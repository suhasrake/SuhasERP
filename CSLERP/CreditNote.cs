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
using CSLERP.FileManager;
using CSLERP.PrintForms;

namespace CSLERP
{
    public partial class CreditNote : System.Windows.Forms.Form
    {
        string docID = "CREDITNOTE";
        string forwarderList = "";
        string approverList = "";
        string userString = "";
        string userCommentStatusString = "";
        Form dtpForm = new Form();
        string commentStatus = "";
        string SLType = "";
        int listOption = 1; //1-Pending, 2-In Process, 3-Approved
        Boolean userIsACommenter = false;
        DocEmpMappingDB demDB = new DocEmpMappingDB();
        DocumentReceiverDB drDB = new DocumentReceiverDB();
        DocCommenterDB docCmtrDB = new DocCommenterDB();
        CreditNoteHeader prevcnh ;
        ListView lvCopy = new ListView();
        System.Data.DataTable TaxDetailsTable = new System.Data.DataTable();
        System.Data.DataTable dtCmtStatus = new DataTable();
        //Panel pnldgv = new Panel(); // panel for gridview
        Panel pnlCmtr = new Panel();
        Panel pnlForwarder = new Panel();
        DataGridView dgvComments = new DataGridView();
        ListView lvCmtr = new ListView(); // list view for choice / selection list
        ListView lvApprover = new ListView();
        Panel pnllv = new Panel(); // panel for listview
        ListView lv = new ListView(); // list view for choice / selection list
        TextBox txtSearch = new TextBox();
        //Panel pnlModel = new Panel();
        Form frmPopup = new Form();
        public CreditNote()
        {
            try
            {
                InitializeComponent();

            }
            catch (Exception ex)
            {

            }
        }

        private void CreditNote_Load(object sender, EventArgs e)
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
            ListFilteredCreditNoteHeader(listOption);
            //tabControl1.SelectedIndexChanged += new EventHandler(TabControl1_SelectedIndexChanged);
        }
        private void ListFilteredCreditNoteHeader(int option)
        {
            try
            {
                grdList.Rows.Clear();
                CreditNoteDB CNDb = new CreditNoteDB();
                forwarderList = demDB.getForwarders(docID, Login.empLoggedIn);
                approverList = demDB.getApprovers(docID, Login.empLoggedIn);

                List<CreditNoteHeader> CNheaders = CNDb.getFilteredCreditNoteHeader(userString, option, userCommentStatusString);
                if (option == 1)
                    lblActionHeader.Text = "List of Action Pending Documents";
                else if (option == 2)
                    lblActionHeader.Text = "List of In-Process Documents";
                else if (option == 3 || option == 6)
                    lblActionHeader.Text = "List of Approved Documents";
                foreach (CreditNoteHeader cnh in CNheaders)
                {
                    if (option == 1)
                    {
                        if (cnh.DocumentStatus == 99)
                            continue;
                    }
                    else
                    {

                    }
                    grdList.Rows.Add();
                    grdList.Rows[grdList.RowCount - 1].Cells["gDocumentID"].Value = cnh.DocumentID;
                    grdList.Rows[grdList.RowCount - 1].Cells["gTemporaryNo"].Value = cnh.TemporaryNo;
                    grdList.Rows[grdList.RowCount - 1].Cells["gTemporaryDate"].Value = cnh.TemporaryDate;
                    grdList.Rows[grdList.RowCount - 1].Cells["CreditNoteNo"].Value = cnh.CreditNoteNo;
                    grdList.Rows[grdList.RowCount - 1].Cells["CreditNoteDate"].Value = cnh.CreditNoteDate;
                    grdList.Rows[grdList.RowCount - 1].Cells["AccountCredit"].Value = cnh.AccountCredit;
                    grdList.Rows[grdList.RowCount - 1].Cells["AccountCreditName"].Value = cnh.AccountCreditName;
                    grdList.Rows[grdList.RowCount - 1].Cells["AmountCredit"].Value = cnh.AmountCredit;
                    grdList.Rows[grdList.RowCount - 1].Cells["gSLType"].Value = cnh.SLType;
                    grdList.Rows[grdList.RowCount - 1].Cells["SLCode"].Value = cnh.SLCode;
                    grdList.Rows[grdList.RowCount - 1].Cells["SLName"].Value = cnh.SLName;
                    grdList.Rows[grdList.RowCount - 1].Cells["ReferenceNo"].Value = cnh.ReferenceNo;
                    grdList.Rows[grdList.RowCount - 1].Cells["ReferenceDate"].Value = cnh.ReferenceDate;
                    grdList.Rows[grdList.RowCount - 1].Cells["Narration"].Value = cnh.Narration;
                    grdList.Rows[grdList.RowCount - 1].Cells["gCreateUser"].Value = cnh.CreateUser;
                    grdList.Rows[grdList.RowCount - 1].Cells["FrowardUser"].Value = cnh.ForwardUser;
                    grdList.Rows[grdList.RowCount - 1].Cells["ApproveUser"].Value = cnh.ApproveUser;
                    grdList.Rows[grdList.RowCount - 1].Cells["Creator"].Value = cnh.CreatorName;
                    grdList.Rows[grdList.RowCount - 1].Cells["Forwarder"].Value = cnh.ForwarderName;
                    grdList.Rows[grdList.RowCount - 1].Cells["Approver"].Value = cnh.ApproverName;
                    grdList.Rows[grdList.RowCount - 1].Cells["gCreateTime"].Value = cnh.CreateTime;
                    grdList.Rows[grdList.RowCount - 1].Cells["gStatus"].Value = cnh.status;
                    grdList.Rows[grdList.RowCount - 1].Cells["gDocumentStatus"].Value = cnh.DocumentStatus;
                    grdList.Rows[grdList.RowCount - 1].Cells["gCreateUser"].Value = cnh.CreateUser;
                    //grdList.Rows[grdList.RowCount - 1].Cells["gDocumentStatusNo"].Value = cnh.DocumentStatus;
                    grdList.Rows[grdList.RowCount - 1].Cells["CmtStatus"].Value = cnh.CommentStatus;
                    grdList.Rows[grdList.RowCount - 1].Cells["Comments"].Value = cnh.Comments;
                    grdList.Rows[grdList.RowCount - 1].Cells["Forwarders"].Value = cnh.ForwarderList;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in CreditNOte Listing");
            }
            setButtonVisibility("init");
            pnlList.Visible = true;

            grdList.Columns["Creator"].Visible = true;
            grdList.Columns["Forwarder"].Visible = true;
            grdList.Columns["Approver"].Visible = true;

        }

        //called only in the beginning
        private void initVariables()
        {

            if (getuserPrivilegeStatus() == 1)
            {
                //user is only a viewer
                listOption = 6;
            }
            docID = Main.currentDocument;
            dtTempDate.Format = DateTimePickerFormat.Custom;
            dtTempDate.CustomFormat = "dd-MM-yyyy";
            dtCreditNoteDate.Format = DateTimePickerFormat.Custom;
            dtCreditNoteDate.CustomFormat = "dd-MM-yyyy";
            dtRefDate.Format = DateTimePickerFormat.Custom;
            dtRefDate.CustomFormat = "dd-MM-yyyy";
            btnForward.Visible = false;
            btnApprove.Visible = false;
            pnlUI.Controls.Add(pnlAddEdit);
            closeAllPanels();
            txtCreditNOteNo.TabIndex = 1;
            dtCreditNoteDate.TabIndex = 2;
            //dtDCDate.TabIndex = 3;
            grdPRDetail.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            userString = Login.userLoggedInName + Main.delimiter1 + Login.userLoggedIn + Main.delimiter1 + Main.delimiter2;
            userCommentStatusString = Login.userLoggedInName + Main.delimiter1 + Login.userLoggedIn + Main.delimiter1 + "0" + Main.delimiter2;
            setButtonVisibility("init");
        }


        private void closeAllPanels()
        {
            try
            {
                pnlList.Visible = false;
                pnlAddEdit.Visible = false;
            }
            catch (Exception)
            {
            }
        }

        //called when new,cancel buttons are clicked.
        //called at the end of event processing for forward, approve,reverse and save
        public void clearData()
        {
            try
            {
                //clear all grid views
                grdPRDetail.Rows.Clear();
                dgvComments.Rows.Clear();
                //----------clear temperory panels
                clearTabPageControls();
                pnlCmtr.Visible = false;
                pnlForwarder.Visible = false;
                pnllv.Visible = false;

                txtTemporarryNo.Text = "";
                txtCreditNOteNo.Text = "";
                dtTempDate.Value = DateTime.Parse("01-01-1900");
                dtCreditNoteDate.Value = DateTime.Parse("01-01-1900");
                txtTotalAmountDebit.Text = "";
                txtAccCode.Text = "";
                txtAccName.Text = "";
                txtRefNo.Text = "";
                dtRefDate.Value = DateTime.Parse("01-01-1900");
                txtnarration.Text = "";
                txtPartyID.Text = "";
                txtPartyname.Text = "";
                txtTotalAmountDebit.Text = "";
                txtamntINWord.Text = "";
                prevcnh = new CreditNoteHeader();
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

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                clearData();
                btnSave.Text = "Save";
                pnlAddEdit.Visible = true;
                closeAllPanels();
                pnlAddEdit.Visible = true;
                setButtonVisibility("btnNew");
            }
            catch (Exception)
            {

            }
        }
        private void btnAddLine_Click_2(object sender, EventArgs e)
        {
            try
            {
                if (grdPRDetail.Rows.Count > 0)
                {
                    if (!verifyAndReworkCreditNoteDetailGridRows())
                    {
                        return;
                    }
                }
                AddPRDetailRow();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this.ToString() + "-" + System.Reflection.MethodBase.GetCurrentMethod().Name + "() : Error");
            }
        }
        private Boolean AddPRDetailRow()
        {
            Boolean status = true;
            try
            {

                grdPRDetail.Rows.Add();
                int kount = grdPRDetail.RowCount;
                grdPRDetail.Rows[kount - 1].Cells["LineNo"].Value = kount;
                grdPRDetail.Rows[grdPRDetail.RowCount - 1].Cells["AccountCode"].Value = "";
                grdPRDetail.Rows[grdPRDetail.RowCount - 1].Cells["AccountName"].Value = "";
                grdPRDetail.Rows[grdPRDetail.RowCount - 1].Cells["Amount"].Value = 0;
            }

            catch (Exception ex)
            {
                MessageBox.Show("AddPRDetailRow() : Error");
            }

            return status;
        }
        private Boolean verifyAndReworkCreditNoteDetailGridRows()
        {
            Boolean status = true;

            try
            {
                if (grdPRDetail.Rows.Count <= 0)
                {
                    MessageBox.Show("No entries in Credit Note details");
                    return false;
                }
                if (!validateItems())
                {
                    MessageBox.Show("Validation failed");
                    return false;
                }
                double amntDebit = 0;
                for (int i = 0; i < grdPRDetail.Rows.Count; i++)
                {
                    //if (((Convert.ToDecimal(grdPRDetail.Rows[i].Cells["AmountDebit"].Value) != 0) &&
                    //   (Convert.ToDecimal(grdPRDetail.Rows[i].Cells["AmountCredit"].Value) != 0)) ||
                    //       ((Convert.ToDecimal(grdPRDetail.Rows[i].Cells["AmountDebit"].Value) == 0) &&
                    //   (Convert.ToDecimal(grdPRDetail.Rows[i].Cells["AmountCredit"].Value) == 0)))

                    //{
                    //    MessageBox.Show("Enter either Debrit or Credit Row:" + (i + 1));
                    //    return false;
                    //}
                    grdPRDetail.Rows[i].Cells["LineNo"].Value = (i + 1);
                    if ((grdPRDetail.Rows[i].Cells["AccountCode"].Value.ToString().Trim().Length == 0) ||
                        (Convert.ToDecimal(grdPRDetail.Rows[i].Cells["Amount"].Value) == 0))
                    {
                        MessageBox.Show("Fill values in row " + (i + 1));
                        return false;
                    }
                    if (grdPRDetail.Rows[i].Cells["AccountCode"].Value.ToString().Equals(txtAccCode.Text))
                    {
                        MessageBox.Show("Credit Acccount Code and Debit Account Code should not be equal. Row: " + (i + 1));
                        return false;
                    }
                    //if ((Convert.ToDecimal(grdPRDetail.Rows[i].Cells["AmountDebit"].Value) != 0))
                    //    debitCount++;
                    //else
                    //    creditCount++;
                    amntDebit = amntDebit + Convert.ToDouble(grdPRDetail.Rows[i].Cells["Amount"].Value);
                }
                //if((grdPRDetail.Rows.Count != 1) && (debitCount > 1 && creditCount > 1))
                //{
                //    MessageBox.Show("Check the debit and Credit columns in detail.");
                //    return false;
                //}
                txtTotalAmountDebit.Text = amntDebit.ToString();
                txtamntINWord.Text = NumberToString.convert(amntDebit.ToString());
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
                for (int i = 0; i < grdPRDetail.Rows.Count - 1; i++)
                {
                    for (int j = i + 1; j < grdPRDetail.Rows.Count; j++)
                    {

                        if (grdPRDetail.Rows[i].Cells[1].Value.ToString() == grdPRDetail.Rows[j].Cells["AccountCode"].Value.ToString())
                        {
                            MessageBox.Show("AccountCode code duplicated.");
                            return false;
                        }
                    }

                }
            }
            catch (Exception)
            {
                status = false;
            }
            return status;
        }
        private List<CreditNoteDetail> getCreditNoteDetails(CreditNoteHeader cnh)
        {
            CreditNoteDetail cnd = new CreditNoteDetail();

            List<CreditNoteDetail> CNDetails = new List<CreditNoteDetail>();
            for (int i = 0; i < grdPRDetail.Rows.Count; i++)
            {
                try
                {
                    cnd = new CreditNoteDetail();
                    cnd.DocumentID = cnh.DocumentID;
                    cnd.TemporaryNo = cnh.TemporaryNo;
                    cnd.TemporaryDate = cnh.TemporaryDate;
                    cnd.AccountDebit = grdPRDetail.Rows[i].Cells["AccountCode"].Value.ToString();
                    if (grdPRDetail.Rows[i].Cells["Amount"].Value != null)
                        cnd.AmountDebit = Convert.ToDecimal(grdPRDetail.Rows[i].Cells["Amount"].Value.ToString().Trim());
                    else
                        cnd.AmountDebit = 0;

                    CNDetails.Add(cnd);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("createAndUpdatePRDetails() : Error creating Journal Details");
                    //status = false;
                }
            }
            return CNDetails;
        }
        private void btnActionPending_Click(object sender, EventArgs e)
        {
            listOption = 1;
            ListFilteredCreditNoteHeader(listOption);
        }

        private void btnApprovalPending_Click(object sender, EventArgs e)
        {
            listOption = 2;
            ListFilteredCreditNoteHeader(listOption);
        }

        private void btnApproved_Click(object sender, EventArgs e)
        {
            if (getuserPrivilegeStatus() == 2)
            {
                listOption = 6; //viewer
            }
            else
            {
                listOption = 3;
            }

            ListFilteredCreditNoteHeader(listOption);
        }
        private void btnSave_Click_1(object sender, EventArgs e)
        {
            Boolean status = true;
            try
            {

                CreditNoteDB CNDB = new CreditNoteDB();
                CreditNoteHeader cnh = new CreditNoteHeader();
                System.Windows.Forms.Button btn = sender as System.Windows.Forms.Button;
                string btnText = btnSave.Text;
                if (!verifyAndReworkCreditNoteDetailGridRows())
                {
                    MessageBox.Show("Validation Failed in Journal voucher Detail");
                    return;
                }
                cnh.DocumentID = docID;
                cnh.CreditNoteDate = dtCreditNoteDate.Value;
                cnh.ReferenceDate = dtRefDate.Value;
                cnh.ReferenceNo = txtRefNo.Text;
                cnh.SLType = SLType;
                cnh.SLCode = txtPartyID.Text;
                cnh.SLName = txtPartyname.Text;
                cnh.AccountCredit = txtAccCode.Text;
                cnh.AccountCreditName = txtAccName.Text;
                cnh.Narration = txtnarration.Text;
                cnh.Comments = docCmtrDB.DGVtoString(dgvComments);
                cnh.ForwarderList = prevcnh.ForwarderList;
                cnh.AmountCredit = Convert.ToDecimal(txtTotalAmountDebit.Text);
                if (!CNDB.validateCreditNoteHeader(cnh))
                {
                    MessageBox.Show("Validation failed");
                    return;
                }
                if (btnText.Equals("Save"))
                {
                    //cnh.TemporaryNo = DocumentNumberDB.getNewNumber(docID, 1);
                    cnh.DocumentStatus = 1; //created
                    cnh.TemporaryDate = UpdateTable.getSQLDateTime();
                }
                else
                {
                    cnh.TemporaryNo = Convert.ToInt32(txtTemporarryNo.Text);
                    cnh.TemporaryDate = prevcnh.TemporaryDate;
                    cnh.DocumentStatus = prevcnh.DocumentStatus;

                }

                if (CNDB.validateCreditNoteHeader(cnh))
                {
                    //--create comment status string
                    docCmtrDB = new DocCommenterDB();
                    if (userIsACommenter)
                    {
                        //if the user is only a commenter and ticked the comment as final; then update his comment string as final
                        //and update the comment status
                        if (chkCommentStatus.Checked)
                        {
                            docCmtrDB = new DocCommenterDB();
                            cnh.CommentStatus = docCmtrDB.createCommentStatusString(prevcnh.CommentStatus, Login.userLoggedIn);
                        }
                        else
                        {
                            cnh.CommentStatus = prevcnh.CommentStatus;
                        }
                    }
                    else
                    {
                        if (commentStatus.Trim().Length > 0)
                        {
                            //clicked the Get Commenter button
                            cnh.CommentStatus = docCmtrDB.createCommenterList(lvCmtr, dtCmtStatus);
                        }
                        else
                        {
                            cnh.CommentStatus = prevcnh.CommentStatus;
                        }
                    }
                    //----------
                    int tmpStatus = 0;
                    if (chkCommentStatus.Checked)
                    {
                        tmpStatus = 1;
                    }
                    if (txtComments.Text.Trim().Length > 0)
                    {
                        cnh.Comments = docCmtrDB.processNewComment(dgvComments, txtComments.Text, Login.userLoggedIn, Login.userLoggedInName, tmpStatus);
                    }
                    //if (Convert.ToInt32(txtTotalAmountCredit.Text) != Convert.ToInt32(txtTotalAmountDebit.Text))
                    //{
                    //    MessageBox.Show("Debit and Credit total should be equal.");
                    //    return;
                    //}
                    List<CreditNoteDetail> CNDetails = getCreditNoteDetails(cnh);
                    if (btnText.Equals("Update"))
                    {
                        if (CNDB.updateCreditHeaderAndDetail(cnh, prevcnh,CNDetails))
                        {
                            MessageBox.Show("Credit Note Details updated");
                            closeAllPanels();
                            listOption = 1;
                            ListFilteredCreditNoteHeader(listOption);
                        }
                        else
                        {
                            status = false;
                        }
                        if (!status)
                        {
                            MessageBox.Show("Failed to update Credit NOte Header");
                        }
                    }

                    else if (btnText.Equals("Save"))
                    {
                        if (CNDB.InsertCreditHeaderAndDetail(cnh, CNDetails))
                        {
                            MessageBox.Show("Credit Note Added");
                            closeAllPanels();
                            listOption = 1;
                            ListFilteredCreditNoteHeader(listOption);
                        }
                        else
                        {
                            status = false;
                        }
                        if (!status)
                        {
                            MessageBox.Show("Failed to Insert Credit NOte  Header");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Credit Note Details Validation failed");
                    status = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("btnSave_Click_1() : Error");
                status = false;
            }
            if (status)
            {
                setButtonVisibility("btnEditPanel"); //activites are same for cancel, forward,approve, reverse and save
            }
        }

        private void btnCancel_Click_1(object sender, EventArgs e)
        {
            clearData();
            closeAllPanels();
            pnlList.Visible = true;
            setButtonVisibility("btnEditPanel");
        }
        private void btnCalculateax_Click_1(object sender, EventArgs e)
        {
            verifyAndReworkCreditNoteDetailGridRows();
        }

        private void grdList_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0)
                    return;
                string columnName = grdList.Columns[e.ColumnIndex].Name;
                if (columnName.Equals("Edit") || columnName.Equals("Approve") || columnName.Equals("LoadDocument") ||
                    columnName.Equals("View"))
                {

                    clearData();
                    setButtonVisibility(columnName);
                    prevcnh = new CreditNoteHeader();
                    prevcnh.CommentStatus = grdList.Rows[e.RowIndex].Cells["CmtStatus"].Value.ToString();
                    prevcnh.DocumentID = grdList.Rows[e.RowIndex].Cells["gDocumentID"].Value.ToString();
                    prevcnh.TemporaryNo = Convert.ToInt32(grdList.Rows[e.RowIndex].Cells["gTemporaryNo"].Value.ToString());
                    prevcnh.TemporaryDate = DateTime.Parse(grdList.Rows[e.RowIndex].Cells["gTemporaryDate"].Value.ToString());
                    if (prevcnh.CommentStatus.IndexOf(userCommentStatusString) >= 0)
                    {
                        // only for commeting and viwing documents
                        userIsACommenter = true;
                        setButtonVisibility("Commenter");
                    }
                    prevcnh.Comments = CreditNoteDB.getUserComments(prevcnh.DocumentID, prevcnh.TemporaryNo, prevcnh.TemporaryDate);
                    docID = grdList.Rows[e.RowIndex].Cells[0].Value.ToString();

                    CreditNoteDB cndb = new CreditNoteDB();
                    int rowID = e.RowIndex;
                    btnSave.Text = "Update";
                    DataGridViewRow row = grdList.Rows[rowID];

                    prevcnh.CreditNoteNo = Convert.ToInt32(grdList.Rows[e.RowIndex].Cells["CreditNoteNo"].Value.ToString());
                    prevcnh.CreditNoteDate = DateTime.Parse(grdList.Rows[e.RowIndex].Cells["CreditNoteDate"].Value.ToString());
                    //--------Load Documents
                    if (columnName.Equals("LoadDocument"))
                    {
                        string hdrString = "Document Temp No:" + prevcnh.TemporaryNo + "\n" +
                            "Document Temp Date:" + prevcnh.TemporaryDate.ToString("dd-MM-yyyy") + "\n" +
                            "Credit Note No:" + prevcnh.CreditNoteNo + "\n" +
                            "Credit Note  Date:" + prevcnh.CreditNoteDate.ToString("dd-MM-yyyy");
                        string dicDir = Main.documentDirectory + "\\" + docID;
                        string subDir = prevcnh.TemporaryNo + "-" + prevcnh.TemporaryDate.ToString("yyyyMMddhhmmss");
                        FileManager.LoadDocuments load = new FileManager.LoadDocuments(dicDir, subDir, docID, hdrString);
                        load.ShowDialog();
                        this.RemoveOwnedForm(load);
                        btnCancel_Click_1(null, null);
                        return;
                    }
                    //--------
                    prevcnh.Narration = grdList.Rows[e.RowIndex].Cells["Narration"].Value.ToString();
                    prevcnh.AmountCredit = Convert.ToDecimal(grdList.Rows[e.RowIndex].Cells["AmountCredit"].Value.ToString());
                    prevcnh.AccountCredit = grdList.Rows[e.RowIndex].Cells["AccountCredit"].Value.ToString();
                    prevcnh.AccountCreditName = grdList.Rows[e.RowIndex].Cells["AccountCreditName"].Value.ToString();
                    prevcnh.SLCode = grdList.Rows[e.RowIndex].Cells["SLCode"].Value.ToString();
                    prevcnh.SLName = grdList.Rows[e.RowIndex].Cells["SLName"].Value.ToString();
                    SLType = grdList.Rows[e.RowIndex].Cells["gSLType"].Value.ToString();
                    prevcnh.SLType = SLType;
                    prevcnh.ReferenceNo = grdList.Rows[e.RowIndex].Cells["ReferenceNo"].Value.ToString();
                    prevcnh.ReferenceDate = DateTime.Parse(grdList.Rows[e.RowIndex].Cells["ReferenceDate"].Value.ToString());

                    prevcnh.status = Convert.ToInt32(grdList.Rows[e.RowIndex].Cells["gStatus"].Value.ToString());
                    prevcnh.DocumentStatus = Convert.ToInt32(grdList.Rows[e.RowIndex].Cells["gDocumentStatus"].Value.ToString());
                    prevcnh.CreateTime = DateTime.Parse(grdList.Rows[e.RowIndex].Cells["gCreateTime"].Value.ToString());
                    prevcnh.CreateUser = grdList.Rows[e.RowIndex].Cells["gCreateUser"].Value.ToString();
                    prevcnh.CreatorName = grdList.Rows[e.RowIndex].Cells["Creator"].Value.ToString();
                    prevcnh.ForwarderName = grdList.Rows[e.RowIndex].Cells["Forwarder"].Value.ToString();
                    prevcnh.ApproverName = grdList.Rows[e.RowIndex].Cells["Approver"].Value.ToString();
                    prevcnh.ForwarderList = grdList.Rows[e.RowIndex].Cells["Forwarders"].Value.ToString();
                    //--comments
                    chkCommentStatus.Checked = false;
                    docCmtrDB = new DocCommenterDB();
                    pnlComments.Controls.Remove(dgvComments);
                    prevcnh.CommentStatus = grdList.Rows[e.RowIndex].Cells["CmtStatus"].Value.ToString();

                    dtCmtStatus = docCmtrDB.splitCommentStatus(prevcnh.CommentStatus);
                    dgvComments = new DataGridView();
                    dgvComments = docCmtrDB.createCommentGridview(prevcnh.Comments);
                    pnlComments.Controls.Add(dgvComments);
                    txtComments.Text = docCmtrDB.getLastUnauthorizedComment(dgvComments, Login.userLoggedIn);
                    dgvComments.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvComments_CellDoubleClick);

                    //---
                    txtTemporarryNo.Text = prevcnh.TemporaryNo.ToString();
                    dtTempDate.Value = prevcnh.TemporaryDate;
                    txtAccCode.Text = prevcnh.AccountCredit;
                    txtAccName.Text = prevcnh.AccountCreditName;
                    txtPartyID.Text = prevcnh.SLCode;
                    txtPartyname.Text = prevcnh.SLName;
                    txtRefNo.Text = prevcnh.ReferenceNo;
                    dtRefDate.Value = prevcnh.ReferenceDate;
                    //txta
                    txtCreditNOteNo.Text = prevcnh.CreditNoteNo.ToString();
                    try
                    {
                        dtCreditNoteDate.Value = prevcnh.CreditNoteDate;
                    }
                    catch (Exception)
                    {
                        dtCreditNoteDate.Value = DateTime.Parse("01-01-1900");
                    }
                    txtnarration.Text = prevcnh.Narration.ToString();
                    decimal totDebit = 0;
                    decimal totCredit = 0;
                    List<CreditNoteDetail> CNDetail = CreditNoteDB.getCreditNoteDetail(prevcnh);
                    grdPRDetail.Rows.Clear();
                    int i = 0;
                    try
                    {
                        foreach (CreditNoteDetail cnd in CNDetail)
                        {
                            AddPRDetailRow();
                            grdPRDetail.Rows[i].Cells["AccountCode"].Value = cnd.AccountDebit;
                            grdPRDetail.Rows[i].Cells["AccountName"].Value = cnd.AccountDebitName;
                            grdPRDetail.Rows[i].Cells["Amount"].Value = cnd.AmountDebit;

                            totDebit = totDebit + cnd.AmountDebit;
                            i++;
                        }
                        txtTotalAmountDebit.Text = totDebit.ToString();
                        txtamntINWord.Text = NumberToString.convert(totDebit.ToString());
                    }
                    catch (Exception ex)
                    {
                    }

                }
                else
                {
                    return;
                }
                btnSave.Text = "Update";
                pnlList.Visible = false;
                pnlAddEdit.Visible = true;
                tabControl1.SelectedTab = txbCNHeader;
                tabControl1.Visible = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show("error");
            }
        }
        private void btnClearEntries_Click_1(object sender, EventArgs e)
        {
            try
            {
                for (int i = grdPRDetail.Rows.Count - 1; i >= 0; i--)
                {
                    grdPRDetail.Rows.RemoveAt(i);
                }
            }
            catch (Exception)
            {
            }
            MessageBox.Show("item cleared.");
        }

        private void btnForward_Click_1(object sender, EventArgs e)
        {
            //removeControlsFromForwarderPanel();
            //lvApprover = new ListView();
            //lvApprover.Clear();
            //pnlForwarder.BorderStyle = BorderStyle.FixedSingle;
            //pnlForwarder.Bounds = new Rectangle(new Point(100, 10), new Size(700, 300));

            frmPopup = new Form();
            frmPopup.StartPosition = FormStartPosition.CenterScreen;
            frmPopup.BackColor = Color.CadetBlue;

            frmPopup.MaximizeBox = false;
            frmPopup.MinimizeBox = false;
            frmPopup.ControlBox = false;
            frmPopup.FormBorderStyle = FormBorderStyle.FixedSingle;
            frmPopup.Size = new Size(450, 300);

            lvApprover = DocEmpMappingDB.ApproverLV(docID, Login.empLoggedIn);
            lvApprover.Bounds = new Rectangle(new Point(0, 0), new Size(450, 250));
            //pnlForwarder.Controls.Remove(lvApprover);
            frmPopup.Controls.Add(lvApprover);

            Button lvForwrdOK = new Button();
            lvForwrdOK.BackColor = Color.Tan;
            lvForwrdOK.Text = "OK";
            lvForwrdOK.Location = new Point(40, 265);
            lvForwrdOK.Click += new System.EventHandler(this.lvForwardOK_Click);
            frmPopup.Controls.Add(lvForwrdOK);

            Button lvForwardCancel = new Button();
            lvForwardCancel.BackColor = Color.Tan;
            lvForwardCancel.Text = "CANCEL";
            lvForwardCancel.Location = new Point(130, 265);
            lvForwardCancel.Click += new System.EventHandler(this.lvForwardCancel_Click);
            frmPopup.Controls.Add(lvForwardCancel);
            ////lvForwardCancel.Visible = false;

            //pnlForwarder.Visible = true;
            //pnlAddEdit.Controls.Add(pnlForwarder);
            //pnlAddEdit.BringToFront();
            //pnlForwarder.BringToFront();

            frmPopup.ShowDialog();
            //pnlForwarder.Focus();

        }

        private void btnApprove_Click_1(object sender, EventArgs e)
        {
            try
            {
                CreditNoteDB cnDB = new CreditNoteDB();
                //FinancialLimitDB flDB = new FinancialLimitDB();
                //if (!flDB.checkEmployeeFinancialLimit(docID, Login.empLoggedIn, Convert.ToDouble(txtvoucherAmountINR.Text), 0))
                //{
                //    MessageBox.Show("No financial power for approving this document");
                //    return;
                //}
                DialogResult dialog = MessageBox.Show("Are you sure to Approve the document ?", "Yes", MessageBoxButtons.YesNo);
                if (dialog == DialogResult.Yes)
                {
                    prevcnh.CommentStatus = DocCommenterDB.removeUnapprovedCommentStatus(prevcnh.CommentStatus);
                    prevcnh.CreditNoteNo = DocumentNumberDB.getNewNumber(docID, 2);
                    if (verifyAndReworkCreditNoteDetailGridRows())
                    {
                        if (cnDB.ApproveCreditNoteHeader(prevcnh))
                        {
                            MessageBox.Show("Credit note Document Approved");
                            if (!updateDashBoard(prevcnh, 2))
                            {
                                MessageBox.Show("DashBoard Fail to update");
                            }
                            closeAllPanels();
                            listOption = 1;
                            ListFilteredCreditNoteHeader(listOption);
                            setButtonVisibility("btnEditPanel"); //activites are same for cance, forward,approce and reverse
                        }
                        else
                            MessageBox.Show("Unable to approve");
                    }
                }
            }
            catch (Exception)
            {
            }

        }
        private void cmbCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //removeControlsFromCommenterPanel();
            //removeControlsFromForwarderPanel();
            //removeControlsFromLVPanel();
            
        }
        private void pnlList_Paint(object sender, PaintEventArgs e)
        {

        }
        //-----
        //comment handling procedurs and functions
        //-----
        private void btnGetComments_Click(object sender, EventArgs e)
        {
            //removeControlsFromCommenterPanel();

            //lvCmtr = new ListView();
            //lvCmtr.Clear();
            //pnlCmtr.BorderStyle = BorderStyle.FixedSingle;
            //pnlCmtr.Bounds = new Rectangle(new Point(100, 10), new Size(700, 300));
            docCmtrDB = new DocCommenterDB();
            frmPopup = new Form();
            frmPopup.StartPosition = FormStartPosition.CenterScreen;
            frmPopup.BackColor = Color.CadetBlue;

            frmPopup.MaximizeBox = false;
            frmPopup.MinimizeBox = false;
            frmPopup.ControlBox = false;
            frmPopup.FormBorderStyle = FormBorderStyle.FixedSingle;

            frmPopup.Size = new Size(450, 300);
            lvCmtr = docCmtrDB.commenterLV(docID);
            docCmtrDB.verifyCommenterList(lvCmtr, dtCmtStatus);
            lvCmtr.Bounds = new Rectangle(new Point(0, 0), new Size(450, 250));
            frmPopup.Controls.Add(lvCmtr);

            Button lvOK = new Button();
            lvOK.BackColor = Color.Tan;
            lvOK.Text = "OK";
            lvOK.Location = new Point(40, 265);
            lvOK.Click += new System.EventHandler(this.lvOK_Click);
            frmPopup.Controls.Add(lvOK);

            Button lvCancel = new Button();
            lvCancel.BackColor = Color.Tan;
            lvCancel.Text = "CANCEL";
            lvCancel.Location = new Point(130, 265);
            lvCancel.Click += new System.EventHandler(this.lvCancel_Click);
            frmPopup.Controls.Add(lvCancel);

            frmPopup.ShowDialog();
            //pnlCmtr.BringToFront();
            //pnlCmtr.Visible = true;
            //pnlComments.Controls.Add(pnlCmtr);
            //pnlComments.BringToFront();
            //pnlCmtr.BringToFront();

        }
        private void lvOK_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show("Update the document for sending the comment requests");
                if (lvCmtr.CheckedItems.Count > 0)
                {
                    foreach (ListViewItem itemRow in lvCmtr.Items)
                    {
                        if (itemRow.Checked)
                        {
                            //MessageBox.Show(itemRow.SubItems[1].Text);
                            commentStatus = commentStatus + itemRow.SubItems[1].Text + Main.delimiter1 +
                                itemRow.SubItems[2].Text + Main.delimiter1 +
                                "0" + Main.delimiter1 + Main.delimiter2;
                        }
                    }
                }
                else
                {
                    //if the existing commenter are removed
                    commentStatus = "Cleared";
                }
                frmPopup.Close();
                frmPopup.Dispose();
            }
            catch (Exception)
            {
            }
        }

        private void lvCancel_Click(object sender, EventArgs e)
        {
            try
            {
                frmPopup.Close();
                frmPopup.Dispose();
            }
            catch (Exception)
            {
            }
        }
        private void lvForwardOK_Click(object sender, EventArgs e)
        {
            try
            {
                {
                    int kount = 0;
                    string approverUID = "";
                    string approverUName = "";
                    foreach (ListViewItem itemRow in lvApprover.Items)
                    {
                        if (itemRow.Checked)
                        {
                            approverUID = itemRow.SubItems[2].Text;
                            approverUName = itemRow.SubItems[1].Text;
                            kount++;
                        }
                    }
                    if (kount == 0)
                    {
                        MessageBox.Show("Select one approver");
                        return;
                    }
                    if (kount > 1)
                    {
                        MessageBox.Show("Select only one approver");
                        return;
                    }
                    else
                    {
                        DialogResult dialog = MessageBox.Show("Are you sure to forward the document ?", "Yes", MessageBoxButtons.YesNo);
                        if (dialog == DialogResult.Yes)
                        {
                            //do forward activities
                            CreditNoteDB cnhDB = new CreditNoteDB();
                            prevcnh.CommentStatus = DocCommenterDB.removeUnapprovedCommentStatus(prevcnh.CommentStatus);
                            prevcnh.ForwardUser = approverUID;
                            prevcnh.ForwarderList = prevcnh.ForwarderList + approverUName + Main.delimiter1 +
                                approverUID + Main.delimiter1 + Main.delimiter2;
                            if (cnhDB.forwardCreditNoteHeader(prevcnh))
                            {
                                frmPopup.Close();
                                frmPopup.Dispose();
                                MessageBox.Show("Document Forwarded");
                                if (!updateDashBoard(prevcnh, 1))
                                {
                                    MessageBox.Show("DashBoard Fail to update");
                                }
                                closeAllPanels();
                                listOption = 1;
                                ListFilteredCreditNoteHeader(listOption);
                                setButtonVisibility("btnEditPanel"); //activites are same for cance, forward,approce and reverse
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void lvForwardCancel_Click(object sender, EventArgs e)
        {
            try
            {
                //lvApprover.CheckBoxes = false;
                //lvApprover.CheckBoxes = true;
                //pnlForwarder.Controls.Remove(lvApprover);
                //pnlForwarder.Visible = false;
                frmPopup.Close();
                frmPopup.Dispose();
            }
            catch (Exception)
            {
            }
        }
        //-----
        private void dgvComments_CellDoubleClick(Object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0)
                    return;
                ////string columnName = grdList.Columns[e.ColumnIndex].Name;
                PrintForms.SimpleReportViewer.ShowDialog(dgvComments.Rows[e.RowIndex].Cells[3].Value.ToString(), "My Message", this);
            }
            catch (Exception ex)
            {
            }
        }
        private Boolean updateDashBoard(CreditNoteHeader cnh, int stat)
        {
            Boolean status = true;
            try
            {
                dashboardalarm dsb = new dashboardalarm();
                DashboardDB ddsDB = new DashboardDB();
                dsb.DocumentID = cnh.DocumentID;
                dsb.TemporaryNo = cnh.TemporaryNo;
                dsb.TemporaryDate = cnh.TemporaryDate;
                dsb.DocumentNo = cnh.CreditNoteNo;
                dsb.DocumentDate = cnh.CreditNoteDate;
                dsb.FromUser = Login.userLoggedIn;
                if (stat == 1)
                {
                    dsb.ActivityType = 2;
                    dsb.ToUser = cnh.ForwardUser;
                    if (!ddsDB.insertDashboardAlarm(dsb))
                    {
                        MessageBox.Show("DashBoard Fail to update");
                        status = false;
                    }
                }
                else if (stat == 2)
                {
                    dsb.ActivityType = 3;
                    List<documentreceiver> docList = DocumentReceiverDB.getDocumentWiseReceiver(prevcnh.DocumentID);
                    foreach (documentreceiver docRec in docList)
                    {
                        dsb.ToUser = docRec.EmployeeID;   //To store UserID Form DocumentReceiver for current Document
                        dsb.DocumentDate = UpdateTable.getSQLDateTime();
                        if (!ddsDB.insertDashboardAlarm(dsb))
                        {
                            MessageBox.Show("DashBoard Fail to update");
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
        private void disableTabPages()
        {
            foreach (TabPage tp in tabControl1.TabPages)
            {
                tp.Enabled = false; ;
            }
        }
        private void enableTabPages()
        {
            foreach (TabPage tp in tabControl1.TabPages)
            {
                tp.Enabled = true;
            }
        }

        //return the previous forward list and forwarder 
        private string getReverseString(string forwarderList)
        {
            string reverseString = "";
            try
            {
                string prevUser = "";
                string[] lst1 = forwarderList.Split(Main.delimiter2);
                for (int i = 0; i < lst1.Length - 1; i++)
                {
                    if (lst1[i].Trim().Length > 1)
                    {
                        string[] lst2 = lst1[i].Split(Main.delimiter1);

                        if (i == (lst1.Length - 2))
                        {
                            if (reverseString.Trim().Length > 0)
                            {
                                reverseString = reverseString + "!@#" + prevUser;
                            }
                        }
                        else
                        {
                            reverseString = reverseString + lst2[0] + Main.delimiter1 + lst2[1] + Main.delimiter1 + Main.delimiter2;
                            prevUser = lst2[1];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return reverseString;
        }

        private void btnReverse_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dialog = MessageBox.Show("Are you sure to Reverse the document ?", "Yes", MessageBoxButtons.YesNo);
                if (dialog == DialogResult.Yes)
                {
                    string s = prevcnh.ForwarderList;
                    string reverseStr = getReverseString(prevcnh.ForwarderList);
                    //do forward activities
                    prevcnh.CommentStatus = DocCommenterDB.removeUnapprovedCommentStatus(prevcnh.CommentStatus);
                    CreditNoteDB cnDB = new CreditNoteDB();
                    if (reverseStr.Trim().Length > 0)
                    {
                        int ind = reverseStr.IndexOf("!@#");
                        prevcnh.ForwarderList = reverseStr.Substring(0, ind);
                        prevcnh.ForwardUser = reverseStr.Substring(ind + 3);
                        prevcnh.DocumentStatus = prevcnh.DocumentStatus - 1;
                    }
                    else
                    {
                        prevcnh.ForwarderList = "";
                        prevcnh.ForwardUser = "";
                        prevcnh.DocumentStatus = 1;
                    }
                    if (cnDB.reverseCreditNoteHeader(prevcnh))
                    {
                        MessageBox.Show("Credit Note Document Reversed");
                        closeAllPanels();
                        listOption = 1;
                        ListFilteredCreditNoteHeader(listOption);
                        setButtonVisibility("btnEditPanel"); //activites are same for cance, forward,approce and reverse
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void showPDFFile(string fname)
        {
            try
            {
                removePDFControls();
                AxAcroPDFLib.AxAcroPDF pdf = new AxAcroPDFLib.AxAcroPDF();
                pdf.Dock = System.Windows.Forms.DockStyle.Fill;
                pdf.Enabled = true;
                pdf.Location = new System.Drawing.Point(0, 0);
                pdf.Name = "pdfReader";
                pdf.OcxState = pdf.OcxState;
                ////pdf.OcxState = ((System.Windows.Forms.AxHost.State)(new System.ComponentModel.ComponentResourceManager(typeof(ViewerWindow)).GetObject("pdf.OcxState")));
                pdf.TabIndex = 1;
                pnlPDFViewer.Controls.Add(pdf);

                pdf.setShowToolbar(false);
                pdf.LoadFile(fname);
                pdf.setView("Fit");
                pdf.Visible = true;
                pdf.setZoom(100);
                pdf.setPageMode("None");
            }
            catch (Exception ex)
            {
            }
        }
        private void btnPDFClose_Click(object sender, EventArgs e)
        {
            removePDFControls();
            showPDFFileGrid();
        }
        private void removePDFControls()
        {
            try
            {
                foreach (Control p in pnlPDFViewer.Controls)
                    if (p.GetType() == typeof(AxAcroPDFLib.AxAcroPDF))
                    {
                        AxAcroPDFLib.AxAcroPDF c = (AxAcroPDFLib.AxAcroPDF)p;
                        c.Visible = false;
                        pnlPDFViewer.Controls.Remove(c);
                        c.Dispose();
                    }
            }
            catch (Exception ex)
            {
            }
        }
        private void showPDFFileGrid()
        {
            try
            {
                foreach (Control p in pnlPDFViewer.Controls)
                    if (p.GetType() == typeof(DataGridView))
                    {
                        p.Visible = true;
                    }
            }
            catch (Exception ex)
            {
            }
        }
        private void removePDFFileGridView()
        {
            try
            {
                foreach (Control p in pnlPDFViewer.Controls)
                    if (p.GetType() == typeof(DataGridView))
                    {
                        p.Dispose();
                    }
            }
            catch (Exception ex)
            {
            }
        }
        private void removeControlsFromCommenterPanel()
        {
            try
            {
                //foreach (Control p in pnlCmtr.Controls)
                //    if (p.GetType() == typeof(ListView) || p.GetType() == typeof(Button))
                //    {
                //        p.Dispose();
                //    }
                pnlCmtr.Controls.Clear();
                Control nc = pnlCmtr.Parent;
                nc.Controls.Remove(pnlCmtr);
            }
            catch (Exception ex)
            {
            }
        }
        private void removeControlsFromForwarderPanel()
        {
            try
            {
                //foreach (Control p in pnlForwarder.Controls)
                //    if (p.GetType() == typeof(ListView) || p.GetType() == typeof(Button))
                //    {
                //        p.Dispose();
                //    }
                pnlForwarder.Controls.Clear();
                Control nc = pnlForwarder.Parent;
                nc.Controls.Remove(pnlForwarder);
            }
            catch (Exception ex)
            {
            }
        }
        //private void removeControlsFromLVPanel()
        //{
        //    try
        //    {
        //        //foreach (Control p in pnllv.Controls)
        //        //    if (p.GetType() == typeof(ListView) || p.GetType() == typeof(Button))
        //        //    {
        //        //        p.Dispose();
        //        //    }
        //        pnllv.Controls.Clear();
        //        Control nc = pnllv.Parent;
        //        nc.Controls.Remove(pnllv);
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}
        private void btnListDocuments_Click(object sender, EventArgs e)
        {
            try
            {
                removePDFFileGridView();
                removePDFControls();
                DataGridView dgvDocumentList = new DataGridView();
                pnlPDFViewer.Controls.Remove(dgvDocumentList);
                dgvDocumentList = DocumentStorageDB.getDocumentDetails(docID, prevcnh.TemporaryNo + "-" + prevcnh.TemporaryDate.ToString("yyyyMMddhhmmss"));
                pnlPDFViewer.Controls.Add(dgvDocumentList);
                dgvDocumentList.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDocumentList_CellContentClick);
            }
            catch (Exception ex)
            {
            }
        }

        private void dgvDocumentList_CellContentClick(Object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataGridView dgv = sender as DataGridView;
                string fileName = "";
                if (e.RowIndex < 0)
                    return;
                if (e.ColumnIndex == 0)
                {
                    removePDFControls();
                    fileName = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                    ////string docDir = Main.documentDirectory + "\\" + docID;
                    string subDir = prevcnh.TemporaryNo + "-" + prevcnh.TemporaryDate.ToString("yyyyMMddhhmmss");
                    ////DocumentStorageDB.createFileFromDB(docID, subDir, fileName);
                    dgv.Enabled = false;
                    fileName = DocumentStorageDB.createFileFromDB(docID, subDir, fileName);
                    ////showPDFFile(fileName);
                    ////dgv.Visible = false;
                    System.Diagnostics.Process.Start(fileName);
                    dgv.Enabled = true;
                }

            }
            catch (Exception ex)
            {
            }
        }
        private void setButtonVisibility(string btnName)
        {
            try
            {
                btnActionPending.Visible = true;
                btnApprovalPending.Visible = true;
                btnApproved.Visible = true;
                btnNew.Visible = false;
                btnExit.Visible = false;
                btnCancel.Visible = false;
                btnSave.Visible = false;
                btnForward.Visible = false;
                btnApprove.Visible = false;
                btnReverse.Visible = false;
                btnGetComments.Visible = false;
                chkCommentStatus.Visible = false;
                txtComments.Visible = false;
                disableTabPages();
                clearTabPageControls();
                //----24/11/2016
                handleNewButton();
                handleGrdViewButton();
                handleGrdEditButton();
                pnlBottomButtons.Visible = true;
                //----
                if (btnName == "init")
                {
                    ////btnNew.Visible = true; 24/11/2016
                    btnExit.Visible = true;

                }
                else if (btnName == "Commenter")
                {
                    btnCancel.Visible = true;
                    btnSave.Visible = true;
                    pnlPDFViewer.Visible = true;
                    tabComments.Enabled = true;
                    tabPDFViewer.Enabled = true;
                    btnGetComments.Visible = false; //earlier Edit enabled this button
                    chkCommentStatus.Visible = true;
                    txtComments.Visible = true;
                }
                else if (btnName == "btnNew")
                {
                    btnNew.Visible = false; //added 24/11/2016
                    btnCancel.Visible = true;
                    btnSave.Visible = true;
                    enableTabPages();
                    pnlPDFViewer.Visible = false;
                    tabComments.Enabled = false;
                    tabPDFViewer.Enabled = false;
                    tabControl1.SelectedTab = txbCNHeader;
                }
                else if (btnName == "btnEditPanel") //called from cancel,save,forward,approve and reverse button events
                {
                    ////btnNew.Visible = true;
                    btnExit.Visible = true;
                }
                //gridview buttons
                else if (btnName == "Edit")
                {
                    pnlBottomButtons.Visible = false; //24/11/2016
                    btnCancel.Visible = true;
                    btnSave.Visible = true;
                    btnGetComments.Visible = true;
                    enableTabPages();
                    pnlPDFViewer.Visible = true;
                    chkCommentStatus.Visible = true;
                    txtComments.Visible = true;
                    tabControl1.SelectedTab = txbCNHeader;
                }
                else if (btnName == "Approve")
                {
                    pnlBottomButtons.Visible = false; //24/11/2016
                    btnCancel.Visible = true;
                    btnForward.Visible = true;
                    btnApprove.Visible = true;
                    btnReverse.Visible = true;
                    // btnQC.Visible = true;
                    disableTabPages();

                    tabControl1.SelectedTab = txbCNHeader;
                }
                else if (btnName == "View")
                {
                    pnlBottomButtons.Visible = false; //24/11/2016
                    btnCancel.Visible = true;
                    disableTabPages();
                    tabComments.Enabled = true;
                    tabPDFViewer.Enabled = true;
                    pnlPDFViewer.Visible = true;
                    tabControl1.SelectedTab = txbCNHeader;
                }
                else if (btnName == "LoadDocument")
                {
                    pnlBottomButtons.Visible = false; //24/11/2016
                }


                pnlEditButtons.Refresh();
                //if the user privilege is only view, show only the Approved documents button
                int ups = getuserPrivilegeStatus();
                if (ups == 1 || ups == 0)
                {
                    grdList.Columns["Edit"].Visible = false;
                    grdList.Columns["Approve"].Visible = false;
                    btnActionPending.Visible = false;
                    btnApprovalPending.Visible = false;
                    btnApproved.Visible = false;
                    if (ups == 1)
                    {
                        grdList.Columns["View"].Visible = true;
                    }
                    else
                    {
                        grdList.Columns["View"].Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        void handleNewButton()
        {
            btnNew.Visible = false;
            if (Main.itemPriv[1])
            {
                btnNew.Visible = true;
            }
        }
        void handleGrdEditButton()
        {
            grdList.Columns["Edit"].Visible = false;
            grdList.Columns["Approve"].Visible = false;
            if (Main.itemPriv[1] || Main.itemPriv[2])
            {
                if (listOption == 1)
                {
                    grdList.Columns["Edit"].Visible = true;
                    grdList.Columns["Approve"].Visible = true;
                }
            }
        }

        void handleGrdViewButton()
        {
            grdList.Columns["View"].Visible = false;
            //if any one of view,add and edit
            if (Main.itemPriv[0] || Main.itemPriv[1] || Main.itemPriv[2])
            {
                //list option 1 should not have view buttuon visible (edit is avialable)
                if (listOption != 1)
                {
                    grdList.Columns["View"].Visible = true;
                }
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
        //call this form when new or edit buttons are clicked
        private void clearTabPageControls()
        {
            try
            {
                removePDFControls();
                removePDFFileGridView();
                removeControlsFromCommenterPanel();
                //removeControlsFromLVPanel();
                removeControlsFromForwarderPanel();
                dgvComments.Rows.Clear();
                chkCommentStatus.Checked = false;
                txtComments.Text = "";
                grdPRDetail.Rows.Clear();
            }
            catch (Exception ex)
            {
            }
        }
        private void txtSearch_TextChangedparty(object sender, EventArgs e)
        {
            frmPopup.Controls.Remove(lvCopy);
            addItemsparty();
        }
        private void addItemsparty()
        {
            lvCopy = new ListView();
            lvCopy.View = System.Windows.Forms.View.Details;
            lvCopy.LabelEdit = true;
            lvCopy.AllowColumnReorder = true;
            lvCopy.CheckBoxes = true;
            lvCopy.FullRowSelect = true;
            lvCopy.GridLines = true;
            lvCopy.Sorting = System.Windows.Forms.SortOrder.Ascending;
            lvCopy.Columns.Add("Select", -2, HorizontalAlignment.Left);
            lvCopy.Columns.Add("Id", -2, HorizontalAlignment.Left);
            lvCopy.Columns.Add("Name", -2, HorizontalAlignment.Left);
            lvCopy.Columns.Add("Type", -2, HorizontalAlignment.Left);
            lvCopy.Sorting = System.Windows.Forms.SortOrder.None;
            lvCopy.ColumnClick += new ColumnClickEventHandler(LvColumnClick); ;
            //this.lvCopy.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.CopylistView2_ItemChecked);
            //lvCopy.Location = new Point(13, 9);
            //lvCopy.Size = new Size(440, 199);
            lvCopy.Bounds = new Rectangle(new Point(0, 0), new Size(450, 250));
            lvCopy.Items.Clear();
            foreach (ListViewItem row in lv.Items)
            {
                string x = row.SubItems[0].Text;
                string no = row.SubItems[1].Text;
                string ch = row.SubItems[2].Text;
                string name = row.SubItems[3].Text;
                if (ch.ToLower().StartsWith(txtSearch.Text))
                {
                    ListViewItem item = new ListViewItem();
                    item.SubItems.Add(no);
                    item.SubItems.Add(ch);
                    item.SubItems.Add(name);
                    item.Checked = false;
                    lvCopy.Items.Add(item);
                }
            }
            lv.Visible = false;
            lvCopy.Visible = true;
            frmPopup.Controls.Add(lvCopy);
        }
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            frmPopup.Controls.Remove(lvCopy);
            addItems();
        }
        private void addItems()
        {
            lvCopy = new ListView();
            lvCopy.View = System.Windows.Forms.View.Details;
            lvCopy.LabelEdit = true;
            lvCopy.AllowColumnReorder = true;
            lvCopy.CheckBoxes = true;
            lvCopy.FullRowSelect = true;
            lvCopy.GridLines = true;
            lvCopy.Sorting = System.Windows.Forms.SortOrder.Ascending;
            lvCopy.Columns.Add("Select", -2, HorizontalAlignment.Left);
            lvCopy.Columns.Add("Id", -2, HorizontalAlignment.Left);
            lvCopy.Columns.Add("Name", -2, HorizontalAlignment.Left);
            lvCopy.Sorting = System.Windows.Forms.SortOrder.None;
            lvCopy.ColumnClick += new ColumnClickEventHandler(LvColumnClick); ;
            //this.lvCopy.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.CopylistView2_ItemChecked);
            //lvCopy.Location = new Point(13, 9);
            //lvCopy.Size = new Size(440, 199);
            lvCopy.Bounds = new Rectangle(new Point(0, 0), new Size(450, 250));
            lvCopy.Items.Clear();
            foreach (ListViewItem row in lv.Items)
            {
                string x = row.SubItems[0].Text;
                string no = row.SubItems[1].Text;
                string ch = row.SubItems[2].Text;
                if (ch.ToLower().StartsWith(txtSearch.Text))
                {
                    ListViewItem item = new ListViewItem();
                    item.SubItems.Add(no);
                    item.SubItems.Add(ch);
                    item.Checked = false;
                    lvCopy.Items.Add(item);
                }
            }
            lv.Visible = false;
            lvCopy.Visible = true;
            frmPopup.Controls.Add(lvCopy);
        }
        //private void CopylistView2_ItemChecked(object sender, ItemCheckedEventArgs e)
        //{
        //    try
        //    {
        //        if (lvCopy.CheckedIndices.Count > 1)
        //        {
        //            MessageBox.Show("Cannot select more than one item");
        //            e.Item.Checked = false;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //    }
        //}
        private void LvColumnClick(object o, ColumnClickEventArgs e)
        {
            try
            {
                if (lv.Visible == true)
                {
                    string first = lv.Items[0].SubItems[e.Column].Text;
                    string last = lv.Items[lv.Items.Count - 1].SubItems[e.Column].Text;
                    System.Windows.Forms.SortOrder sort_order1 = SortingListView.getSortedOrder(first, last);
                    this.lv.ListViewItemSorter = new SortingListView(e.Column, sort_order1);
                }
                else
                {
                    string first = lvCopy.Items[0].SubItems[e.Column].Text;
                    string last = lvCopy.Items[lvCopy.Items.Count - 1].SubItems[e.Column].Text;
                    System.Windows.Forms.SortOrder sort_order1 = SortingListView.getSortedOrder(first, last);
                    this.lvCopy.ListViewItemSorter = new SortingListView(e.Column, sort_order1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorting error");
            }
        }
        private void txtPONo_TextChanged(object sender, EventArgs e)
        {
        }
        private void cmbCustomer_SelectedValueChanged(object sender, EventArgs e)
        {
        }
        private void tabPRHeader_Click(object sender, EventArgs e)
        {

        }
        private void grdPRDetail_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0)
                    return;
                string columnName = grdPRDetail.Columns[e.ColumnIndex].Name;
                try
                {
                    if (columnName.Equals("Delete"))
                    {
                        //delete row
                        DialogResult dialog = MessageBox.Show("Are you sure to Delete the row ?", "Yes", MessageBoxButtons.YesNo);
                        if (dialog == DialogResult.Yes)
                        {
                            grdPRDetail.Rows.RemoveAt(e.RowIndex);
                        }
                        verifyAndReworkCreditNoteDetailGridRows();
                    }
                    if (columnName.Equals("SelAcDebit"))
                    {
                        showDebitAccountLIstView();
                    }
                }

                catch (Exception ex)
                {

                }
            }
            catch (Exception ex)
            {

            }
        }
        private void showDebitAccountLIstView()
        {
            //removeControlsFromLVPanel();
            //pnlAddEdit.Controls.Remove(pnllv);
            if (txtAccCode.Text.Length == 0)
            {
                MessageBox.Show("Select credit  Account Code");
                return;
            }
            //pnllv = new Panel();
            //pnllv.BorderStyle = BorderStyle.FixedSingle;

            //pnllv.Bounds = new Rectangle(new Point(311, 46), new Size(566, 281));
            frmPopup = new Form();
            frmPopup.StartPosition = FormStartPosition.CenterScreen;
            frmPopup.BackColor = Color.CadetBlue;

            frmPopup.MaximizeBox = false;
            frmPopup.MinimizeBox = false;
            frmPopup.ControlBox = false;
            frmPopup.FormBorderStyle = FormBorderStyle.FixedSingle;

            frmPopup.Size = new Size(450, 300);
            lv = AccountCodeDB.getAccountCodeListView();
            lv.ColumnClick += new ColumnClickEventHandler(LvColumnClick);
            //this.lv.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listView1_ItemChecked1);
            lv.Bounds = new Rectangle(new Point(0, 0), new Size(450, 250));
            frmPopup.Controls.Add(lv);

            Button lvOK = new Button();
            lvOK.BackColor = Color.Tan;
            lvOK.Text = "OK";
            lvOK.Location = new Point(44, 265);
            lvOK.Click += new System.EventHandler(this.lvOK_ClickDebit);
            frmPopup.Controls.Add(lvOK);

            Button lvCancel = new Button();
            lvCancel.Text = "CANCEL";
            lvCancel.BackColor = Color.Tan;
            lvCancel.Location = new Point(141, 265);
            lvCancel.Click += new System.EventHandler(this.lvCancel_ClickDebit);
            frmPopup.Controls.Add(lvCancel);

            Label lblSearch = new Label();
            lblSearch.Text = "Search";
            lblSearch.Location = new Point(250, 267);
            lblSearch.Size = new Size(45, 15);
            frmPopup.Controls.Add(lblSearch);

            txtSearch = new TextBox();
            txtSearch.Location = new Point(300, 265);
            txtSearch.TextChanged += new EventHandler(txtSearch_TextChanged);
            frmPopup.Controls.Add(txtSearch);
            txtSearch.Focus();
            frmPopup.ShowDialog();
            //pnlAddEdit.Controls.Add(pnllv);
            //pnllv.BringToFront();
            //pnllv.Visible = true;
            
        }
        private void lvOK_ClickDebit(object sender, EventArgs e)
        {
            try
            {
                if (lv.Visible == true)
                {
                    if (lv.CheckedIndices.Count == 0)
                    {
                        MessageBox.Show("Select one Account.");
                        return;
                    }
                    if (lv.CheckedIndices.Count > 1)
                    {
                        MessageBox.Show("Not allowed to select more than one Account.");
                        return;
                    }
                    foreach (ListViewItem itemRow in lv.Items)
                    {

                        if (itemRow.Checked)
                        {
                            grdPRDetail.CurrentRow.Cells["AccountCode"].Value = itemRow.SubItems[1].Text;
                            grdPRDetail.CurrentRow.Cells["AccountName"].Value = itemRow.SubItems[2].Text;
                            //itemRow.Checked = false;
                            //pnllv.Controls.Remove(lv);
                            //pnllv.Visible = false;
                        }
                    }
                }
                else
                {
                    if (lvCopy.CheckedIndices.Count == 0)
                    {
                        MessageBox.Show("Select one Account.");
                        return;
                    }
                    if (lvCopy.CheckedIndices.Count > 1)
                    {
                        MessageBox.Show("Not allowed to select more than one Account.");
                        return;
                    }
                    foreach (ListViewItem itemRow in lvCopy.Items)
                    {

                        if (itemRow.Checked)
                        {
                            grdPRDetail.CurrentRow.Cells["AccountCode"].Value = itemRow.SubItems[1].Text;
                            grdPRDetail.CurrentRow.Cells["AccountName"].Value = itemRow.SubItems[2].Text;
                            //itemRow.Checked = false;
                            //pnllv.Controls.Remove(lvCopy);
                            //pnllv.Visible = false;
                        }
                    }
                }
                frmPopup.Close();
                frmPopup.Dispose();
            }
            catch (Exception)
            {
            }
        }
        private void lvCancel_ClickDebit(object sender, EventArgs e)
        {
            try
            {
                //lv.CheckBoxes = false;
                //lv.CheckBoxes = true;
                //pnllv.Controls.Remove(lv);
                //pnllv.Visible = false;
                frmPopup.Close();
                frmPopup.Dispose();
            }
            catch (Exception)
            {
            }
        }
        //private void listView1_ItemCheckedDebit(object sender, ItemCheckedEventArgs e)
        //{
        //    try
        //    {
        //        if (lv.CheckedIndices.Count > 1)
        //        {
        //            MessageBox.Show("Cannot select more than one item");
        //            e.Item.Checked = false;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //    }
        //}
        private void showCustomerLIstView()
        {
            //removeControlsFromLVPanel();
            //pnlAddEdit.Controls.Remove(pnllv);
            //btnSelectCreditParty.Enabled = false;
            //tabControl1.Enabled = false;
            //pnllv = new Panel();
            //pnllv.BorderStyle = BorderStyle.FixedSingle;
            //pnllv.Bounds = new Rectangle(new Point(311, 46), new Size(566, 281));
            frmPopup = new Form();
            frmPopup.StartPosition = FormStartPosition.CenterScreen;
            frmPopup.BackColor = Color.CadetBlue;

            frmPopup.MaximizeBox = false;
            frmPopup.MinimizeBox = false;
            frmPopup.ControlBox = false;
            frmPopup.FormBorderStyle = FormBorderStyle.FixedSingle;

            frmPopup.Size = new Size(450, 300);
            lv = ListViewFIll.getSLListView();
            lv.ColumnClick += new ColumnClickEventHandler(LvColumnClick);
            //this.lv.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listView1_ItemChecked3);
            lv.Bounds = new Rectangle(new Point(0,0), new Size(450, 250));
            frmPopup.Controls.Add(lv);

            Button lvOK = new Button();
            lvOK.BackColor = Color.Tan;
            lvOK.Text = "OK";
            lvOK.Location = new Point(44, 265);
            lvOK.Click += new System.EventHandler(this.lvOK_Click3);
            frmPopup.Controls.Add(lvOK);

            Button lvCancel = new Button();
            lvCancel.Text = "CANCEL";
            lvCancel.BackColor = Color.Tan;
            lvCancel.Location = new Point(141, 265);
            lvCancel.Click += new System.EventHandler(this.lvCancel_Click3);
            frmPopup.Controls.Add(lvCancel);

            Label lblSearch = new Label();
            lblSearch.Text = "Search";
            lblSearch.Location = new Point(250, 267);
            lblSearch.Size = new Size(45, 15);
            frmPopup.Controls.Add(lblSearch);

            txtSearch = new TextBox();
            txtSearch.Location = new Point(300, 265);
            txtSearch.TextChanged += new EventHandler(txtSearch_TextChangedparty);
            frmPopup.Controls.Add(txtSearch);
            txtSearch.Focus();
            frmPopup.ShowDialog();
            //pnlAddEdit.Controls.Add(pnllv);
            //pnllv.BringToFront();
            //pnllv.Visible = true;
            
        }
        private void lvOK_Click3(object sender, EventArgs e)
        {
            try
            {
                if (lv.Visible == true)
                {
                    if (lv.CheckedIndices.Count == 0)
                    {
                        MessageBox.Show("Select one party.");
                        return;
                    }
                    if (lv.CheckedIndices.Count > 1)
                    {
                        MessageBox.Show("Not allowed to select more than one Party.");
                        return;
                    }
                    foreach (ListViewItem itemRow in lv.Items)
                    {

                        if (itemRow.Checked)
                        {

                            txtPartyID.Text = itemRow.SubItems[1].Text;
                            txtPartyname.Text = itemRow.SubItems[2].Text;
                            SLType = itemRow.SubItems[3].Text;
                            txtRefNo.Text = "";
                            dtRefDate.Value = DateTime.Parse("01-01-1900");
                            //btnSelectCreditParty.Enabled = true;
                            //tabControl1.Enabled = true;
                            itemRow.Checked = false;
                            //pnllv.Visible = false;
                            //pnllv.Controls.Remove(lv);
                            
                        }
                    }
                }
                else
                {
                    if (lvCopy.CheckedIndices.Count == 0)
                    {
                        MessageBox.Show("Select one party.");
                        return;
                    }
                    if (lvCopy.CheckedIndices.Count > 1)
                    {
                        MessageBox.Show("Not allowed to select more than one party.");
                        return;
                    }
                    foreach (ListViewItem itemRow in lvCopy.Items)
                    {

                        if (itemRow.Checked)
                        {
                            txtPartyID.Text = itemRow.SubItems[1].Text;
                            txtPartyname.Text = itemRow.SubItems[2].Text;
                            SLType = itemRow.SubItems[3].Text;
                            //btnSelectCreditParty.Enabled = true;
                            //tabControl1.Enabled = true;
                            itemRow.Checked = false;
                            //pnllv.Visible = false;
                            //pnllv.Controls.Remove(lvCopy);
                           
                        }
                    }
                }
                frmPopup.Close();
                frmPopup.Dispose();
            }
            catch (Exception)
            {
            }
        }
        private void lvCancel_Click3(object sender, EventArgs e)
        {
            try
            {
                //btnSelectCreditParty.Enabled = true;
                //tabControl1.Enabled = true;
                //lv.CheckBoxes = false;
                //lv.CheckBoxes = true;
                //pnllv.Controls.Remove(lv);
                //pnllv.Visible = false;
                frmPopup.Close();
                frmPopup.Dispose();
            }
            catch (Exception)
            {
            }
        }
        //private void listView1_ItemChecked3(object sender, ItemCheckedEventArgs e)
        //{
        //    try
        //    {
        //        if (lv.CheckedIndices.Count > 1)
        //        {
        //            MessageBox.Show("Cannot select more than one item");
        //            e.Item.Checked = false;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //    }
        //}

        private void showAccountLIstView()
        {
            //removeControlsFromLVPanel();
            ///pnlAddEdit.Controls.Remove(pnllv);
            //btnSelectAccountCode.Enabled = false;
            //pnllv = new Panel();
            //pnllv.BorderStyle = BorderStyle.FixedSingle;

            //pnllv.Bounds = new Rectangle(new Point(311, 46), new Size(566, 281));
            frmPopup = new Form();
            frmPopup.StartPosition = FormStartPosition.CenterScreen;
            frmPopup.BackColor = Color.CadetBlue;

            frmPopup.MaximizeBox = false;
            frmPopup.MinimizeBox = false;
            frmPopup.ControlBox = false;
            frmPopup.FormBorderStyle = FormBorderStyle.FixedSingle;

            frmPopup.Size = new Size(450, 300);
            lv = AccountCodeDB.getAccountCodeListView();
            lv.ColumnClick += new ColumnClickEventHandler(LvColumnClick);
           // this.lv.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listView1_ItemChecked1);
            lv.Bounds = new Rectangle(new Point(0, 0), new Size(450, 250));
            frmPopup.Controls.Add(lv);

            Button lvOK = new Button();
            lvOK.BackColor = Color.Tan;
            lvOK.Text = "OK";
            lvOK.Location = new System.Drawing.Point(44, 265);
            lvOK.Click += new System.EventHandler(this.lvOK_Click2);
            frmPopup.Controls.Add(lvOK);

            Button lvCancel = new Button();
            lvCancel.Text = "CANCEL";
            lvCancel.BackColor = Color.Tan;
            lvCancel.Location = new System.Drawing.Point(141, 265);
            lvCancel.Click += new System.EventHandler(this.lvCancel_Click2);
            frmPopup.Controls.Add(lvCancel);

            Label lblSearch = new Label();
            lblSearch.Text = "Search";
            lblSearch.Location = new Point(250, 267);
            lblSearch.Size = new Size(45, 15);
            frmPopup.Controls.Add(lblSearch);

            txtSearch = new TextBox();
            txtSearch.Location = new Point(300, 265);
            txtSearch.TextChanged += new EventHandler(txtSearch_TextChanged);
            frmPopup.Controls.Add(txtSearch);

            //pnlAddEdit.Controls.Add(pnllv);
            //pnllv.BringToFront();
            //pnllv.Visible = true;
            txtSearch.Focus();
            frmPopup.ShowDialog();
            
        }
        private void lvOK_Click2(object sender, EventArgs e)
        {
            try
            {
                if (lv.Visible == true)
                {
                    if (lv.CheckedIndices.Count == 0)
                    {
                        MessageBox.Show("Select one Account.");
                        return;
                    }
                    if (lv.CheckedIndices.Count > 1)
                    {
                        MessageBox.Show("Not allowed to select more than one Account.");
                        return;
                    }
                    foreach (ListViewItem itemRow in lv.Items)
                    {
                        if (itemRow.Checked)
                        {
                            txtAccCode.Text = itemRow.SubItems[1].Text;
                            txtAccName.Text = itemRow.SubItems[2].Text;
                            ///btnSelectAccountCode.Enabled = true;
                            itemRow.Checked = false;
                            //pnllv.Visible = false;
                            //pnllv.Controls.Remove(lv);
                            frmPopup.Close();
                            frmPopup.Dispose();
                        }
                    }
                }
                else
                {
                    if (lvCopy.CheckedIndices.Count == 0)
                    {
                        MessageBox.Show("Select one Account.");
                        return;
                    }
                    if (lvCopy.CheckedIndices.Count > 1)
                    {
                        MessageBox.Show("Not allowed to select more than one Account.");
                        return;
                    }
                    foreach (ListViewItem itemRow in lvCopy.Items)
                    {

                        if (itemRow.Checked)
                        {
                            txtAccCode.Text = itemRow.SubItems[1].Text;
                            txtAccName.Text = itemRow.SubItems[2].Text;
                            ////btnSelectAccountCode.Enabled = true;
                            itemRow.Checked = false;
                            //pnllv.Visible = false;
                            //pnllv.Controls.Remove(lvCopy);
                            frmPopup.Close();
                            frmPopup.Dispose();
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        private void lvCancel_Click2(object sender, EventArgs e)
        {
            try
            {
                ///btnSelectAccountCode.Enabled = true;
                //pnllv.Visible = false;
                //lv.CheckBoxes = false;
                //lv.CheckBoxes = true;
                //pnllv.Controls.Remove(lv);
                frmPopup.Close();
                frmPopup.Dispose();
            }
            catch (Exception)
            {
            }
        }
        //private void listView1_ItemChecked1(object sender, ItemCheckedEventArgs e)
        //{
        //    try
        //    {
        //        if (lv.CheckedIndices.Count > 1)
        //        {
        //            MessageBox.Show("Cannot select more than one item");
        //            e.Item.Checked = false;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //    }
        //}

        private void btnCalculate_Click(object sender, EventArgs e)
        {
            verifyAndReworkCreditNoteDetailGridRows();
        }

        private void btnSelectAccountCode_Click(object sender, EventArgs e)
        {
            showAccountLIstView();
        }

        private void btnSelectCreditParty_Click(object sender, EventArgs e)
        {
            showCustomerLIstView();
        }

        private void btnSelectRefNo_Click(object sender, EventArgs e)
        {
            //removeControlsFromLVPanel();
            ///pnlAddEdit.Controls.Remove(pnllv);
            if (txtPartyname.Text.Length == 0)
            {
                MessageBox.Show("Select Customer Details");
                return;
            }
            ///btnSelectRefNo.Enabled = false;
            //pnllv = new Panel();
            //pnllv.BorderStyle = BorderStyle.FixedSingle;

            //pnllv.Bounds = new Rectangle(new Point(311, 46), new Size(566, 281));
            frmPopup = new Form();
            frmPopup.StartPosition = FormStartPosition.CenterScreen;
            frmPopup.BackColor = Color.CadetBlue;

            frmPopup.MaximizeBox = false;
            frmPopup.MinimizeBox = false;
            frmPopup.ControlBox = false;
            frmPopup.FormBorderStyle = FormBorderStyle.FixedSingle;

            frmPopup.Size = new Size(450, 300);

            lv = InvoiceOutHeaderDB.getInvoiceOutListView(txtPartyID.Text);
            lv.ColumnClick += new ColumnClickEventHandler(LvColumnClick);
           // this.lv.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listView1_ItemChecked2);
            lv.Bounds = new Rectangle(new Point(0, 0), new Size(450, 250));
            frmPopup.Controls.Add(lv);

            Button lvOK = new Button();
            lvOK.BackColor = Color.Tan;
            lvOK.Text = "OK";
            lvOK.Location = new Point(44, 265);
            lvOK.Click += new System.EventHandler(this.lvOK_Click4);
            frmPopup.Controls.Add(lvOK);

            Button lvCancel = new Button();
            lvCancel.Text = "CANCEL";
            lvCancel.BackColor = Color.Tan;
            lvCancel.Location = new Point(141, 265);
            lvCancel.Click += new System.EventHandler(this.lvCancel_Click4);
            frmPopup.Controls.Add(lvCancel);

            frmPopup.ShowDialog();
            //pnlAddEdit.Controls.Add(pnllv);
            //pnllv.BringToFront();
            //pnllv.Visible = true;
        }
        private void lvOK_Click4(object sender, EventArgs e)
        {
            try
            {
                if (lv.CheckedIndices.Count == 0)
                {
                    MessageBox.Show("Select one Reference.");
                    return;
                }
                if (lv.CheckedIndices.Count > 1)
                {
                    MessageBox.Show("Not allowed to select more than one Reference.");
                    return;
                }
                foreach (ListViewItem itemRow in lv.Items)
                {
                    if (itemRow.Checked)
                    {
                       // btnSelectRefNo.Enabled = true;
                        txtRefNo.Text = itemRow.SubItems[2].Text;
                        dtRefDate.Value = Convert.ToDateTime(itemRow.SubItems[3].Text);
                        //btnSelectRefNo.Enabled = true;
                        itemRow.Checked = false;
                        //pnllv.Controls.Remove(lv);
                        //pnllv.Visible = false;
                        frmPopup.Close();
                        frmPopup.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        private void lvCancel_Click4(object sender, EventArgs e)
        {
            try
            {
                //btnSelectRefNo.Enabled = true;
                //lv.CheckBoxes = false;
                //lv.CheckBoxes = true;
                //pnllv.Controls.Remove(lv);
                //pnllv.Visible = false;
                frmPopup.Close();
                frmPopup.Dispose();
            }
            catch (Exception)
            {
            }
        }

        private void CreditNote_Enter(object sender, EventArgs e)
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
        //private void listView1_ItemChecked2(object sender, ItemCheckedEventArgs e)
        //{
        //    try
        //    {
        //        if (lv.CheckedIndices.Count > 1)
        //        {
        //            MessageBox.Show("Cannot select more than one item");
        //            e.Item.Checked = false;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //    }
        //}

    }
}



