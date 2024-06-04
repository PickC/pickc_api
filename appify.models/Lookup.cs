using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.models
{
    public partial class Lookup
    {
        public Int16 LookupID { get; set; }

        public string? LookupCode { get; set; }

        public string? LookupDescription { get; set; }

        public string? LookupCategory { get; set; }

        public bool Status { get; set; }

        public string? MappingCode { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime  CreatedOn { get; set; }

        public string? ModifiedBy { get; set; }

        public DateTime  ModifiedOn { get; set; }
    }

    public partial class SystemConfigSetting
    {
        public string? SettingValue { get; set; }
        public bool SettingStatus { get; set; }
    }
}
