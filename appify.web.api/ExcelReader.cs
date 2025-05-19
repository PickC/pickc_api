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
using NPOI.SS.Util;
using System;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using Azure.Storage.Blobs.Models;
using System.IO;
using NPOI.HPSF;
using CsvHelper;
namespace appify.web.api
{
    public class ExcelReader
    {
        List<BulkImportedProduct> products = new List<BulkImportedProduct>();

        public string UploadFile(Stream fileStream, string fileName, string folderName)
        {
            try
            {
                //// Folder Name - Vendors/VendorID/Uploads
                var connectionString = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("Azure:StorageConnectionString").Value;
                var containerName = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("Azure:ContainerName").Value;
                var blobServiceClient = new BlobServiceClient(connectionString);

                var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

                containerClient.CreateIfNotExists(PublicAccessType.Blob);
                string blobName = $"{"Vendors/" +folderName+ "/Uploads".TrimEnd('/')}/{fileName}";
                var blobClient = containerClient.GetBlobClient(blobName);
                blobClient.Upload(fileStream, overwrite: true);
                return blobClient.Uri.ToString();

            }
            catch (Exception ex)
            {
                throw new Exception($"Upload failed: {ex.Message}", ex);
            }
        }
        public List<BulkImportedProduct> ReadExcel(Stream stream, Int64 vendorID, string ProductFileName)
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
                if (row == null || row.Cells.Count == 0)
                {
                    continue;
                }

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
                        product.ProductFileName = ProductFileName;
                        product.ProductName = GetCellValue(row, columnMap["Product Name"]);
                        product.BrandName = GetCellValue(row, columnMap["Brand Name"]);
                        product.HSNCode = GetCellValue(row, columnMap["HSN Code"]);
                        product.Color = GetCellValue(row, columnMap["Color"]);
                        product.ProductDescription = GetCellValue(row, columnMap["Product Description"]);
                        product.CategoryID = GetCellValue(row, columnMap["Category ID"]);
                        product.Category = GetCellValue(row, columnMap["Category"]);
                        product.Dimension = GetCellValue(row, columnMap["Dimension"]);
                        product.Size = GetCellValue(row, columnMap["Size"]);
                        product.Price = GetCellValue(row, columnMap["Price"]);
                        product.Stock = GetCellValue(row, columnMap["Stock"]);
                        product.Weight = GetCellValue(row, columnMap["Weight"]);
                        product.Image1 = GetCellValue(row, columnMap["Image1"]);
                        product.Image2 = GetCellValue(row, columnMap["Image2"]);
                        product.Image3 = GetCellValue(row, columnMap["Image3"]);
                        product.Image4 = GetCellValue(row, columnMap["Image4"]);
                        product.Image5 = GetCellValue(row, columnMap["Image5"]);
                        product.IsActive = true;







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
                    //DownloadGoogleDriveImageAsyncNew(item.ProductName, item.Image1, index, 1);
                    //DownloadGoogleDriveImageAsyncNew(item.ProductName, item.Image2, index, 2);
                    //DownloadGoogleDriveImageAsyncNew(item.ProductName, item.Image3, index, 3);
                    //DownloadGoogleDriveImageAsyncNew(item.ProductName, item.Image4, index, 4);
                    //DownloadGoogleDriveImageAsyncNew(item.ProductName, item.Image5, index, 5);
                    DownloadAndUploadToBlob_Sync(item.Image1, item.ProductName, index, 1);
                    DownloadAndUploadToBlob_Sync(item.Image2, item.ProductName, index, 2);
                    DownloadAndUploadToBlob_Sync(item.Image3, item.ProductName, index, 3);
                    DownloadAndUploadToBlob_Sync(item.Image4, item.ProductName, index, 4);
                    DownloadAndUploadToBlob_Sync(item.Image5, item.ProductName, index, 5);

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
                    productCode = productCode.Replace(" ", "_");
                    productCode = Regex.Replace(productCode, @"[^a-zA-Z0-9_]", "");
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
                await blobContainerClient.CreateIfNotExistsAsync();


                var fileName = Path.GetFileName(url);
                var blobClient = blobContainerClient.GetBlobClient(fileName);

                using FileStream uploadFileStream = File.OpenRead(url);
                await blobClient.UploadAsync(uploadFileStream, overwrite: true);
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

        private async void DownloadGoogleDriveImageAsyncNew(string productCode, string url, short ItemNo, short col)
        {
            try
            {
                if (string.IsNullOrEmpty(url)) { return; }
                var fileId = ExtractGoogleDriveFileId(url);
                if (string.IsNullOrEmpty(fileId))
                {
                    throw new ArgumentException("Invalid Google Drive URL format");
                }

                // Build direct download URL
                var directUrl = $"https://drive.google.com/uc?export=download&id={fileId}";
                AzureUploadImagesAsyncNew(productCode,directUrl, ItemNo, col);
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

        private async void AzureUploadImagesAsyncNew(string productCode, string url, short ItemNo, short col)
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


                using var httpClient = new HttpClient();

                var response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    // Get original file extension
                    var fileExtension = GetFileExtensionFromUrl(url);
                    productCode = productCode.Replace(" ", "_");
                    productCode = Regex.Replace(productCode, @"[^a-zA-Z0-9_]", "");
                    // Generate filename: {ProductCode}_{Timestamp}.{ext}
                    var fileName = $"{productCode}_{DateTime.Now:yyyyMMddHHmmss}{fileExtension}";
                    //var fullPath = Path.Combine(productFolder, fileName);
                    //var fileName = Path.GetFileName(url);
                    var blobClient = blobContainerClient.GetBlobClient(fileName);

                    using var stream = await response.Content.ReadAsStreamAsync();
                     blobClient.UploadAsync(stream, overwrite: true);

                    stream.Close();
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
                }

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

        public void DownloadAndUploadToBlob_Sync(string googleDriveUrl, string blobName, short ItemNo, short col)
        {
            if (string.IsNullOrEmpty(googleDriveUrl)) { return; }
            using (var httpClient = new HttpClient())
            {
                
                var fileId = ExtractGoogleDriveFileId(googleDriveUrl);
                if (string.IsNullOrEmpty(fileId))
                {
                    throw new ArgumentException("Invalid Google Drive URL format");
                }

                // Build direct download URL
                var directUrl = $"https://drive.google.com/uc?export=download&id={fileId}";
                // Download the file synchronously
                var response = httpClient.GetAsync(directUrl).GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();

                using (var stream = response.Content.ReadAsStreamAsync().GetAwaiter().GetResult())
                {
                    // Upload synchronously
                    var fileExtension = GetFileExtensionFromUrl(directUrl);
                    blobName = blobName.Replace(" ", "_");
                    blobName = Regex.Replace(blobName, @"[^a-zA-Z0-9_]", "");
                    // Generate filename: {ProductCode}_{Timestamp}.{ext}
                    var fileName = $"{blobName}_{DateTime.Now:yyyyMMddHHmmss}{fileExtension}";
                    var blobUrl = UploadToBlob_Sync(stream, fileName);
                    if (col == 1)
                        products[ItemNo].Image1 = blobUrl;
                    else if (col == 2)
                        products[ItemNo].Image2 = blobUrl;
                    else if (col == 3)
                        products[ItemNo].Image3 = blobUrl;
                    else if (col == 4)
                        products[ItemNo].Image4 = blobUrl;
                    else if (col == 5)
                        products[ItemNo].Image5 = blobUrl;
                    //return blobUrl;
                }
            }
        }

        public byte[] GenerateExcel(List<BulkImportedProductLog> itemData)
        {
            using (var memoryStream = new MemoryStream())
            {
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("Product Logs");

                IRow headerRow = sheet.CreateRow(0);
                string[] headers = { "SL No.", "Product Name", "Brand Name", "HSN Code", "Error", "Remark" };

                for (int i = 0; i < headers.Length; i++)
                {
                    headerRow.CreateCell(i).SetCellValue(headers[i]);
                }

                int rowIndex = 1;
                foreach(var log in itemData)
                {
                    IRow row = sheet.CreateRow(rowIndex++);
                    row.CreateCell(0).SetCellValue(log.ItemNo ?? 0);
                    row.CreateCell(1).SetCellValue(log.ProductName ?? string.Empty);
                    row.CreateCell(2).SetCellValue(log.BrandName ?? string.Empty);
                    row.CreateCell(3).SetCellValue(log.HSNCode ?? string.Empty);
                    row.CreateCell(4).SetCellValue(log.Error ?? string.Empty);
                    row.CreateCell(5).SetCellValue(log.Remarks ?? string.Empty);
                }

                for (int i = 0; i < headers.Length; i++)
                {
                    sheet.AutoSizeColumn(i);
                }

                workbook.Write(memoryStream);
                return memoryStream.ToArray();
                //var fileExtension = ".xlsx";
                //var fileName = $"{"Report"}_{DateTime.Now:yyyyMMddHHmmss}{fileExtension}";
                //var uploadedUrl = UploadFile(memoryStream, fileName, itemData[0].VendorID.ToString());

                //string downloadPath = @"C:\Downloads";
                //if (!Directory.Exists(downloadPath))
                //{
                //    Directory.CreateDirectory(downloadPath);
                //}
                //string fileName = $"ProductLog_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                //filePath = Path.Combine(downloadPath, fileName);
                //using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                //{
                //    workbook.Write(fileStream);
                //}
            }

        }
            public string UploadToBlob_Sync(Stream fileStream, string blobName)
        {
            var connectionString = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("Azure:StorageConnectionString").Value;
            var containerName = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("Azure:ContainerName").Value;

            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            blobClient.Upload(fileStream, overwrite: true); // Synchronous upload

            return blobClient.Uri.ToString();
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
