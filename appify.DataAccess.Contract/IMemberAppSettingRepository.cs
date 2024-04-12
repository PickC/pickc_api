using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using appify.models;

namespace appify.DataAccess.Contract
{
    public interface IMemberAppSettingRepository
    {
        public MemberAppSetting GetAppSetting(long userID, string appName);
        public List<MemberAppSetting> GetAppSettingList(long userID);
        public bool DeleteAppSetting(long userID,string appName);
        public bool saveAppSetting(MemberAppSetting item);
    }
}
