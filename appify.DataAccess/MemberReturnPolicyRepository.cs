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
    public partial class MemberReturnPolicyRepository : IMemberReturnPolicyRepository
    {
        private IConfiguration configuration;
        private string appify_connectionstring;

        public MemberReturnPolicyRepository(IConfiguration config)
        {
            this.configuration = config;
            this.appify_connectionstring = config["ConnectionStrings:appify.connectionstring"].ToString();
        }
        public MemberReturnPolicy GetItem(long memberID)
        {
            MemberReturnPolicy item = new MemberReturnPolicy();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.SELECTMEMBERRETURNPOLICY, memberID);
            item = DataTableHelper.ConvertDataTable<MemberReturnPolicy>(ds.Tables[0]).FirstOrDefault();

            return item;
        }

        public List<MemberReturnPolicy> GetList()
        {
            throw new NotImplementedException();
        }

        public bool Remove(long memberID)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.DELETEMEMBERRETURNPOLICY))
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

        public bool Save(MemberReturnPolicy item)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.SAVEMEMBERRETURNPOLICY))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@MemberID", item.MemberID);
                        cmd.Parameters.AddWithValue("@MaxReturnDays", item.MaxReturnDays);
                        cmd.Parameters.AddWithValue("@IsProductDamaged", item.IsProductDamaged);
                        cmd.Parameters.AddWithValue("@IsWrongSize", item.IsWrongSize);
                        cmd.Parameters.AddWithValue("@InCompatible", item.InCompatible);
                        cmd.Parameters.AddWithValue("@IsDeliveryDelay", item.IsDeliveryDelay);
                        cmd.Parameters.AddWithValue("@IsQualityIssue", item.IsQualityIssue);
                        cmd.Parameters.AddWithValue("@IsDifferentProduct", item.IsDifferentProduct);
                        cmd.Parameters.AddWithValue("@IsNotNeeded", item.IsNotNeeded);
                        cmd.Parameters.AddWithValue("@IsOthers", item.IsOthers);
                        cmd.Parameters.AddWithValue("@IsImage", item.IsImage);
                        cmd.Parameters.AddWithValue("@IsVideo", item.IsVideo);
                        cmd.Parameters.AddWithValue("@Remarks", item.Remarks);

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
    }
}
