using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.models
{
    public class MemberTheme 
    {
        public Int64 MemberID { get; set; }
        public Int64 TemplateID {  get; set; }
        public string Template { get; set; }
        public Int64 ThemeID { get; set; }
        public Int64 CreatedBy { get; set; }
        public Int64 ModifiedBy { get; set; }
        public bool IsActive { get; set; }
        public List<TemplateThemesMember> Themes { get; set; }
    }

    public class MemberThemeHeader
    {
        public Int64 MemberID { get; set; }
        public Int64 TemplateID { get; set; }
        public Int64 ThemeID { get; set; }
        public Int64 CreatedBy { get; set; }
        public Int64 ModifiedBy { get; set; }
        public bool IsActive { get; set; }
    }
}
