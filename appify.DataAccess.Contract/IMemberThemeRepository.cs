using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.DataAccess.Contract
{
    public interface IMemberThemeRepository
    {
        public MemberTheme Save(MemberTheme item);
        public bool Delete(long memberID,long themeID);

        public MemberTheme Get(long memberID,long themeID);

        public List<MemberTheme> ListAll();
    }
}
