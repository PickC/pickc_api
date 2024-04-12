using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.Business.Contract
{
    public interface IThemeMasterBusiness
    {
        public ThemeMaster Save(ThemeMaster item);
        public bool Delete(long themeID);

        public ThemeMaster Get(long themeID);

        public List<ThemeMaster> ListAll();
    }
}
