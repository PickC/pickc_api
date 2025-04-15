using appify.DataAccess.Contract;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using appify.models;
using appify.utility;

namespace appify.DataAccess
{
    public partial class AdminDashboardRepository : IAdminDashboardRepository
    {
        private IConfiguration configuration;
        private string appify_connectionstring;

        public AdminDashboardRepository(IConfiguration config)
        {
            this.configuration = config;
            this.appify_connectionstring = config["ConnectionStrings:appify.connectionstring"].ToString();
        }
        public List<ManagementDashboardSummary> ManagementDashboardSummary(DateTime StartDate, DateTime EndDate)
        {
            List<ManagementDashboardSummary> item = new List<ManagementDashboardSummary>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.LISTMANAGEMENTDASHBOARDSUMMARY, StartDate, EndDate);
                item = DataTableHelper.ConvertDataTable<ManagementDashboardSummary>(ds.Tables[0]);
            }
            return item;
        }
        public List<DashboardTopProducts> DashboardTopProducts(DateTime StartDate, DateTime EndDate)
        {
            List<DashboardTopProducts> item = new List<DashboardTopProducts>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.LISTDASHBOARDTOPPRODUCTS, StartDate, EndDate);
                item = DataTableHelper.ConvertDataTable<DashboardTopProducts>(ds.Tables[0]);
            }
            return item;
        }
        public List<DashboardTopVendors> DashboardTopVendors(DateTime StartDate, DateTime EndDate)
        {
            List<DashboardTopVendors> item = new List<DashboardTopVendors>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.LISTDASHBOARDTOPVENDORS, StartDate, EndDate);
                item = DataTableHelper.ConvertDataTable<DashboardTopVendors>(ds.Tables[0]);
            }
            return item;
        }
        public List<DashboardTopOrdersByCity> DashboardTopOrdersByCity(DateTime StartDate, DateTime EndDate)
        {
            List<DashboardTopOrdersByCity> item = new List<DashboardTopOrdersByCity>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.LISTDASHBOARDTOPORDERSBYCITY, StartDate, EndDate);
                item = DataTableHelper.ConvertDataTable<DashboardTopOrdersByCity>(ds.Tables[0]);
            }
            return item;
        }
        public List<DashboardOrderDeliveryCharges> DashboardOrderDeliveryCharges(DateTime StartDate, DateTime EndDate)
        {
            List<DashboardOrderDeliveryCharges> item = new List<DashboardOrderDeliveryCharges>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.LISTDASHBOARDTOPORDERSCHARGES, StartDate, EndDate);
                item = DataTableHelper.ConvertDataTable<DashboardOrderDeliveryCharges>(ds.Tables[0]);
            }
            return item;
        }
        public List<DashboardMonthlySales> DashboardMonthlySales()
        {
            List<DashboardMonthlySales> item = new List<DashboardMonthlySales>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.LISTDASHBOARDMONTHLYSALES);
                item = DataTableHelper.ConvertDataTable<DashboardMonthlySales>(ds.Tables[0]);
            }
            return item;
        }
        public List<DashboardOnBoardVendors> DashboardOnBoardVendors()
        {
            List<DashboardOnBoardVendors> item = new List<DashboardOnBoardVendors>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.LISTDASHBOARDONBOARDVENDORS);
                item = DataTableHelper.ConvertDataTable<DashboardOnBoardVendors>(ds.Tables[0]);
            }
            return item;
        }
        public List<DashboardTotalRevenue> DashboardTotalRevenue(DateTime StartDate, DateTime EndDate)
        {
            List<DashboardTotalRevenue> item = new List<DashboardTotalRevenue>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.LISTDASHBOARDTOTALREVENUE, StartDate, EndDate);
                item = DataTableHelper.ConvertDataTable<DashboardTotalRevenue>(ds.Tables[0]);
            }
            return item;
        }
        public List<DashboardOrderStatus> DashboardOrderStatus(DateTime StartDate, DateTime EndDate)
        {
            List<DashboardOrderStatus> item = new List<DashboardOrderStatus>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.LISTDASHBOARDORDERSTATUS, StartDate, EndDate);
                item = DataTableHelper.ConvertDataTable<DashboardOrderStatus>(ds.Tables[0]);
            }
            return item;
        }
        public List<ManagementDashboardSummary> OperationsDashboardSummary(DateTime StartDate, DateTime EndDate)
        {
            List<ManagementDashboardSummary> item = new List<ManagementDashboardSummary>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.LISTOPERATIONDASHBOARDSUMMARY, StartDate, EndDate);
                item = DataTableHelper.ConvertDataTable<ManagementDashboardSummary>(ds.Tables[0]);
            }
            return item;
        }
        public List<DashboardTopVendorsProducts> DashboardTopVendorsProducts(DateTime StartDate, DateTime EndDate)
        {
            List<DashboardTopVendorsProducts> item = new List<DashboardTopVendorsProducts>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.LISTDASHBOARDTOPVENDORSPRODUCTS, StartDate, EndDate);
                item = DataTableHelper.ConvertDataTable<DashboardTopVendorsProducts>(ds.Tables[0]);
            }
            return item;
        }
        public List<GlobalSearch> GlobalSearch(string FilterType, string SearchText, short PageNo, short Rows)
        {
            List<GlobalSearch> item = new List<GlobalSearch>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.GLOBALSEARCH, FilterType, SearchText, PageNo, Rows);
                item = DataTableHelper.ConvertDataTable<GlobalSearch>(ds.Tables[0]);
            }
            return item;
        }
    }
}
