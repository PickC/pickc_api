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
        public Int64 ThemeID { get; set; }
        public string PrimaryColor { get; set; }
        public string PrimaryLightColor { get; set; }
        public string BackgroundBoxColor { get; set; }
        public string TextColor { get; set; }
        public string SecondaryColor { get; set; }
        public string ScaffoldBgColor { get; set; }
        public bool IsDark { get; set; }
    }
}
