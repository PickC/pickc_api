using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.Business.Contract
{
    public interface IMemberAppSettingBusiness
    {

        public MemberAppSetting GetMemberAppSetting(long userID);

        public List<MemberAppSetting> ListMemberAppSetting(long userID);

        public bool SaveMemberAppSetting(MemberAppSetting item);

        public bool DeleteMemberAppSetting(long userID);

        public MemberAppSettingCICD GetMemberAppSettingCICD(long userID);
        public List<MemberAppSettingCICD> ListMemberAppSettingCICD();
        //public Int16 RecordCountMemberAppSettingCICD();
        public bool UpdateMemberAppSettingCICD(MemberAppSettingCICD item);
        public MemberAppStatus GetAppStatusCICD(long userID);

        public bool UpdateMemberAppStatus(MemberAppSettingUpdate item);

        public MemberAppSettingStore GetMemberIdByAppName(string appName);


        #region Member App Publish Settings

        public MemberAppPublishSetting GetMemberAppPublishSetting(long userID);
        public List<MemberAppPublishSettingLite> ListMemberAppPublishSetting();
        public bool UpdateMemberAppPublishSetting(MemberAppPublishSetting item);




        #endregion



    }
}
