﻿using System;
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
using CSLERP.PrintForms;
using System.Collections;

namespace CSLERP
{
    public partial class WorkOrder : System.Windows.Forms.Form
    {
        string docID = "WORKORDER";
        DocEmpMappingDB demDB = new DocEmpMappingDB();
        string forwarderList = "";
        string approverList = "";
        double productvalue = 0.0;
        double taxvalue = 0.0;
        workorderheader prevwoh ;
        System.Data.DataTable TaxDetailsTable = new System.Data.DataTable();
        Panel pnllv = new Panel();
        ListView lv = new ListView();
        Form frmPopup = new Form();
        string userString = "";
        string userCommentStatusString = "";
        string commentStatus = "";
        int listOption = 1; //1-Pending, 2-In Process, 3-Approved
        Boolean userIsACommenter = false;
        DocumentReceiverDB drDB = new DocumentReceiverDB();
        DocCommenterDB docCmtrDB = new DocCommenterDB();
        System.Data.DataTable dtCmtStatus = new DataTable();
        Panel pnldgv = new Panel(); // panel for gridview
        Panel pnlCmtr = new Panel();
        Panel pnlForwarder = new Panel();
        TreeView tv = new TreeView();
        DataGridView dgvpt = new DataGridView(); // grid view for Payment Terms
        DataGridView dgvComments = new DataGridView();
        ListView lvCmtr = new ListView(); // list view for choice / selection list
        ListView lvApprover = new ListView();
        int descClickRowIndex = -1;
        RichTextBox txtDesc = new RichTextBox();
        Boolean AddRowClick = false;
        List<documentreceiver> DocREcvList = new List<documentreceiver>();
        string DocRecvListstr = "";
        public WorkOrder()
        {
            try
            {
                InitializeComponent();

            }
            catch (Exception)
            {

            }
        }
        private void WorkOrder_Load(object sender, EventArgs e)
        {
            ////this.FormBorderStyle = FormBorderStyle.None;
            Region = System.Drawing.Region.FromHrgn(Utilities.CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
            initVariables();
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Width -= 100;
            this.Height -= 100;
            ////this.FormBorderStyle = FormBorderStyle.Fixed3D;
            String a = this.Size.ToString();
            grdList.ColumnHeadersDefaultCellStyle.BackColor = Color.LightBlue;
            grdList.EnableHeadersVisualStyles = false;
            DocREcvList = Main.DocumentReceivers.Where(rec => rec.DocumentID == docID).ToList();
            DocRecvListstr = getRecvDocString(DocREcvList);
            ListFilteredWOHeader(listOption);
            //applyPrivilege();
        }
        private string getRecvDocString(List<documentreceiver> rcvlist)
        {
            string strList = "";
            if (rcvlist.Count == 0)
            {
                strList = "";
            }
            else
            {
                ArrayList arrStr = new ArrayList();
                foreach (documentreceiver rcvr in rcvlist)
                {
                    string qry = "( DocumentID = '" + rcvr.DocumentID + "' and OfficeID = '" + rcvr.OfficeID + "')";
                    if ((docID == rcvr.DocumentID) && !arrStr.Contains(qry))
                    {
                        arrStr.Add("( DocumentID = '" + rcvr.DocumentID + "' and OfficeID = '" + rcvr.OfficeID + "')");
                    }
                }
                strList = string.Join(" or ", arrStr.ToArray());
            }
            return strList;
        }
        private string getWOStatusString(int code)
        {
            string statStr = "";
            switch (code)
            {
                case 1:
                    statStr = "Order Approved";
                    break;
                case 2:
                    statStr = "Work Started";
                    break;
                case 3:
                    statStr = "Work In Progress";
                    break;
                case 4:
                    statStr = "Temporary Halt";
                    break;
                case 5:
                    statStr = "Order Cancelled";
                    break;
                case 6:
                    statStr = "Completed";
                    break;
                default:
                    statStr = "";
                    break;
            }
            return statStr;
        }
        //private int getWOStatusCode(string statStr)
        //{
        //    int statCode = 0;


        //    switch (statStr)
        //    {
        //        case "Approved":
        //            statCode = 1;
        //            break;
        //        case "Work Started":
        //            statCode = 2;
        //            break;
        //        case "Work Halted":
        //            statCode = 3;
        //            break;
        //        case "Canceled":
        //            statCode = 4;
        //            break;
        //        case "Closed Partially":
        //            statCode = 5;
        //            break;
        //        case "Cosed After Completion":
        //            statCode = 6;
        //            break;
        //        default:
        //            statStr = "";
        //            break;
        //    }
        //    return statCode;
        //}
        private void ListFilteredWOHeader(int option)
        {
            try
            {
                grdList.Rows.Clear();
                WorkOrderDB wodb = new WorkOrderDB();
                forwarderList = demDB.getForwarders(docID, Login.empLoggedIn);
                approverList = demDB.getApprovers(docID, Login.empLoggedIn);
                List<workorderheader> WOHeaders = wodb.getFilteredWorkOrderHeaders(userString, option, userCommentStatusString, DocRecvListstr);
                if (option == 1)
                    lblActionHeader.Text = "List of Action Pending Documents";
                else if (option == 2)
                    lblActionHeader.Text = "List of In-Process Documents";
                else if (option == 3 || option == 6)
                    lblActionHeader.Text = "List of Approved Documents";
                foreach (workorderheader woh in WOHeaders)
                {
                    if (option == 1)
                    {
                        if (woh.DocumentStatus == 99)
                            continue;
                    }
                    else
                    {

                    }
                    grdList.Rows.Add();
                    grdList.Rows[grdList.RowCount - 1].Cells["gDocumentID"].Value = woh.DocumentID;
                    grdList.Rows[grdList.RowCount - 1].Cells["gDocumentName"].Value = woh.DocumentName;
                    grdList.Rows[grdList.RowCount - 1].Cells["gTemporaryNo"].Value = woh.TemporaryNo;
                    grdList.Rows[grdList.RowCount - 1].Cells["gTemporaryDate"].Value = woh.TemporaryDate;
                    grdList.Rows[grdList.RowCount - 1].Cells["WONo"].Value = woh.WONo;
                    grdList.Rows[grdList.RowCount - 1].Cells["WODate"].Value = woh.WODate;
                    grdList.Rows[grdList.RowCount - 1].Cells["gWORequestNo"].Value = woh.WORequestNo;
                    grdList.Rows[grdList.RowCount - 1].Cells["gWORequestDate"].Value = woh.WORequestDate;
                    grdList.Rows[grdList.RowCount - 1].Cells["gReferenceInternalOrder"].Value = woh.ReferenceInternalOrder;
                    grdList.Rows[grdList.RowCount - 1].Cells["gProjectID"].Value = woh.ProjectID;
                    grdList.Rows[grdList.RowCount - 1].Cells["gOfficeID"].Value = woh.OfficeID;
                    grdList.Rows[grdList.RowCount - 1].Cells["gCustomerID"].Value = woh.CustomerID;
                    grdList.Rows[grdList.RowCount - 1].Cells["gCustomerName"].Value = woh.CustomerName;
                    grdList.Rows[grdList.RowCount - 1].Cells["gCurrencyID"].Value = woh.CurrencyID;
                    grdList.Rows[grdList.RowCount - 1].Cells["gCurrencyName"].Value = woh.CurrencyName;
                    grdList.Rows[grdList.RowCount - 1].Cells["ExchangeRate"].Value = woh.ExchangeRate;
                    grdList.Rows[grdList.RowCount - 1].Cells["gStartDate"].Value = woh.StartDate;
                    grdList.Rows[grdList.RowCount - 1].Cells["gTargetDate"].Value = woh.TargetDate;
                    grdList.Rows[grdList.RowCount - 1].Cells["gPaymentTerms"].Value = woh.PaymentTerms;
                    grdList.Rows[grdList.RowCount - 1].Cells["gPaymentMode"].Value = woh.PaymentMode;
                    grdList.Rows[grdList.RowCount - 1].Cells["gPOAddress"].Value = woh.POAddress;
                    grdList.Rows[grdList.RowCount - 1].Cells["gServiceValue"].Value = woh.ServiceValue;
                    grdList.Rows[grdList.RowCount - 1].Cells["gTaxAmount"].Value = woh.TaxAmount;
                    grdList.Rows[grdList.RowCount - 1].Cells["gTotalAmount"].Value = woh.TotalAmount;
                    grdList.Rows[grdList.RowCount - 1].Cells["ServiceValueINR"].Value = woh.ServiceValueINR;
                    grdList.Rows[grdList.RowCount - 1].Cells["TaxAmountINR"].Value = woh.TaxAmountINR;
                    grdList.Rows[grdList.RowCount - 1].Cells["TotalAmountINR"].Value = woh.TotalAmountINR;
                    grdList.Rows[grdList.RowCount - 1].Cells["TermsAndCond"].Value = woh.TermsAndCond;
                    grdList.Rows[grdList.RowCount - 1].Cells["gRemarks"].Value = woh.Remarks;
                    grdList.Rows[grdList.RowCount - 1].Cells["gStatus"].Value = woh.Status;
                    grdList.Rows[grdList.RowCount - 1].Cells["WorkOrderStatus"].Value = woh.WorkOrderStatus;
                    grdList.Rows[grdList.RowCount - 1].Cells["WOStatusName"].Value = getWOStatusString(woh.WorkOrderStatus);
                    grdList.Rows[grdList.RowCount - 1].Cells["gDocumentStatus"].Value = woh.DocumentStatus;
                    grdList.Rows[grdList.RowCount - 1].Cells["gCreateTime"].Value = woh.CreateTime;
                    grdList.Rows[grdList.RowCount - 1].Cells["gCreator"].Value = woh.CreatorName;
                    grdList.Rows[grdList.RowCount - 1].Cells["gForwarder"].Value = woh.ForwarderName;
                    grdList.Rows[grdList.RowCount - 1].Cells["gApprover"].Value = woh.ApproverName;
                    //grdList.Rows[grdList.RowCount - 1].Cells["gDocumentStatusNo"].Value = ComboFIll.getDocumentStatusString(woh.DocumentStatus);
                    grdList.Rows[grdList.RowCount - 1].Cells["gCreateUser"].Value = woh.CreateUser;
                    //grdList.Rows[grdList.RowCount - 1].Cells["gDocumentStatusNo"].Value = woh.DocumentStatus;
                    grdList.Rows[grdList.RowCount - 1].Cells["ComntStatus"].Value = woh.CommentStatus;
                    grdList.Rows[grdList.RowCount - 1].Cells["Comments"].Value = woh.Comments;
                    grdList.Rows[grdList.RowCount - 1].Cells["ForwarderLists"].Value = woh.ForwarderList;
                    grdList.Rows[grdList.RowCount - 1].Cells["SpecialNote"].Value = woh.SpecialNote.Trim();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in WorkOrder Listing");
            }
            if (listOption == 1 || listOption == 2)
                cmbWOStatFilter.Visible = false;
            else
                cmbWOStatFilter.Visible = true;
            cmbWOStatFilter.SelectedIndex = -1;
            cmbWOStatFilter.SelectedIndex = Structures.ComboFUnctions.getComboIndex(cmbWOStatFilter, "All");
            setButtonVisibility("init");
            pnlList.Visible = true;

            grdList.Columns["gCreator"].Visible = true;
            grdList.Columns["gForwarder"].Visible = true;
            grdList.Columns["gApprover"].Visible = true;
        }
        private void initVariables()
        {

            if (getuserPrivilegeStatus() == 1)
            {
                //user is only a viewer
                listOption = 6;
            }
            docID = Main.currentDocument;
            
            StatusCatalogueDB.fillStatusCatalogueCombo(cmbWOStatFilter, "WORKORDER");
            cmbWOStatFilter.Items.Add(new Structures.ComboBoxItem("All", "All"));
            cmbWOStatFilter.Visible = false;

            CustomerDB.fillLedgerTypeComboNew(cmbContractor, "Contractor");
            //CatalogueValueDB.fillCatalogValueCombo(cmbPaymentTerms, "PaymentTerms");
            CatalogueValueDB.fillCustomerComboNew(cmbPaymentMode, "PaymentMode");
            CurrencyDB.fillCurrencyComboNew(cmbCurrency);
            ProjectHeaderDB.fillprojectCombo(cmbProjectID);
            OfficeDB.fillOfficeCombo(cmbOfficeID);
            //TaxCodeDB.fillTaxCodeCombo(cmbTaxCode);
            dtTemporaryDate.Format = DateTimePickerFormat.Custom;
            dtTemporaryDate.CustomFormat = "dd-MM-yyyy";
            dtTemporaryDate.Enabled = false;
            dtWODate.Format = DateTimePickerFormat.Custom;
            dtWODate.CustomFormat = "dd-MM-yyyy";
            dtWODate.Enabled = false;
            dtWODate.Value = DateTime.Now;
            dtStartDate.Format = DateTimePickerFormat.Custom;
            dtStartDate.CustomFormat = "dd-MM-yyyy";
            dtStartDate.Enabled = true;
            dtTargetDate.Format = DateTimePickerFormat.Custom;
            dtTargetDate.CustomFormat = "dd-MM-yyyy";
            dtTargetDate.Enabled = true;
            txtPaymentTerms.Enabled = false;
            txtTemporaryNo.Enabled = false;
            txtWONo.Enabled = false;
            //txtCreditPeriods.Enabled = true;
            pnlUI.Controls.Add(pnlAddEdit);
            closeAllPanels();
            grdWODetail.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            //create tax details table for tax breakup display
            {
                TaxDetailsTable.Columns.Add("TaxItem", typeof(string));
                TaxDetailsTable.Columns.Add("TaxAmount", typeof(double));
            }
            txtComments.Text = "";

            userString = Login.userLoggedInName + Main.delimiter1 + Login.userLoggedIn + Main.delimiter1 + Main.delimiter2;
            userCommentStatusString = Login.userLoggedInName + Main.delimiter1 + Login.userLoggedIn + Main.delimiter1 + "0" + Main.delimiter2;
            setButtonVisibility("init");
            setTabIndex();
        }
        private void setTabIndex()
        {
            txtTemporaryNo.TabIndex = 0;
            dtTemporaryDate.TabIndex = 1;
            txtWONo.TabIndex = 2;
            dtWODate.TabIndex = 3;
            dtStartDate.TabIndex = 6;
            dtTargetDate.TabIndex = 7;
            cmbContractor.TabIndex = 8;
            cmbProjectID.TabIndex = 9;
            cmbOfficeID.TabIndex = 10;
            txtPaymentTerms.TabIndex = 11;
            btnPaymentTerm.TabIndex = 12;
            cmbPaymentMode.TabIndex = 13;
            cmbCurrency.TabIndex = 14;
            txtExchangeRate.TabIndex = 15;
            txtReferenceInternalOrder.TabIndex = 16;
            btnReferenceInternalOrder.TabIndex = 17;
            txtTermsAndCond.TabIndex = 18;
            btnSelectTermsAndCond.TabIndex = 19;
            txtPOAddress.TabIndex = 20;
            txtRemarks.TabIndex = 21;
            txtSpecialNote.TabIndex = 22;
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

        public void clearData()
        {
            try
            {
                grdWODetail.Rows.Clear();
                dgvComments.Rows.Clear();
                clearTabPageControls();
                pnlCmtr.Visible = false;
                pnlForwarder.Visible = false;
                //----------
                cmbContractor.SelectedIndex = -1;
                cmbCurrency.SelectedIndex = -1;
                cmbProjectID.SelectedIndex = -1;
                cmbOfficeID.SelectedIndex = -1;
               // cmbTaxCode.SelectedIndex = -1;
                cmbPaymentMode.SelectedIndex = -1;
                dtTemporaryDate.Value = DateTime.Parse("1900-01-01");
                dtWODate.Value = DateTime.Parse("1900-01-01");
                dtStartDate.Value = DateTime.Today.Date;
                dtTargetDate.Value = DateTime.Today.Date;

                grdWODetail.Rows.Clear();
                txtExchangeRate.Text = "";
                txtSpecialNote.Text = "";
                txtTermsAndCond.Text = "";
                txtPaymentTerms.Text = "";
                txtTemporaryNo.Text = "";
                txtWONo.Text = "";
                txtPOAddress.Text = "";
                txtRemarks.Text = "";
                txtReferenceInternalOrder.Text = "";
                btnProductValue.Text = "0";
                btnTaxAmount.Text = "0";
                btnTotalAmount.Text = "0";

                txtServiceValue.Text = "";
                txtServiceValueINR.Text = "";
                txtTaxAmount.Text = "";
                txtTaxAmountINR.Text = "";
                txtTotalValue.Text = "";
                txtTotalValueINR.Text = "";
                prevwoh = new workorderheader();
                removeControlsPaymentTermPanel();
                AddRowClick = false;
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
                tabControl1.Visible = true;
                setButtonVisibility("btnNew");
                AddRowClick = false;
            }
            catch (Exception)
            {

            }
        }


        private void btnAddLine_Click(object sender, EventArgs e)
        {
            try
            {
                AddWODetailRow();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this.ToString() + "-" + System.Reflection.MethodBase.GetCurrentMethod().Name + "() : Error");
            }
        }

        private Boolean AddWODetailRow()
        {
            Boolean status = true;
            try
            {
                if (grdWODetail.Rows.Count > 0)
                {
                    if (!verifyAndReworkWODetailGridRows())
                    {
                        return false;
                    }
                }
                grdWODetail.Rows.Add();
                int kount = grdWODetail.RowCount;
                grdWODetail.Rows[kount - 1].Cells[0].Value = kount;
                DataGridViewComboBoxCell ComboColumn2 = (DataGridViewComboBoxCell)(grdWODetail.Rows[kount - 1].Cells["gTCode"]);
                TaxCodeDB.fillTaxCodeGridViewCombo(ComboColumn2, "");
                grdWODetail.Rows[kount - 1].Cells["WorkDescription"].Value = " ";
                grdWODetail.Rows[kount - 1].Cells["WorkLocation"].Value = "";
                grdWODetail.Rows[kount - 1].Cells["Quantity"].Value = 0;
                grdWODetail.Rows[kount - 1].Cells["Price"].Value = 0;
                grdWODetail.Rows[kount - 1].Cells["Value"].Value = 0;
                grdWODetail.Rows[kount - 1].Cells["Tax"].Value = 0;
                grdWODetail.Rows[kount - 1].Cells["WarrantyDays"].Value = 0;
                grdWODetail.Rows[kount - 1].Cells["TaxDetails"].Value = " ";
                var BtnCell = (DataGridViewButtonCell)grdWODetail.Rows[kount - 1].Cells["Delete"];
                BtnCell.Value = "Del";
                if (AddRowClick)
                    grdWODetail.FirstDisplayedScrollingRowIndex = grdWODetail.RowCount - 1;
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
            //enableBottomButtons();
            //pnlBottomActions.Visible = true;
        }

        private Boolean verifyAndReworkWODetailGridRows()
        {
            Boolean status = true;

            try
            {
                double quantity = 0;
                double price = 0;
                double cost = 0.0;
                productvalue = 0.0;
                taxvalue = 0.0;
                string strtaxCode = "";

                if (grdWODetail.Rows.Count <= 0)
                {
                    MessageBox.Show("No entries in Work Order Grid details");
                    btnProductValue.Text = productvalue.ToString();
                    btnTaxAmount.Text = taxvalue.ToString(); //fill this later
                    btnTotalAmount.Text = (productvalue + taxvalue).ToString();
                    txtServiceValue.Text = productvalue.ToString();
                    txtServiceValueINR.Text = (Convert.ToDouble(txtServiceValue.Text) * Convert.ToDouble(txtExchangeRate.Text)).ToString();
                    txtTaxAmount.Text = taxvalue.ToString(); //fill this later
                    txtTaxAmountINR.Text = (Convert.ToDouble(txtTaxAmount.Text) * Convert.ToDouble(txtExchangeRate.Text)).ToString();
                    txtTotalValue.Text = (productvalue + taxvalue).ToString();
                    txtTotalValueINR.Text = (Convert.ToDouble(txtTotalValue.Text) * Convert.ToDouble(txtExchangeRate.Text)).ToString();
                    return false;
                }
                //clear tax details table
                TaxDetailsTable.Rows.Clear();
                for (int i = 0; i < grdWODetail.Rows.Count; i++)
                {
                    if (grdWODetail.Rows[i].Cells["gTCode"].Value == null ||
                        grdWODetail.Rows[i].Cells["gTCode"].Value.ToString().Length == 0)
                    {
                        MessageBox.Show("Fill TaxCode in row " + (i + 1));
                        return false;
                    }
                    grdWODetail.Rows[i].Cells["LineNo"].Value = (i + 1);
                    if (((grdWODetail.Rows[i].Cells["WorkDescription"].Value == null) ||
                        (grdWODetail.Rows[i].Cells["WorkLocation"].Value == null)) ||
                          (grdWODetail.Rows[i].Cells["WorkDescription"].Value.ToString().Trim().Length == 0) ||
                        (grdWODetail.Rows[i].Cells["WorkLocation"].Value.ToString().Trim().Length == 0) ||
                        (grdWODetail.Rows[i].Cells["Quantity"].Value == null) ||
                        (grdWODetail.Rows[i].Cells["Price"].Value == null) ||
                         (grdWODetail.Rows[i].Cells["Value"].Value == null) ||
                        (Convert.ToDouble(grdWODetail.Rows[i].Cells["Quantity"].Value) == 0) ||
                        (Convert.ToDouble(grdWODetail.Rows[i].Cells["Price"].Value) == 0))
                    {
                        MessageBox.Show("Fill values in row " + (i + 1));
                        return false;
                    }

                    quantity = Convert.ToDouble(grdWODetail.Rows[i].Cells["Quantity"].Value);
                    price = Convert.ToDouble(grdWODetail.Rows[i].Cells["Price"].Value);
                    if (price != 0 && quantity != 0)
                    {
                        cost = Math.Round(quantity * price, 2);
                    }

                    try
                    {
                        strtaxCode = grdWODetail.Rows[i].Cells["gTCode"].Value.ToString();
                    }
                    catch (Exception)
                    {
                        strtaxCode = "";
                    }
                    System.Data.DataTable TaxData = TaxCodeWorkingDB.calculateTax(strtaxCode, cost);
                    double ttax1 = 0.0;
                    double ttax2 = 0.0;
                    string strTax = "";
                    for (int j = 0; j < TaxData.Rows.Count; j++)
                    {
                        string tstr = "";
                        try
                        {
                            tstr = TaxData.Rows[j][7].ToString().Trim().Substring(0, TaxData.Rows[j][7].ToString().Trim().IndexOf('-'));
                            if (!(tstr.Length == 0 && tstr.Equals("Dummy")))
                            {
                                ttax1 = Convert.ToDouble(TaxData.Rows[j][6]);
                                string a = Convert.ToString(TaxData.Rows[j][1]);
                                string b = Convert.ToString(TaxData.Rows[j][6]);
                                string c = Convert.ToString(TaxData.Rows[j][7]);
                                strTax = strTax + tstr + "-" +
                                    Convert.ToString(TaxData.Rows[j][6]) + "\n";
                                int taxcodefound = 0;
                                for (int k = 0; k < (TaxDetailsTable.Rows.Count); k++)
                                {
                                    if (TaxDetailsTable.Rows[k][0].ToString().Trim().Equals(tstr))
                                    {
                                        TaxDetailsTable.Rows[k][1] = Convert.ToDouble(TaxDetailsTable.Rows[k][1]) +
                                            Convert.ToDouble(TaxData.Rows[j][6]);
                                        taxcodefound = 1;
                                    }
                                }
                                if (taxcodefound == 0)
                                {
                                    TaxDetailsTable.Rows.Add();
                                    TaxDetailsTable.Rows[TaxDetailsTable.Rows.Count - 1][0] = tstr;
                                    TaxDetailsTable.Rows[TaxDetailsTable.Rows.Count - 1][1] =
                                       Convert.ToDouble(TaxData.Rows[j][6]);
                                }
                            }
                            else
                            {
                                ttax1 = 0.0;
                            }
                        }
                        catch (Exception)
                        {
                            ttax1 = 0.0;
                        }
                        ttax2 = ttax2 + ttax1;
                    }
                    grdWODetail.Rows[i].Cells["Value"].Value = Convert.ToDouble(cost.ToString());
                    grdWODetail.Rows[i].Cells["Tax"].Value = ttax2.ToString();
                    grdWODetail.Rows[i].Cells["TaxDetails"].Value = strTax;
                    productvalue = productvalue + cost;
                    taxvalue = taxvalue + ttax2;

                    //- rewok tax value
                }
                btnProductValue.Text = productvalue.ToString();
                btnTaxAmount.Text = taxvalue.ToString(); //fill this later
                btnTotalAmount.Text = (productvalue + taxvalue).ToString();
                //btnProductValue.Text = txtServiceValue.Text;
                //btnTaxAmount.Text = txtTaxAmount.Text;
                txtServiceValue.Text = productvalue.ToString();
                txtServiceValueINR.Text = (Convert.ToDouble(txtServiceValue.Text) * Convert.ToDouble(txtExchangeRate.Text)).ToString();
                txtTaxAmount.Text = taxvalue.ToString(); //fill this later
                txtTaxAmountINR.Text = (Convert.ToDouble(txtTaxAmount.Text) * Convert.ToDouble(txtExchangeRate.Text)).ToString();
                txtTotalValue.Text = (productvalue + taxvalue).ToString();
                txtTotalValueINR.Text = (Convert.ToDouble(txtTotalValue.Text) * Convert.ToDouble(txtExchangeRate.Text)).ToString();
                if (!validateItems())
                {
                    MessageBox.Show("Validation failed");
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
                for (int i = 0; i < grdWODetail.Rows.Count - 1; i++)
                {
                    for (int j = i + 1; j < grdWODetail.Rows.Count; j++)
                    {

                        if (grdWODetail.Rows[i].Cells[1].Value.ToString() == grdWODetail.Rows[j].Cells["WorkDescription"].Value.ToString())
                        {
                            //duplicate item code
                            MessageBox.Show("Item code duplicated in WO details... please ensure correctness (" +
                                grdWODetail.Rows[i].Cells["WorkDescription"].Value.ToString() + ")");
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

        private List<workorderdetail> getWODetails(workorderheader woh)
        {
            List<workorderdetail> WODetails = new List<workorderdetail>();
            try
            {
                workorderdetail wod = new workorderdetail();

                for (int i = 0; i < grdWODetail.Rows.Count; i++)
                {
                    wod = new workorderdetail();
                    wod.DocumentID = woh.DocumentID;
                    wod.TemporaryNo = woh.TemporaryNo;
                    wod.TemporaryDate = woh.TemporaryDate;
                    wod.StockItemID = grdWODetail.Rows[i].Cells["Item"].Value.ToString().Substring(0, grdWODetail.Rows[i].Cells["Item"].Value.ToString().IndexOf('-'));
                    wod.WorkDescription = grdWODetail.Rows[i].Cells["WorkDescription"].Value.ToString().Trim();
                    wod.WorkLocation = grdWODetail.Rows[i].Cells["WorkLocation"].Value.ToString().Trim();
                    wod.TaxCode = grdWODetail.Rows[i].Cells["gTCode"].Value.ToString().Trim();
                    wod.Quantity = Convert.ToDouble(grdWODetail.Rows[i].Cells["Quantity"].Value);
                    wod.Price = Convert.ToDouble(grdWODetail.Rows[i].Cells["Price"].Value);
                    wod.Tax = Convert.ToDouble(grdWODetail.Rows[i].Cells["Tax"].Value);
                    wod.WarrantyDays = Convert.ToInt32(grdWODetail.Rows[i].Cells["WarrantyDays"].Value);

                    wod.TaxDetails = grdWODetail.Rows[i].Cells["TaxDetails"].Value.ToString();

                    WODetails.Add(wod);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("getWODetails() : Error getting Work Order Details");
                WODetails = null;
            }
            return WODetails;
        }
        private void btnSave_Click(object sender, EventArgs e)
        {

            Boolean status = true;
            try
            {
                WorkOrderDB wodb = new WorkOrderDB();
                workorderheader woh = new workorderheader();
                System.Windows.Forms.Button btn = sender as System.Windows.Forms.Button;
                string btnText = btnSave.Text;
                try
                {
                    if (!verifyAndReworkWODetailGridRows())
                    {
                        return;
                    }
                    woh.DocumentID = docID;
                    ////////woh.CustomerID = cmbCustomer.SelectedItem.ToString().Trim().Substring(0, cmbCustomer.SelectedItem.ToString().Trim().IndexOf('-'));
                    woh.CustomerID = ((Structures.ComboBoxItem)cmbContractor.SelectedItem).HiddenValue;
                   
                    woh.WODate = dtWODate.Value;
                    woh.ReferenceInternalOrder = txtReferenceInternalOrder.Text.ToString();

                    //string indentNostr = txtReferenceInternalOrder.Text;
                    //string[] substrRef = indentNostr.Trim().Split(';');
                    //string DocIDStr = substrRef[0].Trim();
                    string noDateStr = txtReferenceInternalOrder.Text;
                    int trackNo1 = Convert.ToInt32(noDateStr.Substring(0, noDateStr.IndexOf('(')));
                    int findex = noDateStr.IndexOf('(');
                    int sindex = noDateStr.IndexOf(')');
                    string tstr = noDateStr.Substring(findex + 1, (sindex - findex) - 1).Trim();
                    DateTime trackDate1 = Convert.ToDateTime(tstr);

                    woh.WORequestDate = Convert.ToDateTime(tstr); //Indent Date
                    woh.WORequestNo = trackNo1;                 ///Indent No

                    woh.POAddress = txtPOAddress.Text;
                    woh.StartDate = dtStartDate.Value;
                    woh.TargetDate = dtTargetDate.Value;
                    //////////woh.CurrencyID = cmbCurrency.SelectedItem.ToString().Trim().Substring(0, cmbCurrency.SelectedItem.ToString().Trim().IndexOf('-')).Trim();
                    woh.CurrencyID = ((Structures.ComboBoxItem)cmbCurrency.SelectedItem).HiddenValue;
                    woh.ExchangeRate = Convert.ToDecimal(txtExchangeRate.Text);
                    woh.PaymentMode = cmbPaymentMode.SelectedItem.ToString().Trim().Substring(0, cmbPaymentMode.SelectedItem.ToString().Trim().IndexOf('-')).Trim();
                    woh.OfficeID = cmbOfficeID.SelectedItem.ToString().Trim().Substring(0, cmbOfficeID.SelectedItem.ToString().Trim().IndexOf('-')).Trim();
                    woh.ProjectID = cmbProjectID.SelectedItem.ToString();
                    woh.PaymentTerms = txtPaymentTerms.Text;
                    woh.ServiceValue = Convert.ToDouble(txtServiceValue.Text);
                    //woh.TaxCode = cmbTaxCode.SelectedItem.ToString();
                    woh.TaxAmount = Convert.ToDouble(txtTaxAmount.Text);
                    woh.TotalAmount = Convert.ToDouble(txtTotalValue.Text);
                    woh.ServiceValueINR = Convert.ToDouble(txtServiceValueINR.Text);
                    woh.TaxAmountINR = Convert.ToDouble(txtTaxAmountINR.Text);
                    woh.TotalAmountINR = Convert.ToDouble(txtTotalValueINR.Text);
                    woh.TermsAndCond = txtTermsAndCond.Text;
                    woh.SpecialNote = txtSpecialNote.Text.Trim();
                    woh.Remarks = txtRemarks.Text;
                    woh.Comments = docCmtrDB.DGVtoString(dgvComments);
                    woh.ForwarderList = prevwoh.ForwarderList;

                    if(woh.SpecialNote.Trim().Length != 0)
                    {
                        if(woh.SpecialNote.Trim().Length > 500)
                        {
                            MessageBox.Show("Special note character length should not more than 500");
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Validation failed");
                    return;
                }
                if (btnText.Equals("Save"))
                {
                    //woh.TemporaryNo = DocumentNumberDB.getNewNumber(docID, 1);
                    woh.DocumentStatus = 1; //created
                    woh.TemporaryDate = UpdateTable.getSQLDateTime();
                }
                else
                {
                    woh.TemporaryNo = Convert.ToInt32(txtTemporaryNo.Text);
                    woh.TemporaryDate = prevwoh.TemporaryDate;
                    woh.DocumentStatus = prevwoh.DocumentStatus;
                }
                //Replacing single quotes
                woh = verifyHeaderInputString(woh);
                verifyDetailInputString();
                if (wodb.validateWORequestrHeader(woh))
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
                            woh.CommentStatus = docCmtrDB.createCommentStatusString(prevwoh.CommentStatus, Login.userLoggedIn);
                        }
                        else
                        {
                            woh.CommentStatus = prevwoh.CommentStatus;
                        }
                    }
                    else
                    {
                        if (commentStatus.Trim().Length > 0)
                        {
                            //clicked the Get Commenter button
                            woh.CommentStatus = docCmtrDB.createCommenterList(lvCmtr, dtCmtStatus);
                        }
                        else
                        {
                            woh.CommentStatus = prevwoh.CommentStatus;
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
                        woh.Comments = docCmtrDB.processNewComment(dgvComments, txtComments.Text, Login.userLoggedIn, Login.userLoggedInName, tmpStatus);
                    }
                    List<workorderdetail> WODetails = getWODetails(woh);
                    if (btnText.Equals("Update"))
                    {
                        if (wodb.updateWOHeaderAndDetail(woh, prevwoh, WODetails))
                        {
                            MessageBox.Show("Work Order details updated");
                            closeAllPanels();
                            listOption = 1;
                            ListFilteredWOHeader(listOption);
                        }
                        else
                        {
                            status = false;

                        }
                        if (!status)
                        {
                            MessageBox.Show("Failed to update Work Order Header");
                            status = false;
                        }
                    }
                    else if (btnText.Equals("Save"))
                    {
                        if (wodb.InsertWOHeaderAndDetail(woh, WODetails))
                        {
                            MessageBox.Show("Work Order Details Added");
                            closeAllPanels();
                            listOption = 1;
                            ListFilteredWOHeader(listOption);
                        }
                        else
                        {
                            status = false;
                        }
                        if (!status)
                        {
                            MessageBox.Show("Failed to Insert Work Order Header");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Work Order Header Validation failed");
                    status = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("createAndUpdateWODetails() : Error");
                status = false;
            }
            if (status)
            {
                setButtonVisibility("btnEditPanel"); //activites are same for cancel, forward,approve, reverse and save
            }
        }
        private void grdList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0)
                    return;
                string columnName = grdList.Columns[e.ColumnIndex].Name;
                if (columnName.Equals("Edit") || columnName.Equals("Approve") || columnName.Equals("LoadDocument") ||
                    columnName.Equals("View") || columnName.Equals("Print"))
                {
                    clearData();
                    setButtonVisibility(columnName);
                    AddRowClick = false;
                    docID = grdList.Rows[e.RowIndex].Cells[0].Value.ToString();
                    WorkOrderRequestDB wodb = new WorkOrderRequestDB();
                    int rowID = e.RowIndex;
                    btnSave.Text = "Update";
                    DataGridViewRow row = grdList.Rows[rowID];
                    prevwoh = new workorderheader();
                    prevwoh.CommentStatus = grdList.Rows[e.RowIndex].Cells["ComntStatus"].Value.ToString();
                    prevwoh.DocumentID = grdList.Rows[e.RowIndex].Cells["gDocumentID"].Value.ToString();
                    prevwoh.DocumentName = grdList.Rows[e.RowIndex].Cells["gDocumentName"].Value.ToString();
                    prevwoh.TemporaryNo = Convert.ToInt32(grdList.Rows[e.RowIndex].Cells["gTemporaryNo"].Value.ToString());
                    prevwoh.TemporaryDate = DateTime.Parse(grdList.Rows[e.RowIndex].Cells["gTemporaryDate"].Value.ToString());
                    if (prevwoh.CommentStatus.IndexOf(userCommentStatusString) >= 0)
                    {
                        // only for commeting and viwing documents
                        userIsACommenter = true;
                        setButtonVisibility("Commenter");
                    }
                    prevwoh.Comments = WorkOrderDB.getUserComments(prevwoh.DocumentID, prevwoh.TemporaryNo, prevwoh.TemporaryDate);
                    prevwoh.WONo = Convert.ToInt32(grdList.Rows[e.RowIndex].Cells["WONo"].Value.ToString());
                    prevwoh.WODate = DateTime.Parse(grdList.Rows[e.RowIndex].Cells["WODate"].Value.ToString());
                    prevwoh.WORequestNo = Convert.ToInt32(grdList.Rows[e.RowIndex].Cells["gWORequestNo"].Value.ToString());
                    prevwoh.WORequestDate = DateTime.Parse(grdList.Rows[e.RowIndex].Cells["gWORequestDate"].Value.ToString());
                    prevwoh.ReferenceInternalOrder = grdList.Rows[e.RowIndex].Cells["gReferenceInternalOrder"].Value.ToString();
                    prevwoh.ProjectID = grdList.Rows[e.RowIndex].Cells["gProjectID"].Value.ToString();
                    prevwoh.OfficeID = grdList.Rows[e.RowIndex].Cells["gOfficeID"].Value.ToString();
                    prevwoh.CustomerID = grdList.Rows[e.RowIndex].Cells["gCustomerID"].Value.ToString();
                    prevwoh.CustomerName = grdList.Rows[e.RowIndex].Cells["gCustomerName"].Value.ToString();

                    //--------Load Documents
                    if (columnName.Equals("LoadDocument"))
                    {
                        string hdrString = "Document Temp No:" + prevwoh.TemporaryNo + "\n" +
                            "Document Temp Date:" + prevwoh.TemporaryDate.ToString("dd-MM-yyyy") + "\n" +
                            "WORequest No:" + prevwoh.WORequestNo + "\n" +
                            "WORequest Date:" + prevwoh.WORequestDate.ToString("dd-MM-yyyy") + "\n" +
                            "Customer:" + prevwoh.CustomerName;
                        string dicDir = Main.documentDirectory + "\\" + docID;
                        string subDir = prevwoh.TemporaryNo + "-" + prevwoh.TemporaryDate.ToString("yyyyMMddhhmmss");
                        FileManager.LoadDocuments load = new FileManager.LoadDocuments(dicDir, subDir, docID, hdrString);
                        load.ShowDialog();
                        this.RemoveOwnedForm(load);
                        btnCancel_Click_2(null, null);
                        return;
                    }

                    if (columnName.Equals("View"))
                    {
                        tabControl1.TabPages["tabWODetail"].Enabled = true;
                    }
                    //--------
                    prevwoh.CurrencyID = grdList.Rows[e.RowIndex].Cells["gCurrencyID"].Value.ToString();
                    prevwoh.CurrencyName = grdList.Rows[e.RowIndex].Cells["gCurrencyName"].Value.ToString();
                    prevwoh.ExchangeRate = Convert.ToDecimal(grdList.Rows[e.RowIndex].Cells["ExchangeRate"].Value.ToString());
                    prevwoh.StartDate = DateTime.Parse(grdList.Rows[e.RowIndex].Cells["gStartDate"].Value.ToString());
                    prevwoh.TargetDate = DateTime.Parse(grdList.Rows[e.RowIndex].Cells["gTargetDate"].Value.ToString());
                    prevwoh.PaymentTerms = grdList.Rows[e.RowIndex].Cells["gPaymentTerms"].Value.ToString();
                    prevwoh.PaymentMode = grdList.Rows[e.RowIndex].Cells["gPaymentMode"].Value.ToString();
                    //prevwoh.TaxCode = grdList.Rows[e.RowIndex].Cells["gTaxCode"].Value.ToString();
                    prevwoh.POAddress = grdList.Rows[e.RowIndex].Cells["gPOAddress"].Value.ToString();
                    prevwoh.ServiceValue = Convert.ToDouble(grdList.Rows[e.RowIndex].Cells["gServiceValue"].Value.ToString());
                    prevwoh.TotalAmount = Convert.ToDouble(grdList.Rows[e.RowIndex].Cells["gTotalAmount"].Value.ToString());
                    prevwoh.TaxAmount = Convert.ToDouble(grdList.Rows[e.RowIndex].Cells["gTaxAmount"].Value.ToString());
                    prevwoh.ServiceValueINR = Convert.ToDouble(grdList.Rows[e.RowIndex].Cells["ServiceValueINR"].Value.ToString());
                    prevwoh.TotalAmountINR = Convert.ToDouble(grdList.Rows[e.RowIndex].Cells["TotalAmountINR"].Value.ToString());
                    prevwoh.TaxAmountINR = Convert.ToDouble(grdList.Rows[e.RowIndex].Cells["TaxAmountINR"].Value.ToString());
                    prevwoh.TermsAndCond = grdList.Rows[e.RowIndex].Cells["TermsAndCond"].Value.ToString();
                    prevwoh.Remarks = grdList.Rows[e.RowIndex].Cells["gRemarks"].Value.ToString();
                    prevwoh.Status = Convert.ToInt32(grdList.Rows[e.RowIndex].Cells["gStatus"].Value.ToString());
                    //prevwoh.WorkOrderStatus = Convert.ToInt32(grdList.Rows[e.RowIndex].Cells["WorkOrderStatus"].Value.ToString());
                    prevwoh.WorkOrderStatus = Convert.ToInt32(grdList.Rows[e.RowIndex].Cells["WorkOrderStatus"].Value.ToString());
                    prevwoh.DocumentStatus = Convert.ToInt32(grdList.Rows[e.RowIndex].Cells["gDocumentStatus"].Value.ToString());
                    prevwoh.CreateTime = DateTime.Parse(grdList.Rows[e.RowIndex].Cells["gCreateTime"].Value.ToString());
                    prevwoh.CreateUser = grdList.Rows[e.RowIndex].Cells["gCreateUser"].Value.ToString();
                    prevwoh.CreatorName = grdList.Rows[e.RowIndex].Cells["gCreator"].Value.ToString();
                    prevwoh.ForwarderName = grdList.Rows[e.RowIndex].Cells["gForwarder"].Value.ToString();
                    prevwoh.ApproverName = grdList.Rows[e.RowIndex].Cells["gApprover"].Value.ToString();
                    prevwoh.ForwarderList = grdList.Rows[e.RowIndex].Cells["ForwarderLists"].Value.ToString();
                    prevwoh.SpecialNote = grdList.Rows[e.RowIndex].Cells["SpecialNote"].Value.ToString().Trim();
                    //--comments
                    chkCommentStatus.Checked = false;
                    docCmtrDB = new DocCommenterDB();
                    pnlComments.Controls.Remove(dgvComments);
                    prevwoh.CommentStatus = grdList.Rows[e.RowIndex].Cells["ComntStatus"].Value.ToString();

                    dtCmtStatus = docCmtrDB.splitCommentStatus(prevwoh.CommentStatus);
                    dgvComments = new DataGridView();
                    dgvComments = docCmtrDB.createCommentGridview(prevwoh.Comments);
                    pnlComments.Controls.Add(dgvComments);
                    txtComments.Text = docCmtrDB.getLastUnauthorizedComment(dgvComments, Login.userLoggedIn);
                    dgvComments.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvComments_CellDoubleClick);

                    //---


                    //-----
                    ////////cmbCustomer.SelectedIndex = cmbCustomer.FindString(prevwoh.CustomerID);
                    cmbContractor.SelectedIndex =
                        Structures.ComboFUnctions.getComboIndex(cmbContractor, prevwoh.CustomerID);
                    //cmbStatus.SelectedIndex = cmbStatus.FindStringExact(ComboFIll.getStatusString(Convert.ToInt32(grdList.Rows[e.RowIndex].Cells["gStatus"].Value.ToString())));
                    cmbCurrency.SelectedIndex = cmbCurrency.FindString(prevwoh.CurrencyID);
                    cmbCurrency.SelectedIndex =
                        Structures.ComboFUnctions.getComboIndex(cmbCurrency, prevwoh.CurrencyID);
                    //cmbPaymentTerms.SelectedIndex = cmbPaymentTerms.FindString(grdList.Rows[e.RowIndex].Cells["gPaymentTerms"].Value.ToString());
                    cmbPaymentMode.SelectedIndex = cmbPaymentMode.FindString(prevwoh.PaymentMode);
                    cmbProjectID.SelectedIndex = cmbProjectID.FindString(prevwoh.ProjectID);
                    cmbOfficeID.SelectedIndex = cmbOfficeID.FindString(prevwoh.OfficeID);
                    //cmbTaxCode.SelectedIndex = cmbTaxCode.FindString(prevwoh.TaxCode);
                    txtWONo.Text = prevwoh.WONo.ToString();
                    txtTemporaryNo.Text = prevwoh.TemporaryNo.ToString();
                    try
                    {
                        dtWODate.Value = prevwoh.WODate;
                    }
                    catch (Exception)
                    {
                        dtWODate.Value = DateTime.Parse("01-01-1900");
                    }
                    try
                    {
                        dtTemporaryDate.Value = prevwoh.TemporaryDate;
                    }
                    catch (Exception)
                    {
                        dtTemporaryDate.Value = DateTime.Parse("01-01-1900");
                    }
                    //txtWORequestNo1.Text = prevwoh.WORequestNo.ToString();
                    //txtWORequestNo.Text = prevwoh.WORequestNo.ToString();
                    //try
                    //{
                    //    dtWORequestDate1.Value = prevwoh.WORequestDate;
                    //    dtWORequestDate.Value = prevwoh.WORequestDate;
                    //}
                    //catch (Exception)
                    //{
                    //    dtWORequestDate1.Value = DateTime.Parse("01-01-1900");
                    //    dtWORequestDate.Value = DateTime.Parse("01-01-1900");
                    //}
                    dtStartDate.Value = prevwoh.StartDate;
                    dtTargetDate.Value = prevwoh.TargetDate;
                    txtPOAddress.Text = prevwoh.POAddress;
                    txtTermsAndCond.Text = prevwoh.TermsAndCond;
                    txtPaymentTerms.Text = prevwoh.PaymentTerms;
                    btnProductValue.Text = prevwoh.ServiceValue.ToString();
                    btnTaxAmount.Text = prevwoh.TaxAmount.ToString();
                    btnTotalAmount.Text = prevwoh.TotalAmount.ToString();
                    txtRemarks.Text = prevwoh.Remarks;
                    txtReferenceInternalOrder.Text = prevwoh.ReferenceInternalOrder.ToString();

                    txtExchangeRate.Text = prevwoh.ExchangeRate.ToString();
                    txtServiceValue.Text = prevwoh.ServiceValue.ToString();
                    txtServiceValueINR.Text = prevwoh.ServiceValueINR.ToString();
                    txtTaxAmount.Text = prevwoh.TaxAmount.ToString();
                    txtTaxAmountINR.Text = prevwoh.TaxAmountINR.ToString();
                    txtTotalValue.Text = prevwoh.TotalAmount.ToString();
                    txtTotalValueINR.Text = prevwoh.TotalAmountINR.ToString();
                    txtSpecialNote.Text = prevwoh.SpecialNote.ToString().Trim();

                    List<workorderdetail> WODetail = WorkOrderDB.getWorkOrderDetails(prevwoh);
                    grdWODetail.Rows.Clear();
                    int i = 0;
                    foreach (workorderdetail wod in WODetail)
                    {
                        if (!AddWODetailRow())
                        {
                            MessageBox.Show("Error found in WOrk Order Detail. Please correct before updating the details");
                        }
                        else
                        {
                            grdWODetail.Rows[i].Cells["Item"].Value = wod.StockItemID + "-" + wod.Description;
                            grdWODetail.Rows[i].Cells["WorkDescription"].Value = wod.WorkDescription;
                            grdWODetail.Rows[i].Cells["gTCode"].Value = wod.TaxCode;
                            grdWODetail.Rows[i].Cells["WorkLocation"].Value = wod.WorkLocation;
                            grdWODetail.Rows[i].Cells["Quantity"].Value = wod.Quantity;
                            grdWODetail.Rows[i].Cells["Price"].Value = wod.Price;
                            grdWODetail.Rows[i].Cells["Value"].Value = wod.Quantity * wod.Price;
                            grdWODetail.Rows[i].Cells["Tax"].Value = wod.Tax;
                            grdWODetail.Rows[i].Cells["WarrantyDays"].Value = wod.WarrantyDays;
                            grdWODetail.Rows[i].Cells["TaxDetails"].Value = wod.TaxDetails;
                            i++;
                            productvalue = productvalue + wod.Quantity * wod.Price;
                            taxvalue = taxvalue + wod.Tax;
                        }

                    }
                    if (!verifyAndReworkWODetailGridRows())
                    {
                        MessageBox.Show("Error found in Work Order details. Please correct before updating the details");
                    }
                    if (columnName.Equals("Print"))
                    {
                        //PrintPurchaseOrder ppo = new PrintPurchaseOrder();
                        pnlAddEdit.Visible = false;
                        pnlList.Visible = true;
                        grdList.Visible = true;
                        btnNew.Visible = true;
                        btnExit.Visible = true;
                        //CSLERP.PrintForms.PrintPurchaseOrder ppo = new CSLERP.PrintForms.PrintPurchaseOrder();
                        string TotalTaxDetailstr = "";
                        for (int n = 0; n < (TaxDetailsTable.Rows.Count); n++)
                        {
                            TotalTaxDetailstr = TotalTaxDetailstr + Convert.ToString(TaxDetailsTable.Rows[n][0]) + "-" +
                            Convert.ToString(TaxDetailsTable.Rows[n][1]) + "\n";
                        }
                        PrintWorkOrder pwo = new PrintWorkOrder();
                        List<workorderdetail> WODetails = WorkOrderDB.getWorkOrderDetails(prevwoh);
                        pwo.PrintWO(prevwoh, WODetails, TotalTaxDetailstr);
                        btnNew.Visible = true;
                        btnExit.Visible = true;
                        return;
                    }
                    pnlList.Visible = false;
                    pnlAddEdit.Visible = true;
                    tabControl1.SelectedTab = tabWOHeader;
                    tabControl1.Visible = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this.ToString() + "-" + System.Reflection.MethodBase.GetCurrentMethod().Name + "() : Error");
                setButtonVisibility("init");
            }
        }
        private void showWorkOrderHeader(indentserviceheader ish)
        {
            dtStartDate.Value = ish.StartDate;
            dtTargetDate.Value = ish.TargetDate;
            cmbContractor.SelectedIndex = Structures.ComboFUnctions.getComboIndex(cmbContractor, ish.CustomerID);
            
            txtPaymentTerms.Text = ish.PaymentTerms;
            cmbPaymentMode.SelectedIndex = cmbPaymentMode.FindString(ish.PaymentMode);
            cmbCurrency.SelectedIndex = Structures.ComboFUnctions.getComboIndex(cmbCurrency, ish.CurrencyID);
            txtExchangeRate.Text = ish.ExchangeRate.ToString();
            //txtTermsAndCond.Text = ish.TermsAndCond;
            //txtPOAddress.Text = ish.POAddress;
            //txtRemarks.Text = ish.Remarks;

            string topStr = "";
            int topTrackNo = 0;
            DateTime toTrackDate = DateTime.Parse("1900-01-01");
            string docID = "";
            string refIndent = ish.ReferenceInternalOrder;
            string[] mainstrRef = refIndent.Trim().Split(Main.delimiter1);
            foreach (string str in mainstrRef)
            {
                if (str.Trim().Length != 0)
                {
                    string[] tempsubstrRef = str.Trim().Split(';');
                    string tempDocIDStr = tempsubstrRef[0].Trim();
                    string tempnoDateStr = tempsubstrRef[1].Trim();
                    int temptrackNo1 = Convert.ToInt32(tempnoDateStr.Substring(0, tempnoDateStr.IndexOf('(')));
                    if(topTrackNo < temptrackNo1)
                    {
                        topStr = str;
                        topTrackNo = temptrackNo1;
                    }
                    
                }
            }
            string[] substrRef = topStr.Trim().Split(';');
            string DocIDStr = substrRef[0].Trim();
            string noDateStr = substrRef[1].Trim();
            int trackNo1 = Convert.ToInt32(noDateStr.Substring(0, noDateStr.IndexOf('(')));
            int findex = noDateStr.IndexOf('(');
            int sindex = noDateStr.IndexOf(')');
            string tstr = noDateStr.Substring(findex + 1, (sindex - findex) - 1).Trim();
            DateTime trackDate1 = Convert.ToDateTime(tstr);
            string detStr = POPIHeaderDB.getPOPIServiceDetailForIndentService(trackNo1, trackDate1, DocIDStr);
            string[] getsplittedStr = detStr.Split(Main.delimiter1);

            cmbProjectID.SelectedIndex = cmbProjectID.FindString(getsplittedStr[2]);
            cmbOfficeID.SelectedIndex = cmbOfficeID.FindString(getsplittedStr[3]);


            btnProductValue.Text = ish.ServiceValue.ToString();
            btnTaxAmount.Text = ish.TaxAmount.ToString();
            btnTotalAmount.Text = ish.TotalAmount.ToString();
           
            txtServiceValue.Text = ish.ServiceValue.ToString();
            txtServiceValueINR.Text = ish.ServiceValueINR.ToString();
            txtTaxAmount.Text = ish.TaxAmount.ToString();
            txtTaxAmountINR.Text = ish.TaxAmountINR.ToString();
            txtTotalValue.Text = ish.TotalAmount.ToString();
            txtTotalValueINR.Text = ish.TotalAmountINR.ToString();
        
        }
        private void showWorkOrderDetail(List<indentservicedetail> ISList)
        {
            grdWODetail.Rows.Clear();
            int i = 0;
            foreach (indentservicedetail isd in ISList)
            {
                if (!AddWODetailRow())
                {
                    MessageBox.Show("Error found in Indent service Detail. Please correct before updating the details");
                }
                else
                {
                    grdWODetail.Rows[i].Cells["Item"].Value = isd.StockItemID + "-" + isd.Description;
                    grdWODetail.Rows[i].Cells["WorkDescription"].Value = isd.WorkDescription;
                    grdWODetail.Rows[i].Cells["gTCode"].Value = isd.TaxCode;
                    grdWODetail.Rows[i].Cells["WorkLocation"].Value = isd.WorkLocation;
                    grdWODetail.Rows[i].Cells["Quantity"].Value = isd.Quantity;
                    grdWODetail.Rows[i].Cells["Price"].Value = isd.Rate;
                    grdWODetail.Rows[i].Cells["Value"].Value = isd.Quantity * isd.Price;
                    grdWODetail.Rows[i].Cells["Tax"].Value = isd.Tax;
                    grdWODetail.Rows[i].Cells["WarrantyDays"].Value = isd.WarrantyDays;
                    grdWODetail.Rows[i].Cells["TaxDetails"].Value = isd.TaxDetails;
                    i++;
                    productvalue = productvalue + isd.Quantity * isd.Price;
                    taxvalue = taxvalue + isd.Tax;
                }
            }
        }
        private void pnlUI_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnCancel_Click_2(object sender, EventArgs e)
        {
            clearData();
            closeAllPanels();
            pnlList.Visible = true;
            setButtonVisibility("btnEditPanel");
        }

        private void btnAddNewLine_Click(object sender, EventArgs e)
        {
            AddRowClick = true;
            AddWODetailRow();
        }

        private void Calculate_Click(object sender, EventArgs e)
        {
            //if (cmbTaxCode.SelectedIndex == -1)
            //{
            //    MessageBox.Show("select tax Code");
            //    return;
            //}
            if (txtExchangeRate.Text.Length == 0)
            {
                MessageBox.Show("Fill exchange rate");
                return;
            }
            verifyAndReworkWODetailGridRows();
        }

        private void ClearEntries_Click(object sender, EventArgs e)
        {
            try
            {

                DialogResult dialog = MessageBox.Show("Are you sure to clear all entries in grid detail ?", "Yes", MessageBoxButtons.YesNo);
                if (dialog == DialogResult.Yes)
                {
                    grdWODetail.Rows.Clear();
                    MessageBox.Show("Grid items cleared.");
                    verifyAndReworkWODetailGridRows();
                }

            }
            catch (Exception)
            {
            }
        }

        private void grdQIDetail_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0)
                    return;
                string columnName = grdWODetail.Columns[e.ColumnIndex].Name;
                try
                {
                    if (columnName.Equals("Delete"))
                    {
                        //delete row
                        DialogResult dialog = MessageBox.Show("Are you sure to Delete the row ?", "Yes", MessageBoxButtons.YesNo);
                        if (dialog == DialogResult.Yes)
                        {
                            grdWODetail.Rows.RemoveAt(e.RowIndex);
                        }
                        verifyAndReworkWODetailGridRows();
                    }
                    if (columnName.Equals("TaxView"))
                    {
                        //show tax details
                        DialogResult dialog = MessageBox.Show(grdWODetail.Rows[e.RowIndex].Cells["TaxDetails"].Value.ToString(),
                            "Line No : " + (e.RowIndex + 1), MessageBoxButtons.OK);
                    }
                    if (columnName.Equals("Sel"))
                    {
                        showserviceItemsTreeView();
                    }
                    if (columnName.Equals("SelDesc"))
                    {
                        descClickRowIndex = e.RowIndex;
                        string strTest = grdWODetail.Rows[e.RowIndex].Cells["WorkDescription"].Value.ToString().Trim();
                        showPopUpForDescription(strTest);
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
        private void showPopUpForDescription(string str)
        {
            frmPopup = new Form();
            frmPopup.StartPosition = FormStartPosition.CenterScreen;
            frmPopup.BackColor = Color.CadetBlue;
            frmPopup.MaximizeBox = false;
            frmPopup.MinimizeBox = false;
            frmPopup.ControlBox = false;
            frmPopup.FormBorderStyle = FormBorderStyle.FixedSingle;
            frmPopup.Size = new Size(360, 170);

            Label head = new Label();
            head.AutoSize = true;
            head.Location = new System.Drawing.Point(3, 3);
            head.Name = "label2";
            head.Font = new System.Drawing.Font("Arial", 10, FontStyle.Bold);
            head.ForeColor = Color.White;
            head.Size = new System.Drawing.Size(146, 13);
            head.Text = "Fill Description Below";
            frmPopup.Controls.Add(head);

            txtDesc = new RichTextBox();
            txtDesc.Text = str;
            txtDesc.Bounds = new Rectangle(new Point(3, 25), new Size(345, 111));
            frmPopup.Controls.Add(txtDesc);

            Button lvOK = new Button();
            lvOK.Text = "OK";
            lvOK.BackColor = Color.Tan;
            lvOK.Location = new System.Drawing.Point(210, 142);
            lvOK.Size = new System.Drawing.Size(64, 23);
            lvOK.Cursor = Cursors.Hand;
            lvOK.Click += new System.EventHandler(this.lvOK_Click5);
            frmPopup.Controls.Add(lvOK);

            Button lvCancel = new Button();
            lvCancel.Text = "CANCEL";
            lvCancel.BackColor = Color.Tan;
            lvCancel.Location = new System.Drawing.Point(273, 142);
            lvCancel.Size = new System.Drawing.Size(73, 23);
            lvCancel.Cursor = Cursors.Hand;
            lvCancel.Click += new System.EventHandler(this.lvCancel_Click5);
            frmPopup.Controls.Add(lvCancel);

            frmPopup.ShowDialog();
        }
        private void lvOK_Click5(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(txtDesc.Text.Trim()))
                {
                    MessageBox.Show("Description is empty");
                    return;
                }
                grdWODetail.Rows[descClickRowIndex].Cells["WorkDescription"].Value = txtDesc.Text.Trim();
                grdWODetail.FirstDisplayedScrollingRowIndex = grdWODetail.Rows[descClickRowIndex].Index;
                frmPopup.Close();
                frmPopup.Dispose();
            }
            catch (Exception ex)
            {

            }
        }

        private void lvCancel_Click5(object sender, EventArgs e)
        {
            try
            {
                txtDesc.Text = "";
                descClickRowIndex = -1;
                frmPopup.Close();
                frmPopup.Dispose();

            }
            catch (Exception)
            {
            }
        }
        private void removeControlsFromForwarderPanelTV()
        {
            try
            {
                pnlForwarder.Controls.Clear();
                Control nc = pnlForwarder.Parent;
                nc.Controls.Remove(pnlForwarder);
            }
            catch (Exception ex)
            {
            }
        }
        private void showserviceItemsTreeView()
        {
            removeControlsFromForwarderPanelTV();
            tv = new TreeView();
            tv.CheckBoxes = true;
            tv.Nodes.Clear();
            tv.CheckBoxes = true;
            pnlForwarder.BorderStyle = BorderStyle.FixedSingle;
            pnlForwarder.Bounds = new Rectangle(new Point(100, 10), new Size(700, 300));
            Label lbl = new Label();
            lbl.AutoSize = true;
            lbl.Location = new Point(50, 8);
            lbl.Size = new Size(35, 13);

            lbl.Font = new Font("Serif", 10, FontStyle.Bold);
            lbl.ForeColor = Color.Green;
            lbl.Text = "Tree View For Service";
            tv = ServiceItemsDB.getServiceItemTreeView();
            pnlForwarder.Controls.Add(lbl);
            tv.Bounds = new Rectangle(new Point(50, 30), new Size(600, 220));
            pnlForwarder.Controls.Remove(tv);
            pnlForwarder.Controls.Add(tv);
            //tv.cl
            tv.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.tv_AfterCheck);
            Button lvForwrdOK = new Button();
            lvForwrdOK.Text = "OK";
            lvForwrdOK.BackColor = Color.LightGreen;
            lvForwrdOK.Size = new Size(100, 30);
            lvForwrdOK.Location = new Point(50, 260);
            lvForwrdOK.Click += new System.EventHandler(this.tvOK_Click);
            pnlForwarder.Controls.Add(lvForwrdOK);

            Button lvForwardCancel = new Button();
            lvForwardCancel.Text = "Cancel";
            lvForwardCancel.BackColor = Color.LightGreen;
            lvForwardCancel.Size = new Size(100, 30);
            lvForwardCancel.Location = new Point(150, 260);
            lvForwardCancel.Click += new System.EventHandler(this.tvCancel_Click);
            pnlForwarder.Controls.Add(lvForwardCancel);
            ////lvForwardCancel.Visible = false;
            //tv.CheckBoxes = true;
            pnlForwarder.Visible = true;
            pnlAddEdit.Controls.Add(pnlForwarder);
            pnlAddEdit.BringToFront();
            pnlForwarder.BringToFront();
            pnlForwarder.Focus();
        }
        private void tvOK_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> ItemList = GetCheckedNodes(tv.Nodes);
                if (ItemList.Count > 1 || ItemList.Count == 0)
                {
                    MessageBox.Show("select only one item");
                    return;
                }
                foreach (string s in ItemList)
                {
                    grdWODetail.CurrentRow.Cells["Item"].Value = s;
                    //grdPOPIDetail.CurrentRow.Cells["ServiceItem"].Value = s;
                    tv.CheckBoxes = true;
                    pnlForwarder.Controls.Remove(tv);
                    pnlForwarder.Visible = false;
                }

            }
            catch (Exception)
            {
            }
        }
        public List<string> GetCheckedNodes(TreeNodeCollection nodes)
        {
            List<string> nodeList = new List<string>();
            try
            {

                if (nodes == null)
                {
                    return nodeList;
                }

                foreach (TreeNode childNode in nodes)
                {
                    if (childNode.Checked)
                    {
                        nodeList.Add(childNode.Text);
                    }
                    nodeList.AddRange(GetCheckedNodes(childNode.Nodes));
                }

            }
            catch (Exception ex)
            {
            }
            return nodeList;
        }
        private void tvCancel_Click(object sender, EventArgs e)
        {
            try
            {
                //lvApprover.CheckBoxes = false;
                //lvApprover.CheckBoxes = true;
                tv.CheckBoxes = true;
                pnlForwarder.Controls.Remove(tv);
                pnlForwarder.Visible = false;
            }
            catch (Exception)
            {
            }
        }
        private void tv_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Checked == true)
            {
                if (e.Node.Nodes.Count != 0)
                {
                    MessageBox.Show("you are not allowed to select group");
                    e.Node.Checked = false;
                }
            }
        }


        private void showserviceItemsListView()
        {
            //removeControlsPnllvPanel();
            //pnllv = new Panel();
            //pnllv.BorderStyle = BorderStyle.FixedSingle;
            //pnllv.Bounds = new Rectangle(new Point(100, 100), new Size(600, 300));
            frmPopup = new Form();
            frmPopup.StartPosition = FormStartPosition.CenterScreen;
            frmPopup.BackColor = Color.CadetBlue;

            frmPopup.MaximizeBox = false;
            frmPopup.MinimizeBox = false;
            frmPopup.ControlBox = false;
            frmPopup.FormBorderStyle = FormBorderStyle.FixedSingle;

            frmPopup.Size = new Size(450, 300);

            lv = CatalogueValueDB.getCatalogValueListView("ServiceLookup");
            //this.lv.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listView2_ItemCheck3);
            //lv.MultiSelect = false;
            lv.Bounds = new Rectangle(new Point(0, 0), new Size(450, 250));
            frmPopup.Controls.Add(lv);

            Button lvOK = new Button();
            lvOK.BackColor = Color.Tan;
            lvOK.Text = "OK";
            lvOK.Location = new Point(40, 265);
            lvOK.Click += new System.EventHandler(this.lvOK_Clicked3);
            frmPopup.Controls.Add(lvOK);

            Button lvCancel = new Button();
            lvCancel.BackColor = Color.Tan;
            lvCancel.Text = "CANCEL";
            lvCancel.Location = new Point(130, 265);
            lvCancel.Click += new System.EventHandler(this.lvCancel_Clicked3);
            frmPopup.Controls.Add(lvCancel);
            frmPopup.ShowDialog();
            //pnlAddEdit.Controls.Add(pnllv);
            //pnllv.BringToFront();
            //pnllv.Visible = true;
        }
        private void lvOK_Clicked3(object sender, EventArgs e)
        {
            try
            {
                if (!checkLVItemChecked("Item"))
                {
                    return;
                }
                foreach (ListViewItem itemRow in lv.Items)
                {
                    if (itemRow.Checked)
                    {
                        grdWODetail.CurrentRow.Cells["Item"].Value = itemRow.SubItems[1].Text + "-" + itemRow.SubItems[2].Text;
                        frmPopup.Close();
                        frmPopup.Dispose();
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void lvCancel_Clicked3(object sender, EventArgs e)
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
        //private void listView2_ItemCheck3(object sender, ItemCheckedEventArgs e)
        //{
        //    int c = lv.Items.Count;
        //    if (lv.CheckedItems.Count > 1)
        //    {
        //        MessageBox.Show("Cannot select more than one item");
        //        e.Item.Checked = false;
        //    }
        //}
        private void btnTaxDetail_Click(object sender, EventArgs e)
        {
            try
            {
                string strTax = "";
                for (int i = 0; i < (TaxDetailsTable.Rows.Count); i++)
                {
                    strTax = strTax + Convert.ToString(TaxDetailsTable.Rows[i][0]) + "-" +
                    Convert.ToString(TaxDetailsTable.Rows[i][1]) + "\n";
                }
                DialogResult dialog = MessageBox.Show(strTax, "Tax Details", MessageBoxButtons.OK);
            }
            catch (Exception)
            {
                MessageBox.Show("Error showing tax details");
            }
        }

        private void pnlAddEdit_Paint(object sender, PaintEventArgs e)
        {

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
            ListFilteredWOHeader(listOption);
        }

        private void btnApprovalPending_Click(object sender, EventArgs e)
        {
            listOption = 2;
            ListFilteredWOHeader(listOption);
        }
        private void btnActionPending_Click(object sender, EventArgs e)
        {
            listOption = 1;
            ListFilteredWOHeader(listOption);
        }
        private Boolean updateDashBoard(workorderheader woh, int stat)
        {
            Boolean status = true;
            try
            {
                dashboardalarm dsb = new dashboardalarm();
                DashboardDB ddsDB = new DashboardDB();
                dsb.DocumentID = woh.DocumentID;
                dsb.TemporaryNo = woh.TemporaryNo;
                dsb.TemporaryDate = woh.TemporaryDate;
                dsb.DocumentNo = woh.WONo;
                dsb.DocumentDate = woh.WODate;
                dsb.FromUser = Login.userLoggedIn;
                if (stat == 1)
                {
                    dsb.ActivityType = 2;
                    dsb.ToUser = woh.ForwardUser;
                    if (!ddsDB.insertDashboardAlarm(dsb))
                    {
                        MessageBox.Show("DashBoard Fail to update");
                        status = false;
                    }
                }
                else if (stat == 2)
                {
                    dsb.ActivityType = 3;
                    List<documentreceiver> docList = DocumentReceiverDB.getDocumentWiseReceiver(woh.DocumentID);
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
        private void btnForward_Click(object sender, EventArgs e)
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
            frmPopup.ShowDialog();
            //pnlForwarder.Visible = true;
            //pnlAddEdit.Controls.Add(pnlForwarder);
            //pnlAddEdit.BringToFront();
            //pnlForwarder.BringToFront();
            //pnlForwarder.Focus();
        }
        private void cmbCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                customer cust = new customer();
                ////////string custid = cmbCustomer.SelectedItem.ToString().Trim().Substring(0, cmbCustomer.SelectedItem.ToString().Trim().IndexOf('-'));
                string custid = ((Structures.ComboBoxItem)cmbContractor.SelectedItem).HiddenValue;
                cust = CustomerDB.getCustomerDetails(custid);
                string AddCust = cust.BillingAddress ;
                txtPOAddress.Text = AddCust;
            }
            catch (Exception ex)
            {

            }
        }
        private void btnApprove_Click(object sender, EventArgs e)
        {
            try
            {
                WorkOrderDB wodb = new WorkOrderDB();
                ////popiheader popih = new popiheader();
                FinancialLimitDB flDB = new FinancialLimitDB();
                if (!flDB.checkEmployeeFinancialLimit(docID, Login.empLoggedIn, Convert.ToDouble(btnProductValue.Text), 0))
                {
                    MessageBox.Show("No financial power for approving this document");
                    return;
                }
                if (prevwoh.StartDate < DateTime.Now)
                {
                    DialogResult dialog = MessageBox.Show("Are you sure to Approve the document ?", "Yes", MessageBoxButtons.YesNo);
                    if (dialog == DialogResult.Yes)
                    {
                        prevwoh.CommentStatus = DocCommenterDB.removeUnapprovedCommentStatus(prevwoh.CommentStatus);
                        if (prevwoh.Status != 96)
                        {
                            prevwoh.WONo = DocumentNumberDB.getNewNumber(docID, 2);
                        }
                        if (wodb.ApproveWorkOrder(prevwoh))
                        {
                            MessageBox.Show("WO Document Approved");
                            if (!updateDashBoard(prevwoh, 2))
                            {
                                MessageBox.Show("DashBoard Fail to update");
                            }
                            closeAllPanels();
                            listOption = 1;
                            ListFilteredWOHeader(listOption);
                            setButtonVisibility("btnEditPanel"); //activites are same for cance, forward,approce and reverse
                        }
                    }
                }
                else
                    MessageBox.Show("Not allowed to Approve. Start Date is Grater than Today Date");

            }
            catch (Exception)
            {
            }
        }

        private void btnReferenceInternalOrder_Click(object sender, EventArgs e)
        {
            if (txtReferenceInternalOrder.Text.Trim().Length != 0)
            {
                DialogResult dialog = MessageBox.Show("Work Order Details will be removed . Are You sure to continue ?", "Yes", MessageBoxButtons.YesNo);
                if (dialog == DialogResult.Yes)
                {
                    grdWODetail.Rows.Clear();
                    txtReferenceInternalOrder.Text = "";
                    dtStartDate.Value = DateTime.Today.Date;
                    dtTargetDate.Value = DateTime.Today.Date;
                    cmbContractor.SelectedIndex = -1;
                    cmbProjectID.SelectedIndex = -1;
                    cmbOfficeID.SelectedIndex = -1;
                    txtPaymentTerms.Text = "";
                    cmbPaymentMode.SelectedIndex = -1;
                    cmbCurrency.SelectedIndex = -1;
                    txtExchangeRate.Text = "";
                    txtRemarks.Text = "";
                    txtPOAddress.Text = "";
                    txtTermsAndCond.Text = "";
                    txtServiceValue.Text = "";
                    txtServiceValueINR.Text = "";
                    txtTaxAmount.Text = "";
                    txtTaxAmountINR.Text = "";
                    txtTotalValue.Text = "";
                    txtTotalValueINR.Text = "";
                }
                else
                {
                    return;
                }
            }
            frmPopup = new Form();
            frmPopup.StartPosition = FormStartPosition.CenterScreen;
            frmPopup.BackColor = Color.CadetBlue;

            frmPopup.MaximizeBox = false;
            frmPopup.MinimizeBox = false;
            frmPopup.ControlBox = false;
            frmPopup.FormBorderStyle = FormBorderStyle.FixedSingle;

            frmPopup.Size = new Size(600, 300);
            lv = IndentServiceDB.IndentServiceSelectionListView();
            lv.Bounds = new Rectangle(new Point(0, 0), new Size(600, 250));
            frmPopup.Controls.Add(lv);

            Button lvOK = new Button();
            lvOK.BackColor = Color.Tan;
            lvOK.Text = "OK";
            lvOK.Location = new Point(40, 265);
            lvOK.Click += new System.EventHandler(this.lvOK_Clicked);
            frmPopup.Controls.Add(lvOK);

            Button lvCancel = new Button();
            lvCancel.BackColor = Color.Tan;
            lvCancel.Text = "CANCEL";
            lvCancel.Location = new Point(130, 265);
            lvCancel.Click += new System.EventHandler(this.lvCancel_Clicked);
            frmPopup.Controls.Add(lvCancel);
            frmPopup.ShowDialog();


        }
        private void lvOK_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (!checkLVItemChecked("IS"))
                {
                    return;
                }
                string iolist = "";
                ListViewItem CheckeditemRow = new ListViewItem();
                foreach (ListViewItem itemRow in lv.Items)
                {
                    if (itemRow.Checked)
                    {
                        iolist = iolist + itemRow.SubItems[1].Text + "(" + Convert.ToDateTime(itemRow.SubItems[2].Text).ToString("yyyy-MM-dd") + ");";
                        CheckeditemRow = itemRow;
                        break;
                    }
                }
                txtReferenceInternalOrder.Text = iolist;
                showWorkOrderDetail(CheckeditemRow);
                frmPopup.Close();
                frmPopup.Dispose();
            }
            catch (Exception)
            {
            }
        }

        private void lvCancel_Clicked(object sender, EventArgs e)
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
        private void txtPaymentTerms_TextChanged(object sender, EventArgs e)
        {
        }
        private void btnPaymentTerm_Click(object sender, EventArgs e)
        {
            frmPopup = new Form();
            frmPopup.StartPosition = FormStartPosition.CenterScreen;
            frmPopup.BackColor = Color.CadetBlue;

            frmPopup.MaximizeBox = false;
            frmPopup.MinimizeBox = false;
            frmPopup.ControlBox = false;
            frmPopup.FormBorderStyle = FormBorderStyle.FixedSingle;

            frmPopup.Size = new Size(505, 300);

            dgvpt = new DataGridView();
            dgvpt = PTDefinitionDB.AcceptPaymentTerms(txtPaymentTerms.Text);
            dgvpt.Bounds = new System.Drawing.Rectangle(new System.Drawing.Point(0, 0), new Size(505, 250));
            frmPopup.Controls.Add(dgvpt);

            Button dgvptOK = new Button();
            dgvptOK.BackColor = Color.Tan;
            dgvptOK.Text = "OK";
            dgvptOK.Location = new Point(44, 265);
            dgvptOK.Click += new System.EventHandler(this.dgvptOK_Click);
            frmPopup.Controls.Add(dgvptOK);

            Button dgvptCancel = new Button();
            dgvptCancel.Text = "CANCEL";
            dgvptCancel.BackColor = Color.Tan;
            dgvptCancel.Location = new Point(141, 265);
            dgvptCancel.Click += new System.EventHandler(this.dgvptCancel_Click);
            frmPopup.Controls.Add(dgvptCancel);

            Button dgvptAddRow = new Button();
            dgvptAddRow.Text = "Add Credit Row";
            dgvptAddRow.BackColor = Color.Tan;
            dgvptAddRow.Location = new Point(300, 265);
            dgvptAddRow.Click += new System.EventHandler(this.dgvptAddRow_Click);
            frmPopup.Controls.Add(dgvptAddRow);
            frmPopup.ShowDialog();
        }
        private void dgvptOK_Click(object sender, EventArgs e)
        {
            try
            {
                int tperc = 0;
                int totperc = 0;
                int tcrdays = 0;
                int pcrdays = 0;
                int tval = 0;
                string tstr = "";
                string valStr = "";
                for (int i = 0; i < dgvpt.Rows.Count; i++)
                {
                    try
                    {
                        tperc = Convert.ToInt32(dgvpt.Rows[i].Cells["Percentage"].Value);
                        tstr = dgvpt.Rows[i].Cells["Description"].Value.ToString();
                        tcrdays = Convert.ToInt32(dgvpt.Rows[i].Cells["CreditPeriod"].Value);
                    }
                    catch (Exception)
                    {
                        tperc = 0;
                        tstr = "";
                        tcrdays = 0;
                    }
                    totperc = totperc + tperc;
                    if (tstr.Equals("Credit"))
                    {
                        if (!((tcrdays == 0 && tperc == 0) || (tcrdays != 0 && tperc != 0)))
                        {
                            MessageBox.Show("Error in credit entries");
                            return;
                        }
                    }
                    else
                    {
                        if (tcrdays > 0)
                        {
                            MessageBox.Show("Error in credit days");
                            return;
                        }
                    }
                    if (tperc > 0)
                    {
                        try
                        {
                            string val1, val2, val3;
                            //val1 = dgvpt.Rows[i].Cells["Code"].Value.ToString();
                            val2 = dgvpt.Rows[i].Cells["Percentage"].Value.ToString();
                            val3 = dgvpt.Rows[i].Cells["CreditPeriod"].Value.ToString();
                            val1 = dgvpt.Rows[i].Cells["ID"].Value.ToString();
                            valStr = valStr + val1 + "-" + val2 + "-" + val3 + ";";

                        }
                        catch (Exception)
                        {
                        }
                    }
                }

                if (totperc != 100)
                {
                    MessageBox.Show("Error in percentage values");
                    return;
                }
                txtPaymentTerms.Text = valStr.ToString();
                removeControlsFromFrmPopup();
                frmPopup.Close();
                frmPopup.Dispose();

            }
            catch (Exception ex)
            {
            }
        }

        private void dgvptCancel_Click(object sender, EventArgs e)
        {
            try
            {
                removeControlsFromFrmPopup();
                frmPopup.Close();
                frmPopup.Dispose();
            }
            catch (Exception)
            {
            }
        }
        private void removeControlsFromFrmPopup()
        {
            frmPopup.Controls.Clear();
        }
        private void dgvptAddRow_Click(object sender, EventArgs e)
        {
            try
            {
                ////int i = dgv.Rows.Count;
                dgvpt.Rows.Add();
                dgvpt.Rows[dgvpt.Rows.Count - 1].Cells["Code"].Value = dgvpt.Rows[dgvpt.Rows.Count - 2].Cells["Code"].Value;
                dgvpt.Rows[dgvpt.Rows.Count - 1].Cells["ID"].Value = dgvpt.Rows[dgvpt.Rows.Count - 2].Cells["ID"].Value;
                dgvpt.Rows[dgvpt.Rows.Count - 1].Cells["Description"].Value = dgvpt.Rows[dgvpt.Rows.Count - 2].Cells["Description"].Value;
                dgvpt.Rows[dgvpt.Rows.Count - 1].Cells["Percentage"].Value = 0;
                dgvpt.Rows[dgvpt.Rows.Count - 1].Cells["CreditPeriod"].Value = 0;
            }
            catch (Exception ex)
            {
            }
        }
        //-----
        //comment handling procedurs and functions
        //-----
        private void btnSelectCommenters_Click(object sender, EventArgs e)
        {
            //removeControlsFromCommenterPanel();
            //docCmtrDB = new DocCommenterDB();
            //lvCmtr = new ListView();
            //lvCmtr.Clear();
            //pnlCmtr.BorderStyle = BorderStyle.FixedSingle;
            //pnlCmtr.Bounds = new Rectangle(new Point(100, 10), new Size(700, 300));
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
            ////lvCancel.Visible = true;
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
                //frmPopup.Dispose();
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
                //frmPopup.Dispose();
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
                            WorkOrderDB wodb = new WorkOrderDB();
                            prevwoh.CommentStatus = DocCommenterDB.removeUnapprovedCommentStatus(prevwoh.CommentStatus);
                            prevwoh.ForwardUser = approverUID;
                            prevwoh.ForwarderList = prevwoh.ForwarderList + approverUName + Main.delimiter1 +
                                approverUID + Main.delimiter1 + Main.delimiter2;
                            if (wodb.forwardWorkOrder(prevwoh))
                            {
                                frmPopup.Close();
                                frmPopup.Dispose();
                                MessageBox.Show("Document Forwarded");
                                if (!updateDashBoard(prevwoh, 1))
                                {
                                    MessageBox.Show("DashBoard Fail to update");
                                }
                                closeAllPanels();
                                listOption = 1;
                                ListFilteredWOHeader(listOption);
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
                    string reverseStr = getReverseString(prevwoh.ForwarderList);
                    //do forward activities
                    prevwoh.CommentStatus = DocCommenterDB.removeUnapprovedCommentStatus(prevwoh.CommentStatus);
                    WorkOrderDB wodb = new WorkOrderDB();
                    if (reverseStr.Trim().Length > 0)
                    {
                        int ind = reverseStr.IndexOf("!@#");
                        prevwoh.ForwarderList = reverseStr.Substring(0, ind);
                        prevwoh.ForwardUser = reverseStr.Substring(ind + 3);
                        prevwoh.DocumentStatus = prevwoh.DocumentStatus - 1;
                    }
                    else
                    {
                        prevwoh.ForwarderList = "";
                        prevwoh.ForwardUser = "";
                        prevwoh.DocumentStatus = 1;
                    }
                    if (wodb.reverseWO(prevwoh))
                    {
                        MessageBox.Show("Wo Reversed");
                        closeAllPanels();
                        listOption = 1;
                        ListFilteredWOHeader(listOption);
                        setButtonVisibility("btnEditPanel"); //activites are same for cance, forward,approce and reverse
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void btnViewDocument_Click(object sender, EventArgs e)
        {
            try
            {
                removePDFFileGridView();
                removePDFControls();
                DataGridView dgvDocumentList = new DataGridView();
                pnlPDFViewer.Controls.Remove(dgvDocumentList);
                dgvDocumentList = DocumentStorageDB.getDocumentDetails(docID, prevwoh.TemporaryNo + "-" + prevwoh.TemporaryDate.ToString("yyyyMMddhhmmss"));
                pnlPDFViewer.Controls.Add(dgvDocumentList);
                dgvDocumentList.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDocumentList_CellContentClick);
            }
            catch (Exception ex)
            {
            }
        }

        private void btnCloseDocument_Click(object sender, EventArgs e)
        {
            removePDFControls();
            showPDFFileGrid();
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
        private void removeControlsPnllvPanel()
        {
            try
            {
                //foreach (Control p in pnlForwarder.Controls)
                //    if (p.GetType() == typeof(ListView) || p.GetType() == typeof(Button))
                //    {
                //        p.Dispose();
                //    }
                pnllv.Controls.Clear();
                Control nc = pnllv.Parent;
                nc.Controls.Remove(pnllv);
            }
            catch (Exception ex)
            {
            }
        }
        private void removeControlsPaymentTermPanel()
        {
            try
            {
                //foreach (Control p in pnlForwarder.Controls)
                //    if (p.GetType() == typeof(DataGridView) || p.GetType() == typeof(Button))
                //    {
                //        p.Dispose();
                //    }
                pnldgv.Controls.Clear();
                Control nc = pnldgv.Parent;
                nc.Controls.Remove(pnldgv);
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
                    string subDir = prevwoh.TemporaryNo + "-" + prevwoh.TemporaryDate.ToString("yyyyMMddhhmmss");
                    dgv.Enabled = false;
                    ////DocumentStorageDB.createFileFromDB(docID, subDir, fileName);
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
                handleGrdPrintButton();
                pnlBottomButtons.Visible = true;
                //pnlSelectWONO.Enabled = true;
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
                    tabControl1.SelectedTab = tabWOHeader;
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
                    tabControl1.SelectedTab = tabWOHeader;
                }
                else if (btnName == "Approve")
                {
                    pnlBottomButtons.Visible = false; //24/11/2016
                    btnCancel.Visible = true;
                    btnForward.Visible = true;
                    btnApprove.Visible = true;
                    btnReverse.Visible = true;
                    disableTabPages();
                    //pnlSelectWONO.Enabled = false;
                    tabControl1.SelectedTab = tabWOHeader;
                }
                else if (btnName == "View")
                {
                    pnlBottomButtons.Visible = false; //24/11/2016
                    btnCancel.Visible = true;
                    disableTabPages();
                    tabComments.Enabled = true;
                    tabPDFViewer.Enabled = true;
                    pnlPDFViewer.Visible = true;
                    tabControl1.SelectedTab = tabWOHeader;
                    //pnlSelectWONO.Enabled = false;
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
        void handleGrdPrintButton()
        {
            grdList.Columns["Print"].Visible = false;
            //if any one of view,add and edit
            if (Main.itemPriv[0] || Main.itemPriv[1] || Main.itemPriv[2])
            {
                //list option 1 should not have view buttuon visible (edit is avialable)
                if (listOption != 1)
                {
                    grdList.Columns["Print"].Visible = true;
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
                removeControlsFromForwarderPanel();
                dgvComments.Rows.Clear();
                chkCommentStatus.Checked = false;
                txtComments.Text = "";
                grdWODetail.Rows.Clear();
            }
            catch (Exception ex)
            {
            }
        }

        private void tabWOHeader_Click(object sender, EventArgs e)
        {

        }

        private void btnSelectWorkOrderRequest_Click(object sender, EventArgs e)
        {
            ////btnSelectWorkOrderRequest.Enabled = false;
            //removeControlsPnllvPanel();
            //pnllv = new Panel();
            //pnllv.BorderStyle = BorderStyle.FixedSingle;
            //pnllv.Bounds = new Rectangle(new Point(100, 100), new Size(600, 300));
            frmPopup = new Form();
            frmPopup.StartPosition = FormStartPosition.CenterScreen;
            frmPopup.BackColor = Color.CadetBlue;

            frmPopup.MaximizeBox = false;
            frmPopup.MinimizeBox = false;
            frmPopup.ControlBox = false;
            frmPopup.FormBorderStyle = FormBorderStyle.FixedSingle;

            frmPopup.Size = new Size(450, 300);
            lv = WorkOrderRequestDB.WorkOrderRequestListView();
            //this.lv.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listView2_ItemCheck);
            lv.Bounds = new Rectangle(new Point(0, 0), new Size(450, 250));
            frmPopup.Controls.Add(lv);

            Button lvOK = new Button();
            lvOK.BackColor = Color.Tan;
            lvOK.Text = "OK";
            lvOK.Location = new Point(40, 265);
            lvOK.Click += new System.EventHandler(this.lvOK_Clicked1);
            frmPopup.Controls.Add(lvOK);

            Button lvCancel = new Button();
            lvCancel.BackColor = Color.Tan;
            lvCancel.Text = "CANCEL";
            lvCancel.Location = new Point(130, 265);
            lvCancel.Click += new System.EventHandler(this.lvCancel_Clicked1);
            frmPopup.Controls.Add(lvCancel);
            frmPopup.ShowDialog();
            //pnlAddEdit.Controls.Add(pnllv);
            //pnllv.BringToFront();
            //pnllv.Visible = true;

        }
        private void lvOK_Clicked1(object sender, EventArgs e)
        {
            try
            {
                if (!checkLVItemChecked("Item"))
                {
                    return;
                }
                //pnllv.Visible = false;
                //btnSelectWorkOrderRequest.Enabled = true;
                foreach (ListViewItem itemRow in lv.Items)
                {
                    if (itemRow.Checked)
                    {
                        if (!WorkOrderDB.checkAvailabilityOfWo(Convert.ToInt32(itemRow.SubItems[1].Text), Convert.ToDateTime(itemRow.SubItems[2].Text)))
                        {
                            DialogResult dialog = MessageBox.Show("Work Order ALready Prepared. Do you want to proceed again ?", "Yes", MessageBoxButtons.YesNo);
                            if (dialog == DialogResult.Yes)
                            {
                                showWorkOrderDetail(itemRow);
                                frmPopup.Close();
                                frmPopup.Dispose();
                            }
                        }
                        else
                        {
                            showWorkOrderDetail(itemRow);
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
        private void showWorkOrderDetail(ListViewItem itemRow)
        {
            int indentNO = Convert.ToInt32(itemRow.SubItems[1].Text);
            DateTime indentDate = Convert.ToDateTime(itemRow.SubItems[2].Text);
            IndentServiceDB isdb = new IndentServiceDB();
            indentserviceheader ishd = isdb.getFilteredIndentServiceHeaders("", 6, "").
                                    FirstOrDefault(ish => ish.WORequestNo == indentNO && ish.WORequestDate == indentDate);
            if(ishd != null)
            {
                showWorkOrderHeader(ishd);
                List<indentservicedetail> ISDetail = IndentServiceDB.getIndentServiceDetails(ishd);
                showWorkOrderDetail(ISDetail);
                tabControl1.Visible = true;
            }
        }
        private void lvCancel_Clicked1(object sender, EventArgs e)
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
        //private void listView2_ItemCheck1(object sender, ItemCheckedEventArgs e)
        //{
        //    int c = lv.Items.Count;
        //    if (lv.CheckedItems.Count > 1)
        //    {
        //        MessageBox.Show("Cannot select more than one item");
        //        e.Item.Checked = false;
        //    }

        //}
        private void btnSelectTermsAndCond_Click(object sender, EventArgs e)
        {
            frmPopup = new Form();
            frmPopup.StartPosition = FormStartPosition.CenterScreen;
            frmPopup.BackColor = Color.CadetBlue;

            frmPopup.MaximizeBox = false;
            frmPopup.MinimizeBox = false;
            frmPopup.ControlBox = false;
            frmPopup.FormBorderStyle = FormBorderStyle.FixedSingle;

            frmPopup.Size = new Size(650, 300);
            lv = DocTcMappingDB.getTCListViewForPerticularDoc("WORKORDER");
            string[] strArry = txtTermsAndCond.Text.Split(new string[] { ";" }, StringSplitOptions.None);
            for (int i = 0; i < strArry.Length; i++)
            {
                try
                {
                    foreach (ListViewItem itemRow in lv.Items)
                    {
                        if (itemRow.SubItems[1].Text.Trim().Equals(strArry[i]))
                        {
                            itemRow.Checked = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
            lv.Bounds = new Rectangle(new Point(0, 0), new Size(650, 250));
            frmPopup.Controls.Add(lv);

            Button lvOK = new Button();
            lvOK.BackColor = Color.Tan;
            lvOK.Text = "OK";
            lvOK.Location = new Point(40, 265);
            lvOK.Click += new System.EventHandler(this.lvOK_Clicked2);
            frmPopup.Controls.Add(lvOK);

            Button lvCancel = new Button();
            lvCancel.BackColor = Color.Tan;
            lvCancel.Text = "CANCEL";
            lvCancel.Location = new Point(130, 265);
            lvCancel.Click += new System.EventHandler(this.lvCancel_Clicked2);
            frmPopup.Controls.Add(lvCancel);
            frmPopup.ShowDialog();
            //pnlAddEdit.Controls.Add(pnllv);
            //pnllv.BringToFront();
            //pnllv.Visible = true;
        }
        private void lvOK_Clicked2(object sender, EventArgs e)
        {
            try
            {
                string tclist = "";
                foreach (ListViewItem itemRow in lv.Items)
                {
                    if (itemRow.Checked)
                    {
                        tclist = tclist + itemRow.SubItems[1].Text + ";";
                    }
                }
                txtTermsAndCond.Text = tclist;
                frmPopup.Close();
                frmPopup.Dispose();
            }
            catch (Exception)
            {
            }
        }

        private void lvCancel_Clicked2(object sender, EventArgs e)
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

        private void txtExchangeRate_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (grdWODetail.Rows.Count != 0 && txtServiceValue.Text.Length != 0
                    && txtTotalValue.Text.Length != 0 && txtExchangeRate.Text.Length != 0)
                {
                    double dd = Convert.ToDouble(txtExchangeRate.Text);
                    txtServiceValueINR.Text = (Convert.ToDouble(txtServiceValue.Text) * dd).ToString();
                    txtTotalValueINR.Text = (Convert.ToDouble(txtTotalValue.Text) * dd).ToString();
                    txtTaxAmountINR.Text = (Convert.ToDouble(txtTaxAmount.Text) * dd).ToString();
                }
                if (txtExchangeRate.Text.Length == 0)
                {
                    txtServiceValueINR.Text = "";
                    txtTotalValueINR.Text = "";
                    txtTaxAmountINR.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this.ToString() + "-" + System.Reflection.MethodBase.GetCurrentMethod().Name + "() : Error");
            }
        }
        private Boolean checkLVItemChecked(string str)
        {
            Boolean status = true;
            try
            {
                if (lv.CheckedIndices.Count > 1)
                {
                    MessageBox.Show("Only one " + str + " allowed");
                    return false;
                }
                if (lv.CheckedItems.Count == 0)
                {
                    MessageBox.Show("select one " + str);
                    return false;
                }
            }
            catch (Exception)
            {
            }
            return status;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            removeControlsPaymentTermPanel();
        }
        //-- Validating Header and Detail String For Single Quotes

        private workorderheader verifyHeaderInputString(workorderheader woh)
        {
            try
            {
                woh.SpecialNote = Utilities.replaceSingleQuoteWithDoubleSingleQuote(woh.SpecialNote.Trim());
                woh.Remarks = Utilities.replaceSingleQuoteWithDoubleSingleQuote(woh.Remarks);
                woh.ReferenceInternalOrder = Utilities.replaceSingleQuoteWithDoubleSingleQuote(woh.ReferenceInternalOrder);
                woh.CustomerID = Utilities.replaceSingleQuoteWithDoubleSingleQuote(woh.CustomerID);
                woh.TermsAndCond = Utilities.replaceSingleQuoteWithDoubleSingleQuote(woh.TermsAndCond);
                woh.POAddress = Utilities.replaceSingleQuoteWithDoubleSingleQuote(woh.POAddress);
            }
            catch (Exception ex)
            {
            }
            return woh;
        }
        private void verifyDetailInputString()
        {
            try
            {
                for (int i = 0; i < grdWODetail.Rows.Count; i++)
                {
                    grdWODetail.Rows[i].Cells["Item"].Value = Utilities.replaceSingleQuoteWithDoubleSingleQuote(grdWODetail.Rows[i].Cells["Item"].Value.ToString());
                    grdWODetail.Rows[i].Cells["WorkDescription"].Value = Utilities.replaceSingleQuoteWithDoubleSingleQuote(grdWODetail.Rows[i].Cells["WorkDescription"].Value.ToString());
                    grdWODetail.Rows[i].Cells["WorkLocation"].Value = Utilities.replaceSingleQuoteWithDoubleSingleQuote(grdWODetail.Rows[i].Cells["WorkLocation"].Value.ToString());
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void cmbWOStatFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string selectedValue = ((Structures.ComboBoxItem)cmbWOStatFilter.SelectedItem).HiddenValue;
                if(selectedValue == null || selectedValue.Length == 0)
                {
                    return;
                }
                if (selectedValue == "All")
                {
                    foreach (DataGridViewRow row in grdList.Rows)
                    {
                        row.Visible = true;
                    }
                }
                else
                {
                    foreach (DataGridViewRow row in grdList.Rows)
                    {
                        if (row.Cells["WorkOrderStatus"].Value.ToString() == selectedValue)
                            row.Visible = true;
                        else
                            row.Visible = false;
                    }
                }
            }
            catch(Exception ex)
            {

            }
        }

        private void WorkOrder_Enter(object sender, EventArgs e)
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

