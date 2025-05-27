using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static appify.models.HomePageProductByCategory;

namespace appify.Business.Contract
{
    public interface IWebAdminBusiness
    {
        public User RegisterUser(User user);
        public bool RemoveUser(Int32 UserID);
        public User GetUser(Int32 userID);
        public Int64 GetUsersCount();
        public List<User> ListbyPageView(int parentID, int pageNo, int rows);
        public bool CheckUser(string userID);
        public bool ResetPassword(string emailID, string password);
        public User LogIn(string emailID, string password);
        public bool UserLogOut(long userID);
        public List<SellerList> GetSellerList();
        public List<SellerOrderList> GetSellerOrderList();
        public bool SettlementStatusUpdate(long OrderID, bool Status);
        public List<ProductMasterByVendor> GetProducts(long userID);
    }
}
