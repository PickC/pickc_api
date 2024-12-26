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
    public partial class VendorPaymentRepository : IVendorPaymentRepository
    {
        private IConfiguration configuration;
        private string appify_connectionstring;
        public VendorPaymentRepository(IConfiguration configuration) { 
            this.configuration = configuration;
            this.appify_connectionstring = configuration["ConnectionStrings:appify.connectionstring"].ToString();
        }
        public VendorPayment SaveVendorPayment(VendorPayment item)
        {
            var result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.SAVEVENDORPAYMENT))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@PaymentID", item.PaymentID);
                        cmd.Parameters.AddWithValue("@VendorID", item.VendorID);
                        cmd.Parameters.AddWithValue("@SubscriptionType", item.SubscriptionType);
                        cmd.Parameters.AddWithValue("@PaymentAmount", item.PaymentAmount);
                        cmd.Parameters.AddWithValue("@TaxAmount", item.TaxAmount);
                        cmd.Parameters.AddWithValue("@TotalAmount", item.TotalAmount);
                        cmd.Parameters.AddWithValue("@ReferenceNo", item.ReferenceNo);
                        cmd.Parameters.AddWithValue("@PaymentStatus", item.PaymentStatus);
                        cmd.Parameters.AddWithValue("@ReceiptNo", item.receipt);
                        cmd.Parameters.AddWithValue("@PaymentSignature", item.PaymentSignature);

                        SqlParameter outPutParameter = new SqlParameter();
                        outPutParameter.ParameterName = "@NewPaymentID";
                        outPutParameter.SqlDbType = System.Data.SqlDbType.BigInt;
                        outPutParameter.Direction = System.Data.ParameterDirection.Output;
                        cmd.Parameters.Add(outPutParameter);

                        con.Open();
                        result = Convert.ToBoolean(cmd.ExecuteNonQuery());


                        item.PaymentID = Convert.ToInt64(outPutParameter.Value);

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
        public bool RemoveVendorPayment(Int64 PaymentID)
        {
            var result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.REMOVEVENDORPAYMENT))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@PaymentID", PaymentID);

                        con.Open();
                        result = Convert.ToBoolean(cmd.ExecuteNonQuery());

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
        public VendorPayment Get(Int64 PaymentID)
        {
            VendorPayment item = new VendorPayment();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.SELECTVENDORPAYMENT, PaymentID);
            item = DataTableHelper.ConvertDataTable<VendorPayment>(ds.Tables[0]).FirstOrDefault();

            return item;
        }
        public List<VendorPayment> GetAll()
        {
            List<VendorPayment> payments = new List<VendorPayment>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LISTALLVENDORPAYMENT);
            payments = DataTableHelper.ConvertDataTable<VendorPayment>(ds.Tables[0]);

            return payments;
        }
        public List<VendorPayment> PaymentListbyRows(int pageNo, int rows)
        {
            List<VendorPayment> payments = new List<VendorPayment>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LISTALLROWSVENDORPAYMENT, pageNo, rows);
            payments = DataTableHelper.ConvertDataTable<VendorPayment>(ds.Tables[0]);

            return payments;
        }
        public VendorPayment GetPaymentStatus(Int64 VendorID)
        {
            VendorPayment item = new VendorPayment();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.VENDORPAYMENTSTATUS, VendorID);
            item = DataTableHelper.ConvertDataTable<VendorPayment>(ds.Tables[0]).FirstOrDefault();

            return item;
        }
        public bool UpdateReferenceNo(VendorPaymentStatus item)
        {
            var result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.UPDATEVENDORPAYMENTREFERENCENO))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        
                        cmd.Parameters.AddWithValue("@PaymentID", item.PaymentID);
                        cmd.Parameters.AddWithValue("@VendorID", item.VendorID);
                        cmd.Parameters.AddWithValue("@ReferenceNo", item.ReferenceNo);
                        cmd.Parameters.AddWithValue("@PaymentSignature", item.PaymentSignature);
                        con.Open();
                        result = Convert.ToBoolean(cmd.ExecuteNonQuery());

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
        public List<VendorPayment> ListByVendor(Int64 VendorID) 
        {
            List<VendorPayment> payments = new List<VendorPayment>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LISTALLBYVENDOR, VendorID);
            payments = DataTableHelper.ConvertDataTable<VendorPayment>(ds.Tables[0]);

            return payments;

        }
    }
}
