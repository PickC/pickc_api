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
        public TemplateMaster SaveTemplate(TemplateMaster item);
        public bool DeleteTemplate(long templateID);
        public TemplateMaster GetTemplate(long templateID);
        public List<TemplateMaster> ListAllTemplate();
        public List<TemplatesMaster> ViewAllTemplateList();
        public TemplateThemePages GetTemplateByTheme(long templateID, long themeID);
        public ThemeMaster SaveTheme(ThemeMaster item);
        public bool DeleteTheme(long themeID);

        public ThemeMaster GetTheme(long themeID);

        public List<ThemeMaster> ListAllTheme();
    }
}
