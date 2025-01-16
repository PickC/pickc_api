using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.Business.Contract
{
    public interface IWebAdminBusiness
    {
        public User RegisterUser(User user);
        public bool RemoveUser(Int32 UserID);
        public User GetUser(Int32 userID);
        public bool CheckUser(string userID);
        public bool ResetPassword(long userID, string password);
        public User LogIn(string emailID, string password);
        public bool UserLogOut(long userID);
    }
}
