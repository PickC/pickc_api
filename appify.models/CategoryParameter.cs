using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.models
{
    public partial class CategoryParameter
    {
        public long ParameterID { get; set; }
        public long CategoryID { get; set; }
        public string ParameterName { get; set; }

        public string? HintText { get; set; }
        public string? ParameterType { get; set; }
        public bool IsActive { get; set; }
        public Int16 CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public Int16 ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string Category { get; set; }
        public long ParentID { get; set; }
        public string ParentCategory { get; set; }


    }

    public partial class CategoryParameterLite
    {
        public long ParameterID { get; set; }
        public string ParameterName { get; set; }

        public string? HintText { get; set; }
        public string? ParameterType { get; set; }
    

    }


    public partial class ParameterType
    {
        public long ParameterID { get; set; }
        public bool IsMultipleValue { get; set; }
        public string ParameterName { get; set; }


    }
}
