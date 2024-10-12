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
    public partial class LookupBusiness : ILookupBusiness
    {
        private ILookUpRepository repository;

        public LookupBusiness(ILookUpRepository repository)
        {
            this.repository = repository;
        }

        public bool DeleteLookUp(long lookupID)
        {
            return repository.DeleteLookUp(lookupID);
        }

        public List<Lookup> GetAllList()
        {
            return repository.GetAllList();
        }

        public List<SystemConfigSetting> GetSystemConfigurationSettings(string SettingKey)
        {
            return repository.GetSystemConfigurationSettings(SettingKey);
        }

        public List<Lookup> GetList(string category)
        {
            return repository.GetList(category);
        }

        public List<Lookup> GetList(string category, string userID)
        {
            return repository.GetList(category,userID);
        }

        public Lookup GetLookUp(short lookupID)
        {
            return repository.GetLookUp(lookupID);
        }

        public Lookup GetLookUp(string lookupCode, string category)
        {
            return repository.GetLookUp(lookupCode, category);
        }
        public bool HasLookUp(short lookupID)
        {
            throw new NotImplementedException();
        }

        public Lookup SaveLookUp(Lookup item)
        {
            return repository.SaveLookUp(item);

        }

        public List<LookupStartUpList> GetListForStartup(string category)
        {
            return repository.GetListForStartup(category);
        }
    }
}
