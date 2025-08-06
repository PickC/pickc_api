using appify.models;


namespace appify.Business.Contract
{
	public interface ISubscriptionFeatureBusiness
	{
        public SubscriptionFeatureLite GetSubscriptionFeature(short featureID);

        public List<SubscriptionFeatureLite> ListSubscriptionFeature();

        public SubscriptionFeature SaveSubscriptionFeature(SubscriptionFeature item);

        public bool DeleteSubscriptionFeature(short featureID);



    }
}
