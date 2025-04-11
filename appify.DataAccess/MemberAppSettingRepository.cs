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
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Xml.Linq;

namespace appify.DataAccess
{
    public partial class MemberAppSettingRepository : IMemberAppSettingRepository
    {

        private IConfiguration configuration;
        private string appify_connectionstring;

        public MemberAppSettingRepository(IConfiguration config)
        {
            this.configuration = config;
            this.appify_connectionstring = config["ConnectionStrings:appify.connectionstring"].ToString();
        }
        public bool DeleteMemberAppSetting(long userID)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.DELETEMEMBERAPPSETTING))
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

        public MemberAppSetting GetMemberAppSetting(long userID)
        {
            MemberAppSetting item = new MemberAppSetting();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.SELECTMEMBERAPPSETTING, userID);
                item = DataTableHelper.ConvertDataTable<MemberAppSetting>(ds.Tables[0]).FirstOrDefault();
            }
            return item;
        }

        public List<MemberAppSetting> ListMemberAppSetting(long userID)
        {
            List<MemberAppSetting> items = new List<MemberAppSetting>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.LISTMEMBERAPPSETTING, userID);
                items = DataTableHelper.ConvertDataTable<MemberAppSetting>(ds.Tables[0]);
            }
            return items;
        }

        public bool SaveMemberAppSetting(MemberAppSetting item)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.SAVEMEMBERAPPSETTING))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@UserID", item.UserID);
                        cmd.Parameters.AddWithValue("@AppName", item.AppName);
                        cmd.Parameters.AddWithValue("@ShortDescription", item.ShortDescription);
                        cmd.Parameters.AddWithValue("@LongDescription", item.LongDescription);
                        cmd.Parameters.AddWithValue("@AppName1", item.AppName1);
                        cmd.Parameters.AddWithValue("@AppName2", item.AppName2);
                        cmd.Parameters.AddWithValue("@AppLogo", item.AppLogo);
                        cmd.Parameters.AddWithValue("@AppIcon", item.AppIcon);
                        cmd.Parameters.AddWithValue("@AppIconTransparent", item.AppIconTransparent);
                        cmd.Parameters.AddWithValue("@AndroidBundleID", item.AndroidBundleID);
                        cmd.Parameters.AddWithValue("@AppleBundleID", item.AppleBundleID);
                        cmd.Parameters.AddWithValue("@AppleAppID", item.AppleAppID);
                        cmd.Parameters.AddWithValue("@AndroidAppURL", item.AndroidAppURL);
                        cmd.Parameters.AddWithValue("@AppleAppURL", item.AppleAppURL);
                        cmd.Parameters.AddWithValue("@FireBaseProjectID", item.FireBaseProjectID);
                        cmd.Parameters.AddWithValue("@Website", item.Website);
                        cmd.Parameters.AddWithValue("@Keywords", item.Keywords);
                        cmd.Parameters.AddWithValue("@DeploymentStatusAndroid", item.DeploymentStatusAndroid);
                        cmd.Parameters.AddWithValue("@DeploymentStatusApple", item.DeploymentStatusApple);
                        cmd.Parameters.AddWithValue("@MobileLink", item.MobileLink);
                        cmd.Parameters.AddWithValue("@TabLink", item.TabLink);
                        cmd.Parameters.AddWithValue("@ImageLink", item.ImageLink);
                        cmd.Parameters.AddWithValue("@KYCLink", item.KYCLink);
                        cmd.Parameters.AddWithValue("@CompanyDescription", item.CompanyDescription);
                        cmd.Parameters.AddWithValue("@PlaystoreDescription", item.PlaystoreDescription);
                        cmd.Parameters.AddWithValue("@AppstoreWords", item.AppstoreWords);
                        cmd.Parameters.AddWithValue("@Subtitle", item.Subtitle);
                        cmd.Parameters.AddWithValue("@IsEmailSent", item.IsEmailSent);
                        cmd.Parameters.AddWithValue("@Comments", item.Comments);
                        cmd.Parameters.AddWithValue("@OnboardedBy", item.OnboardedBy);
                        cmd.Parameters.AddWithValue("@CreatedBy", item.CreatedBy);
                        cmd.Parameters.AddWithValue("@ModifiedBy", item.ModifiedBy);

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

        #region Member App Settings CICD


        public MemberAppSettingCICD GetMemberAppSettingCICD(long userID)
        {
            MemberAppSettingCICD item = new MemberAppSettingCICD();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.SELECTMEMBERAPPSETTINGSCICD, userID);
                item = DataTableHelper.ConvertDataTable<MemberAppSettingCICD>(ds.Tables[0]).FirstOrDefault();
            }
            return item;
        }

        public List<MemberAppSettingCICD> ListMemberAppSettingCICD()
        {
            List<MemberAppSettingCICD> items = new List<MemberAppSettingCICD>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.LISTMEMBERAPPSETTINGSCICD);
                items = DataTableHelper.ConvertDataTable<MemberAppSettingCICD>(ds.Tables[0]);
            }
            return items;
        }
        //public Int16 RecordCountMemberAppSettingCICD()
        //{
        //    short count;
        //    DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.RECORDCOUNTMEMBERAPPSETTINGSCICD);
        //    count = Convert.ToInt16(ds.Tables[0]);

        //    return count;
        //}
        public bool UpdateMemberAppSettingCICD(MemberAppSettingCICD item)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.UPDATEMEMBERAPPSETTINGSCICD))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@UserID", item.UserID);
                        cmd.Parameters.AddWithValue("@AppName", item.AppName);
                        cmd.Parameters.AddWithValue("@AppLogo", item.AppLogo);
                        cmd.Parameters.AddWithValue("@AppIcon", item.AppIcon);
                        cmd.Parameters.AddWithValue("@AppIconTransparent", item.AppIconTransparent);
                        cmd.Parameters.AddWithValue("@AndroidBundleID", item.AndroidBundleID);
                        cmd.Parameters.AddWithValue("@AppleBundleID", item.AppleBundleID);
                        cmd.Parameters.AddWithValue("@AppleAppID", item.AppleAppID);
                        cmd.Parameters.AddWithValue("@AndroidAppURL", item.AndroidAppURL);
                        cmd.Parameters.AddWithValue("@AppleAppURL", item.AppleAppURL);
                        cmd.Parameters.AddWithValue("@FireBaseProjectID", item.FireBaseProjectID);
                        cmd.Parameters.AddWithValue("@MobileNo", item.MobileNo);
                        cmd.Parameters.AddWithValue("@ModifiedBy", item.ModifiedBy);




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

        #endregion


        #region Member App Publish Settings  


        public MemberAppPublishSetting GetMemberAppPublishSetting(long userID)
        {
            MemberAppPublishSetting item = new MemberAppPublishSetting();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.SELECTMEMBERAPPPUBLISHSETTINGS, userID);
                item = DataTableHelper.ConvertDataTable<MemberAppPublishSetting>(ds.Tables[0]).FirstOrDefault();
            }
            return item;
        }

        public List<MemberAppPublishSettingLite> ListMemberAppPublishSetting()
        {
            List<MemberAppPublishSettingLite> items = new List<MemberAppPublishSettingLite>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.LISTMEMBERAPPPUBLISHSETTINGS);
                items = DataTableHelper.ConvertDataTable<MemberAppPublishSettingLite>(ds.Tables[0]);
            }
            return items;
        }
        //public Int16 RecordCountMemberAppSettingCICD()
        //{
        //    short count;
        //    DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.RECORDCOUNTMEMBERAPPSETTINGSCICD);
        //    count = Convert.ToInt16(ds.Tables[0]);

        //    return count;
        //}
        public bool UpdateMemberAppPublishSetting(MemberAppPublishSetting item)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.UPDATEMEMBERAPPPUBLISHSETTINGS))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@UserID", item.UserID);
                        cmd.Parameters.AddWithValue("@AppName", item.AppName);
                        cmd.Parameters.AddWithValue("@MobileNo", item.MobileNo);
                        cmd.Parameters.AddWithValue("@Password", item.Password);
                        cmd.Parameters.AddWithValue("@IsEmailSent", item.IsEmailSent);
                        cmd.Parameters.AddWithValue("@AndroidAppURL", item.AndroidAppURL);
                        cmd.Parameters.AddWithValue("@AppleAppURL", item.AppleAppURL);
                        cmd.Parameters.AddWithValue("@DeploymentStatusAndroid", item.DeploymentStatusAndroid);
                        cmd.Parameters.AddWithValue("@DeploymentStatusApple", item.DeploymentStatusApple);
                        cmd.Parameters.AddWithValue("@AppLogo", item.AppLogo);
                        cmd.Parameters.AddWithValue("@AppIcon", item.AppIcon);
                        cmd.Parameters.AddWithValue("@Website", item.Website);
                        cmd.Parameters.AddWithValue("@MobileLink", item.MobileLink);
                        cmd.Parameters.AddWithValue("@TabLink", item.TabLink);
                        cmd.Parameters.AddWithValue("@ImageLink", item.ImageLink);
                        cmd.Parameters.AddWithValue("@KycLink", item.KycLink);
                        cmd.Parameters.AddWithValue("@CompanyDescription", item.CompanyDescription);
                        cmd.Parameters.AddWithValue("@PlayStoreDescription", item.PlayStoreDescription);
                        cmd.Parameters.AddWithValue("@AppstoreWords", item.AppstoreWords);
                        cmd.Parameters.AddWithValue("@Subtitle", item.Subtitle);
                        cmd.Parameters.AddWithValue("@Comments", item.Comments);
                        cmd.Parameters.AddWithValue("@OnBoardedBy", item.OnBoarderBy);
                        cmd.Parameters.AddWithValue("@ShortDescription", item.ShortDescription);
                        cmd.Parameters.AddWithValue("@ModifiedBy", item.ModifiedBy);
                        cmd.Parameters.AddWithValue("@PrivacyPolicyLink", item.PrivacyPolicyLink);

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

        #endregion




        public MemberAppStatus GetAppStatusCICD(long userID)
        {
            MemberAppStatus item = new MemberAppStatus();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.SELECTMEMBERAPPSETTING, userID);
                item = DataTableHelper.ConvertDataTable<MemberAppStatus>(ds.Tables[0]).FirstOrDefault();
            }
            return item;
        }

     



        public bool UpdateMemberAppStatus(MemberAppSettingUpdate item)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.SAVEMEMBERAPPSETTING))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@UserID", item.VendorID);
                        cmd.Parameters.AddWithValue("@AppName", item.AppName);
                        cmd.Parameters.AddWithValue("@DeploymentStatusAndroid", item.DeploymentStatusAndroid);
                        cmd.Parameters.AddWithValue("@DeploymentStatusApple", item.DeploymentStatusApple);
                        cmd.Parameters.AddWithValue("@AndroidAppURL", item.AndroidAppURL);
                        cmd.Parameters.AddWithValue("@AppleAppURL", item.AppleAppURL);

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
