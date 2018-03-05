using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Drawing;

namespace CSLERP.DBData
{
    public class invoiceoutheader
    {
        public int RowID { get; set; }
        public double FreightCharge { get; set; }
        public string DocumentID { get; set; }
        public string DocumentName { get; set; }
        public int TemporaryNo { get; set; }
        public DateTime TemporaryDate { get; set; }
        public int InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public int TrackingNo { get; set; }
        public DateTime TrackingDate { get; set; }
        public string ConsigneeID { get; set; }
        public string ConsigneeName { get; set; }
        public string TermsOfPayment { get; set; }
        public string Description { get; set; }
        public string TransportationMode { get; set; }
        public string TransportationModeName { get; set; }
        public string TransportationType { get; set; }
        public string TransportationTypeName { get; set; }
        public string Transporter { get; set; }
        public string TransporterName { get; set; }
        public string CurrencyID { get; set; }
        public string DeliveryAddress { get; set; }
        public string DeliveryStateCode { get; set; }
        public int BankAcReference { get; set; }

        public string ProjectID { get; set; }
        public double INRConversionRate { get; set; }
        public string ADCode { get; set; }
        public string EntryPort { get; set; }
        public string ExitPort { get; set; }
        public string FinalDestinatoinCountryID { get; set; }
        public string FinalDestinatoinCountryName { get; set; }
        public string OriginCountryID { get; set; }
        public string OriginCountryName { get; set; }
        public string FinalDestinationPlace { get; set; }
        public string PreCarriageTransportationMode { get; set; }
        public string PreCarriageTransportationName { get; set; }
        public string PreCarrierReceiptPlace { get; set; }
        public string TermsOfDelivery { get; set; }
        public string Remarks { get; set; }
        public string CommentStatus { get; set; }
        public string Comments { get; set; }
        public DateTime CreateTime { get; set; }
        public string CreateUser { get; set; }
        public string ForwardUser { get; set; }
        public string ApproveUser { get; set; }
        public string CreatorName { get; set; }
        public string ForwarderName { get; set; }
        public string ApproverName { get; set; }
        public string ForwarderList { get; set; }
        public int status { get; set; }
        public int DocumentStatus { get; set; }
        public double ProductValue { get; set; }
        public double TaxAmount { get; set; }
        public double ProductValueINR { get; set; }
        public double TaxAmountINR { get; set; }
        public double InvoiceAmount { get; set; }
        public double INRAmount { get; set; }
        public invoiceoutheader()
        {
            Comments = "";
        }

        public string TrackingNos { get; set; }
        public string TrackingDates { get; set; }

        //SJV Ref In INvoice
        public int SJVTNo { get; set; }
        public DateTime SJVTDate { get; set; }
        public int SJVNo { get; set; }
        public DateTime SJVDate { get; set; }
    }
    public class invoiceoutdetail
    {
        public int RowID { get; set; }
        public string DocumentID { get; set; }
        public int TemporaryNo { get; set; }
        public DateTime TemporaryDate { get; set; }
        public int PONo { get; set; }
        public DateTime PODate { get; set; }
        public string StockItemID { get; set; }
        public string StockItemName { get; set; }
        public string ModelNo { get; set; }
        public string ModelName { get; set; }
        public string Unit { get; set; }
        public string CustomerItemDescription { get; set; }
        // public string WorkDescription { get; set; }
        public double Quantity { get; set; }
        public double Price { get; set; }
        public double Tax { get; set; }
        public int WarrantyDays { get; set; }
        public int POItemReferenceNo { get; set; }
        public string TaxDetails { get; set; }
        public int MRNNo { get; set; }
        public DateTime MRNDate { get; set; }
        public string BatchNo { get; set; }
        public string SerielNo { get; set; }
        public DateTime ExpiryDate { get; set; }
        public double PurchaseQuantity { get; set; }
        public double PurchasePrice { get; set; }
        public double PurchaseTax { get; set; }
        public string SupplierID { get; set; }
        public string SupplierName { get; set; }
        public int StockReferenceNo { get; set; }
        public string TaxCode { get; set; }
    }
    class InvoiceOutHeaderDB
    {
        public List<invoiceoutheader> getFilteredInvoiceOutHeader(string userList, int opt, string userCommentStatusString)
        {
            invoiceoutheader ioh;
            List<invoiceoutheader> IOHeaders = new List<invoiceoutheader>();
            try
            {
                //approved user comment status string
                string acStr = "";
                try
                {
                    acStr = userCommentStatusString.Substring(0, userCommentStatusString.Length - 2) + "1" + Main.delimiter2;
                }
                catch (Exception ex)
                {
                    acStr = "";
                }
                //-----
                string query1 = "select RowID, DocumentID, DocumentName,TemporaryNo,TemporaryDate," +
                    " InvoiceNo,InvoiceDate,TrackingNos,TrackingDates,Consignee,ConsigneeName,TermsOfPayment,Description,TransportationMode,TransportationModeName,TransportationType," +
                    " TransportationTypeName,Transporter,TransporterName,CurrencyID,INRConversionRate,ADCode,EntryPort, " +
                    " ExitPort,FinalDestinationCountryID,FinalDestinationCountryName,OriginCountryID,OriginCountryName," +
                    "FinalDestinationPlace,PreCarriageTransportationMode,PreCarriageTransportationName,PreCarrierReceiptPlace,TermsOfDelivery,Remarks," +
                    "CommentStatus,CreateUser,ForwardUser,ApproveUser,CreatorName,CreateTime,ForwarderName,ApproverName,ForwarderList,Status,DocumentStatus,ProductValue,TaxAmount,InvoiceAmount,INRAmount,ProjectID " +
                    ", ProductValueINR, TaxAmountINR,BankAcReference,FreightCharge,DeliveryAddress,DeliveryStateCode,SJVTNo,SJVTDate,SJVNo,SJVDate from ViewInvoiceOutHeader" +
                    " where ((forwarduser='" + Login.userLoggedIn + "' and DocumentStatus between 2 and 98) " +
                    " or (createuser='" + Login.userLoggedIn + "' and DocumentStatus=1)" +
                    " or (commentStatus like '%" + userCommentStatusString + "%' and DocumentStatus between 1 and 98)) and Status not in (7,98) order by TemporaryDate desc,DocumentID asc,TemporaryNo desc";

                string query2 = "select RowID, DocumentID, DocumentName,TemporaryNo,TemporaryDate," +
                    " InvoiceNo,InvoiceDate,TrackingNos,TrackingDates,Consignee,ConsigneeName,TermsOfPayment,Description,TransportationMode,TransportationModeName,TransportationType," +
                    " TransportationTypeName,Transporter,TransporterName,CurrencyID,INRConversionRate,ADCode,EntryPort, " +
                    " ExitPort,FinalDestinationCountryID,FinalDestinationCountryName,OriginCountryID,OriginCountryName," +
                    "FinalDestinationPlace,PreCarriageTransportationMode,PreCarriageTransportationName,PreCarrierReceiptPlace,TermsOfDelivery,Remarks," +
                    "CommentStatus,CreateUser,ForwardUser,ApproveUser,CreatorName,CreateTime,ForwarderName,ApproverName,ForwarderList,Status,DocumentStatus,ProductValue,TaxAmount,InvoiceAmount,INRAmount,ProjectID " +
                    " , ProductValueINR, TaxAmountINR,BankAcReference,FreightCharge,DeliveryAddress,DeliveryStateCode,SJVTNo,SJVTDate,SJVNo,SJVDate from ViewInvoiceOutHeader" +
                    " where ((createuser='" + Login.userLoggedIn + "'  and DocumentStatus between 2 and 98 ) " +
                    " or (ForwarderList like '%" + userList + "%' and DocumentStatus between 2 and 98 and ForwardUser <> '" + Login.userLoggedIn + "')" +
                    " or (commentStatus like '%" + acStr + "%' and DocumentStatus between 1 and 98)) and Status not in (7,98) order by TemporaryDate desc,DocumentID asc,TemporaryNo desc";

                string query3 = "select RowID, DocumentID, DocumentName,TemporaryNo,TemporaryDate," +
                    " InvoiceNo,InvoiceDate,TrackingNos,TrackingDates,Consignee,ConsigneeName,TermsOfPayment,Description,TransportationMode,TransportationModeName,TransportationType," +
                    " TransportationTypeName,Transporter,TransporterName,CurrencyID,INRConversionRate,ADCode,EntryPort, " +
                    " ExitPort,FinalDestinationCountryID,FinalDestinationCountryName,OriginCountryID,OriginCountryName," +
                    "FinalDestinationPlace,PreCarriageTransportationMode,PreCarriageTransportationName,PreCarrierReceiptPlace,TermsOfDelivery,Remarks," +
                    "CommentStatus,CreateUser,ForwardUser,ApproveUser,CreatorName,CreateTime,ForwarderName,ApproverName,ForwarderList,Status,DocumentStatus,ProductValue,TaxAmount,InvoiceAmount,INRAmount,ProjectID " +
                    " , ProductValueINR, TaxAmountINR,BankAcReference,FreightCharge,DeliveryAddress,DeliveryStateCode,SJVTNo,SJVTDate,SJVNo,SJVDate from ViewInvoiceOutHeader" +
                    " where ((createuser='" + Login.userLoggedIn + "'" +
                    " or ForwarderList like '%" + userList + "%'" +
                    " or commentStatus like '%" + acStr + "%'" +
                    " or approveUser='" + Login.userLoggedIn + "')" +
                    " and DocumentStatus = 99  and Status = 1)  order by InvoiceDate desc,DocumentID asc,InvoiceNo desc";

                string query6 = "select RowID, DocumentID, DocumentName,TemporaryNo,TemporaryDate," +
                    " InvoiceNo,InvoiceDate,TrackingNos,TrackingDates,Consignee,ConsigneeName,TermsOfPayment,Description,TransportationMode,TransportationModeName,TransportationType," +
                    " TransportationTypeName,Transporter,TransporterName,CurrencyID,INRConversionRate,ADCode,EntryPort, " +
                    " ExitPort,FinalDestinationCountryID,FinalDestinationCountryName,OriginCountryID,OriginCountryName," +
                    "FinalDestinationPlace,PreCarriageTransportationMode,PreCarriageTransportationName,PreCarrierReceiptPlace,TermsOfDelivery,Remarks," +
                    "CommentStatus,CreateUser,ForwardUser,ApproveUser,CreatorName,CreateTime,ForwarderName,ApproverName,ForwarderList,Status,DocumentStatus,ProductValue,TaxAmount,InvoiceAmount,INRAmount,ProjectID " +
                    " , ProductValueINR, TaxAmountINR,BankAcReference,FreightCharge,DeliveryAddress,DeliveryStateCode,SJVTNo,SJVTDate,SJVNo,SJVDate from ViewInvoiceOutHeader" +
                    " where  DocumentStatus = 99  and Status = 1order by InvoiceDate desc,DocumentID asc,InvoiceNo desc";

                SqlConnection conn = new SqlConnection(Login.connString);
                string query = "";
                switch (opt)
                {
                    case 1:
                        query = query1;
                        break;
                    case 2:
                        query = query2;
                        break;
                    case 3:
                        query = query3;
                        break;
                    //case 4:
                    //    query = query4;
                    //    break;
                    //case 5:
                    //    query = query5;
                    //    break;
                    case 6:
                        query = query6;
                        break;
                    default:
                        query = "";
                        break;
                }
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ioh = new invoiceoutheader();
                    ioh.RowID = reader.GetInt32(0);
                    ioh.DocumentID = reader.GetString(1);
                    ioh.DocumentName = reader.GetString(2);
                    ioh.TemporaryNo = reader.GetInt32(3);
                    ioh.TemporaryDate = reader.GetDateTime(4);
                    ioh.InvoiceNo = reader.GetInt32(5);
                    if (!reader.IsDBNull(6))
                    {
                        ioh.InvoiceDate = reader.GetDateTime(6);
                    }


                    //ioh.TrackingNo = reader.GetInt32(7);
                    //ioh.TrackingDate = reader.GetDateTime(8);
                    ioh.TrackingNos = reader.IsDBNull(7) ? "" : reader.GetString(7);
                    ioh.TrackingDates = reader.IsDBNull(8) ? "" : reader.GetString(8);
                    ioh.ConsigneeID = reader.GetString(9);
                    ioh.ConsigneeName = reader.GetString(10);
                    ioh.TermsOfPayment = reader.GetString(11);
                    ioh.Description = reader.GetString(12);

                    ioh.TransportationMode = reader.IsDBNull(13) ? "" : reader.GetString(13);
                    ioh.TransportationModeName = reader.IsDBNull(14) ? "" : reader.GetString(14);
                    ioh.TransportationType = reader.IsDBNull(15) ? "" : reader.GetString(15);
                    ioh.TransportationTypeName = reader.IsDBNull(16) ? "" : reader.GetString(16);
                    ioh.Transporter = reader.IsDBNull(17) ? "" : reader.GetString(17);
                    ioh.TransporterName = reader.IsDBNull(18) ? "" : reader.GetString(18);

                    //popih.ValidityDate = reader.GetDateTime(12);
                    ioh.CurrencyID = reader.GetString(19);
                    // ioh.TaxCode = reader.GetString(20);
                    ioh.INRConversionRate = reader.GetDouble(20);
                    ioh.ADCode = reader.GetString(21);
                    ioh.EntryPort = reader.GetString(22);
                    ioh.ExitPort = reader.GetString(23);

                    ioh.FinalDestinatoinCountryID = reader.GetString(24);
                    if (!reader.IsDBNull(25))
                    {
                        ioh.FinalDestinatoinCountryName = reader.GetString(25);
                    }
                    else
                    {
                        ioh.FinalDestinatoinCountryName = "";
                    }

                    ioh.OriginCountryID = reader.GetString(26);
                    if (!reader.IsDBNull(27))
                    {
                        ioh.OriginCountryName = reader.GetString(27);
                    }
                    else
                    {
                        ioh.OriginCountryName = "";
                    }

                    ioh.FinalDestinationPlace = reader.GetString(28);
                    ioh.PreCarriageTransportationMode = reader.GetString(29);
                    if (!reader.IsDBNull(30))
                    {
                        ioh.PreCarriageTransportationName = reader.GetString(30);
                    }
                    else
                    {
                        ioh.PreCarriageTransportationName = "";
                    }

                    ioh.PreCarrierReceiptPlace = reader.GetString(31);
                    ioh.TermsOfDelivery = reader.GetString(32);
                    ioh.Remarks = reader.GetString(33);
                    if (!reader.IsDBNull(34))
                    {
                        ioh.CommentStatus = reader.GetString(34);
                    }
                    else
                    {
                        ioh.CommentStatus = "";
                    }
                    ioh.CreateUser = reader.GetString(35);
                    ioh.ForwardUser = reader.GetString(36);
                    ioh.ApproveUser = reader.GetString(37);
                    ioh.CreatorName = reader.GetString(38);
                    ioh.CreateTime = reader.GetDateTime(39);
                    ioh.ForwarderName = reader.GetString(40);
                    ioh.ApproverName = reader.GetString(41);
                    if (!reader.IsDBNull(42))
                    {
                        ioh.ForwarderList = reader.GetString(42);
                    }
                    else
                    {
                        ioh.ForwarderList = "";
                    }

                    // ioh.Remarks = reader.GetString(44);
                    ioh.status = reader.GetInt32(43);
                    ioh.DocumentStatus = reader.GetInt32(44);
                    ioh.ProductValue = reader.GetDouble(45);
                    ioh.TaxAmount = reader.GetDouble(46);
                    ioh.InvoiceAmount = reader.GetDouble(47);
                    ioh.INRAmount = reader.GetDouble(48);
                    ioh.ProjectID = reader.IsDBNull(49) ? "" : reader.GetString(49);
                    ioh.ProductValueINR = reader.GetDouble(50);
                    ioh.TaxAmountINR = reader.GetDouble(51);
                    ioh.BankAcReference = reader.IsDBNull(52) ? 0 : reader.GetInt32(52);
                    ioh.FreightCharge = reader.IsDBNull(53) ? 0 : reader.GetDouble(53);
                    ioh.DeliveryAddress = reader.IsDBNull(54) ? "" : reader.GetString(54);
                    ioh.DeliveryStateCode = reader.IsDBNull(55) ? "" : reader.GetString(55);

                    ioh.SJVTNo = reader.IsDBNull(56) ? 0 : reader.GetInt32(56);
                    ioh.SJVTDate = reader.IsDBNull(57) ? DateTime.Parse("1900-01-01") : reader.GetDateTime(57);
                    ioh.SJVNo = reader.IsDBNull(58) ? 0 : reader.GetInt32(58);
                    ioh.SJVDate = reader.IsDBNull(59)?DateTime.Parse("1900-01-01"):reader.GetDateTime(59);
                    IOHeaders.Add(ioh);
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error querying PO Product Inward Header Details");
            }
            return IOHeaders;
        }



        public static List<invoiceoutdetail> getInvoiceOutDetail(invoiceoutheader ioh)
        {
            invoiceoutdetail iod;
            List<invoiceoutdetail> IODetail = new List<invoiceoutdetail>();
            try
            {
                string query = "";
                SqlConnection conn = new SqlConnection(Login.connString);
                string query1 = "select RowID,DocumentID,TemporaryNo, TemporaryDate,StockItemID,StockItemName,ModelNo,ModelName, " +
                   " ISNULL(CustomerItemDescription,' ')," +
                   " Quantity,Price,Tax,WarrantyDays,TaxDetails,POItemReferenceNo,MRNNo,MRNDate,BatchNo,ExpiryDate,SerialNO, " +
                   " PurchaseQuantity, PurchasePrice, PurchaseTax, SupplierId,SupplierName, StockReferenceNo,Unit,TaxCode,PONo,PODate " +
                   "from ViewInvoiceOutDetail  where " +
                   " DocumentID='" + ioh.DocumentID + "'" +
                   " and TemporaryNo=" + ioh.TemporaryNo +
                   " and TemporaryDate='" + ioh.TemporaryDate.ToString("yyyy-MM-dd") + "'" +
                   " order by StockItemID";
                SqlCommand cmd = new SqlCommand(query1, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    iod = new invoiceoutdetail();
                    iod.RowID = reader.GetInt32(0);
                    iod.DocumentID = reader.GetString(1);
                    iod.TemporaryNo = reader.GetInt32(2);
                    iod.TemporaryDate = reader.GetDateTime(3).Date;
                    iod.StockItemID = reader.GetString(4);
                    if (!reader.IsDBNull(5))
                    {
                        iod.StockItemName = reader.GetString(5);
                    }
                    else
                    {
                        iod.StockItemName = "";
                    }
                    iod.ModelNo = reader.IsDBNull(6) ? "NA" : reader.GetString(6);
                    iod.ModelName = reader.IsDBNull(7) ? "NA" : reader.GetString(7);
                    iod.CustomerItemDescription = reader.GetString(8);
                    iod.Quantity = reader.GetDouble(9);
                    iod.Price = reader.GetDouble(10);
                    iod.Tax = reader.GetDouble(11);
                    iod.WarrantyDays = reader.GetInt32(12);
                    iod.TaxDetails = reader.GetString(13);
                    iod.POItemReferenceNo = reader.GetInt32(14);

                    iod.MRNNo = reader.GetInt32(15);
                    iod.MRNDate = reader.GetDateTime(16);
                    iod.BatchNo = reader.GetString(17);
                    iod.SerielNo = reader.IsDBNull(19) ? "NA" : reader.GetString(19);
                    iod.ExpiryDate = reader.GetDateTime(18);
                    iod.PurchaseQuantity = reader.GetDouble(20);
                    iod.PurchasePrice = reader.GetDouble(21);
                    iod.PurchaseTax = reader.GetDouble(22);
                    iod.SupplierID = reader.GetString(23);
                    iod.SupplierName = reader.IsDBNull(24) ? "NA" : reader.GetString(24);
                    iod.StockReferenceNo = reader.GetInt32(25);
                    iod.Unit = reader.IsDBNull(26) ? "" : reader.GetString(26);
                    iod.TaxCode = reader.IsDBNull(27) ? "" : reader.GetString(27);
                    iod.PONo = reader.IsDBNull(28) ? 0 : reader.GetInt32(28);
                    iod.PODate = reader.IsDBNull(29) ? DateTime.Parse("1900-01-01") : reader.GetDateTime(29);
                    IODetail.Add(iod);
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error querying InvoiceOutHeader Details");
            }
            return IODetail;
        }
        public Boolean validateInvoiceOutHeader(invoiceoutheader ioh)
        {
            Boolean status = true;
            try
            {
                if (ioh.DocumentID.Trim().Length == 0 || ioh.DocumentID == null)
                {
                    return false;
                }

                if (ioh.ConsigneeID.Trim().Length == 0 || ioh.ConsigneeID == null)
                {
                    return false;
                }
                //if (ioh.TrackingNo == 0)
                //{
                //    return false;
                //}
                ////if (ioh.FreightCharge == 0)
                ////{
                ////    return false;
                ////}
                if (ioh.TrackingNos == null || ioh.TrackingNos.Trim().Length == 0)
                {
                    return false;
                }
                if (ioh.DeliveryAddress == null || ioh.DeliveryAddress.Trim().Length == 0)
                {
                    return false;
                }
                if (ioh.TrackingDates == null || ioh.TrackingDates.Trim().Length == 0)
                {
                    return false;
                }
                if (ioh.TermsOfPayment.Trim().Length == 0 || ioh.TermsOfPayment == null)
                {
                    return false;
                }
                if (ioh.ProjectID.Trim().Length == 0 || ioh.ProjectID == null)
                {
                    return false;
                }
                if ((ioh.DocumentID == "PRODUCTEXPORTINVOICEOUT") || (ioh.DocumentID == "PRODUCTINVOICEOUT"))
                {
                    if (ioh.TransportationMode.Trim().Length == 0 || ioh.TransportationType == null)
                    {
                        return false;
                    }
                    if (ioh.Transporter.Trim().Length == 0 || ioh.Transporter == null)
                    {
                        return false;
                    }
                    if (ioh.TransportationType.Trim().Length == 0 || ioh.ConsigneeID == null)
                    {
                        return false;
                    }
                }
                if (ioh.CurrencyID.Trim().Length == 0 || ioh.CurrencyID == null)
                {
                    return false;
                }
                //if (ioh.TaxCode.Trim().Length == 0 || ioh.TaxCode == null)
                //{
                //    return false;
                //}
                if ((ioh.DocumentID == "PRODUCTEXPORTINVOICEOUT") || (ioh.DocumentID == "SERVICEEXPORTINVOICEOUT"))
                {
                    if (ioh.ADCode.Trim().Length == 0 || ioh.ADCode == null)
                    {
                        return false;
                    }
                    if (ioh.EntryPort.Trim().Length == 0 || ioh.EntryPort == null)
                    {
                        return false;
                    }
                    if (ioh.ExitPort.Trim().Length == 0 || ioh.ExitPort == null)
                    {
                        return false;
                    }
                    if (ioh.FinalDestinatoinCountryID.Trim().Length == 0 || ioh.FinalDestinatoinCountryID == null)
                    {
                        return false;
                    }
                    if (ioh.PreCarriageTransportationMode.Trim().Length == 0 || ioh.PreCarriageTransportationMode == null)
                    {
                        return false;
                    }
                    if (ioh.PreCarrierReceiptPlace.Trim().Length == 0 || ioh.PreCarrierReceiptPlace == null)
                    {
                        return false;
                    }
                    if (ioh.TermsOfDelivery.Trim().Length == 0 || ioh.TermsOfDelivery == null)
                    {
                        return false;
                    }
                    if (ioh.OriginCountryID.Trim().Length == 0 || ioh.OriginCountryID == null)
                    {
                        return false;
                    }
                    if (ioh.FinalDestinationPlace.Trim().Length == 0 || ioh.FinalDestinationPlace == null)
                    {
                        return false;
                    }
                }

                if (ioh.INRConversionRate == 0)
                {
                    return false;
                }
                if (ioh.BankAcReference == 0)
                {
                    return false;
                }
                if (ioh.ProductValue == 0)
                {
                    return false;
                }
                //if (ioh.INRAmount == 0)
                //{
                //    return false;
                //}
                if (ioh.InvoiceAmount == 0)
                {
                    return false;
                }
                if (ioh.ProductValueINR == 0)
                {
                    return false;
                }
                if (ioh.INRAmount == 0)
                {
                    return false;
                }
                if (ioh.Remarks.Trim().Length == 0 || ioh.Remarks == null)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
            }
            return status;
        }
        public Boolean forwardInvoiceHeader(invoiceoutheader ioh)
        {
            Boolean status = true;
            string utString = "";
            try
            {
                string updateSQL = "update InvoiceOutHeader set DocumentStatus=" + (ioh.DocumentStatus + 1) +
                    ", forwardUser='" + ioh.ForwardUser + "'" +
                    ", commentStatus='" + ioh.CommentStatus + "'" +
                    ", ForwarderList='" + ioh.ForwarderList + "'" +
                    " where DocumentID='" + ioh.DocumentID + "'" +
                    " and TemporaryNo=" + ioh.TemporaryNo +
                    " and TemporaryDate='" + ioh.TemporaryDate.ToString("yyyy-MM-dd") + "'";
                utString = utString + updateSQL + Main.QueryDelimiter;
                utString = utString +
                ActivityLogDB.PrepareActivityLogQquerString("update", "InvoiceOutHeader", "", updateSQL) +
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

        public Boolean reverseInvoiceOut(invoiceoutheader ioh)
        {
            Boolean status = true;
            string utString = "";
            try
            {
                string updateSQL = "update InvoiceOutHeader set DocumentStatus=" + ioh.DocumentStatus +
                    ", forwardUser='" + ioh.ForwardUser + "'" +
                    ", commentStatus='" + ioh.CommentStatus + "'" +
                    ", ForwarderList='" + ioh.ForwarderList + "'" +
                    " where DocumentID='" + ioh.DocumentID + "'" +
                    " and TemporaryNo=" + ioh.TemporaryNo +
                    " and TemporaryDate='" + ioh.TemporaryDate.ToString("yyyy-MM-dd") + "'";
                utString = utString + updateSQL + Main.QueryDelimiter;
                utString = utString +
                ActivityLogDB.PrepareActivityLogQquerString("update", "InvoiceOutHeader", "", updateSQL) +
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

        public Boolean ApproveInvoiceOut(invoiceoutheader ioh)
        {
            Boolean status = true;
            string utString = "";
            try
            {
                string updateSQL = "update InvoiceOutHeader set DocumentStatus=99, status=1 " +
                    ", ApproveUser='" + Login.userLoggedIn + "'" +
                    ", commentStatus='" + ioh.CommentStatus + "'" +
                    ", InvoiceNo=" + ioh.InvoiceNo +
                    ", InvoiceDate=convert(date, getdate())" +
                    " where DocumentID='" + ioh.DocumentID + "'" +
                    " and TemporaryNo=" + ioh.TemporaryNo +
                    " and TemporaryDate='" + ioh.TemporaryDate.ToString("yyyy-MM-dd") + "'";
                utString = utString + updateSQL + Main.QueryDelimiter;
                utString = utString +
                ActivityLogDB.PrepareActivityLogQquerString("update", "InvoiceOutHeader", "", updateSQL) +
                Main.QueryDelimiter;

                string narration = "Sales against Invoice No " + ioh.InvoiceNo + "," +
               "Dated " + UpdateTable.getSQLDateTime().ToString("dd-MM-yyyy") + "," +
               "Party:" + ioh.ConsigneeName;

                int SJVNo = 0; //Journal No
                DateTime SJVDate = DateTime.Parse("1900-01-01"); //Journal Date
                //int SJVTempNo = 0; //Temporary No
                //DateTime SJVTempDate = DateTime.Parse("1900-01-01"); //Temporary Date

                if(ioh.SJVNo == 0 && ioh.SJVTNo > 0) // JV Available but not approved
                {
                    SJVNo = DocumentNumberDB.getNewNumber("SJV", 2);
                    SJVDate = UpdateTable.getSQLDateTime();
                }
                else //JV Available and approved // JV Not available
                {
                    SJVNo = ioh.SJVNo;
                    SJVDate = ioh.SJVDate;
                }

                //if (ioh.SJVTNo == 0)
                //{
                //    SJVTempNo = DocumentNumberDB.getNewNumber("SJV", 1);
                //    SJVTempDate = UpdateTable.getSQLDateTime();
                //}
                //else
                //{
                //    SJVTempNo = ioh.SJVNo;
                //    SJVTempDate = ioh.SJVDate;
                //}
                updateSQL = "update SJVHeader set DocumentStatus=99, status=1 ,InvReferenceNo = " + ioh.RowID +
                  ", ApproveUser='" + Login.userLoggedIn + "'" +
                  ", JournalNo=" + SJVNo +
                  ", JournalDate='" + SJVDate.ToString("yyyy-MM-dd") +
                   "', Narration='" + narration + "'" +
                  " where InvDocumentID='" + ioh.DocumentID + "'" +
                  " and InvTempNo=" + ioh.TemporaryNo +
                  " and InvTempDate='" + ioh.TemporaryDate.ToString("yyyy-MM-dd") + "'";
                utString = utString + updateSQL + Main.QueryDelimiter;
                utString = utString +
                ActivityLogDB.PrepareActivityLogQquerString("update", "SJVHeader", "", updateSQL) +
                Main.QueryDelimiter;

                updateSQL = "update InvoiceOutHeader set SJVNo='" + SJVNo + "'" +
                  ", SJVDate='" + SJVDate.ToString("yyyy-MM-dd") + "'" +
                 " where DocumentID='" + ioh.DocumentID + "'" +
                 " and TemporaryNo=" + ioh.TemporaryNo +
                 " and TemporaryDate='" + ioh.TemporaryDate.ToString("yyyy-MM-dd") + "'";
                utString = utString + updateSQL + Main.QueryDelimiter;
                utString = utString +
                ActivityLogDB.PrepareActivityLogQquerString("update", "InvoiceOutHeader", "", updateSQL) +
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

        public static string getUserComments(string docid, int tempno, DateTime tempdate)
        {
            string cmtString = "";
            try
            {
                SqlConnection conn = new SqlConnection(Login.connString);
                string query = "select comments from InvoiceOutHeader where DocumentID='" + docid + "'" +
                        " and TemporaryNo=" + tempno +
                        " and TemporaryDate='" + tempdate.ToString("yyyy-MM-dd") + "'";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    cmtString = reader.GetString(0);
                }
                conn.Open();
            }
            catch (Exception ex)
            {
            }
            return cmtString;
        }
        public static ListView getInvoiceOutListView(string custID)
        {
            ListView lv = new ListView();
            try
            {

                lv.View = View.Details;
                lv.LabelEdit = true;
                lv.AllowColumnReorder = true;
                lv.CheckBoxes = true;
                lv.FullRowSelect = true;
                lv.GridLines = true;
                lv.Sorting = System.Windows.Forms.SortOrder.Ascending;
                InvoiceOutHeaderDB IODB = new InvoiceOutHeaderDB();
                List<invoiceoutheader> IOList = IODB.getInvoiceOutHeaderDetail(custID);
                ////int index = 0;
                lv.Columns.Add("Select", -2, HorizontalAlignment.Left);
                lv.Columns.Add("Document ID", -2, HorizontalAlignment.Left);
                lv.Columns.Add("Invoice No", -2, HorizontalAlignment.Left);
                lv.Columns.Add("Invoice Date", -2, HorizontalAlignment.Left);
                lv.Columns.Add("Consignee", -2, HorizontalAlignment.Left);
                lv.Columns.Add("Invoice Amount", -2, HorizontalAlignment.Left);
                foreach (invoiceoutheader ioh in IOList)
                {
                    ListViewItem item1 = new ListViewItem();
                    item1.Checked = false;
                    item1.SubItems.Add(ioh.DocumentID.ToString());
                    item1.SubItems.Add(ioh.InvoiceNo.ToString());
                    item1.SubItems.Add(ioh.InvoiceDate.ToString("yyyy-MM-dd"));
                    item1.SubItems.Add(ioh.ConsigneeID + "-" + ioh.ConsigneeName);
                    item1.SubItems.Add(ioh.InvoiceAmount.ToString());
                    lv.Items.Add(item1);
                }
            }
            catch (Exception)
            {

            }
            return lv;
        }
        public List<invoiceoutheader> getInvoiceOutHeaderDetail(string custID)
        {
            invoiceoutheader ioh;
            List<invoiceoutheader> IOHeaders = new List<invoiceoutheader>();
            try
            {
                string query = "select DocumentID, DocumentName," +
                    " InvoiceNo,InvoiceDate,Consignee,ConsigneeName,InvoiceAmount from ViewInvoiceOutHeader" +
                    " where DocumentStatus = 99 and Status = 1 and Consignee = '" + custID + "' order by InvoiceDate desc";

                SqlConnection conn = new SqlConnection(Login.connString);

                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ioh = new invoiceoutheader();
                    ioh.DocumentID = reader.GetString(0);
                    ioh.DocumentName = reader.GetString(1);
                    ioh.InvoiceNo = reader.GetInt32(2);
                    ioh.InvoiceDate = reader.GetDateTime(3);
                    ioh.ConsigneeID = reader.GetString(4);
                    ioh.ConsigneeName = reader.GetString(5);
                    ioh.InvoiceAmount = reader.GetDouble(6);
                
                    IOHeaders.Add(ioh);
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error querying InvoiceOut Header Details");
            }
            return IOHeaders;
        }
        public static string getIOHDtlsForProjectTrans(string projectID)
        {
            string str = "";
            try
            {
                SqlConnection conn = new SqlConnection(Login.connString);
                string query = "select COUNT(*), SUM(InvoiceAmount) from InvoiceOutHeader where ProjectID = '" + projectID + "' and DocumentStatus = 99";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    double dd = reader.IsDBNull(1) ? 0 : reader.GetDouble(1);
                    str = reader.GetInt32(0) + "-" + dd;
                }
                conn.Close();
            }
            catch (Exception ex)
            {
            }
            return str;
        }
        public static List<invoiceoutheader> getRVINFOForProjectTrans(string projectID)
        {
            invoiceoutheader ioh;
            List<invoiceoutheader> IOHeaders = new List<invoiceoutheader>();
            try
            {
                SqlConnection conn = new SqlConnection(Login.connString);
                string query = "select InvoiceNo,InvoiceDate,ConsigneeName,ProductValue,TaxAmount,InvoiceAmount,ProjectID from ViewInvoiceOutHeader where ProjectID = '" + projectID + "' and DocumentStatus = 99";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ioh = new invoiceoutheader();
                    ioh.InvoiceNo = reader.GetInt32(0);
                    ioh.InvoiceDate = reader.GetDateTime(1);
                    ioh.ConsigneeName = reader.GetString(2);
                    ioh.ProductValue = reader.GetDouble(3);
                    ioh.TaxAmount = reader.GetDouble(4);
                    ioh.InvoiceAmount = reader.GetDouble(5);
                    ioh.ProjectID = reader.GetString(6);
                    IOHeaders.Add(ioh);
                }
                conn.Close();
            }
            catch (Exception ex)
            {
            }
            return IOHeaders;
        }
        public Boolean updateInvoiceOutHeaderAndDetail(invoiceoutheader ioh, invoiceoutheader previoh, List<invoiceoutdetail> iodList)
        {
            Boolean status = true;
            string utString = "";
            try
            {
                string updateSQL = "update InvoiceOutHeader set " +
                   " TrackingNos='" + ioh.TrackingNos + // For List of POSElected
                    "',TrackingDates='" + ioh.TrackingDates + // For List of PODateSElected
                   "', Consignee='" + ioh.ConsigneeID +
                     "', TermsOfPayment='" + ioh.TermsOfPayment +
                   "', TransportationMode='" + ioh.TransportationMode +
                   "', TransportationType='" + ioh.TransportationType +
                   "', Transporter ='" + ioh.Transporter +
                   "', CurrencyID='" + ioh.CurrencyID +
                    "', ProjectID='" + ioh.ProjectID +
                       "', DeliveryAddress='" + ioh.DeliveryAddress +
                    "', DeliveryStateCode='" + ioh.DeliveryStateCode +
                   "', INRConversionRate='" + ioh.INRConversionRate +
                   "', ADCode='" + ioh.ADCode +
                     "', EntryPort='" + ioh.EntryPort +
                   "', ExitPort='" + ioh.ExitPort +
                     "', FinalDestinationCountryID='" + ioh.FinalDestinatoinCountryID +
                   "', OriginCountryID ='" + ioh.OriginCountryID +
                      "', FinalDestinationPlace='" + ioh.FinalDestinationPlace +
                   "', PreCarriageTransportationMode ='" + ioh.PreCarriageTransportationMode +
                     "', PreCarrierReceiptPlace='" + ioh.PreCarrierReceiptPlace +
                   "', TermsOfDelivery ='" + ioh.TermsOfDelivery +
                   "', Remarks='" + ioh.Remarks +
                   "', Comments='" + ioh.Comments +
                   "', CommentStatus='" + ioh.CommentStatus +
                   "', ProductValue=" + ioh.ProductValue +
                    ", BankAcReference=" + ioh.BankAcReference +
                    ", FreightCharge=" + ioh.FreightCharge +
                   ", TaxAmount=" + ioh.TaxAmount +
                       ", ProductValueINR=" + ioh.ProductValueINR +
                   ", TaxAmountINR=" + ioh.TaxAmountINR +
                   ", InvoiceAmount=" + ioh.InvoiceAmount +
                   ", INRAmount=" + ioh.INRAmount +
                   ", ForwarderList='" + ioh.ForwarderList +
                   "' where DocumentID='" + ioh.DocumentID + "'" +
                   " and TemporaryNo=" + ioh.TemporaryNo +
                   " and TemporaryDate='" + ioh.TemporaryDate.ToString("yyyy-MM-dd") + "'";
                utString = utString + updateSQL + Main.QueryDelimiter;
                utString = utString +
                ActivityLogDB.PrepareActivityLogQquerString("update", "InvoiceOutHeader", "", updateSQL) +
                Main.QueryDelimiter;

                updateSQL = "Delete from InvoiceOutDetail where DocumentID='" + ioh.DocumentID + "'" +
                     " and TemporaryNo=" + ioh.TemporaryNo +
                     " and TemporaryDate='" + ioh.TemporaryDate.ToString("yyyy-MM-dd") + "'";
                utString = utString + updateSQL + Main.QueryDelimiter;
                utString = utString +
                    ActivityLogDB.PrepareActivityLogQquerString("delete", "InvoiceOutDetail", "", updateSQL) +
                    Main.QueryDelimiter;
                foreach (invoiceoutdetail iod in iodList)
                {
                    updateSQL = "insert into InvoiceOutDetail " +
                    "(DocumentID,TemporaryNo,TemporaryDate,PONo,PODate,StockItemID,ModelNo,CustomerItemDescription,TaxCode,Quantity,Price," +
                     " Tax,WarrantyDays,TaxDetails,POItemReferenceNo,MRNNo,MRNDate,BatchNo,ExpiryDate,SerialNO, " +
                   " PurchaseQuantity, PurchasePrice, PurchaseTax, SupplierId, StockReferenceNo) " +
                    "values ('" + ioh.DocumentID + "'," +
                    ioh.TemporaryNo + "," +
                    "'" + ioh.TemporaryDate.ToString("yyyy-MM-dd") + "'," +
                      iod.PONo + "," +
                    "'" + iod.PODate.ToString("yyyy-MM-dd") + "'," +
                    "'" + iod.StockItemID + "'," +
                       "'" + iod.ModelNo + "'," +
                    "'" + iod.CustomerItemDescription + "'," +
                    "'" + iod.TaxCode + "'," +
                    iod.Quantity + "," +
                    iod.Price + "," +
                    iod.Tax + "," +
                    iod.WarrantyDays + "," +
                    "'" + iod.TaxDetails + "',"
                    + iod.POItemReferenceNo + "," +
                    iod.MRNNo + "," +
                    "'" + iod.MRNDate.ToString("yyyy-MM-dd") + "'," +
                    "'" + iod.BatchNo + "'," +
                    "'" + iod.ExpiryDate.ToString("yyyy-MM-dd") + "'," +
                   "'" + iod.SerielNo + "'," +
                     iod.PurchaseQuantity + "," +
                    iod.PurchasePrice + "," +
                    iod.PurchaseTax + "," +
                    "'" + iod.SupplierID + "'," +
                      iod.StockReferenceNo + ")";
                    utString = utString + updateSQL + Main.QueryDelimiter;
                    utString = utString +
                    ActivityLogDB.PrepareActivityLogQquerString("insert", "InvoiceOutDetail", "", updateSQL) +
                    Main.QueryDelimiter;
                }
                if (!UpdateTable.UT(utString))
                {
                    status = false;
                    MessageBox.Show("Transaction Exception Occured");
                }
            }
            catch (Exception ex)
            {
                status = false;
            }
            return status;
        }
        public Boolean InsertInvoiceOutHeaderAndDetail(invoiceoutheader ioh, List<invoiceoutdetail> iodList, out int Tno)
        {

            Boolean status = true;
            string utString = "";
            string updateSQL = "";
            Tno = 0;
            try
            {
                ioh.TemporaryNo = DocumentNumberDB.getNumber(ioh.DocumentID, 1);
                if (ioh.TemporaryNo <= 0)
                {
                    MessageBox.Show("Error in Creating New Number");
                    return false;
                }
                Tno = ioh.TemporaryNo;
                updateSQL = "update DocumentNumber set TempNo =" + ioh.TemporaryNo +
                    " where FYID='" + Main.currentFY + "' and DocumentID='" + ioh.DocumentID + "'";
                utString = utString + updateSQL + Main.QueryDelimiter;
                utString = utString +
                   ActivityLogDB.PrepareActivityLogQquerString("update", "DocumentNumber", "", updateSQL) +
                   Main.QueryDelimiter;

                updateSQL = "insert into InvoiceOutHeader " +
                    "(DocumentID,TemporaryNo,TemporaryDate,InvoiceNo,InvoiceDate,TrackingNos,TrackingDates," +
                    "Consignee,TermsOfPayment,TransportationMode,TransportationType,DeliveryAddress,DeliveryStateCode,Transporter,CurrencyID,ProjectID,INRConversionRate,ADCode,EntryPort,ExitPort," +
                    "FinalDestinationCountryID,OriginCountryID,FinalDestinationPlace,PreCarriageTransportationMode,PreCarrierReceiptPlace,TermsOfDelivery,Remarks,Comments,CommentStatus," +
                    "BankAcReference,FreightCharge,ProductValue,TaxAmount,ProductValueINR,TaxAmountINR,InvoiceAmount,INRAmount,ForwarderList,Status,DocumentStatus,CreateTime,CreateUser)" +
                    " values (" +
                    "'" + ioh.DocumentID + "'," +
                    ioh.TemporaryNo + "," +
                    "'" + ioh.TemporaryDate.ToString("yyyy-MM-dd") + "'," +
                    ioh.InvoiceNo + "," +
                    "'" + ioh.InvoiceDate.ToString("yyyy-MM-dd") + "'," +
                     "'" + ioh.TrackingNos + "'," +             // For List of POSElected
                    "'" + ioh.TrackingDates + "'," +     // For List of PODateSElected
                    "'" + ioh.ConsigneeID + "'," +
                    "'" + ioh.TermsOfPayment + "'," +
                    "'" + ioh.TransportationMode + "'," +
                    "'" + ioh.TransportationType + "'," +

                     "'" + ioh.DeliveryAddress + "'," +
                    "'" + ioh.DeliveryStateCode + "'," +

                    "'" + ioh.Transporter + "'," +
                      "'" + ioh.CurrencyID + "'," +
                     "'" + ioh.ProjectID + "'," +
                    +ioh.INRConversionRate + "," +
                    "'" + ioh.ADCode + "'," +
                    "'" + ioh.EntryPort + "'," +
                    "'" + ioh.ExitPort + "'," +
                    "'" + ioh.FinalDestinatoinCountryID + "'," +

                       "'" + ioh.OriginCountryID + "'," +
                    "'" + ioh.FinalDestinationPlace + "'," +
                    "'" + ioh.PreCarriageTransportationMode + "'," +
                    "'" + ioh.PreCarrierReceiptPlace + "'," +
                      "'" + ioh.TermsOfDelivery + "'," +

                       "'" + ioh.Remarks + "'," +
                         "'" + ioh.Comments + "'," +
                    "'" + ioh.CommentStatus + "'," +
                    ioh.BankAcReference + "," +
                     ioh.FreightCharge + "," +
                    ioh.ProductValue + "," +
                    ioh.TaxAmount + "," +
                       ioh.ProductValueINR + "," +
                    ioh.TaxAmountINR + "," +
                    ioh.InvoiceAmount + "," +
                     ioh.INRAmount + "," +
                    "'" + ioh.ForwarderList + "'," +
                    ioh.status + "," +
                    ioh.DocumentStatus + "," +
                     "GETDATE()" + "," +
                    "'" + Login.userLoggedIn + "')";
                utString = utString + updateSQL + Main.QueryDelimiter;
                utString = utString +
                ActivityLogDB.PrepareActivityLogQquerString("insert", "InvoiceOutHeader", "", updateSQL) +
                Main.QueryDelimiter;

                updateSQL = "Delete from InvoiceOutDetail where DocumentID='" + ioh.DocumentID + "'" +
                   " and TemporaryNo=" + ioh.TemporaryNo +
                   " and TemporaryDate='" + ioh.TemporaryDate.ToString("yyyy-MM-dd") + "'";
                utString = utString + updateSQL + Main.QueryDelimiter;
                utString = utString +
                    ActivityLogDB.PrepareActivityLogQquerString("delete", "InvoiceOutDetail", "", updateSQL) +
                    Main.QueryDelimiter;
                foreach (invoiceoutdetail iod in iodList)
                {
                    updateSQL = "insert into InvoiceOutDetail " +
                    "(DocumentID,TemporaryNo,TemporaryDate,PONo,PODate,StockItemID,ModelNo,CustomerItemDescription,TaxCode,Quantity,Price," +
                     " Tax,WarrantyDays,TaxDetails,POItemReferenceNo,MRNNo,MRNDate,BatchNo,ExpiryDate,SerialNO, " +
                   " PurchaseQuantity, PurchasePrice, PurchaseTax, SupplierId, StockReferenceNo) " +
                    "values ('" + ioh.DocumentID + "'," +
                    ioh.TemporaryNo + "," +
                    "'" + ioh.TemporaryDate.ToString("yyyy-MM-dd") + "'," +
                     iod.PONo + "," +
                    "'" + iod.PODate.ToString("yyyy-MM-dd") + "'," +
                    "'" + iod.StockItemID + "'," +
                       "'" + iod.ModelNo + "'," +
                    "'" + iod.CustomerItemDescription + "'," +
                    "'" + iod.TaxCode + "'," +
                    iod.Quantity + "," +
                    iod.Price + "," +
                    iod.Tax + "," +
                    iod.WarrantyDays + "," +
                    "'" + iod.TaxDetails + "',"
                    + iod.POItemReferenceNo + "," +
                    iod.MRNNo + "," +
                    "'" + iod.MRNDate.ToString("yyyy-MM-dd") + "'," +
                    "'" + iod.BatchNo + "'," +
                    "'" + iod.ExpiryDate.ToString("yyyy-MM-dd") + "'," +
                   "'" + iod.SerielNo + "'," +
                     iod.PurchaseQuantity + "," +
                    iod.PurchasePrice + "," +
                    iod.PurchaseTax + "," +
                    "'" + iod.SupplierID + "'," +
                      iod.StockReferenceNo + ")";
                    utString = utString + updateSQL + Main.QueryDelimiter;
                    utString = utString +
                    ActivityLogDB.PrepareActivityLogQquerString("insert", "InvoiceOutDetail", "", updateSQL) +
                    Main.QueryDelimiter;
                }
                if (!UpdateTable.UT(utString))
                {
                    status = false;
                }
            }
            catch (Exception ex)
            {
                status = false;
                MessageBox.Show("Transaction Exception Occured");
            }
            return status;
        }
        public Boolean updateIOInStock(List<invoiceoutdetail> IODetails)
        {
            Boolean status = true;
            // string utString = "";
            try
            {
                foreach (invoiceoutdetail iod in IODetails)
                {
                    double quant = iod.Quantity;
                    int RefNo = iod.StockReferenceNo;
                    updateRefNoWiseIODetailInStock(quant, RefNo);
                }
            }
            catch (Exception ex)
            {
                status = false;
            }
            return status;
        }
        public void updateRefNoWiseIODetailInStock(double quantity, int stockrefno)
        {
            //Boolean status = true;
            string utString = "";
            try
            {
                string updateSQL = "update Stock set  " +
                    " PresentStock=" + "( (select PresentStock from Stock where RowID = " + stockrefno + ")-" + quantity + ")" +
                    ", IssueQuantity=" + "( (select IssueQuantity from Stock where RowID = " + stockrefno + ")+" + quantity + ")" +
                    " where RowID=" + stockrefno;
                utString = utString + updateSQL + Main.QueryDelimiter;
                utString = utString +
                ActivityLogDB.PrepareActivityLogQquerString("update", "Stock", "", updateSQL) +
                Main.QueryDelimiter;

                if (!UpdateTable.UT(utString))
                {
                    //status = false;
                    MessageBox.Show("updateRefNoWiseIODetailInStock() : failed to Update In Reference Number Wise Invoice out Detail in stock");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("updateRefNoWiseIODetailInStock() : failed to Update In Reference Number Wise Invoice Out Detail in stock");
                return;
            }
            //return status;
        }

        public static invoiceoutdetail getItemWiseTotalQuantForPerticularPOInInvoiceOUt(int poRefNo, string docIDstr)
        {
            invoiceoutdetail iod = new invoiceoutdetail();
            //List<mrnheader> MrnHeaders = new List<mrnheader>();
            try
            {
                string[] str = docIDstr.Split(';');
                SqlConnection conn = new SqlConnection(Login.connString);
                string query = "select b.POItemReferenceNo,sum(b.Quantity) as TotQuant " +
                        " from  InvoiceOutDetail b,InvoiceOutHeader a " +
                        "where a.TemporaryNo = b.TemporaryNo and a.TemporaryDate = b.TemporaryDate and a.DocumentID=b.DocumentID and a.Status = 1 and a.DocumentStatus = 99" +
                        " and b.POItemReferenceNo = " + poRefNo +
                        " and b.DocumentID in ('" + str[0] + "','" + str[1] + "')" +
                        " group by b.POItemReferenceNo";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    iod.POItemReferenceNo = reader.GetInt32(0);
                    iod.Quantity = reader.GetDouble(1); // Total Quantity
                }
                conn.Close();
            }
            catch (Exception ex)
            {
            }
            return iod;
        }

        //   Codes for ReportPOAnalysis
        public List<ReportPO> getIODetailForpartWise(int opt1, int opt2, DateTime fromDate, DateTime toDate)
        {
            // opt1 : PO Wise
            // opt2 : SeviceWise
            // opt3 : PartyWise
            // opt4 : RegionWIse
            ReportPO rpo;
            List<ReportPO> POList = new List<ReportPO>();
            try
            {
                string query = "";
                if (opt1 == 1 && opt2 == 1)
                {
                    query = "select a.DocumentID,a.Consignee,b.Name,SUM(a.ProductValueINR) from InvoiceOutHeader a, Customer b " +
                        " where a.Consignee = b.CustomerID and a.DocumentID not in ('SERVICERCINVOICEOUT') and a.InvoiceDate <= '" + toDate.ToString("yyyy-MM-dd") + "'" +
                        " and a.InvoiceDate >= '" + fromDate.ToString("yyyy-MM-dd") + "'" + " and a.DocumentStatus = 99 and a.Status = 1 " +
                        "group by a.DocumentID,a.Consignee,b.Name";
                }
                else if (opt1 == 1)
                {
                    query = "select a.DocumentID,a.Consignee,b.Name,SUM(a.ProductValueINR) from InvoiceOutHeader a, Customer b" +
                        " where a.Consignee = b.CustomerID and a.DocumentID in ( 'PRODUCTEXPORTINVOICEOUT','PRODUCTINVOICEOUT' )" +
                        " and a.InvoiceDate <= '" + toDate.ToString("yyyy-MM-dd") + "'" +
                        " and a.InvoiceDate >= '" + fromDate.ToString("yyyy-MM-dd") + "'" + " and a.DocumentStatus = 99 and a.Status = 1 " +
                        " group by a.DocumentID,a.Consignee,b.Name";
                }
                else
                {
                    query = "select a.DocumentID,a.Consignee,b.Name,SUM(a.ProductValueINR) from InvoiceOutHeader a, Customer b" +
                        " where a.Consignee = b.CustomerID and a.DocumentID in ( 'SERVICEINVOICEOUT','SERVICEEXPORTINVOICEOUT' )" +
                        " and a.InvoiceDate <= '" + toDate.ToString("yyyy-MM-dd") + "'" +
                        " and a.InvoiceDate >= '" + fromDate.ToString("yyyy-MM-dd") + "'" + " and a.DocumentStatus = 99 and a.Status = 1 " +
                        " group by a.DocumentID,a.Consignee,b.Name";
                }

                SqlConnection conn = new SqlConnection(Login.connString);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    rpo = new ReportPO();
                    rpo.DocumentID = reader.GetString(0);
                    rpo.PartyID = reader.GetString(1);
                    rpo.Name = reader.GetString(2);
                    rpo.Value = reader.GetDouble(3);
                    if (rpo.DocumentID.Equals("PRODUCTEXPORTINVOICEOUT") || rpo.DocumentID.Equals("PRODUCTINVOICEOUT"))
                        rpo.DocumentType = "Product";
                    else if (rpo.DocumentID.Equals("SERVICEEXPORTINVOICEOUT") || rpo.DocumentID.Equals("SERVICEINVOICEOUT"))
                        rpo.DocumentType = "Service";
                    POList.Add(rpo);
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error querying InvoiceOUt Header Details");
            }
            return POList;
        }
        public List<ReportPO> getIODetailForRegionWise(int opt1, int opt2, DateTime fromDate, DateTime toDate)
        {
            // opt1 : PO Wise
            // opt2 : SeviceWise
            // opt3 : PartyWise
            // opt4 : RegionWIse
            ReportPO rpo;
            List<ReportPO> POList = new List<ReportPO>();
            try
            {
                string query = "";
                if (opt1 == 1 && opt2 == 1)
                {
                    query = "select a.DocumentID,c.RegionID,d.Name, sum(a.ProductValueINR) " +
                        "from InvoiceOutHeader a,Customer b, Office c, Region d " +
                        "where a.Consignee = b.CustomerID and b.OfficeID = c.OfficeID and c.RegionID = d.RegionID and a.DocumentID not in ('SERVICERCINVOICEOUT') and" +
                         " a.InvoiceDate <= '" + toDate.ToString("yyyy-MM-dd") + "'" +
                        " and a.InvoiceDate >= '" + fromDate.ToString("yyyy-MM-dd") + "'" + " and a.DocumentStatus = 99 and a.Status = 1 " +
                        " group by a.DocumentID,c.RegionID, d.Name";
                }
                else if (opt1 == 1)
                {
                    query = "select a.DocumentID,c.RegionID,d.Name, sum(a.ProductValueINR) " +
                        "from InvoiceOutHeader a,Customer b, Office c, Region d " +
                        "where a.Consignee = b.CustomerID and b.OfficeID = c.OfficeID and c.RegionID = d.RegionID and a.DocumentID in ( 'PRODUCTEXPORTINVOICEOUT','PRODUCTINVOICEOUT' )" +
                         " and a.InvoiceDate <= '" + toDate.ToString("yyyy-MM-dd") + "'" +
                        " and a.InvoiceDate >= '" + fromDate.ToString("yyyy-MM-dd") + "'" + " and a.DocumentStatus = 99 and a.Status = 1 " +
                        " group by a.DocumentID,c.RegionID, d.Name";
                }
                else
                {
                    query = "select a.DocumentID,c.RegionID,d.Name, sum(a.ProductValueINR) " +
                        "from InvoiceOutHeader a,Customer b, Office c, Region d " +
                        "where a.Consignee = b.CustomerID and b.OfficeID = c.OfficeID and c.RegionID = d.RegionID and a.DocumentID in ( 'SERVICEINVOICEOUT','SERVICEEXPORTINVOICEOUT' )" +
                         " and a.InvoiceDate <= '" + toDate.ToString("yyyy-MM-dd") + "'" +
                        " and a.InvoiceDate >= '" + fromDate.ToString("yyyy-MM-dd") + "'" + " and a.DocumentStatus = 99 and a.Status = 1 " +
                        " group by a.DocumentID,c.RegionID, d.Name";
                }

                SqlConnection conn = new SqlConnection(Login.connString);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    rpo = new ReportPO();
                    rpo.DocumentID = reader.GetString(0);
                    rpo.RegionID = reader.GetString(1);  // as region ID
                    rpo.Name = reader.GetString(2);  // as region name
                    rpo.Value = reader.GetDouble(3);
                    if (rpo.DocumentID.Equals("PRODUCTEXPORTINVOICEOUT") || rpo.DocumentID.Equals("PRODUCTINVOICEOUT"))
                        rpo.DocumentType = "Product";
                    else if (rpo.DocumentID.Equals("SERVICEEXPORTINVOICEOUT") || rpo.DocumentID.Equals("SERVICEINVOICEOUT"))
                        rpo.DocumentType = "Service";
                    POList.Add(rpo);
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error querying InvoiceOUt Header Details");
            }
            return POList;
        }
        public List<ReportPO> getIODetailForDocumentWise(int opt1, int opt2, DateTime fromDate, DateTime toDate)
        {
            // opt1 : PO Wise
            // opt2 : SeviceWise
            // opt3 : PartyWise
            // opt4 : RegionWIse
            ReportPO rpo;
            List<ReportPO> POList = new List<ReportPO>();
            try
            {
                string query = "";
                if (opt1 == 1 && opt2 == 1)
                {
                    query = "select DocumentID,sum(ProductValueINR) from InvoiceOutHeader" +
                         " where DocumentID not in ('SERVICERCINVOICEOUT') and InvoiceDate <= '" + toDate.ToString("yyyy-MM-dd") + "'" +
                        " and InvoiceDate >='" + fromDate.ToString("yyyy-MM-dd") + "'" + " and DocumentStatus = 99 and Status = 1 " +
                        " group by DocumentID ";
                }
                else if (opt1 == 1)
                {
                    query = "select DocumentID,sum(ProductValueINR) from InvoiceOutHeader" +
                        " where DocumentID in ( 'PRODUCTEXPORTINVOICEOUT','PRODUCTINVOICEOUT' )" +
                        " and InvoiceDate <= '" + toDate.ToString("yyyy-MM-dd") + "'" +
                        " and InvoiceDate >= '" + fromDate.ToString("yyyy-MM-dd") + "'" + " and DocumentStatus = 99 and Status = 1 " +
                        "group by DocumentID ";
                }
                else
                {
                    query = "select DocumentID,sum(ProductValueINR) from InvoiceOutHeader" +
                        " where DocumentID in ( 'SERVICEINVOICEOUT','SERVICEEXPORTINVOICEOUT' )" +
                        " and InvoiceDate <= '" + toDate.ToString("yyyy-MM-dd") + "'" +
                        " and InvoiceDate >= '" + fromDate.ToString("yyyy-MM-dd") + "'" + " and DocumentStatus = 99 and Status = 1 " +
                        "group by DocumentID ";
                }

                SqlConnection conn = new SqlConnection(Login.connString);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    rpo = new ReportPO();
                    rpo.DocumentID = reader.GetString(0);
                    rpo.Value = reader.GetDouble(1);
                    if (rpo.DocumentID.Equals("PRODUCTEXPORTINVOICEOUT") || rpo.DocumentID.Equals("PRODUCTINVOICEOUT"))
                        rpo.DocumentType = "Product";
                    else if (rpo.DocumentID.Equals("SERVICEEXPORTINVOICEOUT") || rpo.DocumentID.Equals("SERVICEINVOICEOUT"))
                        rpo.DocumentType = "Service";
                    POList.Add(rpo);
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error querying InvoiceOUt Header Details");
            }
            return POList;
        }
        public static double GetInvoiceOutQuantity(int trackingno, DateTime trackingdate,
            string documentID, string stockitemid, int poitemreferenceno)
        {
            double iqty = 0.0;
            try
            {
                string docString = "";
                string trackNos = "";
                string trackDates = "";
                if (documentID == "POPRODUCTINWARD")
                {
                    docString = "'PRODUCTINVOICEOUT', 'PRODUCTEXPORTINVOICEOUT'";
                }
                if (documentID == "POSERVICEINWARD")
                {
                    docString = "'SERVICEINVOICEOUT', 'SERVICEEXPORTINVOICEOUT'";
                }
                string query = "select Quantity,TrackingNos,TrackingDates from ViewInvoiceOutItemwiseQuantity " +
                    " where documentID in (" + docString + ")" +
                    " and StockItemID =  '" + stockitemid + "'" +
                    " and Status = 1 and DocumentStatus = 99" +
                    " and POItemReferenceNo =  " + poitemreferenceno +
                    " and TrackingNos like '%" + trackingno + ";%'" +
                    " and TrackingDates like '%" + trackingdate.ToString("yyyy-MM-dd") + ";%'";
                SqlConnection conn = new SqlConnection(Login.connString);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    trackNos = reader.GetString(1);
                    trackDates = reader.GetString(2);

                    string[] trackNosStr = trackNos.Split(';');
                    string[] trackDatesStr = trackDates.Split(';');

                    int indexofTrackNo = Array.IndexOf(trackNosStr, trackingno.ToString()); //get index of required track no in string array
                                                                                            //If required Track No is not found in trackNosStr array then index will be -1
                    if (indexofTrackNo != -1)
                    {
                        DateTime TrackDateFound = Convert.ToDateTime(trackDatesStr[indexofTrackNo]);  // get track date from trackDatesStr with track no index
                        DateTime TrackDateRequired = Convert.ToDateTime(trackingdate);

                        if (TrackDateFound == TrackDateRequired)
                        {
                            iqty = iqty + reader.GetDouble(0);
                        }
                    }
                }
                conn.Close();
            }
            catch (Exception ex)
            {
            }
            return iqty;
        }
        public static string getCustIDOfINvoiceOUT(invoiceoutheader ioh)
        {
            string custID = "";
            try
            {
                string query = "select Consignee" +
                     " from InvoiceOutHeader" +
                   " where DocumentID='" + ioh.DocumentID + "'" +
                  " and TemporaryNo=" + ioh.TemporaryNo +
                  " and TemporaryDate='" + ioh.TemporaryDate.ToString("yyyy-MM-dd") + "'";

                SqlConnection conn = new SqlConnection(Login.connString);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    custID = reader.IsDBNull(0) ? "" : reader.GetString(0);
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error querying Invoice Header CustoemrID");
            }
            return custID;
        }
        public static string getInvoiceNos(string docid, int trno, DateTime dt)
        {
            string invNos = "";
            try
            {
                string query = "select invoiceno, invoicedate from ViewInvoiceOutVsPOInSummary" +
                     " where trackingdocumentid='" +docid +"'"+
                     " and TrackingNo="+trno+
                     " and TrackingDate='" + dt.ToString("yyyy-MM-dd") + "'";

                SqlConnection conn = new SqlConnection(Login.connString);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    invNos = invNos+ reader.GetInt32(0)+":"+ reader.GetDateTime(1).ToString("dd-MM-yyyy")+",";
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error querying Invoice Header CustoemrID");
            }
            return invNos;
        }
    }
}
