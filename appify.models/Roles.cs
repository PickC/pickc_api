using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.models
{
    public partial class Roles
    {
        public short RoleID { get; set; }
        public string RoleCode { get; set; }
        public string RoleDescription { get; set; }
        public bool IsActive { get; set; }
        public Int16 CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public Int16 ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }

        public List<RoleRights>? RoleRights { get; set; }

    }

}
