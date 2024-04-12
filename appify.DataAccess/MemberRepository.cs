using appify.DataAccess.Contract;
using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Data;
using appify.utility;
using appify.dbroutine;
using System.Dynamic;
using Microsoft.Data.SqlClient;

namespace appify.DataAccess
{
    public partial class MemberRepository : IMemberRepository
    {
        private IConfiguration configuration;
        private string appify_connectionstring;

        public MemberRepository(IConfiguration config)
        {
            this.configuration = config;
            this.appify_connectionstring = config["ConnectionStrings:appify.connectionstring"].ToString();
        }

        public bool DeleteMember(long userID)
        {
            throw new NotImplementedException();
        }

        public bool RemoveMemberByMobileNo(string mobileNo, string password) {

            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.DELETEMEMBERBYMOBILENO))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@MobileNo", mobileNo);
                        cmd.Parameters.AddWithValue("@Password", password);
                        cmd.Parameters.AddWithValue("@MemberType", 1001);


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

        public List<Member> GetAllMembers()
        {
            List<Member> members = new List<Member>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LISTMEMBERS);
            members = DataTableHelper.ConvertDataTable<Member>(ds.Tables[0]);

            return members;
        }


        public bool CheckMemberOnlinePaymentStatus(long userID) {

            bool result= false;
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.MEMBERONLINEPAYMENTSTATUS, userID);
            result = Convert.ToBoolean(ds.Tables[0].Rows[0][0].ToString());

            return result;
        }
        public List<Member> GetAllVendors(int pageNo, int rows)
        {
            List<Member> members = new List<Member>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.PAGEVIEWMEMBER,pageNo,rows);
            members = DataTableHelper.ConvertDataTable<Member>(ds.Tables[0]);

            if (members?.Any()==true)
            {
                members = members.Where(m => m.MemberType == (short)MemberType.VENDOR).ToList();
            }

            return members;

        }


 



        public Member GetMember(long userID)
        {
            Member member = new Member();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.SELECTMEMBER, userID);
            member = DataTableHelper.ConvertDataTable<Member>(ds.Tables[0]).FirstOrDefault();

            return member;
        }

        public Int32 MemberOrderCount(long userID) {

            Int32 count=0;
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.ORDERCOUNTBYCUSTOMER, userID);
            count = Convert.ToInt32(ds.Tables[0].Rows[0][0].ToString()); 

            return count;

        }

        public Member IsMemberExist(string emailID, string mobileNo,short memberType,Int64 parentID)
        {
            Member member = new Member();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.CHECKMEMBER, emailID,mobileNo,memberType,parentID);
            member = DataTableHelper.ConvertDataTable<Member>(ds.Tables[0]).FirstOrDefault();

            return member;

        }

        public Member RegisterMember(Member member)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.SAVEMEMBER))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@UserID", member.UserID);
                        cmd.Parameters.AddWithValue("@EmailID", member.EmailID);
                        cmd.Parameters.AddWithValue("@MobileNo", member.MobileNo);
                        cmd.Parameters.AddWithValue("@Password", member.Password);
                        cmd.Parameters.AddWithValue("@FirstName", member.FirstName);
                        cmd.Parameters.AddWithValue("@LastName", member.LastName);
                        cmd.Parameters.AddWithValue("@MemberType", member.MemberType);
                        cmd.Parameters.AddWithValue("@OTP", member.OTP);
                        cmd.Parameters.AddWithValue("@IsOTPSent", member.IsOTPSent);
                        cmd.Parameters.AddWithValue("@OTPSentDate", member.OTPSentDate);
                        cmd.Parameters.AddWithValue("@IsResendOTP", member.IsResendOTP);
                        cmd.Parameters.AddWithValue("@IsOTPVerified", member.IsOTPVerified);
                        cmd.Parameters.AddWithValue("@IsEmailVerified", member.IsEmailVerified);
                        cmd.Parameters.AddWithValue("@ProfilePhoto", member.ProfilePhoto);
                        cmd.Parameters.AddWithValue("@ParentID", member.ParentID);
                        cmd.Parameters.AddWithValue("@IsRegisteredByMobile", member.IsRegisteredByMobile);
                        cmd.Parameters.AddWithValue("@IsOnlinePaymentEnabled", member.IsOnlinePaymentEnabled);
                        cmd.Parameters.AddWithValue("@IsEnterprise", member.IsEnterprise);
                        cmd.Parameters.AddWithValue("@IsEcommerce", member.IsEcommerce);
                        cmd.Parameters.AddWithValue("@Token", member.Token);
                        //cmd.Parameters.AddWithValue("@IsRegisteredByMobile", true);
                        //cmd.Parameters.Add(new SqlParameter("@NewMemberID", SqlDbType.BigInt).Direction = ParameterDirection.Output);


                        //Add the output parameter to the command object
                        SqlParameter outPutParameter = new SqlParameter();
                        outPutParameter.ParameterName = "@NewMemberID";
                        outPutParameter.SqlDbType = System.Data.SqlDbType.BigInt;
                        outPutParameter.Direction = System.Data.ParameterDirection.Output;
                        cmd.Parameters.Add(outPutParameter);

                        con.Open();
                        result = Convert.ToBoolean(cmd.ExecuteNonQuery());

                        member.UserID = Convert.ToInt64(outPutParameter.Value);

                        con.Close();
                    }

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return member;
        }


        public bool RemoveMember(long userID)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.DELETEMEMBER))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@UserID", userID);
                        

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

        public Member MemberLogIn(string emailID, string mobileNo, string password, Int64 parentID)
        {

            try
            {
                Member member = new Member();
                DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.MEMBERLOGIN, emailID, mobileNo, password,parentID);
                member = DataTableHelper.ConvertDataTable<Member>(ds.Tables[0]).FirstOrDefault();

                return member;
            }
            catch (Exception ex)
            {
                throw;
            }

        }
         


        public bool MemberLogOut(long userID)
        {
            var result = false;
 
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.MEMBERLOGOUT))
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

        public object MemberDashboard(long userID)
        {
            throw new NotImplementedException();
        }

        
    }
}
