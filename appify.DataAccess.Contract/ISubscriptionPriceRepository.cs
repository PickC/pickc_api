using appify.models;


namespace appify.DataAccess.Contract
{
	public interface ISubscriptionPriceRepository
	{
		public SubscriptionPrice GetSubscriptionPrice(short priceID);

        public List<SubscriptionPrice> ListSubscriptionPrice();
        public List<SubscriptionPrice> ListSubscriptionPriceByPlan(short planID);

         

        public bool SaveSubscriptionPrice(SubscriptionPrice item);

        public bool DeleteSubscriptionPrice(short priceID);


    }
}
