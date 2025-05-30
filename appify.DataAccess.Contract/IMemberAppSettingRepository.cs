using appify.models;


namespace appify.DataAccess.Contract
{
	public interface IMemberAppSettingRepository
	{
		public MemberAppSetting GetMemberAppSetting(long userID);

		public List<MemberAppSetting> ListMemberAppSetting(long userID);

		public bool SaveMemberAppSetting(MemberAppSetting item);

		public bool DeleteMemberAppSetting(long userID);

        public long GetMemberIdByAppName(string appName);


        #region Member App CICD Settings

        public MemberAppSettingCICD GetMemberAppSettingCICD(long userID);
        public List<MemberAppSettingCICD> ListMemberAppSettingCICD();
        //public Int16 RecordCountMemberAppSettingCICD();
        public bool UpdateMemberAppSettingCICD(MemberAppSettingCICD item);
        public MemberAppStatus GetAppStatusCICD(long userID);
        public bool UpdateMemberAppStatus(MemberAppSettingUpdate item);

        #endregion

        #region Member App Publish Settings

        public MemberAppPublishSetting GetMemberAppPublishSetting(long userID);
        public List<MemberAppPublishSettingLite> ListMemberAppPublishSetting();
        public bool UpdateMemberAppPublishSetting(MemberAppPublishSetting item);

        #endregion


    }
}
