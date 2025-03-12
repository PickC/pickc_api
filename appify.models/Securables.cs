using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.models
{
    public partial class Securables
    {
        public short SecurableID { get; set; }
        public string PageName { get; set; }
        public Int16 PageLink { get; set; }
        public short ParentID  { get; set; }

    }

    //public partial class SecurablesFunction
    //{
    //    public Int32 FunctionID { get; set; }
    //    public Int32 SecurableID { get; set; }
    //    public string FunctionName { get; set; }
    //    public Int16 AccessLevel { get; set; }
    //    public DateTime CreatedOn { get; set; }
    //    public DateTime ModifiedOn { get; set; }
    //    public bool IsActive { get; set; }
    //}
}
