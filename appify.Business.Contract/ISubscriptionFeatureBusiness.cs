using appify.models;


namespace appify.Business.Contract
{
	public interface ISubscriptionFeatureBusiness
	{
        public SubscriptionFeature GetSubscriptionFeature(short featureID);

        public List<SubscriptionFeature> ListSubscriptionFeature();

        public bool SaveSubscriptionFeature(SubscriptionFeature item);

        public bool DeleteSubscriptionFeature(short featureID);



    }
}
