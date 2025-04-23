using appify.models;


namespace appify.Business.Contract
{
	public interface ISubscriptionPriceBusiness
	{
		public SubscriptionPrice GetSubscriptionPrice(short priceID);

		public List<SubscriptionPrice> ListSubscriptionPrice();

        public List<SubscriptionPrice> ListSubscriptionPriceByPlan(short planID);

        public bool SaveSubscriptionPrice(SubscriptionPrice item);

		public bool DeleteSubscriptionPrice(short priceID);

	}
}
