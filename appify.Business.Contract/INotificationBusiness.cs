using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.Business.Contract
{
    public interface INotificationBusiness
    {
        public Notifications SendEmail(Notifications notifications);
    }
}
