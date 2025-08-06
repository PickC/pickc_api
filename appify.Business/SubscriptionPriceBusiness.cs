using appify.models;
using appify.Business.Contract;
using appify.DataAccess.Contract;
using appify.models;
using System;


namespace appify.Business
{
	public partial class SubscriptionPriceBusiness : ISubscriptionPriceBusiness 
	{
		ISubscriptionPriceRepository repository;
		public SubscriptionPriceBusiness (ISubscriptionPriceRepository repository){
			this.repository = repository;
		}
		public SubscriptionPriceLite GetSubscriptionPrice(short priceID)
		{
			return repository.GetSubscriptionPrice(priceID);
		}

        public List<SubscriptionPriceLite> ListSubscriptionPrice()
        {
            return repository.ListSubscriptionPrice();
        }

        public List<SubscriptionPrice> ListSubscriptionPriceByPlan(short planID)
        {
            return repository.ListSubscriptionPriceByPlan(planID);
        }

        public SubscriptionPrice SaveSubscriptionPrice(SubscriptionPrice item)
		{
			return repository.SaveSubscriptionPrice(item);
		}

        public bool DeleteSubscriptionPrice(short priceID)
        {
			return repository.DeleteSubscriptionPrice(priceID);
		}

	}
}
