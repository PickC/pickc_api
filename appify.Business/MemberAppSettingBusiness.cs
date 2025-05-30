using appify.Business.Contract;
using appify.DataAccess.Contract;
using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.Business
{
    public partial class MemberAppSettingBusiness : IMemberAppSettingBusiness
    {
        IMemberAppSettingRepository memberAppSettingRepository;
        public MemberAppSettingBusiness(IMemberAppSettingRepository repository)
        {
            this.memberAppSettingRepository = repository;
        }

        public bool DeleteMemberAppSetting(long userID)
        {
            return memberAppSettingRepository.DeleteMemberAppSetting(userID);
        }

        public MemberAppSetting GetMemberAppSetting(long userID)
        {
            return memberAppSettingRepository.GetMemberAppSetting(userID);
        }

        public long GetMemberIdByAppName(string appName) { 
            return memberAppSettingRepository.GetMemberIdByAppName(appName); 
        }
        
        


        public List<MemberAppSetting> ListMemberAppSetting(long userID)
        {
            return memberAppSettingRepository.ListMemberAppSetting(userID);
        }

        public bool SaveMemberAppSetting(MemberAppSetting item)
        {
            return memberAppSettingRepository.SaveMemberAppSetting(item);
        }
        #region Member App CICD Settings

        public MemberAppSettingCICD GetMemberAppSettingCICD(long userID)
        {
            return memberAppSettingRepository.GetMemberAppSettingCICD(userID);
        }
        public List<MemberAppSettingCICD> ListMemberAppSettingCICD()
        {
            return memberAppSettingRepository.ListMemberAppSettingCICD();
        }
        public bool UpdateMemberAppSettingCICD(MemberAppSettingCICD item)
        {
            return memberAppSettingRepository.UpdateMemberAppSettingCICD(item);
        }

        //public short RecordCountMemberAppSettingCICD() { 
        //    throw new NotImplementedException();

        //}

        public MemberAppStatus GetAppStatusCICD(long userID)
        {
            return memberAppSettingRepository.GetAppStatusCICD(userID);
        }


        public bool UpdateMemberAppStatus(MemberAppSettingUpdate item) { 
            return memberAppSettingRepository.UpdateMemberAppStatus(item);
        }



        #endregion

        #region Member App Publish Settings

        public MemberAppPublishSetting GetMemberAppPublishSetting(long userID)
        {
            return memberAppSettingRepository.GetMemberAppPublishSetting(userID);
        }
        public List<MemberAppPublishSettingLite> ListMemberAppPublishSetting()
        {
            return memberAppSettingRepository.ListMemberAppPublishSetting();
        }
        public bool UpdateMemberAppPublishSetting(MemberAppPublishSetting item)
        {
            return memberAppSettingRepository.UpdateMemberAppPublishSetting(item);
        }

        //public short RecordCountMemberAppSettingCICD() { 
        //    throw new NotImplementedException();

        //}

        #endregion



    }
}
