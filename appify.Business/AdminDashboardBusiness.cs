using appify.Business.Contract;
using appify.DataAccess.Contract;
using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.Business
{
    public partial class AdminDashboardBusiness : IAdminDashboardBusiness
    {
        IAdminDashboardRepository repository;
        public AdminDashboardBusiness(IAdminDashboardRepository repository) { 
            this.repository = repository;
        }
        public List<ManagementDashboardSummary> ManagementDashboardSummary(DateTime StartDate, DateTime EndDate)
        {
            return repository.ManagementDashboardSummary(StartDate, EndDate);
        }
        public List<DashboardTopProducts> DashboardTopProducts(DateTime StartDate, DateTime EndDate)
        {
            return repository.DashboardTopProducts(StartDate, EndDate);
        }
        public List<DashboardTopVendors> DashboardTopVendors(DateTime StartDate, DateTime EndDate)
        {
            return repository.DashboardTopVendors(StartDate, EndDate);
        }
        public List<DashboardTopOrdersByCity> DashboardTopOrdersByCity(DateTime StartDate, DateTime EndDate)
        {
            return repository.DashboardTopOrdersByCity(StartDate, EndDate);
        }
        public List<DashboardOrderDeliveryCharges> DashboardOrderDeliveryCharges(DateTime StartDate, DateTime EndDate)
        {
            return repository.DashboardOrderDeliveryCharges(StartDate, EndDate);
        }
        public List<DashboardMonthlySales> DashboardMonthlySales()
        {
            return repository.DashboardMonthlySales();
        }
        public List<DashboardOnBoardVendors> DashboardOnBoardVendors()
        {
            return repository.DashboardOnBoardVendors();
        }
        public List<DashboardTotalRevenue> DashboardTotalRevenue(DateTime StartDate, DateTime EndDate)
        {
            return repository.DashboardTotalRevenue(StartDate, EndDate);
        }
        public List<DashboardOrderStatus> DashboardOrderStatus(DateTime StartDate, DateTime EndDate)
        {
            return repository.DashboardOrderStatus(StartDate, EndDate);
        }
        public List<ManagementDashboardSummary> OperationsDashboardSummary(DateTime StartDate, DateTime EndDate)
        {
            return repository.OperationsDashboardSummary(StartDate, EndDate);
        }
        public List<DashboardTopVendorsProducts> DashboardTopVendorsProducts(DateTime StartDate, DateTime EndDate)
        {
            return repository.DashboardTopVendorsProducts(StartDate, EndDate);
        }
        public List<GlobalSearch> GlobalSearch(string FilterType, string SearchText, short PageNo, short Rows)
        {
            return repository.GlobalSearch(FilterType, SearchText, PageNo, Rows);
        }
    }
}
