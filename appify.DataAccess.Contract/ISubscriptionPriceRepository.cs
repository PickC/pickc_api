using appify.models;


namespace appify.DataAccess.Contract
{
	public interface ISubscriptionPriceRepository
	{
		public SubscriptionPriceLite GetSubscriptionPrice(short priceID);

        public List<SubscriptionPriceLite> ListSubscriptionPrice();
        public List<SubscriptionPrice> ListSubscriptionPriceByPlan(short planID);

         

        public SubscriptionPrice SaveSubscriptionPrice(SubscriptionPrice item);

        public bool DeleteSubscriptionPrice(short priceID);


    }
}
