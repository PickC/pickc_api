using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.Business.Contract
{
    public interface IAdminDashboardBusiness
    {
        public List<ManagementDashboardSummary> ManagementDashboardSummary(DateTime StartDate, DateTime EndDate);
        public List<DashboardTopProducts> DashboardTopProducts(DateTime StartDate, DateTime EndDate);
        public List<DashboardTopVendors> DashboardTopVendors(DateTime StartDate, DateTime EndDate);
        public List<DashboardTopOrdersByCity> DashboardTopOrdersByCity(DateTime StartDate, DateTime EndDate);
        public List<DashboardOrderDeliveryCharges> DashboardOrderDeliveryCharges(DateTime StartDate, DateTime EndDate);
        public List<DashboardMonthlySales> DashboardMonthlySales();
        public List<DashboardOnBoardVendors> DashboardOnBoardVendors();
        public List<DashboardTotalRevenue> DashboardTotalRevenue(DateTime StartDate, DateTime EndDate);
        public List<DashboardOrderStatus> DashboardOrderStatus(DateTime StartDate, DateTime EndDate);
        public List<ManagementDashboardSummary> OperationsDashboardSummary(DateTime StartDate, DateTime EndDate);
        public List<DashboardTopVendorsProducts> DashboardTopVendorsProducts(DateTime StartDate, DateTime EndDate);
    }
}
