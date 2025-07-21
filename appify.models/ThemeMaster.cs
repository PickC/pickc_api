using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.models
{
    public class TemplateMaster
    {
        public Int64 TemplateID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Banner { get; set; }
        public string Code { get; set; }
        public bool IsActive { get; set; }
    }

    public class TemplatesMaster
    {
        public Int64 TemplateID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Banner { get; set; }
        public List<TemplateThemes> Themes { get; set; }
    }
    public class TemplateThemes
    {
        public Int64 ThemeID { get; set; }
        public string ThemeName { get; set; }
        public string ThemeColor { get; set; }
        public string DarkThemeColor { get; set; }
        public string ThemePagesJSON { get; set; }
    }

    public class TemplateThemesMember
    {
        public Int64 ThemeID { get; set; }
        public string ThemeName { get; set; }
        public string ThemeColor { get; set; }
        public string DarkThemeColor { get; set; }
        public string ThemeJSON { get; set; }
    }
    public class TemplateThemePages
    {
        public string pages { get; set; }
    }
    public class ThemeMaster
    {
        public Int64 ThemeID { get; set; }
        public string ThemeName { get; set; }
        public string ThemeColor { get; set; }
        public string DarkThemeColor { get; set; }
        public Int64 TemplateID { get; set; }
        public string ThemeJSON { get; set; }
        public string ThemePagesJSON { get; set; }
        public bool IsThemeAvailable { get; set; }
        public bool IsDark { get; set; }
        public bool IsActive { get; set; }
    }
}
