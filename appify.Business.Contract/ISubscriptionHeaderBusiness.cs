using appify.models;


namespace appify.Business.Contract
{
	public interface ISubscriptionHeaderBusiness
	{
		public SubscriptionHeaderLite GetSubscriptionHeader(short planID);

		public List<SubscriptionHeaderLite> ListSubscriptionHeader();

		public SubscriptionHeader SaveSubscriptionHeader(SubscriptionHeader item);

		public bool DeleteSubscriptionHeader(short planID);


    }
}
