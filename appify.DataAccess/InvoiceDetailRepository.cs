using appify.DataAccess.Contract;
using appify.models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.DataAccess
{
    public class InvoiceDetailRepository : IInvoiceDetailRepository
    {

        private IConfiguration configuration;
        private string appify_connectionstring;


        public InvoiceDetailRepository(IConfiguration config)
        {
            this.configuration = config;
            this.appify_connectionstring = config["ConnectionStrings:appify.connectionstring"].ToString();
        }

        public InvoiceDetail Get(short itemID, long invoiceID)
        {
            throw new NotImplementedException();
        }

        public List<InvoiceDetail> GetAll(long invoiceID)
        {
            throw new NotImplementedException();
        }

        public void Remove(short itemID, long invoiceID)
        {
            throw new NotImplementedException();
        }

        public bool Save(InvoiceDetail item)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.SAVEINVOICEDETAIL))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                        cmd.Parameters.AddWithValue("@ItemID", item.ItemID);
                        cmd.Parameters.AddWithValue("@InvoiceID", item.InvoiceID);
                        cmd.Parameters.AddWithValue("@ProductID", item.ProductID);
                        cmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                        cmd.Parameters.AddWithValue("@UnitPrice", item.UnitPrice);
                        cmd.Parameters.AddWithValue("@DiscountType", item.DiscountType);
                        cmd.Parameters.AddWithValue("@DiscountAmount", item.DiscountAmount);
                        cmd.Parameters.AddWithValue("@CGST", item.CGST);
                        cmd.Parameters.AddWithValue("@SGST", item.SGST);
                        cmd.Parameters.AddWithValue("@IGST", item.IGST);
                        cmd.Parameters.AddWithValue("@TaxAmount", item.TaxAmount);
                        cmd.Parameters.AddWithValue("@SellingAmount", item.SellingAmount);
                        cmd.Parameters.AddWithValue("@SellingPrice", item.SellingPrice);

                        con.Open();
                        result = Convert.ToBoolean(cmd.ExecuteNonQuery());

                        //item.OrderID = Convert.ToInt64(outPutParameter.Value);

                        con.Close();
                    }

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return result;
        }
    }
}
