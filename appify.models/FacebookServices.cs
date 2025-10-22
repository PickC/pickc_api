using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.models
{
    public partial class MetaBusinessProfile
    {
        public short MTBID { get; set; }
        public string BusinessID { get; set; }
        public long VendorID { get; set; }
        public string BusinessName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string BusinessMobileNo { get; set; }
        public string AccessToken { get; set; }
        public DateTime ExpiryDate { get; set; }
        public short CreatedBy { get; set; }
        public short ModifiedBy { get; set; }
        public bool IsActive { get; set; }
    }
    public partial class MetaProduct
    {
        public short MTPID { get; set; }
        public string ProductID { get; set; }
        public long VendorID { get; set; }
        public string BusinessID { get; set; }
        public string CatalogID { get; set; }
        public long RetailerID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }
        public string Brand { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public string Availability { get; set; }
        public short CreatedBy { get; set; }
        public short ModifiedBy { get; set; }
        public bool IsActive { get; set; }
    }

    public partial class EventRequest
    {
        public string EventName { get; set; }
        public string EventId { get; set; }       // optional, we generate if missing
        public IEnumerable<string> ContentIds { get; set; }
        public object[] Contents { get; set; }    // flexible contents structure
        public decimal? Value { get; set; }
        public string Currency { get; set; }
        public string Fbp { get; set; }
        public string Fbc { get; set; }
        // PII should not come raw from client ideally. If you must accept it, hash here.
        public string Email { get; set; }
        public string Phone { get; set; }
        // optional: client IP and user agent if available
    }
    public partial class MetaApiConfig
    {
        public string APIUrl { get; set; }
        public string APIVersion { get; set; }
        public string AccessToken { get; set; }

    }
    public partial class MetaBusinessConfig
    {
        public short MTBID { get; set; }
        public string BusinessID { get; set; }
        public long VendorID { get; set; }
        public string BusinessName { get; set; }
        public string AccessToken { get; set; }
        public string PixelID { get; set; }
        public string PixelAccessToken { get; set; }
    }
    public partial class MetaCatalogConfig
    {
        public short MTCID { get; set; }
        public string CatalogID { get; set; }
        public long VendorID { get; set; }
        public string BusinessID { get; set; }
        public string CatalogName { get; set; }
    }
    public partial class MetaCatalog
    {
        public short MTCID { get; set; }
        public long VendorID { get; set; }
        public string CatalogID { get; set; }
        public string BusinessID { get; set; }
        public string CatalogName { get; set; }
        public short CreatedBy { get; set; }
        public short ModifiedBy { get; set; }
        public bool IsActive { get; set; }
    }
    public partial class ParamProductsFromCatalog
    {
        public long VendorID { get; set; }
        public string BusinessID { get; set; }
        public string CatalogID { get; set; }
    }
    public partial class ParamAllProductsToCatalog
    {
        public long VendorID { get; set; }
        public short SourceID { get; set; }
        public string BusinessID { get; set; }
        public string CatalogID { get; set; }
    }
    public partial class MetaBusinessProfileDelete
    {
        public short MTBID { get; set; }
        public long VendorID { get; set; }
        public string BusinessID { get; set; }
    }
    public partial class MetaCatalogDelete
    {
        public short MTCID { get; set; }
        public long VendorID { get; set; }
        public string BusinessID { get; set; }
        public string CatalogID { get; set; }
    }
    public partial class MetaCatalogProuctDelete
    {
        public short MTPID { get; set; }
        public long VendorID { get; set; }
        public string BusinessID { get; set; }
        public string ProductID { get; set; }
    }
    public partial class MetaCatalogProduct
    {
        public short MTPID { get; set; }
        public string ProductID { get; set; }
        public long VendorID { get; set; }
        public string BusinessID { get; set; }
        public string CatalogID { get; set; }
        public long RetailerID { get; set; }
        public short CreatedBy { get; set; }
        public short ModifiedBy { get; set; }
        public bool IsActive { get; set; }
    }
    public partial class InstagramAccount
    {
        public string CatalogID { get; set; }
        public string InstagramID { get; set; }
    }
}
