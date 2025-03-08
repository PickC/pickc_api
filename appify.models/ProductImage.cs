using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.models
{
    public partial class ProductImage
    {
        public Int64 ImageID { get; set; }

        public Int64 ProductID { get; set; }

        public string? ImageName { get; set; }

        public bool? IsActive { get; set; }

        public Int16? ContentType { get; set; }

        public DateTime? CreatedOn { get; set; }

    }

    public partial class ProductImageNew
    {
        public Int64 ImageID { get; set; }

        public string? ImageName { get; set; }

    }
}
