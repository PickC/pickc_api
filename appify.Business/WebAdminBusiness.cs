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
    }
}
