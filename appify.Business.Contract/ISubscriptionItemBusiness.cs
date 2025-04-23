using appify.models;


namespace appify.Business.Contract
{
	public interface ISubscriptionItemBusiness
	{
		public SubscriptionItem GetSubscriptionItem(short itemID,short planID,short featureID);

		public List<SubscriptionItem> ListSubscriptionItem(short planID);

		public bool SaveSubscriptionItem(SubscriptionItem item);

		public bool DeleteSubscriptionItem(short itemID, short planID, short featureID);

	}
}
