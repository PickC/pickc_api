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

namespace appify.web.api
{
    public class ExcelReader
    {
        public List<ProductDto> ReadExcel(Stream stream)
        {
            var products = new List<ProductDto>();

            // NPOI Workbook initialization
            IWorkbook workbook = new XSSFWorkbook(stream);
            ISheet sheet = workbook.GetSheetAt(0); // First sheet
            IRow headerRow = sheet.GetRow(0);

            // Column index mapping
            var columnMap = new Dictionary<string, int>
        {
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
            { "image1",    -1 },
            { "image2",    -1 },
            { "image3",    -1 },
            { "image4",    -1 },
            { "image5",    -1 },
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

                        var product = new ProductDto
                        {
                            ProductName = GetCellValue(row, columnMap["Product Name"]),
                            BrandName = GetCellValue(row, columnMap["Brand Name"]),
                            HSNCode = GetCellValue(row, columnMap["HSN Code"]),
                            Color = GetCellValue(row, columnMap["Color"]),
                            ProductDescription = GetCellValue(row, columnMap["Product Description"]),
                            CategoryID = GetCellValue(row, columnMap["Category ID"]),
                            Category = GetCellValue(row, columnMap["Category"]),
                            Dimension = GetCellValue(row, columnMap["Dimension"]),
                            Size = GetCellValue(row, columnMap["Size"]),
                            Price = GetCellValue(row, columnMap["Price"]),
                            Stock = GetCellValue(row, columnMap["Stock"]),
                            Weight = GetCellValue(row, columnMap["Weight"]),
                            Image1 = GetCellValue(row, columnMap["image1"]),
                            Image2 = GetCellValue(row, columnMap["image2"]),
                            Image3 = GetCellValue(row, columnMap["image3"]),
                            Image4 = GetCellValue(row, columnMap["image4"]),
                            Image5 = GetCellValue(row, columnMap["image5"]),

                        };


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
                foreach (var item in products)
                {
                    DownloadGoogleDriveImage(item.ProductName, item.Image2);
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


        private void DownloadGoogleDriveImage(string productCode, string url)
        {
            string baseFolder = @"D:\Downloads";

            try
            {
                // Create product-specific folder if it doesn't exist
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
