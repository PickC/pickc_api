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

        public bool DeleteAppSetting(long userID,string appName)
        {
            return memberAppSettingRepository.DeleteAppSetting(userID, appName);
        }

        public MemberAppSetting GetAppSetting(long userID,string appName)
        {
            return memberAppSettingRepository.GetAppSetting(userID, appName);
        }

        public List<MemberAppSetting> GetAppSettingList(long userID)
        {
            return memberAppSettingRepository.GetAppSettingList(userID);
        }

        public bool saveAppSetting(MemberAppSetting item)
        {
            return memberAppSettingRepository.saveAppSetting(item);
        }
    }
}
