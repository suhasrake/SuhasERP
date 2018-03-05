using System;
using System.Collections.Generic;
using System.Collections;
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
using System.Collections.ObjectModel;
using CSLERP.FileManager;

namespace CSLERP
{
    public partial class ProjectTransaction : System.Windows.Forms.Form
    {
        string fileDir = "";
        string docID = "";
        string[] Headers = { "PO Received", "Work Order Issued" , "Material Supplied" , "Material Payment",
                                    "WO payment","Employee Payments","Other payments","Receipts"};
        string prev = "";
        public ProjectTransaction()
        {
            try
            {
                InitializeComponent();

            }
            catch (Exception)
            {

            }
        }
        private void ProjectTransaction_Load(object sender, EventArgs e)
        {
            //////this.FormBorderStyle = FormBorderStyle.None;
            Region = System.Drawing.Region.FromHrgn(Utilities.CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
            initVariables();
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Width -= 100;
            this.Height -= 100;

            String a = this.Size.ToString();
            grdMainList.ColumnHeadersDefaultCellStyle.BackColor = Color.LightBlue;
            grdMainList.EnableHeadersVisualStyles = false;
            ShowControl();
        }
        private void ShowControl()
        {
            pnlList.Visible = true;
            grdMainList.Visible = true;
            grdDetailList.Visible = false;
            btnCancel.Visible = false;
        }
        private void initVariables()
        {

            docID = Main.currentDocument;
            dtStartDate.Format = DateTimePickerFormat.Custom;
            dtStartDate.CustomFormat = "dd-MM-yyyy";
            dtTargetDate.Format = DateTimePickerFormat.Custom;
            dtTargetDate.CustomFormat = "dd-MM-yyyy";
            pnlUI.Controls.Add(pnlList);
            enableBottomButtons();
            ProjectHeaderDB.fillprojectCombo(cmbProject);
            grdMainList.Visible = true;
            grdMainList.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            grdDetailList.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            grdDetailList.Columns["TotalAmount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }
        private void closeAllPanels()
        {
            try
            {
                pnlList.Visible = false;
            }
            catch (Exception)
            {

            }
        }

        public void clearData()
        {
            try
            {
                grdDetailList.Rows.Clear();
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
            btnExit.Visible = true;
            pnlBottomButtons.Visible = true;
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            clearData();
            grdMainList.Visible = true;
            grdDetailList.Visible = false;
            btnCancel.Visible = false;
        }
        private void grdMainList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0)
                    return;
                string columnName = grdMainList.Columns[e.ColumnIndex].Name;
                if (columnName.Equals("Detail"))
                {
                    if (Convert.ToInt32(grdMainList.CurrentRow.Cells["gNo"].Value) == 0)
                    {
                        MessageBox.Show("No of Item is zero");
                        return;
                    }
                    grdDetailList.Rows.Clear();
                    grdDetailList.Visible = true;
                    btnCancel.Visible = true;
                    
                    string Head = grdMainList.CurrentRow.Cells["Received"].Value.ToString();
                    if(Head.Equals("PO Received"))
                    {
                        List<popiheader> ppopi = POPIHeaderDB.getPOPIINFOForProjectTrans(cmbProject.SelectedItem.ToString());
                        int i = 1;
                        grdDetailList.Columns["DocumentNo"].HeaderText = "Tracking No";
                        grdDetailList.Columns["DocumentDate"].HeaderText = "Tracking Date";
                        grdDetailList.Columns["Customer"].HeaderText = "Customer";
                        grdDetailList.Columns["CustPODate"].HeaderText = "CustPODate";
                        grdDetailList.Columns["gValue"].HeaderText = "Value";
                        grdDetailList.Columns["TaxAmount"].HeaderText = "Tax Amount";
                        grdDetailList.Columns["CustPONo"].Visible = true;
                        grdDetailList.Columns["CustPODate"].Visible = true;
                        grdDetailList.Columns["gValue"].Visible = true;
                        grdDetailList.Columns["TaxAmount"].Visible = true;
                        foreach (popiheader popih in ppopi)
                        {
                            grdDetailList.Rows.Add();
                            grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["LineNo"].Value = i;
                            grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["DocumentNo"].Value = popih.TrackingNo;
                            grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["DocumentDate"].Value = popih.TrackingDate;
                            grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["Customer"].Value = popih.CustomerName;
                            grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["gValue"].Value = popih.ProductValue;
                            grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["TaxAmount"].Value = popih.TaxAmount;
                            grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["TotalAmount"].Value = popih.POValue;
                            grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["CustPONo"].Value = popih.CustomerPONO;
                            grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["CustPODate"].Value = popih.CustomerPODate;
                            i++;
                        }
                    }
                    else if (Head.Equals("Work Order Issued"))
                    {
                        int i = 1;
                        grdDetailList.Columns["DocumentNo"].HeaderText = "WO No";
                        grdDetailList.Columns["DocumentDate"].HeaderText = "WO Date";
                        grdDetailList.Columns["Customer"].HeaderText = "Customer";
                        grdDetailList.Columns["gValue"].HeaderText = "Value";
                        grdDetailList.Columns["TaxAmount"].HeaderText = "Tax Amount";
                        grdDetailList.Columns["CustPONo"].Visible = false;
                        grdDetailList.Columns["CustPODate"].Visible = false;
                        grdDetailList.Columns["gValue"].Visible = true;
                        grdDetailList.Columns["TaxAmount"].Visible = true;
                        List<workorderheader> wohList = WorkOrderDB.getRVINFOForProjectTrans(cmbProject.SelectedItem.ToString());
                        foreach (workorderheader woh in wohList)
                        {
                            grdDetailList.Rows.Add();
                            grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["LineNo"].Value = i;
                            grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["DocumentNo"].Value = woh.WONo;
                            grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["DocumentDate"].Value = woh.WODate;
                            grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["Customer"].Value = woh.CustomerName;
                            grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["gValue"].Value = woh.ServiceValue;
                            grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["TaxAmount"].Value = woh.TaxAmount;
                            grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["TotalAmount"].Value = woh.TotalAmount;
                            i++;
                        }
                    }
                    else if (Head.Equals("Material Supplied"))
                    {
                        int i = 1;
                        grdDetailList.Columns["DocumentNo"].HeaderText = "Invoice No";
                        grdDetailList.Columns["DocumentDate"].HeaderText = "Invoice Date";
                        grdDetailList.Columns["Customer"].HeaderText = "Customer";
                        grdDetailList.Columns["gValue"].HeaderText = "Value";
                        grdDetailList.Columns["TaxAmount"].HeaderText = "Tax Amount";
                        grdDetailList.Columns["CustPONo"].Visible = false;
                        grdDetailList.Columns["CustPODate"].Visible = false;
                        grdDetailList.Columns["gValue"].Visible = true;
                        grdDetailList.Columns["TaxAmount"].Visible = true;
                        List<invoiceoutheader> iohList = InvoiceOutHeaderDB.getRVINFOForProjectTrans(cmbProject.SelectedItem.ToString());
                        foreach (invoiceoutheader ioh in iohList)
                        {
                            grdDetailList.Rows.Add();
                            grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["LineNo"].Value = i;
                            grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["DocumentNo"].Value = ioh.InvoiceNo;
                            grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["DocumentDate"].Value = ioh.InvoiceDate;
                            grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["Customer"].Value = ioh.ConsigneeName;
                            grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["gValue"].Value = ioh.ProductValue;
                            grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["TaxAmount"].Value = ioh.TaxAmount;
                            grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["TotalAmount"].Value = ioh.InvoiceAmount;
                            //grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["CustPONo"].Value = ioh.InvoiceNo;
                            //grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["CustPODate"].Value = ioh.InvoiceDate;
                            i++;
                        }
                    }
                    else if (Head.Equals("Material Payment"))
                    {
                        int i = 1;
                        grdDetailList.Columns["DocumentNo"].HeaderText = "Voucher No";
                        grdDetailList.Columns["DocumentDate"].HeaderText = "Voucher Date";
                        grdDetailList.Columns["gValue"].HeaderText = "Bill No";
                        grdDetailList.Columns["TaxAmount"].HeaderText = "Bill Date";
                        grdDetailList.Columns["CustPONo"].Visible = false;
                        grdDetailList.Columns["CustPODate"].Visible = true;
                        grdDetailList.Columns["CustPODate"].HeaderText = "SLType";
                        grdDetailList.Columns["Customer"].HeaderText = "SLName";
                        grdDetailList.Columns["gValue"].Visible = true;
                        grdDetailList.Columns["TaxAmount"].Visible = true;
                        List<paymentvoucher> pvhList = PaymentVoucherDB.getRVINFOForProjectTrans(cmbProject.SelectedItem.ToString(),1);
                        foreach (paymentvoucher pvh in pvhList)
                        {
                            grdDetailList.Rows.Add();
                            grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["LineNo"].Value = i;
                            grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["DocumentNo"].Value = pvh.VoucherNo;
                            grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["DocumentDate"].Value = pvh.VoucherDate;
                            grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["CustPODate"].Value = pvh.SLType;
                            grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["Customer"].Value = pvh.SLName;
                            grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["gValue"].Value = pvh.BillNo;
                            grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["TaxAmount"].Value = pvh.BillDate;
                            grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["TotalAmount"].Value = pvh.AmountDebit;
                            i++;
                        }
                    }
                    else if (Head.Equals("WO payment"))
                    {
                        int i = 1;
                        grdDetailList.Columns["DocumentNo"].HeaderText = "Voucher No";
                        grdDetailList.Columns["DocumentDate"].HeaderText = "Voucher Date";
                        grdDetailList.Columns["gValue"].HeaderText = "Bill No";
                        grdDetailList.Columns["TaxAmount"].HeaderText = "Bill Date";
                        grdDetailList.Columns["CustPONo"].Visible = false;
                        grdDetailList.Columns["CustPODate"].Visible = true;
                        grdDetailList.Columns["CustPODate"].HeaderText = "SLType";
                        grdDetailList.Columns["Customer"].HeaderText = "SLName";
                        grdDetailList.Columns["gValue"].Visible = true;
                        grdDetailList.Columns["TaxAmount"].Visible = true;
                        List<paymentvoucher> pvhList = PaymentVoucherDB.getRVINFOForProjectTrans(cmbProject.SelectedItem.ToString(),2);
                        foreach (paymentvoucher pvh in pvhList)
                        {
                            grdDetailList.Rows.Add();
                            grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["LineNo"].Value = i;
                            grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["DocumentNo"].Value = pvh.VoucherNo;
                            grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["DocumentDate"].Value = pvh.VoucherDate;
                            grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["CustPODate"].Value = pvh.SLType;
                            grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["Customer"].Value = pvh.SLName;
                            grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["gValue"].Value = pvh.BillNo;
                            grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["TaxAmount"].Value = pvh.BillDate;
                            grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["TotalAmount"].Value = pvh.AmountDebit;
                            i++;
                        }
                    }
                    else if (Head.Equals("Employee Payments"))
                    {

                    }
                    else if (Head.Equals("Other payments"))
                    {
                        int i = 1;
                        grdDetailList.Columns["DocumentNo"].HeaderText = "Voucher No";
                        grdDetailList.Columns["DocumentDate"].HeaderText = "Voucher Date";
                        grdDetailList.Columns["gValue"].HeaderText = "Bill No";
                        grdDetailList.Columns["TaxAmount"].HeaderText = "Bill Date";
                        grdDetailList.Columns["CustPONo"].Visible = false;
                        grdDetailList.Columns["CustPODate"].Visible = true;
                        grdDetailList.Columns["CustPODate"].HeaderText = "SLType";
                        grdDetailList.Columns["Customer"].HeaderText = "SLName";
                        grdDetailList.Columns["gValue"].Visible = true;
                        grdDetailList.Columns["TaxAmount"].Visible = true;
                        List<paymentvoucher> pvhList = PaymentVoucherDB.getRVINFOForProjectTrans(cmbProject.SelectedItem.ToString(), 3);
                        foreach (paymentvoucher pvh in pvhList)
                        {
                            grdDetailList.Rows.Add();
                            grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["LineNo"].Value = i;
                            grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["DocumentNo"].Value = pvh.VoucherNo;
                            grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["DocumentDate"].Value = pvh.VoucherDate;
                            grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["CustPODate"].Value = pvh.SLType;
                            grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["Customer"].Value = pvh.SLName;
                            grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["gValue"].Value = pvh.BillNo;
                            grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["TaxAmount"].Value = pvh.BillDate;
                            grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["TotalAmount"].Value = pvh.AmountDebit;
                            i++;
                        }
                    }
                    else if (Head.Equals("Receipts"))
                    {
                        int i = 1;
                        grdDetailList.Columns["DocumentNo"].HeaderText = "Voucher No";
                        grdDetailList.Columns["DocumentDate"].HeaderText = "Voucher Date";
                        grdDetailList.Columns["CustPONo"].Visible = false;
                        grdDetailList.Columns["CustPODate"].Visible = true;
                        grdDetailList.Columns["CustPODate"].HeaderText = "SLType";
                        grdDetailList.Columns["Customer"].HeaderText = "SLName";
                        grdDetailList.Columns["gValue"].Visible = false;
                        grdDetailList.Columns["TaxAmount"].Visible = false;
                        List<ReceiptVoucherHeader> rvhlist = ReceiptVoucherDB.getRVINFOForProjectTrans(cmbProject.SelectedItem.ToString());
                        foreach (ReceiptVoucherHeader rvh in rvhlist)
                        {
                            grdDetailList.Rows.Add();
                            grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["LineNo"].Value = i;
                            grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["DocumentNo"].Value = rvh.VoucherNo;
                            grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["DocumentDate"].Value = rvh.VoucherDate;
                            grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["CustPODate"].Value = rvh.SLType;
                            grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["Customer"].Value = rvh.SLName;
                            grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["gValue"].Value = "";
                            grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["TaxAmount"].Value = "";
                            grdDetailList.Rows[grdDetailList.RowCount - 1].Cells["TotalAmount"].Value = rvh.VoucherAmount;
                            i++;
                        }
                    }
                    else
                    {
                        grdDetailList.Rows.Clear();
                        grdDetailList.Visible = false;
                        btnCancel.Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        private string showStatusString(int stat)
        {
            string str = "";
            if (stat == 1)
                str = "Forwarded";
            else
                str = "Pendinig";
            return str;
        }
        private void grdList_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void pnlList_Paint(object sender, PaintEventArgs e)
        {

        }
        private void cmbProject_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbProject.SelectedIndex != -1)
                {
                    grdMainList.Rows.Clear();
                    grdDetailList.Rows.Clear();
                    grdDetailList.Visible = false;
                    btnCancel.Visible = false;
                    ProjectHeaderDB phdb = new ProjectHeaderDB();
                    List<projectheader> PHList = phdb.getFilteredProjectHeader("", 6);
                    foreach (projectheader head in PHList)
                    {
                        if (head.ProjectID.Equals(cmbProject.SelectedItem))
                        {
                            txtProjectID.Text = head.ProjectID;
                            txtClient.Text = head.CustomerID + "-" + head.CustomerName;
                            dtStartDate.Value = head.StartDate;
                            dtTargetDate.Value = head.TargetDate;
                        }
                    }
                    addGridRows();
                }
            }
            catch (Exception edx)
            {
            }
        }
        private void addGridRows()
        {
           
            string str = "";
            for (int i = 0; i < Headers.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        str = POPIHeaderDB.getPOPIDtlsForProjectTrans(txtProjectID.Text);
                        break;
                    case 1:
                        str = WorkOrderDB.getWODtlsForProjectTrans(txtProjectID.Text);
                        break;
                    case 2:
                        str = InvoiceOutHeaderDB.getIOHDtlsForProjectTrans(txtProjectID.Text);
                        break;
                    case 3:
                        str = PaymentVoucherDB.getPVHDtlsForProjectTrans(txtProjectID.Text,1);
                        break;
                    case 4:
                        str = PaymentVoucherDB.getPVHDtlsForProjectTrans(txtProjectID.Text,2);
                        break;
                    case 5:
                        str = "";
                        break;
                    case 6:
                        str = PaymentVoucherDB.getPVHDtlsForProjectTrans(txtProjectID.Text, 3);
                        break;
                    case 7:
                        str = ReceiptVoucherDB.getRVDtlsForProjectTrans(txtProjectID.Text);
                        break;
                    default:
                        break;
                }
                
                if (str.Length != 0)
                {
                    grdMainList.Rows.Add();
                    grdMainList.Rows[grdMainList.RowCount - 1].Cells["Received"].Value = Headers[i];
                    grdMainList.Rows[grdMainList.RowCount - 1].Cells["gNo"].Value = str.Substring(0, str.IndexOf('-'));
                    grdMainList.Rows[grdMainList.RowCount - 1].Cells["Value"].Value = str.Substring(str.IndexOf('-') + 1);
                }
                else
                {
                    grdMainList.Rows.Add();
                    grdMainList.Rows[grdMainList.RowCount - 1].Cells["Received"].Value = Headers[i];
                    grdMainList.Rows[grdMainList.RowCount - 1].Cells["gNo"].Value = "0";
                    grdMainList.Rows[grdMainList.RowCount - 1].Cells["Value"].Value = "0";
                }
            }
        }

        private void ProjectTransaction_Enter(object sender, EventArgs e)
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


