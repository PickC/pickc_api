using appify.models;
using appify.Business.Contract;
using appify.DataAccess.Contract;
using appify.models;
using System;


namespace appify.Business
{
	public partial class SubscriptionHeaderBusiness : ISubscriptionHeaderBusiness 
	{
		ISubscriptionHeaderRepository repository;
		public SubscriptionHeaderBusiness (ISubscriptionHeaderRepository repository){
			this.repository = repository;
		}
		public SubscriptionHeaderLite GetSubscriptionHeader(short planID)
		{
			return repository.GetSubscriptionHeader(planID);
		}

		public List<SubscriptionHeaderLite> ListSubscriptionHeader()
		{
			return repository.ListSubscriptionHeader();
		}

		public SubscriptionHeader SaveSubscriptionHeader(SubscriptionHeader item)
		{
			return repository.SaveSubscriptionHeader(item);
		}

        public bool DeleteSubscriptionHeader(short planID)
        {
			return repository.DeleteSubscriptionHeader(planID);
		}

	}
}
