using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.Business.Contract
{
    public interface IMemberThemeBusiness
    {
        public MemberThemeHeader Save(MemberThemeHeader item);
        public bool Delete(long memberID, long templateID, long themeID);

        public MemberTheme Get(long memberID);

        public List<MemberTheme> ListAll();
    }
}
