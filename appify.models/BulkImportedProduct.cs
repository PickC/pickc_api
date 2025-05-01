using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace appify.models
{
	public partial class BulkImportedProduct
	{
		public BulkImportedProduct() { }


		public Int64  ItemID { get; set; }
		public string?  ProductFileName { get; set; }
		public Int16?  ItemNo { get; set; }
		public Int16?  VendorID { get; set; }
		public string? ProductName { get; set; }
		public string? BrandName { get; set; }
		public string?  HSNCode { get; set; }
		public string?  Color { get; set; }
		public string? ProductDescription { get; set; }
		public string?  CategoryID { get; set; }
		public string?  Category { get; set; }
		public string?  Dimension { get; set; }
		public decimal? Price { get; set; }
		public Int32?  Stock { get; set; }
		public string?  Weight { get; set; }
		public string?  Image1 { get; set; }
		public string?  Image2 { get; set; }
		public string?  Image3 { get; set; }
		public string?  Image4 { get; set; }
		public string?  Image5 { get; set; }
		public DateTime?  CreatedOn { get; set; }
		public DateTime?  ModifiedOn { get; set; }
		public string?  Remarks { get; set; }
		public string?  ErrorMessage { get; set; }
		public bool?  IsActive { get; set; }
	}
}




