using appify.models;


namespace appify.DataAccess.Contract
{
	public interface IMemberAppSettingRepository
	{
		public MemberAppSetting GetMemberAppSetting(long userID);

		public List<MemberAppSetting> ListMemberAppSetting(long userID);

		public bool SaveMemberAppSetting(MemberAppSetting item);

		public bool DeleteMemberAppSetting(long userID);
        public MemberAppSetting GetMemberAppSettingWeb(long userID);
        public List<MemberAppSetting> ListMemberAppSettingWeb();
        public MemberAppStatus GetAppStatusWeb(long userID);
        public bool SaveMemberAppSettingWeb(MemberAppSetting item);
    }
}
