using appify.models;


namespace appify.DataAccess.Contract
{
	public interface ISubscriptionFeatureRepository
	{
		public SubscriptionFeature GetSubscriptionFeature(short featureID);

		public List<SubscriptionFeature> ListSubscriptionFeature();

		public bool SaveSubscriptionFeature(SubscriptionFeature item);

		public bool DeleteSubscriptionFeature(short featureID);

	}
}
