using appify.models;


namespace appify.Business.Contract
{
	public interface ISubscriptionHeaderBusiness
	{
		public SubscriptionHeader GetSubscriptionHeader(short planID);

		public List<SubscriptionHeader> ListSubscriptionHeader();

		public bool SaveSubscriptionHeader(SubscriptionHeader item);

		public bool DeleteSubscriptionHeader(short planID);


    }
}
