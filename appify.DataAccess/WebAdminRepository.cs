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
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using IP2Location;

namespace appify.DataAccess
{
    public partial class WebAdminRepository : IWebAdminRepository
    {
        private IConfiguration configuration;
        private string appify_connectionstring;

        public WebAdminRepository(IConfiguration config)
        {
            this.configuration = config;
            this.appify_connectionstring = config["ConnectionStrings:appify.connectionstring"].ToString();
        }

        public User RegisterUser(User user)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.SAVEUSER))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@UserID", user.UserID);
                        cmd.Parameters.AddWithValue("@UserName", user.UserName);
                        cmd.Parameters.AddWithValue("@Password", user.Password);
                        cmd.Parameters.AddWithValue("@UserGroup", user.UserGroup);
                        cmd.Parameters.AddWithValue("@UserDesignation", user.UserDesignation);
                        cmd.Parameters.AddWithValue("@EmployeeID", user.EmployeeID);
                        cmd.Parameters.AddWithValue("@ICNo", user.ICNo);
                        cmd.Parameters.AddWithValue("@EmailID", user.EmailID);
                        cmd.Parameters.AddWithValue("@ContactNo", user.ContactNo);
                        cmd.Parameters.AddWithValue("@IsActive", user.IsActive);
                        cmd.Parameters.AddWithValue("@IsAllowLogOn", user.IsAllowLogOn);
                        cmd.Parameters.AddWithValue("@IsOperational", user.IsOperational);
                        cmd.Parameters.AddWithValue("@CreatedBy", user.CreatedBy);
                        cmd.Parameters.AddWithValue("@CreatedOn", user.CreatedOn);
                        cmd.Parameters.AddWithValue("@ModifiedBy", user.ModifiedBy);
                        cmd.Parameters.AddWithValue("@ModifiedOn", user.ModifiedOn);
                        cmd.Parameters.AddWithValue("@BranchID", user.BranchID);
                        cmd.Parameters.AddWithValue("@OTPNo", user.OTPNo);
                        cmd.Parameters.AddWithValue("@IsOTPSent", user.IsOTPSent);
                        cmd.Parameters.AddWithValue("@OTPSentDate", user.OTPSentDate);
                        cmd.Parameters.AddWithValue("@IsOTPReSent", user.IsOTPReSent);
                        cmd.Parameters.AddWithValue("@OTPSentCount", user.OTPSentCount);
                        cmd.Parameters.AddWithValue("@IsOTPVerified", user.IsOTPVerified);
                        cmd.Parameters.AddWithValue("@RoleCode", user.RoleCode);


                        //Add the output parameter to the command object
                        SqlParameter outPutParameter = new SqlParameter();
                        outPutParameter.ParameterName = "@NewUserID";
                        outPutParameter.SqlDbType = System.Data.SqlDbType.SmallInt;
                        outPutParameter.Direction = System.Data.ParameterDirection.Output;
                        cmd.Parameters.Add(outPutParameter);

                        con.Open();
                        result = Convert.ToBoolean(cmd.ExecuteNonQuery());

                        if (outPutParameter.Value != null && outPutParameter.Value != "" && outPutParameter.Value != System.DBNull.Value)
                            user.UserID = Convert.ToInt32(outPutParameter.Value);
                        else
                            user.UserID =0;
                        con.Close();
                    }

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return user;
        }

        public bool RemoveUser(Int32 UserID)
        {

            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.DELETEUSER))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@UserID", UserID);

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
        public User GetUser(Int32 userID)
        {
            User user = new User();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.SELECTUSER, userID);
            user = DataTableHelper.ConvertDataTable<User>(ds.Tables[0]).FirstOrDefault();

            return user;
        }
        public bool CheckUser(string userID)
        {
            var user = false;
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.CHECKDUSER, userID);
            user = Convert.ToBoolean(ds.Tables[0].Rows[0][0].ToString());

            return user;
        }
        public User LogIn(string emailID, string password)
        {

            try
            {
                User user = new User();
                DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LOGINUSER, emailID, password);
                user = DataTableHelper.ConvertDataTable<User>(ds.Tables[0]).FirstOrDefault();

                return user;
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public bool UserLogOut(long userID)
        {
            var result = false;

            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.LOGOUTUSER))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.Add(new SqlParameter("@UserID", userID).Direction = ParameterDirection.Input);
                        con.Open();

                        result = Convert.ToBoolean(cmd.ExecuteNonQuery());

                        con.Close();
                    }
                }

                return result;

            }
            catch (Exception ex)
            {

                throw ex;
            }


        }
        public bool ResetPassword(long userID, string password)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.UPDATEPASSWORD))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@UserID", userID);
                        cmd.Parameters.AddWithValue("@Password", password);

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
