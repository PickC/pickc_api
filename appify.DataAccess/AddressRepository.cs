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
using System.Data;
using System.Data.SqlClient;

namespace appify.DataAccess
{
    public partial class AddressRepository : IAddressRepository
    {
        private IConfiguration configuration;
        private string appify_connectionstring;

        public AddressRepository(IConfiguration config)
        {
            this.configuration = config;
            this.appify_connectionstring = config["ConnectionStrings:appify.connectionstring"].ToString();
        }

        public bool DeleteAddress(long addressID, long linkID)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.DELETEADDRESS))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@AddressID", addressID);
                        cmd.Parameters.AddWithValue("@LinkID", linkID);


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

        public Address GetAddress(long addressID, long linkID)
        {
            Address item = new Address();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.SELECTADDRESS,addressID,linkID);
            item = DataTableHelper.ConvertDataTable<Address>(ds.Tables[0]).FirstOrDefault();

            return item;
        }


        public Address GetDefaultAddress(long linkID)
        {
            Address item = new Address();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.SELECTDEFAULTADDRESS, linkID);
            item = DataTableHelper.ConvertDataTable<Address>(ds.Tables[0]).FirstOrDefault();

            return item;
        }



        public List<Address> GetList(long linkID)
        {
            List<Address> items = new List<Address>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LISTADDRESS,linkID);
            items = DataTableHelper.ConvertDataTable<Address>(ds.Tables[0]);

            return items;
        }

        public List<Address> GetAddressList()
        {
            List<Address> items = new List<Address>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LISTALLADDRESS);
            items = DataTableHelper.ConvertDataTable<Address>(ds.Tables[0]);

            return items;
        }

        public Address SaveAddress(Address item)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.SAVEADDRESS))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        
                        cmd.Parameters.AddWithValue("@AddressID", item.AddressID);
                        cmd.Parameters.AddWithValue("@LinkID", item.LinkID);
                        cmd.Parameters.AddWithValue("@AddressType",item.AddressType);
                        cmd.Parameters.AddWithValue("@HouseNo", item.HouseNo);
                        cmd.Parameters.AddWithValue("@Address1", item.Address1);
                        cmd.Parameters.AddWithValue("@Address2",item.Address2);
                        cmd.Parameters.AddWithValue("@Landmark", item.Landmark);
                        cmd.Parameters.AddWithValue("@City", item.City);
                        cmd.Parameters.AddWithValue("@ZipCode",item.ZipCode);
                        cmd.Parameters.AddWithValue("@AlternateNo", item.AlternateNo);
                        cmd.Parameters.AddWithValue("@State",item.State);
                        cmd.Parameters.AddWithValue("@Country", item.Country);
                        cmd.Parameters.AddWithValue("@Latitude", item.Latitude);
                        cmd.Parameters.AddWithValue("@Longitude",item.Longitude);
                        cmd.Parameters.AddWithValue("@IsDefault", item.IsDefault);
                        cmd.Parameters.AddWithValue("@LocationID", item.LocationID);

                        SqlParameter outPutParameter = new SqlParameter();
                        outPutParameter.ParameterName = "@NewAddressID";
                        outPutParameter.SqlDbType = System.Data.SqlDbType.BigInt;
                        outPutParameter.Direction = System.Data.ParameterDirection.Output;
                        cmd.Parameters.Add(outPutParameter);

                        
                        con.Open();
                        result = Convert.ToBoolean(cmd.ExecuteNonQuery());


                        item.AddressID = Convert.ToInt64(outPutParameter.Value);
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
