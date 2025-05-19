using PdfSharp.Drawing;
using PdfSharp.Pdf;


namespace appify.utility
{
    internal class GeneratePDF
    {
    }

    public partial class GenerateVendorSOA
    {


        public async Task<VendorData> GetVendorData(long vendorID)
        {
            // Replace with your actual data access code
            // This is just sample data matching your example
            return await Task.FromResult(new VendorData
            {
                VendorId = vendorID,
                VendorName = "M R P Stores",
                Address = "1292,Sadashiv peth SHOP NO 3\nSneha chimnya ganpati chowk,Pune\nPune,Maharastra",
                Email = "support@themrpstore.com",
                ReportPeriod = "27 April 2025 - 3 May 2025",
                SettlementDate = "5 May 2025",
                PaymentReference = "PAY987654",
                AccountNumber = "XXXX1234",
                Orders = new List<OrderData>
            {
                new OrderData {
                    OrderId = "OD29882504029",
                    Date = "2025-04-29",
                    Status = "Delivered",
                    OrderPrice = 500.00m,
                    DeliveryCharges = 170.00m,
                    TransactionCharges = 13.4m,
                    Commission = 67.00m,
                    GstOnCommission = 12.06m,
                    Tcs = 0.12m
                },
                new OrderData {
                    OrderId = "OD29882504027",
                    Date = "2025-04-28",
                    Status = "Delivered",
                    OrderPrice = 1200.00m,
                    DeliveryCharges = 116.00m,
                    TransactionCharges = 26.32m,
                    Commission = 131.60m,
                    GstOnCommission = 23.68m,
                    Tcs = 0.23m
                },
                new OrderData {
                    OrderId = "OD29882504025",
                    Date = "2025-04-26",
                    Status = "RTO",
                    OrderPrice = 2000.00m,
                    DeliveryCharges = 160.00m
                },
                new OrderData {
                    OrderId = "OD29882504023",
                    Date = "2025-04-25",
                    Status = "Delivered",
                    OrderPrice = 800.00m,
                    DeliveryCharges = 132.00m,
                    TransactionCharges = 18.64m,
                    Commission = 93.20m,
                    GstOnCommission = 16.78m,
                    Tcs = 0.16m
                }
            }
            });
        }

        public  byte[] GeneratePdfStatement(VendorData vendorData)
        {
            PdfDocument document = new PdfDocument();
            document.Info.Title = $"Vendor Statement - {vendorData.VendorName}";

            // Page 1 - Header and Summary
            PdfPage page1 = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page1);
            double yPos = 50;

            // Draw header
            DrawHeader(gfx, page1, vendorData, ref yPos);

            // Draw order summary
            DrawOrderSummary(gfx, vendorData, ref yPos);

            // Page 2 - Order Details
            PdfPage page2 = document.AddPage();
            gfx = XGraphics.FromPdfPage(page2);
            yPos = 50;

            // Draw order details
            DrawOrderDetails(gfx, vendorData, ref yPos);

            // Draw settlement summary
            DrawSettlementSummary(gfx, vendorData, ref yPos);

            // Draw notes
            DrawNotes(gfx, ref yPos);

            // Save to memory stream
            using (MemoryStream ms = new MemoryStream())
            {
                document.Save(ms, false);
                return ms.ToArray();
            }
        }

        private void DrawHeader(XGraphics gfx, PdfPage page, VendorData vendorData, ref double yPos)
        {
            XFont fontTitle = new ("Arial", 10,XFontStyleEx.Bold);
            XFont fontRegular = new XFont("Arial", 10);

            // Company header
            gfx.DrawString("APPIFYRETAIL Pvt Ltd", fontTitle, XBrushes.Black,
                          new XRect(0, yPos, page.Width, 20), XStringFormats.TopCenter);
            yPos += 25;

            gfx.DrawString("Statement of Account", new XFont("Arial", 14, XFontStyleEx.Bold), XBrushes.Black,
                          new XRect(0, yPos, page.Width, 20), XStringFormats.TopCenter);
            yPos += 20;

            gfx.DrawString("SYY11, Wework Kirshe Emerald, GSTIN:29AABBCCDD11E125", fontRegular, XBrushes.Black,
                          new XRect(0, yPos, page.Width, 20), XStringFormats.TopCenter);
            yPos += 30;

            // Vendor info
            gfx.DrawString(vendorData.VendorName, new XFont("Arial", 12, XFontStyleEx.Bold),
                          XBrushes.Black, 50, yPos);
            yPos += 15;

            foreach (var line in vendorData.Address.Split('\n'))
            {
                gfx.DrawString(line, fontRegular, XBrushes.Black, 50, yPos);
                yPos += 15;
            }

            gfx.DrawString($"Email: {vendorData.Email}", fontRegular, XBrushes.Black, 50, yPos);
            yPos += 25;

            // Report info
            gfx.DrawString($"Report Period: {vendorData.ReportPeriod}", fontRegular, XBrushes.Black, 50, yPos);
            yPos += 15;
            gfx.DrawString($"Generated On: {DateTime.Now:dd MMMM yyyy}", fontRegular, XBrushes.Black, 50, yPos);
            yPos += 30;
        }

        private void DrawOrderSummary(XGraphics gfx, VendorData vendorData, ref double yPos)
        {
            gfx.DrawString("Orders Summary", new XFont("Arial", 12, XFontStyleEx.Bold),
                          XBrushes.Black, 50, yPos);
            yPos += 20;

            // Calculate summary values
            decimal totalOrderValue = 0;
            decimal totalGst = 7788.00m; // This should be calculated from orders in real implementation
            decimal totalCommission = 0;
            decimal totalShipping = 0;
            decimal totalReturns = 0;

            foreach (var order in vendorData.Orders)
            {
                totalOrderValue += order.OrderPrice + order.DeliveryCharges;
                totalCommission += order.Commission;
                totalShipping += order.DeliveryCharges;
                if (order.Status == "RTO") totalReturns += order.OrderPrice + order.DeliveryCharges;
            }

            decimal netPayable = totalOrderValue - totalReturns - totalCommission -
                                (totalCommission * 0.18m) - // GST on commission
                                (totalCommission * 0.18m * 0.01m); // TCS

            var summaryData = new Dictionary<string, string>
        {
            { "Total Orders Processed", vendorData.Orders.Count.ToString() },
            { "Total Order Value", totalOrderValue.ToString("₹#,##0.00") },
            { "Total GST (CGST + SGST)", totalGst.ToString("₹#,##0.00") },
            { "Total Commission", totalCommission.ToString("₹#,##0.00") },
            { "Total Shipping Fees", totalShipping.ToString("₹#,##0.00") },
            { "Total Returns/Refunds", totalReturns.ToString("₹#,##0.00") },
            { "Net Amount Payable", netPayable.ToString("₹#,##0.00") }
        };

            DrawTwoColumnTable(gfx, summaryData, 50, ref yPos, 250);
        }

        private void DrawOrderDetails(XGraphics gfx, VendorData vendorData, ref double yPos)
        {
            gfx.DrawString("Order Details", new XFont("Arial", 12, XFontStyleEx.Bold),
                          XBrushes.Black, 50, yPos);
            yPos += 20;

            var headers = new string[] { "Order ID", "Date", "Status", "Order Price", "Delivery", "Total",
                                   "Txn Charges", "GST/IGST", "Commission", "GST on Comm", "TCS", "Payout" };
            var columnWidths = new double[] { 80, 50, 50, 50, 50, 50, 50, 50, 50, 50, 40, 50 };

            var orders = new List<string[]>();
            foreach (var order in vendorData.Orders)
            {
                decimal totalAmount = order.OrderPrice + order.DeliveryCharges;
                decimal finalPayout = order.Status == "RTO" ? 0 :
                    totalAmount - order.Commission - order.GstOnCommission - order.Tcs;

                orders.Add(new string[] {
                order.OrderId,
                order.Date,
                order.Status,
                order.OrderPrice.ToString("#,##0.00"),
                order.DeliveryCharges.ToString("#,##0.00"),
                totalAmount.ToString("#,##0.00"),
                order.TransactionCharges.ToString("0.00"),
                "0.00", // GST/IGST
                order.Commission.ToString("0.00"),
                order.GstOnCommission.ToString("0.00"),
                order.Tcs.ToString("0.00"),
                finalPayout.ToString("#,##0.00")
            });
            }

            DrawMultiColumnTable(gfx, headers, orders, columnWidths, 50, ref yPos);
        }

        private void DrawSettlementSummary(XGraphics gfx, VendorData vendorData, ref double yPos)
        {
            yPos += 20;
            gfx.DrawString("Settlement Summary", new XFont("Arial", 12, XFontStyleEx.Bold),
                          XBrushes.Black, 50, yPos);
            yPos += 20;

            var settlementData = new Dictionary<string, string>
        {
            { "Amount Settled", "₹3,514.72" }, // Should be calculated
            { "Settlement Date", vendorData.SettlementDate },
            { "Payment Reference", vendorData.PaymentReference },
            { "Status", $"Settled (Bank Transfer to {vendorData.AccountNumber})" }
        };

            DrawTwoColumnTable(gfx, settlementData, 50, ref yPos, 250);
        }

        private void DrawNotes(XGraphics gfx, ref double yPos)
        {
            yPos += 30;
            gfx.DrawString("Notes:", new XFont("Arial", 10, XFontStyleEx.Bold), XBrushes.Black, 50, yPos);
            yPos += 15;

            string[] notes = {
            "• Total Order Value includes 18% GST (9% CGST + 9% SGST).",
            "• Transaction charges 2%+18 GST (Razorpay)",
            "• Commission is 10% of the base price (excluding GST).",
            "• GST on Appify Commission 18%",
            "• TCS on GST 1%",
            "• Shipping fee is min. ₹50 per order.",
            "• Returns/Refunds deduct the full order value (e.g., OD29882504025).",
            "• Note: This is an auto-generated report. For discrepancies, contact support@appi-ly.al within 7 days."
        };

            foreach (var note in notes)
            {
                gfx.DrawString(note, new XFont("Arial", 9), XBrushes.Black, 50, yPos);
                yPos += 15;
            }
        }

        private void DrawTwoColumnTable(XGraphics gfx, Dictionary<string, string> data, double x, ref double y, double firstColWidth)
        {
            double secondColWidth = 100;
            double rowHeight = 20;
            XFont fontRegular = new XFont("Arial", 9);
            XFont fontBold = new XFont("Arial", 9, XFontStyleEx.Bold);

            foreach (var item in data)
            {
                // Draw left column (description)
                gfx.DrawRectangle(XPens.LightGray, x, y, firstColWidth, rowHeight);
                gfx.DrawString(item.Key, fontBold, XBrushes.Black, x + 5, y + 14);

                // Draw right column (value)
                gfx.DrawRectangle(XPens.LightGray, x + firstColWidth, y, secondColWidth, rowHeight);
                gfx.DrawString(item.Value, fontRegular, XBrushes.Black,
                              x + firstColWidth + secondColWidth - 5 - gfx.MeasureString(item.Value, fontRegular).Width,
                              y + 14);

                y += rowHeight;
            }
        }

        private void DrawMultiColumnTable(XGraphics gfx, string[] headers, List<string[]> rows, double[] columnWidths, double x, ref double y)
        {
            double rowHeight = 20;
            XFont fontRegular = new XFont("Arial", 8);
            XFont fontHeader = new XFont("Arial", 8, XFontStyleEx.Bold);

            // Draw headers
            double currentX = x;
            for (int i = 0; i < headers.Length; i++)
            {
                gfx.DrawRectangle(XPens.LightGray, currentX, y, columnWidths[i], rowHeight);
                gfx.DrawString(headers[i], fontHeader, XBrushes.Black, currentX + 5, y + 14);
                currentX += columnWidths[i];
            }
            y += rowHeight;

            // Draw rows
            foreach (var row in rows)
            {
                currentX = x;
                for (int i = 0; i < row.Length; i++)
                {
                    gfx.DrawRectangle(XPens.LightGray, currentX, y, columnWidths[i], rowHeight);
                    gfx.DrawString(row[i], fontRegular, XBrushes.Black, currentX + 5, y + 14);
                    currentX += columnWidths[i];
                }
                y += rowHeight;
            }
        }

    }


    // Data model classes
    public class VendorData
    {
        public long VendorId { get; set; }
        public string VendorName { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public List<OrderData> Orders { get; set; }
        public string ReportPeriod { get; set; }
        public string SettlementDate { get; set; }
        public string PaymentReference { get; set; }
        public string AccountNumber { get; set; }
    }

    public class OrderData
    {
        public string OrderId { get; set; }
        public string Date { get; set; }
        public string Status { get; set; }
        public decimal OrderPrice { get; set; }
        public decimal DeliveryCharges { get; set; }
        public decimal TransactionCharges { get; set; }
        public decimal Commission { get; set; }
        public decimal GstOnCommission { get; set; }
        public decimal Tcs { get; set; }
    }
}
