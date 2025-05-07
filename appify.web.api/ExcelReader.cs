using Microsoft.AspNetCore.Mvc;
using NPOI;
using NPOI.OOXML;
using NPOI.SS.UserModel;
using NPOI.XSSF.Model;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using NPOI.XSSF.UserModel;
using System.Net;
using appify.models;
using Azure.Storage.Blobs;
using System.Security.Policy;
using System.Text.RegularExpressions;
using Google.Api.Gax.ResourceNames;
using NPOI.SS.Formula.Functions;
namespace appify.web.api
{
    public class ExcelReader
    {
        List<BulkImportedProduct> products = new List<BulkImportedProduct>();
        public List<BulkImportedProduct> ReadExcel(Stream stream, Int64 vendorID = 0)
        {


            // NPOI Workbook initialization
            IWorkbook workbook = new XSSFWorkbook(stream);
            ISheet sheet = workbook.GetSheetAt(0); // First sheet
            IRow headerRow = sheet.GetRow(0);

            // Column index mapping
            var columnMap = new Dictionary<string, int>
        {
            {"SL No.",-1 },
            { "Product Name", -1 },
            { "Brand Name", -1 },
            { "HSN Code", -1 },
            { "Color", -1 },
            { "Product Description", -1 },
            { "Category ID", -1 },
            { "Category", -1 },
            { "Dimension", -1 },
            { "Size", -1 },
            { "Price", -1 },
            { "Stock", -1 },
            { "Weight", -1 },
            { "Image1",    -1 },
            { "Image2",    -1 },
            { "Image3",    -1 },
            { "Image4",    -1 },
            { "Image5",    -1 },
        };

            // Find column indexes
            for (int i = 0; i < headerRow.LastCellNum; i++)
            {
                var cellValue = headerRow.GetCell(i)?.ToString()?.Trim();
                if (cellValue != null && columnMap.ContainsKey(cellValue))
                {
                    columnMap[cellValue] = i;
                }
            }


            // Read data rows
            for (int rowIdx = 1; rowIdx <= sheet.LastRowNum; rowIdx++)
            {
                IRow row = sheet.GetRow(rowIdx);
                if (row == null) continue;

                try
                {
                    if (!string.IsNullOrEmpty(GetCellValue(row, columnMap["Product Name"]).ToString()))
                    {

                        //var product = new BulkImportedProduct
                        //{
                        //    VendorID = vendorID,
                        //    ItemNo = GetCellValue(row, columnMap["SL No."]).ToString().Length>0? Convert.ToInt16(GetCellValue(row, columnMap["SL No."]).ToString()) :Convert.ToInt16(0),
                        //    ProductName = GetCellValue(row, columnMap["Product Name"]),
                        //    BrandName = GetCellValue(row, columnMap["Brand Name"]),
                        //    HSNCode = GetCellValue(row, columnMap["HSN Code"]),
                        //    Color = GetCellValue(row, columnMap["Color"]),
                        //    ProductDescription = GetCellValue(row, columnMap["Product Description"]),
                        //    CategoryID = GetCellValue(row, columnMap["Category ID"]),
                        //    Category = GetCellValue(row, columnMap["Category"]),
                        //    Dimension = GetCellValue(row, columnMap["Dimension"]),
                        //    Size = GetCellValue(row, columnMap["Size"]),
                        //    Price = GetCellValue(row, columnMap["Price"]).ToString().Length > 0 ? Convert.ToInt16(GetCellValue(row, columnMap["Price"]).ToString()) : Convert.ToInt16(0),
                        //    Stock = GetCellValue(row, columnMap["Stock"]).ToString().Length > 0 ? Convert.ToInt16(GetCellValue(row, columnMap["Stock"]).ToString()) : Convert.ToInt16(0),
                        //    Weight = GetCellValue(row, columnMap["Weight"]),
                        //    Image1 = GetCellValue(row, columnMap["Image1"]),
                        //    Image2 = GetCellValue(row, columnMap["Image2"]),
                        //    Image3 = GetCellValue(row, columnMap["Image3"]),
                        //    Image4 = GetCellValue(row, columnMap["Image4"]),
                        //    Image5 = GetCellValue(row, columnMap["Image5"]),



                        //};

                        var product = new BulkImportedProduct();

                        product.VendorID = vendorID;
                        product.ItemNo = GetCellValue(row, columnMap["SL No."]).ToString().Length > 0 ? Convert.ToInt16(GetCellValue(row, columnMap["SL No."]).ToString()) : Convert.ToInt16(0);
                        product.ProductName = GetCellValue(row, columnMap["Product Name"]);
                        product.BrandName = GetCellValue(row, columnMap["Brand Name"]);
                        product.HSNCode = GetCellValue(row, columnMap["HSN Code"]);
                        product.Color = GetCellValue(row, columnMap["Color"]);
                        product.ProductDescription = GetCellValue(row, columnMap["Product Description"]);
                        product.CategoryID = GetCellValue(row, columnMap["Category ID"]);
                        product.Category = GetCellValue(row, columnMap["Category"]);
                        product.Dimension = GetCellValue(row, columnMap["Dimension"]);
                        product.Size = GetCellValue(row, columnMap["Size"]);
                        product.Price = GetCellValue(row, columnMap["Price"]).ToString().Length > 0 ? Convert.ToInt16(GetCellValue(row, columnMap["Price"]).ToString()) : Convert.ToInt16(0);
                        product.Stock = GetCellValue(row, columnMap["Stock"]).ToString().Length > 0 ? Convert.ToInt16(GetCellValue(row, columnMap["Stock"]).ToString()) : Convert.ToInt16(0);
                        product.Weight = GetCellValue(row, columnMap["Weight"]);
                        product.Image1 = GetCellValue(row, columnMap["Image1"]);
                        product.Image2 = GetCellValue(row, columnMap["Image2"]);
                        product.Image3 = GetCellValue(row, columnMap["Image3"]);
                        product.Image4 = GetCellValue(row, columnMap["Image4"]);
                        product.Image5 = GetCellValue(row, columnMap["Image5"]);








                        if (!string.IsNullOrEmpty(product.ProductName))
                        {
                            products.Add(product);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log row error but continue processing
                    Console.WriteLine($"Error processing row {rowIdx + 1}: {ex.Message}");
                }
            }

            // download the images from google drive



            if (products.Count > 0)
            {
                short index = 0;
                foreach (var item in products)
                {
                    DownloadGoogleDriveImageAsync(item.ProductName, item.Image1, index, 1);
                    DownloadGoogleDriveImageAsync(item.ProductName, item.Image2, index, 2);
                    DownloadGoogleDriveImageAsync(item.ProductName, item.Image3, index, 3);
                    DownloadGoogleDriveImageAsync(item.ProductName, item.Image4, index, 4);
                    DownloadGoogleDriveImageAsync(item.ProductName, item.Image5, index, 5);
                    index += 1;
                }

            }


            return products;
        }

        private string GetCellValue(IRow row, int colIndex)
        {
            return colIndex >= 0 ? row.GetCell(colIndex)?.ToString()?.Trim() : null;
        }

        private decimal TryParseDecimal(string value)
        {
            return decimal.TryParse(value, out var result) ? result : 0;
        }

        private int TryParseInt(string value)
        {
            return int.TryParse(value, out var result) ? result : 0;
        }


        private async void DownloadGoogleDriveImageAsync(string productCode, string url, short ItemNo, short col)
        {
            string baseFolder = @"C:\Downloads";
            try
            {
                if (string.IsNullOrEmpty(url)) { return; }
                // Create product-specific folder if it doesn't exist
                productCode = SanitizeFolderName(productCode);
                var productFolder = Path.Combine(baseFolder, productCode);
                if (!Directory.Exists(productFolder))
                {
                    Directory.CreateDirectory(productFolder);
                    Console.WriteLine($"Created folder: {productFolder}");
                }

                using (var client = new WebClient())
                {
                    // Extract file ID from URL (handles multiple URL formats)
                    var fileId = ExtractGoogleDriveFileId(url);
                    if (string.IsNullOrEmpty(fileId))
                    {
                        throw new ArgumentException("Invalid Google Drive URL format");
                    }

                    // Build direct download URL
                    var directUrl = $"https://drive.google.com/uc?export=download&id={fileId}";

                    // Get original file extension
                    var fileExtension = GetFileExtensionFromUrl(url);

                    // Generate filename: {ProductCode}_{Timestamp}.{ext}
                    var fileName = $"{productCode}_{DateTime.Now:yyyyMMddHHmmss}{fileExtension}";
                    var fullPath = Path.Combine(productFolder, fileName);

                    // Download with timeout
                    client.DownloadFile(directUrl, fullPath);
                    Console.WriteLine($"Downloaded: {fullPath}");
                    AzureUploadImagesAsync(fullPath, ItemNo, col);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to download {url}");
                Console.WriteLine($"Details: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
            }
        }

        private async void AzureUploadImagesAsync(string url, short ItemNo, short col)
        {
            string storageConnectionString;
            string containerName;
            var UploadedUrl = "";
            try
            {
                storageConnectionString = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("Azure:StorageConnectionString").Value;
                containerName = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("Azure:ContainerName").Value;

                var blobContainerClient = new BlobContainerClient(storageConnectionString, containerName);
                blobContainerClient.CreateIfNotExistsAsync();


                var fileName = Path.GetFileName(url);
                var blobClient = blobContainerClient.GetBlobClient(fileName);

                using FileStream uploadFileStream = File.OpenRead(url);
                blobClient.UploadAsync(uploadFileStream, overwrite: true);
                uploadFileStream.Close();
                if (col == 1)
                    products[ItemNo].Image1 = blobClient.Uri.ToString();
                else if (col == 2)
                    products[ItemNo].Image2 = blobClient.Uri.ToString();
                else if (col == 3)
                    products[ItemNo].Image3 = blobClient.Uri.ToString();
                else if (col == 4)
                    products[ItemNo].Image4 = blobClient.Uri.ToString();
                else if (col == 5)
                    products[ItemNo].Image5 = blobClient.Uri.ToString();
                //UploadedUrl = blobClient.Uri.ToString();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to upload {url}");
                Console.WriteLine($"Details: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
            }
        }

        public static string SanitizeFolderName(string folderName)
        {
            // Remove or replace characters invalid in Windows folder names
            string invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            string invalidReStr = $"[{invalidChars}]";

            return Regex.Replace(folderName, invalidReStr, "_");
        }
        private string ExtractGoogleDriveFileId(string url)
        {
            // Handle multiple Google Drive URL formats:
            // 1. https://drive.google.com/file/d/FILE_ID/view?usp=sharing
            // 2. https://drive.google.com/open?id=FILE_ID
            // 3. https://docs.google.com/uc?id=FILE_ID

            var patterns = new[]
            {
        new { Prefix = "/file/d/", Suffix = "/" },
        new { Prefix = "?id=", Suffix = "&" },
        new { Prefix = "uc?id=", Suffix = "&" }
    };

            foreach (var pattern in patterns)
            {
                int start = url.IndexOf(pattern.Prefix);
                if (start >= 0)
                {
                    start += pattern.Prefix.Length;
                    int end = url.IndexOf(pattern.Suffix, start);
                    if (end == -1) end = url.Length;
                    return url.Substring(start, end - start);
                }
            }

            return null;
        }

        private string GetFileExtensionFromUrl(string url)
        {
            try
            {
                // Try to get extension from URL
                var uri = new Uri(url);
                var path = uri.AbsolutePath;
                var ext = Path.GetExtension(path);

                // Default to .jpg if extension can't be determined
                return string.IsNullOrEmpty(ext) ? ".jpg" : ext;
            }
            catch
            {
                return ".jpg";
            }
        }


    }


    public class ProductDto
    {
        public short SLNo { get; set; }
        public string ProductName { get; set; }
        public string BrandName { get; set; }
        public string HSNCode { get; set; }
        public string Color { get; set; }
        public string ProductDescription { get; set; }
        public string CategoryID { get; set; }
        public string Category { get; set; }
        public string Dimension { get; set; }
        public string Size { get; set; }
        public string Price { get; set; }
        public string Stock { get; set; }
        public string Weight { get; set; }
        public string Image1 { get; set; }
        public string Image2 { get; set; }
        public string Image3 { get; set; }
        public string Image4 { get; set; }
        public string Image5 { get; set; }
    }
}
