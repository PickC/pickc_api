using appify.models;


namespace appify.DataAccess.Contract
{
	public interface IMemberAppSettingRepository
	{
		public MemberAppSetting GetMemberAppSetting(long userID);

		public List<MemberAppSetting> ListMemberAppSetting(long userID);

		public bool SaveMemberAppSetting(MemberAppSetting item);

		public bool DeleteMemberAppSetting(long userID);

	}
}
