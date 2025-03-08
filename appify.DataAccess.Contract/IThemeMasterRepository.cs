using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.DataAccess.Contract
{
    public interface IThemeMasterRepository
    {
        public ThemeMaster Save(ThemeMaster item);
        public bool Delete(long themeID);

        public ThemeMaster Get(long themeID); 

        public List<ThemeMaster> ListAll();

    }
}
