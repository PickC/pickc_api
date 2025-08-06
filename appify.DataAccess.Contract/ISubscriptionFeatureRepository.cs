using appify.models;


namespace appify.DataAccess.Contract
{
	public interface ISubscriptionFeatureRepository
	{
		public SubscriptionFeatureLite GetSubscriptionFeature(short featureID);

		public List<SubscriptionFeatureLite> ListSubscriptionFeature();

		public SubscriptionFeature SaveSubscriptionFeature(SubscriptionFeature item);

		public bool DeleteSubscriptionFeature(short featureID);

	}
}
