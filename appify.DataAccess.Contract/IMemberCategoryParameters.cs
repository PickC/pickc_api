using appify.models;


namespace appify.DataAccess.Contract
{
	public interface IMemberCategoryParametersRepository
	{
		public MemberCategoryParameters GetMemberCategoryParameters(Int64 ID);

		public List<MemberCategoryParameters> ListMemberCategoryParameters(long ProductID);

		public bool SaveMemberCategoryParameters(MemberCategoryParameters item);

		public bool DeleteMemberCategoryParameters(Int64 ID);

	}
}
