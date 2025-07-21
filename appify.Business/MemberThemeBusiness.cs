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

        public bool Delete(long memberID, long templateID, long themeID)
        {
            return repository.Delete(memberID, templateID, themeID);
        }

        public MemberTheme Get(long memberID)
        {
            MemberTheme item = new MemberTheme();
            TemplateThemesMember templateThemes = new TemplateThemesMember();
            item = repository.Get(memberID);
            if (item != null)
            {
                item.Themes = repository.ListAllThemesByTemplate(item.TemplateID);
            }
            return item;
        }

        public List<MemberTheme> ListAll()
        {
            return repository.ListAll();
        }

        public MemberThemeHeader Save(MemberThemeHeader item)
        {
            return repository.Save(item);
        }
    }
}
