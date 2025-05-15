using appify.models;
using appify.Business.Contract;
using appify.DataAccess.Contract;
using System;
using System.Drawing;
using static System.Net.Mime.MediaTypeNames;


namespace appify.Business
{
	public partial class BulkImportedProductBusiness : IBulkImportedProductBusiness
    {
        private IProductRepository productRepository;
        private IProductPriceRepository priceRepository;
        private IProductImageRepository imageRepository;
        private IMemberCategoryParametersRepository parametersRepository;
        IBulkImportedProductRepository repository;
        private readonly IProductBusiness productBusiness;
        public BulkImportedProductBusiness (IBulkImportedProductRepository repository, IProductRepository productRepository,
                                IProductImageRepository imageRepository,
                                IProductPriceRepository priceRepository,
                                IMemberCategoryParametersRepository parametersRepository, IProductBusiness productBusiness)
        {
			this.repository = repository;
            this.productRepository = productRepository;
            this.priceRepository = priceRepository;
            this.imageRepository = imageRepository;
            this.parametersRepository = parametersRepository;
            this.productBusiness = productBusiness;
        }
		public BulkImportedProduct GetBulkImportedProduct(Int64 itemID)
		{
			return repository.GetBulkImportedProduct(itemID);
		}

        public List<BulkImportedProduct> ListBulkImportedProduct(short vendorID, string productFileName)
		{
			return repository.ListBulkImportedProduct(vendorID,productFileName);
		}


		public List<BulkImportedProduct> SaveBulkImportedProducts(List<BulkImportedProduct> item) {

            List<BulkImportedProduct> bulkImportedProduct = new List<BulkImportedProduct>();
            foreach (BulkImportedProduct product in item)
			{
                bulkImportedProduct.Add(repository.SaveBulkImportedProduct(product));
				if (bulkImportedProduct == null) {
					break;
				}
            }

			return bulkImportedProduct;
        }
        public bool SaveBulkImportedProductsToMain(long VendorID)
		{
			return repository.SaveBulkImportedProductsToMain(VendorID);
		}
        public string checkProductFileName(string productFileName)
        {
            return repository.checkProductFileName(productFileName);
        }
        public BulkImportedProduct SaveBulkImportedProduct(BulkImportedProduct item)
		{
			return repository.SaveBulkImportedProduct(item);
		}

		public bool DeleteBulkImportedProduct(Int64 itemID)
		{
			return repository.DeleteBulkImportedProduct(itemID);
		}

		public bool SaveBulkImportedProductsToMain(List<BulkImportedProduct> item)
		{
            var result = false;

            ProductMaster productMaster = new ProductMaster();
            ProductPrice productPrice = new ProductPrice();
            ProductImage productImage = new ProductImage();
            /////// Product Master
            foreach(var productItem in item)
            {
                try
                {
                    productMaster.ProductID = 0;
                    productMaster.VendorID = productItem.VendorID;
                    productMaster.ProductName = productItem.ProductName;
                    productMaster.Description = productItem.ProductDescription;
                    productMaster.Category = Convert.ToInt32(productItem.CategoryID);
                    productMaster.Brand = productItem.BrandName;
                    productMaster.Size = productItem.Size;
                    productMaster.Color = productItem.Color;
                    productMaster.Weight = 0;
                    productMaster.HSNCode = productItem.HSNCode;
                    productMaster.UOM = 0;
                    productMaster.Currency = "INR";
                    productMaster.ImageID = 0;
                    productMaster.PriceID = 0;
                    productMaster.Source = "BULK";
                    productMaster.SKU = "";
                    productMaster.IsAvailable = true;
                    productMaster.StockQty = 0;
                    productMaster.IsNew = true;

                    productMaster = productRepository.SaveBulkImportedProduct(productMaster);
                    if(productMaster != null && productMaster.ProductID!=0)
                    {
                        var mappings = GetVariantMappings(productItem.Size, productItem.Price, productItem.Weight, productItem.Stock);
                        foreach (var itemData in mappings)
                        {
                            productPrice.PriceID = 0;
                            productPrice.ProductID = productMaster.ProductID;
                            productPrice.Size = itemData.Size;
                            productPrice.Price = itemData.Price;
                            productPrice.Stock = itemData.Stock;
                            productPrice.Weight = itemData.Weight;
                            productPrice.Discount = 0;
                            productPrice.DiscountType = 0;
                            productPrice.EffectiveDate = DateTime.Now;

                            priceRepository.SaveBulkPrice(productPrice);
                        }

                        string[] imageArray = { productItem.Image1, productItem.Image2, productItem.Image3, productItem.Image4 };

                        // Iterate using foreach
                        foreach (string image in imageArray)
                        {
                            if (String.IsNullOrEmpty(image)) { continue; }
                            productImage.ImageID = 0;
                            productImage.ProductID = productMaster.ProductID;
                            productImage.ImageName = image;
                            imageRepository.AddProductBulkImage(productImage);
                        }
                        result = true;
                        this.productBusiness.UpdateProductImagePrice(productMaster.ProductID);

                    }
                }
                catch (Exception ex)
                {
                    this.productBusiness.UpdateBulkImportedProductRemark(productItem.ItemID, "Failed",ex.Message.ToString());
                }
                this.productBusiness.UpdateBulkImportedProductRemark(productItem.ItemID, "Success", "");
            }

            return result;
        }

        public List<VariantData> GetVariantMappings(string sizesA, string pricesA, string weightsA, string stockA)
        {
            var variants = sizesA.Split(',');
            var prices = pricesA.Split(',');
            var weights = weightsA.Split(',');
            var stocks = stockA.Split(',');

            var result = new List<VariantData>();

            for (int i = 0; i < variants.Length; i++)
            {
                string variant = variants[i];

                // Use price if exists, else last price
                decimal price = i < prices.Length ? Convert.ToDecimal(prices[i]) : Convert.ToDecimal(prices[prices.Length - 1]);

                // Use weight if exists, else last weight
                decimal weight = i < weights.Length ? Convert.ToDecimal(weights[i]) : Convert.ToDecimal(weights[weights.Length - 1]);

                short stock = i < stocks.Length ? Convert.ToInt16(stocks[i]) : Convert.ToInt16(stocks[stocks.Length - 1]);
                result.Add(new VariantData
                {
                    Size = variant,
                    Price = price,
                    Weight = weight,
                    Stock = stock
                });
            }

            return result;
        }

        public List<BulkImportedProductLog> GetBulkImportedProductsLogs(long vendorID, string productFileName)
        {
            return repository.GetBulkImportedProductsLogs(vendorID, productFileName);
        }
        public List<BulkImportedProductHistory> GetBulkImportedProductsHistory(long vendorID)
        {
            return repository.GetBulkImportedProductsHistory(vendorID);
        }
    }
    public class VariantData
    {
        public string Size { get; set; }
        public decimal Price { get; set; }
        public decimal Weight { get; set; }
        public short Stock { get; set; }
    }
}
