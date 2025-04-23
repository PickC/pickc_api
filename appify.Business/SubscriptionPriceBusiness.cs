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
		public SubscriptionPrice GetSubscriptionPrice(short priceID)
		{
			return repository.GetSubscriptionPrice(priceID);
		}

        public List<SubscriptionPrice> ListSubscriptionPrice()
        {
            return repository.ListSubscriptionPrice();
        }

        public List<SubscriptionPrice> ListSubscriptionPriceByPlan(short planID)
        {
            return repository.ListSubscriptionPriceByPlan(planID);
        }

        public bool SaveSubscriptionPrice(SubscriptionPrice item)
		{
			return repository.SaveSubscriptionPrice(item);
		}

        public bool DeleteSubscriptionPrice(short priceID)
        {
			return repository.DeleteSubscriptionPrice(priceID);
		}

	}
}
