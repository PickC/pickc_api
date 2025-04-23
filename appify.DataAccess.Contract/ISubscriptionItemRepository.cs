using appify.models;


namespace appify.DataAccess.Contract
{
	public interface ISubscriptionItemRepository
	{

        public SubscriptionItem GetSubscriptionItem(short itemID, short planID, short featureID);

        public List<SubscriptionItem> ListSubscriptionItem(short planID);

        public bool SaveSubscriptionItem(SubscriptionItem item);

        public bool DeleteSubscriptionItem(short itemID, short planID, short featureID);

    }
}
