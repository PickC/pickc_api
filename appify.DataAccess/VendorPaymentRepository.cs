using appify.DataAccess.Contract;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using appify.models;
using appify.utility;
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
        public bool SaveVendorPayment(VendorPayment item)
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
        public decimal GetPaymentStatus(Int64 VendorID)
        {
            decimal item = new decimal();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.VENDORPAYMENTSTATUS, VendorID);
            item = Convert.ToDecimal(ds.Tables[0].Rows[0][0].ToString());

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
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.VENDORPAYMENTSTATUS, VendorID);
            payments = DataTableHelper.ConvertDataTable<VendorPayment>(ds.Tables[0]);

            return payments;

        }
    }
}
