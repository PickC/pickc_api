using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.DataAccess.Contract
{
    public interface ISubscriptionRepository
    {

        public bool Save(Subscription item);
        public bool Delete(int subscriptionID);

        public Subscription Get(int subscriptionID);

        public List<Subscription> List();
         

    }
}
