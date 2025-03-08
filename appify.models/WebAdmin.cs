using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace appify.models
{
    public partial class WebAdmin
    {
    }
    public partial class User
    {
        public short UserID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public Int16 Department { get; set; }
        public Int16 UserDesignation { get; set; }
        public string EmployeeID { get; set; }
        //public string ICNo { get; set; }
        public string EmailID { get; set; }
        public string ContactNo { get; set; }
        public bool IsActive { get; set; }
        public bool IsAccepted { get; set; }
        public bool IsAllowLogOn { get; set; }
        public bool IsOperational { get; set; }
        public Int16? CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public Int16? ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        //public Int64 BranchID { get; set; }
        //public string OTPNo { get; set; }
        //public bool IsOTPSent { get; set; }
        //public DateTime OTPSentDate { get; set; }
        //public bool IsOTPReSent { get; set; }
        //public Int16 OTPSentCount { get; set; }
        //public bool IsOTPVerified { get; set; }
        public Int32 RoleID { get; set; }

        public string? RoleCode { get; set; }
        public string? RoleDescription { get; set; }
        
        public string? UserCreated { get; set; }
        public string? ModifiedUser { get; set; }
    }


    public partial class Roles
    {
        public Int32 RoleID { get; set; }
        public string RoleCode { get; set; }
        public string RoleDescription { get; set; }
        public bool IsActive { get; set; }
        public Int16 CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public Int16 ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }

    }
    public partial class RolesDecativate
    {
        public string RoleCode { get; set; }
        public Int16 ModifiedBy { get; set; }

    }
    public partial class RolesAccessType
    {
        public Int32 LookupID { get; set; }
        public string LookupCategory {  get; set; }
        public string LookupCode { get; set; }
        public string LookupDescription { get; set; }
        public string MappingCode { get; set; }
    }
    public partial class SellerList
    {
        public Int32 UserID { get; set; }
        public string Logo { get; set; }
        public string AppName { get; set; }
        public DateTime RegDate { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public Int32 TotalOrders {  get; set; }
        public string ContactNo { get; set; }

    }
    public partial class SellerOrderList
    {
        public int OrderID { get; set; }
        public string OrderNo { get; set; }
        public int VendorID { get; set; }
        public string AppName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; }
        public string PaymentMode { get; set; }
        public string SettlementStatus { get; set; }
    }
    public partial class Securables
    {
        public Int32 SecurableID { get; set; }
        public string WebPageLink { get; set; }
        public Int16 AccessLevel { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
    }

    public partial class  SecurablesFunction
    {
        public Int32 FunctionID { get; set; }
        public Int32 SecurableID { get; set; }
        public string FunctionName { get; set; }
        public Int16 AccessLevel { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public bool IsActive { get; set; }
    }
    public partial class ManagementDashboardSummary
    {
        public string MetricName { get; set; }
        public string MetricValue { get; set; }
        public string MetricPercentage { get; set; }
        public string MetricPercentageValue {  get; set; }

    }

    public partial class DashboardTopProducts
    {
        public string ProductName { get; set; }
        public Int32 TotalSales { get; set; }
    }
    public partial class DashboardTopVendors
    {
        public string VendorName { get; set; }
        public Int32 TotalSales { get; set; }
    }
    public partial class DashboardTopOrdersByCity
    {
        public string CityName { get; set; }
        public Int32 TotalSales { get; set; }
    }
    public partial class  DashboardOrderDeliveryCharges
    {
        public long VendorID { get; set; }
        public string AppName { get; set; }
        public decimal TotalPrice {  get; set; }
        public decimal TotalDeliveryCharges {  get; set; }
        public decimal GrandTotal {  get; set; }
    }
    public partial class  DashboardMonthlySales
    {
        public string Name { get; set; }
        public Int32 TotalSales { get; set; }
    }
    public partial class DashboardOnBoardVendors
    {
        public string Name { get; set; }
        public Int32 TotalVendors { get; set; }
    }
    public partial class DashboardTotalRevenue
    {
        public long VendorID { get; set; }
        public string AppName { get; set; }
        public decimal TotalCOD { get; set; }
        public decimal TotalOnline {  get; set; }
        public decimal TotalRevenue { get; set; }
    }
    public partial class DashboardOrderStatus
    {
        public string Type { get; set; }
        public decimal Total {  get; set; }
    }
    public partial class DashboardTopVendorsProducts
    {
        public long ProductID { get; set; }
        public string ImageName { get; set; }
        public string ProductName { get; set; }
        public Int32 StockRemaining { get; set; }
    }
}
