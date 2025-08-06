using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.models
{
    public partial class RoleRights
    {
        public short RoleID { get; set; }
        public short SecurableID { get; set; }

        public bool IsAdd { get; set; }
        public bool IsEdit { get; set; }
        public bool IsView { get; set; }
        public bool IsDelete { get; set; }
        public bool IsDownload { get; set; }
        public Int16 CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public Int16 ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }

        public string? PageLink { get; set; }
        public string? PageName { get; set; }
        public string? ParentName { get; set; }
    }
}
