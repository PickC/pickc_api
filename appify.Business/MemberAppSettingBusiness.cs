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

        public List<MemberAppSetting> ListMemberAppSetting(long userID)
        {
            return memberAppSettingRepository.ListMemberAppSetting(userID);
        }

        public bool SaveMemberAppSetting(MemberAppSetting item)
        {
            return memberAppSettingRepository.SaveMemberAppSetting(item);
        }
    }
}
