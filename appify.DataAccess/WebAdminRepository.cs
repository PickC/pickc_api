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
using System.Data.SqlClient;
using IP2Location;
using static appify.models.HomePageProductByCategory;

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
                        cmd.Parameters.AddWithValue("@Department", user.Department);
                        cmd.Parameters.AddWithValue("@UserDesignation", user.UserDesignation);
                        cmd.Parameters.AddWithValue("@EmployeeID", user.EmployeeID);
                        cmd.Parameters.AddWithValue("@EmailID", user.EmailID);
                        cmd.Parameters.AddWithValue("@ContactNo", user.ContactNo);
                        cmd.Parameters.AddWithValue("@IsActive", user.IsActive);
                        cmd.Parameters.AddWithValue("@IsAccepted", user.IsAccepted);
                        cmd.Parameters.AddWithValue("@IsAllowLogOn", user.IsAllowLogOn);
                        cmd.Parameters.AddWithValue("@IsOperational", user.IsOperational);
                        cmd.Parameters.AddWithValue("@CreatedBy", user.CreatedBy);
                        cmd.Parameters.AddWithValue("@CreatedOn", user.CreatedOn);
                        cmd.Parameters.AddWithValue("@ModifiedBy", user.ModifiedBy);
                        cmd.Parameters.AddWithValue("@ModifiedOn", user.ModifiedOn);
                        cmd.Parameters.AddWithValue("@RoleID", user.RoleID);


                        //Add the output parameter to the command object
                        SqlParameter outPutParameter = new SqlParameter();
                        outPutParameter.ParameterName = "@NewUserID";
                        outPutParameter.SqlDbType = System.Data.SqlDbType.SmallInt;
                        outPutParameter.Direction = System.Data.ParameterDirection.Output;
                        cmd.Parameters.Add(outPutParameter);

                        con.Open();
                        result = Convert.ToBoolean(cmd.ExecuteNonQuery());

                        if (outPutParameter.Value != null && outPutParameter.Value != "" && outPutParameter.Value != System.DBNull.Value)
                            user.UserID = Convert.ToInt16(outPutParameter.Value);
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
        public long GetUsersCount()
        {

            Int64 item = new Int64();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.ROWCOUNTUSER);
            item = Convert.ToInt64(ds.Tables[0].Rows[0][0].ToString());

            return item;
        }
        public List<User> ListbyPageView(int pageNo, int rows)
        {
            List<User> item = new List<User>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.PAGEVIEWLISTUSER, pageNo, rows);
            item = DataTableHelper.ConvertDataTable<User>(ds.Tables[0]);

            return item;
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
        public bool ResetPassword(string emailID, string password)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.UPDATEPASSWORDUSER))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@EmailID", emailID);
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
        public List<SellerList> GetSellerList()
        {
            List<SellerList> seller = new List<SellerList>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LISTSELLER);
            seller = DataTableHelper.ConvertDataTable<SellerList>(ds.Tables[0]);

            return seller;
        }
        public List<SellerOrderList> GetSellerOrderList()
        {
            List<SellerOrderList> seller = new List<SellerOrderList>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.SELLERORDERLIST);
            seller = DataTableHelper.ConvertDataTable<SellerOrderList>(ds.Tables[0]);

            return seller;
        }

        public bool SettlementStatusUpdate(long OrderID, bool Status)
        {

            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.SELLETMENTSTATUSUPDATE))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@OrderID", OrderID);
                        cmd.Parameters.AddWithValue("@Status", Status);

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
        public List<ProductMasterByVendor> GetProducts(long userID)
        {
            List<ProductMasterByVendor> items = new List<ProductMasterByVendor>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LISTPRODUCTMASTERNEW, userID);
            items = DataTableHelper.ConvertDataTable<ProductMasterByVendor>(ds.Tables[0]);

            return items;
        }
    }
}
