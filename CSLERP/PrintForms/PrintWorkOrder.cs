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
    public class PrintWorkOrder
    {


        public void PrintWO(workorderheader woh, List<workorderdetail> WODetail, string totalTaxDetail)
        {
            string HeaderString = "No.: S-" + woh.WONo + Main.delimiter1 + "Date: " + woh.WODate.ToString("dd-MM-yyyy") + Main.delimiter1 +
                            "Address:\n" + woh.CustomerName + "\n" + woh.POAddress + Main.delimiter1 +
                            "Payment Terms:\n" + PTDefinitionDB.getPaymentTermString(woh.PaymentTerms) + Main.delimiter1 +
                            "Target Date:\n" + woh.TargetDate.ToString("dd-MM-yyyy");
            string footer1 = "Amount Chargeable(In Words)\n\n";
            string ColHeader = "SI No.;Description of Work;Location;Quantity;Rate;Amount";
            string footer2 = "";
            string footer3 = "for CELLCOMM SOLUTION LIMITED;Authorised Signatory";
            string termsAndCond = getTCString(woh.TermsAndCond);
            double totQuant = 0.00;
            double totAmnt = 0.00;
            int n = 1;
            string ColDetailString = "";
            var count = WODetail.Count();

            //+ : main.delimeter1
            //; : main.delimiter2

            foreach (workorderdetail wod in WODetail)
            {
                if (n == count)
                {
                    //ColDetailString = ColDetailString + n + "+" + wod.Description + "+" + wod.WorkLocation + "+" + wod.Quantity + "+"
                    //                   + wod.Price + "+" + (wod.Quantity * wod.Price);
                    ColDetailString = ColDetailString + n + Main.delimiter1 + wod.Description + Main.delimiter1 + wod.WorkLocation + Main.delimiter1 + wod.Quantity + Main.delimiter1
                                       + wod.Price + Main.delimiter1 + (wod.Quantity * wod.Price);
                    if (wod.Tax != 0)
                    {
                        //ColDetailString = ColDetailString + ";" +
                        //    "" + "+" +
                        //    wod.TaxCode + "+" +
                        //    "" + "+" +
                        //    "" + "+" +
                        //    "" + "+"  +
                        //    wod.Tax;
                        ColDetailString = ColDetailString + Main.delimiter2 +
                           "" + Main.delimiter1 +
                           wod.TaxCode + Main.delimiter1 +
                           "" + Main.delimiter1 +
                           "" + Main.delimiter1 +
                           "" + Main.delimiter1 +
                           wod.Tax;
                    }
                }
                else
                {
                    //ColDetailString = ColDetailString + n + "+" + wod.Description + "+" + wod.WorkLocation + "+" + wod.Quantity + "+"
                    //                    + wod.Price + "+" + (wod.Quantity * wod.Price) + ";";
                    ColDetailString = ColDetailString + n + Main.delimiter1 + wod.Description + Main.delimiter1 + wod.WorkLocation + Main.delimiter1 + wod.Quantity + Main.delimiter1
                                       + wod.Price + Main.delimiter1 + (wod.Quantity * wod.Price) + Main.delimiter2;
                    if (wod.Tax != 0)
                    {
                        //ColDetailString = ColDetailString +
                        //    "" + "+" +
                        //    wod.TaxCode + "+" +
                        //    "" + "+" +
                        //    "" + "+" +
                        //    "" + "+" +
                        //    wod.Tax + ";";

                        ColDetailString = ColDetailString +
                           "" + Main.delimiter1 +
                           wod.TaxCode + Main.delimiter1 +
                           "" + Main.delimiter1 +
                           "" + Main.delimiter1 +
                           "" + Main.delimiter1 +
                           wod.Tax + Main.delimiter2;
                    }
                }
                totQuant = totQuant + wod.Quantity;
                totAmnt = totAmnt + (wod.Quantity * wod.Price);
                n++;
            }
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = "Save As PDF";
                sfd.Filter = "Pdf files (*.Pdf)|*.pdf";
                sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                sfd.FileName = woh.DocumentID + "-" + woh.WONo;
                //sfd.ShowDialog();
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
                //String imageURL = @"D:\Smrutiranjan\PurchaseOrder\index.jpg";
                //iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(imageURL);
                String URL = "Cellcomm2.JPG";
                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(URL);
                img.Alignment = Element.ALIGN_LEFT;
                //--

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
                //----
                Paragraph paragraph = new Paragraph(new Phrase("PURCHASE ORDER", font2));
                paragraph.Alignment = Element.ALIGN_CENTER;

                //PrintPurchaseOrder prog = new PrintPurchaseOrder();
                string[] HeaderStr = HeaderString.Split(Main.delimiter1);

                PdfPTable table = new PdfPTable(2);

                table.SpacingBefore = 20f;
                table.WidthPercentage = 100;
                float[] HWidths = new float[] { 4f, 3f };
                table.SetWidths(HWidths);
                PdfPCell cell;
                for (int i = 0; i < HeaderStr.Length; i++)
                {
                    cell = new PdfPCell();
                    string[] subHdr = HeaderStr[i].Split(Main.delimiter1);
                    if (i == 2)
                    {
                        cell = new PdfPCell(new Phrase(HeaderStr[i].Trim(), font1));
                        cell.Rowspan = 2;
                        cell.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                        table.AddCell(cell);
                    }
                    else
                    {
                        table.AddCell(new PdfPCell(new Phrase(HeaderStr[i].Trim(), font1)));
                    }
                }
                string[] ColHeaderStr = ColHeader.Split(';');

                PdfPTable table1 = new PdfPTable(6);
                table1.DefaultCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                table1.WidthPercentage = 100;
                float[] width = new float[] { 0.5f, 4.5f, 2f, 2f, 2f, 3f };
                table1.SetWidths(width);

                for (int i = 0; i < ColHeaderStr.Length; i++)
                {
                    PdfPCell hcell = new PdfPCell(new Phrase(ColHeaderStr[i].Trim(), font2));
                    hcell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    table1.AddCell(hcell);
                }
                //---
                PdfPCell foot = new PdfPCell(new Phrase(""));
                foot.Colspan = 6;
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
                        if (j == 3 || j == 4 || j == 5)
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
                            pcell = new PdfPCell(new Phrase(str[j], font1));
                            pcell.Border = 0;
                        }
                        //pcell.Border = 0;
                        //if (i == (DetailStr.Length - 1))
                        //{
                        //    pcell.MinimumHeight = 100;
                        //}
                        //else
                        pcell.MinimumHeight = 10;
                        pcell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                        pcell.BorderWidthLeft = 0.01f;
                        pcell.BorderWidthRight = 0.01f;
                        table1.AddCell(pcell);
                    }

                    if (track == 1)
                    {
                        for (int j = 0; j < 6; j++)
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
                double roundedAmt = Math.Round(woh.TotalAmount, 0);
                double diffAmount = roundedAmt - woh.TotalAmount;
                if (diffAmount != 0)
                {
                    table1.AddCell("");
                    table1.AddCell("");
                    PdfPCell cellRound = new PdfPCell(new Phrase("Round off Adj.", font1));
                    cellRound.Colspan = 3;
                    cellRound.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    table1.AddCell(cellRound);
                    table1.AddCell(new Phrase(String.Format("{0:0.00}", diffAmount), font1));
                }
                table1.AddCell("");
                table1.AddCell("");
                PdfPCell cellTotal = new PdfPCell(new Phrase("Total", font1));
                cellTotal.Colspan = 3;
                cellTotal.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                table1.AddCell(cellTotal);
                table1.AddCell(new Phrase(String.Format("{0:0.00}", roundedAmt), font1));

                //-----
                string total = footer1 + NumberToString.convert(roundedAmt.ToString());
                PdfPCell fcell1 = new PdfPCell(new Phrase((total), font3));
                fcell1.Colspan = 6;
                fcell1.MinimumHeight = 50;
                fcell1.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                fcell1.BorderWidthBottom = 0;
                table1.AddCell(fcell1);

                if (woh.SpecialNote.Trim().Length != 0)
                {
                    footer2 = "Note:\n" + woh.SpecialNote.Trim();
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
                fcell3.Colspan = 2;
                //fcell3.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                //fcell3.BorderWidthTop = 0.5f;
                fcell3.BorderWidthRight = 0.5f;
                fcell3.BorderWidthBottom = 0.5f;
                fcell3.MinimumHeight = 50;
                table1.AddCell(fcell3);
                table1.KeepRowsTogether(table1.Rows.Count - 4, table1.Rows.Count);
                //--------------------

                double dd = 0;
                PdfPTable taxTab = new PdfPTable(3);
                taxTab.WidthPercentage = 100;

                float[] twidth = new float[] { 3f, 3f, 10f };
                taxTab.SetWidths(twidth);
                if (woh.TaxAmount != 0)
                {
                    PdfPCell pcell;
                    pcell = new PdfPCell(new Phrase("Tax Details", font2));
                    taxTab.AddCell(pcell);
                    PdfPCell pcellc = new PdfPCell(new Phrase("Amount(" + woh.CurrencyID + ")", font2));
                    taxTab.AddCell(pcellc);
                    PdfPCell pcelllst = new PdfPCell(new Phrase("", font1));
                    pcelllst.Border = 0;
                    taxTab.AddCell(pcelllst);

                    string[] tax = totalTaxDetail.Split('\n');
                    for (int i = 0; i < tax.Length - 1; i++)
                    {

                        string[] subtax = tax[i].Split('-');
                        PdfPCell pcell1;
                        pcell1 = new PdfPCell(new Phrase(subtax[0], font1));
                        PdfPCell pcell2;
                        pcell2 = new PdfPCell(new Phrase(String.Format("{0:0.00}", Convert.ToDecimal(subtax[1])), font1));
                        PdfPCell pcell3 = new PdfPCell(new Phrase("", font1));
                        pcell3.Border = 0;
                        taxTab.AddCell(pcell1);
                        taxTab.AddCell(pcell2);
                        taxTab.AddCell(pcell3);
                    }
                    taxTab.AddCell(new Phrase("Total Tax Amount", font2));
                    taxTab.AddCell(new Phrase(String.Format("{0:0.00}", Convert.ToDecimal(woh.TaxAmount)), font2));
                    PdfPCell pcellt = new PdfPCell(new Phrase("", font1));
                    pcellt.Border = 0;
                    taxTab.AddCell(pcellt);
                    taxTab.KeepTogether = true;
                    taxTab.SpacingAfter = 2f;
                    taxTab.SpacingBefore = 3f;
                }

                //--------------------
                PdfPTable TCTab = new PdfPTable(2);
                if (woh.TermsAndCond.Trim().Length != 0)
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
                        for (int i = 0; i < ParaTC.Length + 1; i++)
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
                    }
                    try
                    {
                        TCTab.KeepRowsTogether(0, 3);
                    }
                    catch (Exception ex)
                    {
                    }
                }
                doc.Add(tableMain);
                //doc.Add(img);
                //doc.Add(ourAddr);
                doc.Add(paragraph);
                doc.Add(table);
                doc.Add(table1);
                if (woh.TaxAmount != 0)
                    doc.Add(taxTab);
                if (woh.TermsAndCond.Length != 0)
                    doc.Add(TCTab);
                doc.Close();
                MessageBox.Show("Saved Sucessfully.");
            }
            catch (Exception ie)
            {
                MessageBox.Show("Failed TO Save");
            }
        }
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
                catch (DocumentException de)
                {
                    MessageBox.Show("Error found in Purchase Order details.");
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
                }
            }
            return TCString;
        }
    }
}
