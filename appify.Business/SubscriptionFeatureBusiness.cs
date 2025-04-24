using appify.models;
using appify.Business.Contract;
using appify.DataAccess.Contract;
using appify.models;
using System;


namespace appify.Business
{
	public partial class SubscriptionFeatureBusiness : ISubscriptionFeatureBusiness 
	{
		ISubscriptionFeatureRepository repository;
		public SubscriptionFeatureBusiness (ISubscriptionFeatureRepository repository){
			this.repository = repository;
		}
		public SubscriptionFeatureLite GetSubscriptionFeature(short featureID)
		{
			return repository.GetSubscriptionFeature(featureID);
		}

		public List<SubscriptionFeatureLite> ListSubscriptionFeature()
		{
			return repository.ListSubscriptionFeature();
		}

		public SubscriptionFeature SaveSubscriptionFeature(SubscriptionFeature item)
		{
			return repository.SaveSubscriptionFeature(item);
		}

		public bool DeleteSubscriptionFeature(short featureID)
		{
			return repository.DeleteSubscriptionFeature(featureID);
		}

	}
}
