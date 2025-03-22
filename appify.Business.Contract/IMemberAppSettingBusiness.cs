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


    }
}
