using appify.models;


namespace appify.DataAccess.Contract
{
	public interface ISubscriptionHeaderRepository
	{
		public SubscriptionHeaderLite GetSubscriptionHeader(short planID);

		public List<SubscriptionHeaderLite> ListSubscriptionHeader();

		public SubscriptionHeader SaveSubscriptionHeader(SubscriptionHeader item);

        public bool DeleteSubscriptionHeader(short planID);

	}
}
