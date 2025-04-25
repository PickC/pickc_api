using appify.models;


namespace appify.Business.Contract
{
	public interface ISubscriptionItemBusiness
	{
		public SubscriptionItemLite GetSubscriptionItem(short itemID,short planID,short featureID);

		public List<SubscriptionItemLite> ListSubscriptionItem(short planID);

		public SubscriptionItem SaveSubscriptionItem(SubscriptionItem item);

		public bool DeleteSubscriptionItem(short itemID, short planID, short featureID);

	}
}
