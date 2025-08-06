using appify.models;
using appify.Business.Contract;
using appify.DataAccess.Contract;
using appify.models;
using System;


namespace appify.Business
{
	public partial class SubscriptionItemBusiness : ISubscriptionItemBusiness 
	{
		ISubscriptionItemRepository repository;
		public SubscriptionItemBusiness (ISubscriptionItemRepository repository){
			this.repository = repository;
		}
		public SubscriptionItemLite GetSubscriptionItem(short itemID, short planID, short featureID)
        {
			return repository.GetSubscriptionItem(itemID,planID,featureID);
		}

		public List<SubscriptionItemLite> ListSubscriptionItem(short planID)
		{
			return repository.ListSubscriptionItem(planID);
		}

		public SubscriptionItem SaveSubscriptionItem(SubscriptionItem item)
		{
			return repository.SaveSubscriptionItem(item);
		}

		public bool DeleteSubscriptionItem(short itemID, short planID, short featureID)
        {
			return repository.DeleteSubscriptionItem(itemID,planID,featureID);
		}

	}
}
