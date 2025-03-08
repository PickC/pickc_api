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
    public partial class ThemeMasterBusiness : IThemeMasterBusiness
    {

        private IThemeMasterRepository repository;

        public ThemeMasterBusiness(IThemeMasterRepository repository)
        {
            this.repository = repository;
        }
        public bool Delete(long themeID)
        {
            return repository.Delete(themeID);
        }

        public ThemeMaster Get(long themeID)
        {
            return repository.Get(themeID);
        }

        public List<ThemeMaster> ListAll()
        {
            return repository.ListAll();
        }

        public ThemeMaster Save(ThemeMaster item)
        {
            return repository.Save(item);
        }
    }
}
