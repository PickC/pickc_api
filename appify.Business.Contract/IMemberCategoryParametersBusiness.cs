using appify.models;


namespace appify.Business.Contract
{
	public interface IMemberCategoryParametersBusiness
	{
		public MemberCategoryParameters GetMemberCategoryParameters(Int64 ID);

		public List<MemberCategoryParameters> ListMemberCategoryParameters(long ProductID);

		public bool SaveMemberCategoryParameters(MemberCategoryParameters item);

		public bool DeleteMemberCategoryParameters(Int64 ID);

	}
}
