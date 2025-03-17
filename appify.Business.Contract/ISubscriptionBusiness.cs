using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.Business.Contract
{
    public interface ISubscriptionBusiness
    {

        public bool Save(Subscription item);
        public bool Delete(int subscriptionID);

        public Subscription Get(int subscriptionID);

        public List<Subscription> List();
         
    }
}
