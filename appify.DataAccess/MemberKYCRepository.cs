using appify.DataAccess.Contract;
using appify.models;
using appify.utility;
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
    public partial class MemberKYCRepository : IMemberKYCRepository
    {

        private IConfiguration configuration;
        private string appify_connectionstring;

        public MemberKYCRepository(IConfiguration config)
        {
            this.configuration = config;
            this.appify_connectionstring = config["ConnectionStrings:appify.connectionstring"].ToString();
        }
        public bool Delete(long memberID)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.DELETEMEMBERKYC))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@MemberID", memberID);
                        


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

        public MemberKYC Get(long memberID)
        {
            MemberKYC item = new MemberKYC();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.SELECTMEMBERKYC, memberID);
            item = DataTableHelper.ConvertDataTable<MemberKYC>(ds.Tables[0]).FirstOrDefault();

            return item;
        }

        public List<MemberKYC> ListAll()
        {
            List<MemberKYC> item = new List<MemberKYC>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LISTMEMBERKYC);
            item = DataTableHelper.ConvertDataTable<MemberKYC>(ds.Tables[0]);

            return item;
        }

        public MemberKYC Save(MemberKYC item)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.SAVEMEMBERKYC))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;


                        cmd.Parameters.AddWithValue("@MemberID", item.MemberID);
                        cmd.Parameters.AddWithValue("@PAN", item.PAN);
                        cmd.Parameters.AddWithValue("@GST", item.GST);
                        cmd.Parameters.AddWithValue("@AadharNo", item.AadharNo);
                        cmd.Parameters.AddWithValue("@BankName", item.BankName);
                        cmd.Parameters.AddWithValue("@BankAccountNo", item.BankAccountNo);
                        cmd.Parameters.AddWithValue("@IFSC", item.IFSC);
                        cmd.Parameters.AddWithValue("@BankAccountType", item.BankAccountType);
                        cmd.Parameters.AddWithValue("@ChequeImage", item.ChequeImage);
                        cmd.Parameters.AddWithValue("@PANImage", item.PANImage);
                        cmd.Parameters.AddWithValue("@GSTImage", item.GSTImage);
                        cmd.Parameters.AddWithValue("@AadharImage1", item.AadharImage);
                        cmd.Parameters.AddWithValue("@AadharImage2", item.AadharImage2);
                        cmd.Parameters.AddWithValue("@KVICNo", item.KVICNo);
                        cmd.Parameters.AddWithValue("@Address", item.Address);
                        cmd.Parameters.AddWithValue("@AddressImage", item.AddressImage);



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

            return item;
        }
    }
}
