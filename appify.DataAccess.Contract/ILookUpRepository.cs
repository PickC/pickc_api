using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.DataAccess.Contract
{
    public interface ILookUpRepository
    {
        public bool HasLookUp(short lookupID);
        public bool SaveLookUp(Lookup item);
        public bool DeleteLookUp(short lookupID);

        public Lookup GetLookUp(short lookupID);
        public List<Lookup> GetList(string category);
        public List<SystemConfigSetting> GetSystemConfigurationSettings(string SettingKey);
        public List<Lookup> GetList(string category,string userID);

        public List<Lookup> GetAllList();
    }
}
