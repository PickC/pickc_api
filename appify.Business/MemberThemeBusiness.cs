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
    public partial class MemberThemeBusiness : IMemberThemeBusiness
    {
        private IMemberThemeRepository repository;

        public MemberThemeBusiness(IMemberThemeRepository repository)
        {
            this.repository = repository;
        }

        public bool Delete(long memberID, long themeID)
        {
            return repository.Delete(memberID, themeID);
        }

        public MemberTheme Get(long memberID, long themeID)
        {
            return repository.Get(memberID, themeID);
        }

        public List<MemberTheme> ListAll()
        {
            return repository.ListAll();
        }

        public MemberTheme Save(MemberTheme item)
        {
            return repository.Save(item);
        }
    }
}
