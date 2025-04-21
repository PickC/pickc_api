using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace appify.models
{
	public partial class MemberCategoryParameters
	{
		public MemberCategoryParameters() { }


		public Int64  ID { get; set; }
		public Int64  UserID { get; set; }
		public Int64  ProductID { get; set; }
		public Int64  ParameterID { get; set; }
		public string  ParameterValue { get; set; }
		public DateTime  CreatedOn { get; set; }
		public bool  IsActive { get; set; }

        public string? ParameterName { get; set; }
    }

    public partial class MemberCategoryParametersLite
    {
        public MemberCategoryParametersLite() { }

        public string? ParameterName { get; set; }
        public string ParameterValue { get; set; }


    }

}




