using CSLERP.DBData;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static iTextSharp.text.Font;
namespace CSLERP.PrintForms
{
    public class PrintPurchaseOrder
    {


        public void PrintPO(poheader poh, List<podetail> PODetail, string taxStr)
        {
            Dictionary<string, string> companyInfo = getCompanyInformation();
            //string stateDetail = 
            customer custDetail = CustomerDB.getCustomerDetailForPO(poh.CustomerID);
            string[] companyBillingAdd = CompanyAddressDB.getCompTopBillingAdd(Login.companyID);
            string poNoSuffix = "";
            if (poh.DocumentID=="POGENERAL")
            {
                poNoSuffix = "G-";
            }
            else if (poh.DocumentID == "PURCHASEORDER")
            {
                poNoSuffix = "P-";
            }
            string supplAdd = "Supplier:\n" + custDetail.name + Main.delimiter1 + "\n" + custDetail.BillingAddress + "\n";
            if (custDetail.StateName.ToString().Length != 0)
                supplAdd = supplAdd + "Sate Name:" + custDetail.StateName;
            if (custDetail.StateCode.ToString().Length != 0)
                supplAdd = supplAdd + "\nState Code:" + custDetail.StateCode;
            if (custDetail.OfficeName.ToString().Length != 0)
                supplAdd = supplAdd + "\nGST:" + custDetail.OfficeName; // For GST Code
            //; : main.delimiter2
            //$ : main.delimiter1
            string InvoiceTo = "Invoice To: \n" + companyBillingAdd[0] + Main.delimiter1 + "\n" + companyBillingAdd[1] + "\nGST:" + companyInfo["GST"] + "\nCIN:" + companyInfo["CIN"] + "\nPAN:" + companyInfo["PAN"];
            string DespatchTo = "Dispatch To:\n" + companyBillingAdd[0] + Main.delimiter1 + "\n" + poh.DeliveryAddress + "\nGST:" + companyInfo["GST"];
            string HeaderString = supplAdd +
                            Main.delimiter2 + "PO No : "+poNoSuffix + poh.PONo + Main.delimiter2 + "Date: " + poh.PODate.ToString("dd-MM-yyyy") + 
                            Main.delimiter2 + "Supplier Ref./Order No.\n" + poh.ReferenceQuotation.Replace(";", ",\n") + 
                            Main.delimiter2 + "Despatch Through:\n" + CatalogueValueDB.getParamValue("TransportationMode", poh.TransportationMode) +
                             Main.delimiter2 + InvoiceTo+
                             Main.delimiter2 + "Freight:\n" + CatalogueValueDB.getParamValue("Freight", poh.FreightTerms) + 
                             Main.delimiter2 + "Delivery Period:\n" + poh.DeliveryPeriod + " Days" + Main.delimiter2 +
                             DespatchTo +
                            Main.delimiter2 + "Tax And Duties:\n" + CatalogueValueDB.getParamValue("TaxStatus", poh.TaxTerms) +
                            Main.delimiter2 + "Mode/Terms of Payment:\n" + CatalogueValueDB.getParamValue("PaymentMode", poh.ModeOfPayment) + "\n" + PTDefinitionDB.getPaymentTermString(poh.PaymentTerms);
            string footer1 = "Amount In Words\n\n";
            string ColHeader = "SI No.;Description of Goods;Quantity;Unit;Unit Rate;Amount;Warranty\nIn Days";
            string footer2 = "";
            string footer3 = "for CELLCOMM SOLUTION LIMITED;Authorised Signatory";
            string termsAndCond = getTCString(poh.TermsAndCondition);
            double totQuant = 0.00;
            double totAmnt = 0.00;
            int n = 1;
            string ColDetailString = "";
            var count = PODetail.Count();
            foreach (podetail pod in PODetail)
            {
                if (n == count)
                {
                    //ColDetailString = ColDetailString + n + "+" + pod.Description + "+" + pod.Quantity + "+"
                    //                  + pod.Unit + "+" + pod.Price + "+" + (pod.Quantity * pod.Price) + "+" + pod.WarrantyDays;
                    ColDetailString = ColDetailString + n + Main.delimiter1 + pod.Description + Main.delimiter1 + pod.Quantity + Main.delimiter1
                                     + pod.Unit + Main.delimiter1 + pod.Price + Main.delimiter1 + (pod.Quantity * pod.Price) + Main.delimiter1 + pod.WarrantyDays;
                    if (pod.Tax != 0)
                    {
                        //ColDetailString = ColDetailString + ";" +"" + "+" + pod.TaxCode + "+" + "" + "+"
                        //              + "" + "+" + "" + "+" + pod.Tax + "+" + "";
                        ColDetailString = ColDetailString + Main.delimiter2 + "" + Main.delimiter1 + pod.TaxCode + Main.delimiter1 + "" + Main.delimiter1
                                      + "" + Main.delimiter1 + "" + Main.delimiter1 + pod.Tax + Main.delimiter1 + "";
                    }
                   // ColDetailString = ColDetailString
                }
                else
                {
                    //ColDetailString = ColDetailString + n + "+" + pod.Description + "+" + pod.Quantity + "+"
                    //                  + pod.Unit + "+" + pod.Price + "+" + (pod.Quantity * pod.Price) + "+" + pod.WarrantyDays + ";";
                    ColDetailString = ColDetailString + n + Main.delimiter1 + pod.Description + Main.delimiter1 + pod.Quantity + Main.delimiter1
                                     + pod.Unit + Main.delimiter1 + pod.Price + Main.delimiter1 + (pod.Quantity * pod.Price) + Main.delimiter1 + pod.WarrantyDays + Main.delimiter2;
                    if (pod.Tax != 0)
                    {
                        //ColDetailString = ColDetailString + "" + "+" + pod.TaxCode + "+" + "" + "+"
                        //             + "" + "+" + "" + "+" + pod.Tax + "+" + "" + Main.delimiter2;
                        ColDetailString = ColDetailString + "" + Main.delimiter1 + pod.TaxCode + Main.delimiter1 + "" + Main.delimiter1
                                    + "" + Main.delimiter1 + "" + Main.delimiter1 + pod.Tax + Main.delimiter1 + "" + Main.delimiter2;
                    }

                }
                totQuant = totQuant + pod.Quantity;
                totAmnt = totAmnt + (pod.Quantity * pod.Price);
                n++;
            }
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = "Save As PDF";
                sfd.Filter = "Pdf files (*.Pdf)|*.pdf";
                sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                
                sfd.FileName = poh.DocumentID + "-" + poh.PONo;

                if (sfd.ShowDialog() == DialogResult.Cancel || sfd.FileName == "")
                {
                    return;
                }
                FileStream fs = new FileStream(sfd.FileName + ".pdf", FileMode.Create, FileAccess.Write);
                Rectangle rec = new Rectangle(PageSize.A4);
                iTextSharp.text.Document doc = new iTextSharp.text.Document(rec);
                PdfWriter writer = PdfWriter.GetInstance(doc, fs);
                MyEvent evnt = new MyEvent();
                writer.PageEvent = evnt;

                doc.Open();
                Font font1 = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font font2 = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font font3 = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.ITALIC, BaseColor.BLACK);
                String URL = "Cellcomm2.JPG";
                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(URL);
                img.Alignment = Element.ALIGN_LEFT;


                PdfPTable tableMain = new PdfPTable(2);

                tableMain.WidthPercentage = 100;
                PdfPCell cellImg = new PdfPCell();
                Paragraph pp = new Paragraph();
                pp.Add(new Chunk(img, 0, 0));
                cellImg.AddElement(pp);
                cellImg.Border = 0;
                tableMain.AddCell(cellImg);

                PdfPCell cellAdd = new PdfPCell();
                Paragraph ourAddr = new Paragraph("");
                CompanyDetailDB compDB = new CompanyDetailDB();
                cmpnydetails det = compDB.getdetails().FirstOrDefault(comp => comp.companyID == 1);
                if (det != null)
                {
                    string addr = det.companyname + "\n" + det.companyAddress;
                    ourAddr = new Paragraph(new Phrase(addr, font2));
                    ourAddr.Alignment = Element.ALIGN_RIGHT;
                }
                cellAdd.AddElement(ourAddr);
                cellAdd.Border = 0;
                tableMain.AddCell(cellAdd);


                Paragraph paragraph = new Paragraph(new Phrase("PURCHASE ORDER", font2));
                paragraph.Alignment = Element.ALIGN_CENTER;

                PrintPurchaseOrder prog = new PrintPurchaseOrder();
                string[] HeaderStr = HeaderString.Split(Main.delimiter2);

                PdfPTable table = new PdfPTable(7);

                table.SpacingBefore = 20f;
                table.WidthPercentage = 100;
                float[] HWidths = new float[] { 0.5f, 8f, 2f, 1.5f, 1.5f, 3f, 1.5f };
                table.SetWidths(HWidths);
                PdfPCell cell;
                int[] arr = { 6, 7, 9, 10 };
                float wid = 0;
                for (int i = 0; i < HeaderStr.Length; i++)
                {
                    if (i == 0 || i == 5 || i == 8)
                    {
                        string[] format = HeaderStr[i].Split(Main.delimiter1);
                        Phrase phr = new Phrase();
                        phr.Add(new Chunk(format[0], font2));
                        phr.Add(new Chunk(format[1], font1));
                        //cell = new PdfPCell(new Phrase(HeaderStr[i].Trim(), font1));
                        cell = new PdfPCell(phr);
                        cell.Rowspan = 2;
                        cell.Colspan = 2;
                        cell.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                        //wid = cell.MinimumHeight / 2;
                        table.AddCell(cell);

                    }
                    else if (arr.Contains(i))
                    {
                        cell = new PdfPCell(new Phrase(HeaderStr[i].Trim(), font1));
                        cell.Colspan = 5;
                        cell.MinimumHeight = wid;
                        table.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Phrase(HeaderStr[i].Trim(), font1));
                        if (i % 2 != 0)
                            cell.Colspan = 3;
                        else
                            cell.Colspan = 2;
                        table.AddCell(cell);
                    }
                }
                string[] ColHeaderStr = ColHeader.Split(';');

                PdfPTable table1 = new PdfPTable(7);
                table1.DefaultCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                table1.WidthPercentage = 100;
                float[] width = new float[] { 0.5f, 8f, 2f, 1.5f, 2f, 2.5f, 1.5f };
                table1.SetWidths(width);

                for (int i = 0; i < ColHeaderStr.Length; i++)
                {
                    if (i == 4 || i == 5)
                    {
                        PdfPCell hcell = new PdfPCell(new Phrase(ColHeaderStr[i].Trim() + "\n(" + poh.CurrencyID + ")", font2));
                        hcell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                        table1.AddCell(hcell);
                    }
                    else
                    {
                        PdfPCell hcell = new PdfPCell(new Phrase(ColHeaderStr[i].Trim(), font2));
                        hcell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                        table1.AddCell(hcell);
                    }
                }
                //---
                PdfPCell foot = new PdfPCell(new Phrase(""));
                foot.Colspan = 7;
                foot.BorderWidthTop = 0;
                foot.MinimumHeight = 0.5f;
                table1.AddCell(foot);

                table1.HeaderRows = 2;
                table1.FooterRows = 1;

                table1.SkipFirstHeader = false;
                table1.SkipLastFooter = true;
                //--- 
                int track = 0;
                decimal dc1 = 0;
                decimal dc2 = 0;

                string[] DetailStr = ColDetailString.Split(Main.delimiter2);
                float hg = 0f;
                for (int i = 0; i < DetailStr.Length; i++)
                {
                    track = 0;
                    hg = table1.GetRowHeight(i + 1);
                    string[] str = DetailStr[i].Split(Main.delimiter1);
                    for (int j = 0; j < str.Length; j++)
                    {
                        PdfPCell pcell;

                        if (j == 2 || j == 4 || j == 5)
                        {
                            decimal p = 1;
                            if (Decimal.TryParse(str[j], out p))
                                pcell = new PdfPCell(new Phrase(String.Format("{0:0.00}", Convert.ToDecimal(str[j])), font1));
                            else
                                pcell = new PdfPCell(new Phrase(""));
                            pcell.Border = 0;
                            if (j == 5)
                            {
                                if (str[0].Length == 0)
                                {
                                    pcell.BorderWidthBottom = 0.01f;
                                    track = 1;
                                    dc2 = Convert.ToDecimal(str[j]);
                                }
                                else
                                    dc1 = Convert.ToDecimal(str[j]);
                            }

                        }
                        else
                        {
                            if (j == 6)
                            {
                                if ((str[j].Trim().Length == 0 || Convert.ToInt32(str[j]) == 0) && (track != 1))
                                    pcell = new PdfPCell(new Phrase("NA", font1));
                                else
                                    pcell = new PdfPCell(new Phrase(str[j], font1));

                            }
                            else if (j == 3)
                            {
                                int m = 1;
                                if (Int32.TryParse(str[j], out m) == true)
                                {
                                    if (Convert.ToInt32(str[j]) == 0)
                                        pcell = new PdfPCell(new Phrase("", font1));
                                    else
                                        pcell = new PdfPCell(new Phrase(str[j], font1));
                                }
                                else
                                    pcell = new PdfPCell(new Phrase(str[j], font1));

                            }
                            else
                                pcell = new PdfPCell(new Phrase(str[j], font1));

                            pcell.Border = 0;
                        }

                        //if (i == DetailStr.Length - 1)
                        //{
                        //    pcell.MinimumHeight = 50;
                        //}
                        //else
                        pcell.MinimumHeight = 10;
                        //pcell.MinimumHeight = 20;
                        if (j == 1)
                            pcell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                        else
                            pcell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                        pcell.BorderWidthLeft = 0.01f;
                        pcell.BorderWidthRight = 0.01f;

                        table1.AddCell(pcell);

                    }
                    //foreach()
                    if (track == 1)
                    {
                        for (int j = 0; j < 7; j++)
                        {
                            PdfPCell pcell1;

                            if (j == 5)
                            {
                                pcell1 = new PdfPCell(new Phrase(String.Format("{0:0.00}", Convert.ToDecimal(dc1 + dc2)), font1));
                                pcell1.Border = 0;
                                pcell1.BorderWidthBottom = 0.01f;
                            }
                            else
                            {
                                pcell1 = new PdfPCell(new Phrase(""));
                                pcell1.Border = 0;
                            }
                            pcell1.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                            pcell1.BorderWidthLeft = 0.01f;
                            pcell1.BorderWidthRight = 0.01f;
                            table1.AddCell(pcell1);
                        }
                    }
                }

                double roundedAmt = Math.Round(poh.POValue, 0);
                double diffAmount = roundedAmt - poh.POValue;

                if (diffAmount != 0)
                {
                    table1.AddCell("");
                    table1.AddCell("");
                    PdfPCell cellRound = new PdfPCell(new Phrase("Round off Adj.", font1));
                    cellRound.Colspan = 3;
                    cellRound.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    table1.AddCell(cellRound);
                    table1.AddCell(new Phrase(String.Format("{0:0.00}", diffAmount), font1));
                    table1.AddCell(""); 
                }

                table1.AddCell("");
                table1.AddCell("");
                PdfPCell cellTotal = new PdfPCell(new Phrase("Total", font1));
                cellTotal.Colspan = 3;
                cellTotal.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                table1.AddCell(cellTotal);
                table1.AddCell(new Phrase(String.Format("{0:0.00}", roundedAmt), font1));
                table1.AddCell("");

                string total = footer1 + NumberToString.convert(roundedAmt.ToString()).Replace("INR", poh.CurrencyID) + "\n\n";
                PdfPCell fcell1 = new PdfPCell(new Phrase((total), font1));
                fcell1.Colspan = 6;
                fcell1.MinimumHeight = 50;
                fcell1.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                fcell1.BorderWidthBottom = 0;
                fcell1.BorderWidthRight = 0;
                fcell1.BorderWidthTop = 0;
                table1.AddCell(fcell1);

                PdfPCell fcell4 = new PdfPCell(new Phrase("E. & O.E", font1));
                //fcell4.MinimumHeight = 50;
                fcell4.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                fcell4.BorderWidthBottom = 0;
                //fcell4.BorderWidthRight = 0;
                fcell4.BorderWidthLeft = 0;
                fcell4.BorderWidthTop = 0;
                table1.AddCell(fcell4);

                if (poh.SpecialNote.Trim().Length != 0)
                {
                    footer2 = "Note:\n" + poh.SpecialNote.Trim();
                }
                PdfPCell fcell2 = new PdfPCell(new Phrase(footer2, font1));
                fcell2.Colspan = 4;
                fcell2.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                fcell2.BorderWidthTop = 0;
                fcell2.BorderWidthRight = 0;
                table1.AddCell(fcell2);
                string[] ft = footer3.Split(';');

                PdfPCell fcell3 = new PdfPCell();
                Chunk ch1 = new Chunk(ft[0], font1);
                Chunk ch2 = new Chunk(ft[1], font1);
                Phrase phrase = new Phrase();
                phrase.Add(ch1);
                for (int i = 0; i < 3; i++)
                    phrase.Add(Chunk.NEWLINE);
                phrase.Add(ch2);

                Paragraph para = new Paragraph();
                para.Add(phrase);
                para.Alignment = Element.ALIGN_RIGHT;
                fcell3.AddElement(para);
                fcell3.Border = 0;
                fcell3.Colspan = 4;
                fcell3.BorderWidthTop = 0f;
                fcell3.BorderWidthLeft = 0f;
                fcell3.BorderWidthRight = 0.5f;
                fcell3.BorderWidthBottom = 0.5f;
                fcell3.MinimumHeight = 50;
                table1.AddCell(fcell3);
                table1.KeepRowsTogether(table1.Rows.Count - 4, table1.Rows.Count);
                PdfPTable taxTab = new PdfPTable(3);
                //taxTab.
                taxTab.WidthPercentage = 100;
               
                float[] twidth = new float[] {3f, 3f, 10f };
                taxTab.SetWidths(twidth);
                double dd = 0;
                if (poh.TaxAmount != 0)
                {
                    PdfPCell pcell;
                    pcell = new PdfPCell(new Phrase("Tax Details", font2));
                    taxTab.AddCell(pcell);
                    PdfPCell pcellc = new PdfPCell(new Phrase("Amount("+poh.CurrencyID+")", font2));
                    taxTab.AddCell(pcellc);
                    PdfPCell pcelllst = new PdfPCell(new Phrase("", font1));
                    pcelllst.Border = 0;
                    taxTab.AddCell(pcelllst);
                    //for (int i = 0; i < 2; i++)
                    //{

                    //    if (i == 1)
                    //        pcell = new PdfPCell(new Phrase("Tax Details", font2));
                    //    else

                    //    //pcell.Border = 0;
                    //    //pcell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    //    //pcell.MinimumHeight = 20;
                    //    //pcell.BorderWidthLeft = 0.01f;
                    //    //pcell.BorderWidthRight = 0.01f;
                    //    table1.AddCell(pcell);
                    //}
                    string[] tax = taxStr.Split(Main.delimiter2);
                    for (int i = 0; i < tax.Length - 1; i++)
                    {
                        string[] subtax = tax[i].Split(Main.delimiter1);
                        PdfPCell pcell1;
                        pcell1 = new PdfPCell(new Phrase(subtax[0], font1));
                        PdfPCell pcell2;
                        pcell2 = new PdfPCell(new Phrase(String.Format("{0:0.00}", Convert.ToDecimal(subtax[1])), font1));
                        PdfPCell pcell3 = new PdfPCell(new Phrase("", font1));
                        pcell3.Border = 0;
                        //taxTab.AddCell(pcell3);
                        //pcell1.Border = 0;
                        //pcell1.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                        //pcell1.BorderWidthLeft = 0.01f;
                        //pcell1.BorderWidthRight = 0.01f;
                        //if (i == (tax.Length - 2))
                        //{
                        //    pcell1.MinimumHeight = 100;
                        //}
                        //else
                        //pcell1.MinimumHeight = 20;
                        taxTab.AddCell(pcell1);
                        //pcell2.MinimumHeight = 20;
                        taxTab.AddCell(pcell2);
                        taxTab.AddCell(pcell3);
                    }
                    taxTab.AddCell(new Phrase("Total Tax Amount",font2));
                    taxTab.AddCell(new Phrase(String.Format("{0:0.00}", Convert.ToDecimal(poh.TaxAmount)), font2));
                    PdfPCell pcellt = new PdfPCell(new Phrase("", font1));
                    pcellt.Border = 0;
                    taxTab.AddCell(pcellt);
                    taxTab.KeepTogether = true;
                    taxTab.SpacingAfter = 2f;
                    taxTab.SpacingBefore = 3f;
                }
                PdfPTable TCTab = new PdfPTable(2);
                if (poh.TermsAndCondition.Trim().Length != 0)
                {
                    Chunk TCchunk = new Chunk("Terms And Conditoins:\n", font2);
                    TCchunk.SetUnderline(0.2f, -2f);
                    TCTab = new PdfPTable(2);
                    TCTab.WidthPercentage = 100;
                    PdfPCell TCCell = new PdfPCell();
                    TCCell.Colspan = 2;
                    TCCell.Border = 0;
                    TCCell.AddElement(TCchunk);
                    TCTab.AddCell(TCCell);
                    try
                    {
                        string[] ParaTC = termsAndCond.Split(Main.delimiter2);
                        for (int i = 0; i < ParaTC.Length - 1; i++)
                        {
                            TCCell = new PdfPCell();
                            TCCell.Colspan = 2;
                            TCCell.Border = 0;
                            Paragraph header = new Paragraph();
                            Paragraph details = new Paragraph();
                            details.IndentationLeft = 12f;
                            details.IndentationRight = 12f;
                            string paraHeaderStr = (i + 1) + ". " + ParaTC[i].Substring(0, ParaTC[i].IndexOf(Main.delimiter1)) + ":";
                            string paraFooterStr = ParaTC[i].Substring(ParaTC[i].IndexOf(Main.delimiter1) + 1);
                            header.Add(new Phrase(paraHeaderStr, font2));
                            details.Add(new Phrase(paraFooterStr, font1));
                            TCCell.AddElement(header);
                            TCCell.AddElement(details);
                            TCTab.AddCell(TCCell);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this.ToString() + "-" + System.Reflection.MethodBase.GetCurrentMethod().Name + "() : Error-" + ex.ToString());
                    }
                    try
                    {
                        if (TCTab.Rows.Count >= 3)
                        {
                            TCTab.KeepRowsTogether(0, 3);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this.ToString() + "-" + System.Reflection.MethodBase.GetCurrentMethod().Name + "() : Error-" + ex.ToString());
                    }
                }
                doc.Add(tableMain);
                //doc.Add(jpg);
                //doc.Add(img);
                doc.Add(paragraph);
                doc.Add(table);
                doc.Add(table1);
                if (poh.TaxAmount != 0)
                    doc.Add(taxTab);
                if (poh.TermsAndCondition.Trim().Length != 0)
                    doc.Add(TCTab);
                doc.Close();
                MessageBox.Show("Document Saved");
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to Save Document");
            }
        }
        //private string getReferenceQuotNo(string quot)
        //{
        //    string str = "";
        //    try
        //    {
        //        if (quot.Length != 0)
        //        {
        //            string[] QuotItem = quot.Split(';');
        //            for (int i = 0; i < QuotItem.Length - 1; i++)
        //            {
        //                //str = str + get.Substring(0, get.IndexOf('('));
        //                if (str.Length == 0)
        //                    str = str + QuotItem[i].Substring(0, QuotItem[i].IndexOf('('));
        //                else
        //                    str = str + "," + QuotItem[i].Substring(0, QuotItem[i].IndexOf('('));
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("getReferenceQuotNo() exception");
        //    }
        //    return str;
        //}
        protected class MyEvent : PdfPageEventHelper
        {

            PdfTemplate total;
            Font font2 = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
            public override void OnOpenDocument(PdfWriter writer, iTextSharp.text.Document document)
            {
                total = writer.DirectContent.CreateTemplate(40, 16);
            }
            public override void OnEndPage(PdfWriter writer, iTextSharp.text.Document document)
            {
                PdfPTable table = new PdfPTable(3);
                try
                {
                    table.SetWidths(new int[] { 20, 5, 20 });
                    table.DefaultCell.FixedHeight = 10;
                    table.DefaultCell.Border = 0;
                    table.DefaultCell.Border = Rectangle.NO_BORDER;
                    PdfPCell cell = new PdfPCell();
                    cell.Border = 0;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.Phrase = new Phrase("");
                    table.AddCell(cell);


                    cell = new PdfPCell();
                    cell.Border = 0;
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.Phrase = new Phrase(String.Format("Page " + document.PageNumber.ToString() + " of"), font2);
                    table.AddCell(cell);
                    Image img = Image.GetInstance(total);
                    string alt = img.Alt;
                    cell = new PdfPCell(Image.GetInstance(total));
                    cell.Border = 0;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    table.TotalWidth = document.PageSize.Width
                            - document.LeftMargin - document.RightMargin;
                    table.WriteSelectedRows(0, -1, document.LeftMargin,
                            document.BottomMargin - 15, writer.DirectContent);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(this.ToString() + "-" + System.Reflection.MethodBase.GetCurrentMethod().Name + "() : Error-" + ex.ToString());
                }
            }
            public override void OnCloseDocument(PdfWriter writer, iTextSharp.text.Document document)
            {
                ColumnText.ShowTextAligned(total, Element.ALIGN_LEFT, new Phrase((writer.CurrentPageNumber - 1).ToString(), font2), 4, 4, 0);
            }
        }
        protected string getTCString(string TC)
        {
            string TCString = "";
            string s;
            string[] str = TC.Trim().Split(new string[] { ";" }, StringSplitOptions.None);
            for (int i = 0; i < str.Length - 1; i++)
            {

                try
                {
                    TCString = TCString + TermsAndConditionsDB.getTCDetailsNew(Convert.ToInt32(str[i]));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this.ToString() + "-" + System.Reflection.MethodBase.GetCurrentMethod().Name + "() : Error-" + ex.ToString());
                }
            }
            return TCString;
        }
        private Dictionary<string, string> getCompanyInformation()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            CompanyDataDB dbrecord = new CompanyDataDB();
            try
            {
                List<cmpnydata> data = dbrecord.getData(Login.companyID.ToString());

                //string[] idArr = { "GSTNO", "CIN", "PAN" };
                foreach (cmpnydata cd in data)
                {
                    if (cd.DataID.Equals("GSTNO"))
                    {
                        dict.Add("GST", cd.DataValue);
                    }
                    else if (cd.DataID.Equals("CIN"))
                    {
                        dict.Add("CIN", cd.DataValue);
                    }
                    else if (cd.DataID.Equals("PAN"))
                    {
                        dict.Add("PAN", cd.DataValue);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("getCompanyInformation() exception");
            }
            return dict;
        }
    }
    //public static class Extensions
    //{
    //    //Creating an Extension method to compare two string using contains with case insensitive
    //    public static bool CaseInsensitiveContains(this string text, string value,
    //        StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase)
    //    {
    //        return text.IndexOf(value, stringComparison) >= 0;
    //    }
    //}

}
