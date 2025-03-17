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
    public partial class SubscriptionBusiness:ISubscriptionBusiness
    {
        private ISubscriptionRepository repository;
        public SubscriptionBusiness(ISubscriptionRepository repository) { 
            this.repository = repository;
        }

        public bool Delete(int subscriptionID)
        {
            return repository.Delete(subscriptionID);
        }

        public Subscription Get(int subscriptionID)
        {
            return repository.Get(subscriptionID);
        }

        public List<Subscription> List()
        {
            return repository.List();
        }
         

        public bool Save(Subscription item)
        {
            return repository.Save(item);
        }
    }
}
