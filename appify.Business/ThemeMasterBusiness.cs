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

        public TemplateMaster SaveTemplate(TemplateMaster item)
        {
            return repository.SaveTemplate(item);
        }
        public bool DeleteTemplate(long templateID)
        {
            return repository.DeleteTemplate(templateID);
        }
        public TemplateMaster GetTemplate(long templateID)
        {
            return repository.GetTemplate(templateID);
        }
        public List<TemplateMaster> ListAllTemplate()
        {
            return repository.ListAllTemplate();
        }
        public List<TemplatesMaster> ViewAllTemplateList()
        {
            List <TemplatesMaster> templaItem = new List<TemplatesMaster>();
            List<TemplateThemes> templateThemes = new List<TemplateThemes>();
            templaItem = repository.ViewAllTemplateList();
            if (templaItem.Any()==true)
            {
                foreach (var theme in templaItem)
                {
                    theme.Themes = repository.ListAllThemesByTemplate(theme.TemplateID);
                }
            }
            return templaItem;
        }
        public TemplateThemePages GetTemplateByTheme(long templateID, long themeID)
        {
            return repository.GetTemplateByTheme(templateID, themeID);
        }
        public bool DeleteTheme(long themeID)
        {
            return repository.DeleteTheme(themeID);
        }

        public ThemeMaster GetTheme(long themeID)
        {
            return repository.GetTheme(themeID);
        }

        public List<ThemeMaster> ListAllTheme()
        {
            return repository.ListAllTheme();
        }

        public ThemeMaster SaveTheme(ThemeMaster item)
        {
            return repository.SaveTheme(item);
        }
    }
}
