/*
 * Company: AppifyRetail.
 * Author: Gurjeet
 * Version: 1.1
 * Date: 2024-09-01
 * Description:
*/
using appify.DataAccess.Contract;
using appify.models;
using appify.utility;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace appify.DataAccess
{
    public class InvoiceHeaderRepository : IInvoiceHeaderRepository
    {

        private IConfiguration configuration;
        private string appify_connectionstring;
        public InvoiceHeaderRepository(IConfiguration config)
        {
            this.configuration = config;
            this.appify_connectionstring = config["ConnectionStrings:appify.connectionstring"].ToString();
        }

        public InvoiceHeader Get(long invoiceID)
        {
            throw new NotImplementedException();
        }

        public List<InvoiceHeader> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Remove(long invoiceID)
        {
            throw new NotImplementedException();
        }

        public InvoiceHeader Save(InvoiceHeader item)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.SAVEINVOICEHEADER))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                        cmd.Parameters.AddWithValue("@InvoiceID", item.InvoiceID);
                        cmd.Parameters.AddWithValue("@InvoiceNo", item.InvoiceNo);
                        cmd.Parameters.AddWithValue("@OrderID", item.OrderID);
                        cmd.Parameters.AddWithValue("@MemberID", item.MemberID);
                        cmd.Parameters.AddWithValue("@SellerID", item.SellerID);
                        cmd.Parameters.AddWithValue("@InvoiceAmount", item.InvoiceAmount);
                        cmd.Parameters.AddWithValue("@TaxAmount", item.TaxAmount);
                        cmd.Parameters.AddWithValue("@TotalAmount", item.TotalAmount);
                         


                        //Add the output parameter to the command object
                        SqlParameter outPutParameter = new SqlParameter();
                        outPutParameter.ParameterName = "@NewInvoiceID";
                        outPutParameter.SqlDbType = System.Data.SqlDbType.BigInt;
                        outPutParameter.Direction = System.Data.ParameterDirection.Output;
                        cmd.Parameters.Add(outPutParameter);

                        con.Open();
                        result = Convert.ToBoolean(cmd.ExecuteNonQuery());

                        item.InvoiceID = Convert.ToInt64(outPutParameter.Value);

                        con.Close();
                    }

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return item;
        }

        public InvoiceReport PrintInvoice(Int64 orderID)
        {

            InvoiceReport invoiceReport = new InvoiceReport();


            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.PRINTINVOICEHEADER, orderID);
            invoiceReport = DataTableHelper.ConvertDataTable<InvoiceReport>(ds.Tables[0]).FirstOrDefault();


            List<InvoiceItemReport> items = new List<InvoiceItemReport>();
            DataSet dsitem = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.PRINTINVOICEDETAIL, orderID);
            items = DataTableHelper.ConvertDataTable<InvoiceItemReport>(dsitem.Tables[0]);

            //decimal deliveryCharges = Math.Round(Convert.ToDecimal((invoiceReport.DeliveryCost * 100) / (100 + 18)), 2, MidpointRounding.AwayFromZero);

            /* Add Delivery as Line Item */
            //items.Add(new InvoiceItemReport
            //{
            //    ProductID ="0",
            //    ProductName="Shipping Charges",
            //    Description = "Shipping Charges",
            //    UnitPrice = deliveryCharges ,
            //    Quantity = 1,
            //    CGST = 0.00M,
            //    SGST=0.00M,
            //    IGST= Math.Round(Convert.ToDecimal((invoiceReport.DeliveryCost * 18)/100),2,MidpointRounding.AwayFromZero),
            //    SellingAmount = Math.Round(invoiceReport.DeliveryCost,2,MidpointRounding.AwayFromZero)

            //});


            invoiceReport.InvoiceItems.AddRange(items);


            
            

            return invoiceReport;
        }
        public ReceiptReport PrintReceipt(Int64 vendorID)
        {
            ReceiptReport invoiceReport = new ReceiptReport();

            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.PRINTVENDORRECEIPT, vendorID);
            invoiceReport = DataTableHelper.ConvertDataTable<ReceiptReport>(ds.Tables[0]).FirstOrDefault();
            return invoiceReport;
        }
    }
}
