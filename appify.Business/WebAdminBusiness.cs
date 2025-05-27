using appify.Business.Contract;
using appify.DataAccess.Contract;
using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static appify.models.HomePageProductByCategory;

namespace appify.Business
{
    public class WebAdminBusiness : IWebAdminBusiness
    {
       private IWebAdminRepository repository;
        public WebAdminBusiness(IWebAdminRepository repository)
        {
            this.repository = repository;
        }
        public User RegisterUser(User member)
        {
            return repository.RegisterUser(member);
        }
        public bool RemoveUser(Int32 UserID)
        {
            return repository.RemoveUser(UserID);
        }
        public User GetUser(Int32 userID)
        {
            return repository.GetUser(userID);
        }
        public Int64 GetUsersCount()
        {
            return repository.GetUsersCount();
        }
        public List<User> ListbyPageView(int parentID, int pageNo=0, int rows = 0)
        {
            return repository.ListbyPageView(parentID, pageNo, rows);
        }
        public bool CheckUser(string userID)
        {
            return repository.CheckUser(userID);
        }
        public bool ResetPassword(string emailID, string password)
        {
            return repository.ResetPassword(emailID, password);
        }
        public User LogIn(string emailID, string password)
        {
            return repository.LogIn(emailID, password);
        }
        public bool UserLogOut(long userID)
        {
            return repository.UserLogOut(userID);
        }
        public List<SellerList> GetSellerList()
        {
            return repository.GetSellerList();
        }
        public List<SellerOrderList> GetSellerOrderList()
        {
            return repository.GetSellerOrderList();
        }
        public bool SettlementStatusUpdate(long OrderID, bool Status)
        {
            return repository.SettlementStatusUpdate(OrderID, Status);
        }
        public List<ProductMasterByVendor> GetProducts(long userID)
        {
            return repository.GetProducts(userID);
        }
    }
}
