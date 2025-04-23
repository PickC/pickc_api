using appify.models;


namespace appify.DataAccess.Contract
{
	public interface ISubscriptionHeaderRepository
	{
		public SubscriptionHeader GetSubscriptionHeader(short planID);

		public List<SubscriptionHeader> ListSubscriptionHeader();

		public bool SaveSubscriptionHeader(SubscriptionHeader item);

        public bool DeleteSubscriptionHeader(short planID);

	}
}
