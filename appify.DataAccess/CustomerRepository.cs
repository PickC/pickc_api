using appify.DataAccess.Contract;
using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Data;
using appify.utility;
using appify.dbroutine;
using System.Dynamic;
using Microsoft.Data.SqlClient;
using Azure;

namespace appify.DataAccess
{
    public class CustomerRepository : ICustomerRepository
    {
        private IConfiguration configuration;
        private string appify_connectionstring;

        public CustomerRepository(IConfiguration config)
        {
            this.configuration = config;
            this.appify_connectionstring = config["ConnectionStrings:appify.connectionstring"].ToString();
        }

        public List<Member> GetAllCustomersByVendor(long vendorID, int pageNo, int rows)
        {
            List<Member> customers = new List<Member>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.PAGEVIEWCUSTOMERBYMEMBER, vendorID, pageNo, rows);
            customers = DataTableHelper.ConvertDataTable<Member>(ds.Tables[0]);

            return customers;
        }

        public List<MemberProduct> ProductList(long vendorID) {
            List<MemberProduct> products = new List<MemberProduct>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.PRODUCTSBYVENDOR, vendorID);
            products = DataTableHelper.ConvertDataTable<MemberProduct>(ds.Tables[0]);

            return products;


        }
        public List<MemberProduct> ProductListNew(long vendorID)
        {
            List<MemberProduct> products = new List<MemberProduct>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.PRODUCTSBYVENDORNEW, vendorID);
            products = DataTableHelper.ConvertDataTable<MemberProduct>(ds.Tables[0]);

            return products;


        }

        public List<MemberProduct> ProductListByCategory(long vendorID, long CategoryID, int pageNo, int rows)
        {
            List<MemberProduct> products = new List<MemberProduct>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.PRODUCTSBYCATEGORY, vendorID, CategoryID, pageNo, rows);
            products = DataTableHelper.ConvertDataTable<MemberProduct>(ds.Tables[0]);

            return products;


        }
        public MemberAllDetail GetMemberAllDetails(long userID)
        {
            MemberAllDetail item = new MemberAllDetail();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.MEMBERALLDETAILS, userID);
            item = DataTableHelper.ConvertDataTable<MemberAllDetail>(ds.Tables[0]).FirstOrDefault();

            return item;
        }
        public HomePageProductByCategory GetProductListByVAUA(long userID)
        {
            HomePageProductByCategory item = new HomePageProductByCategory();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.PRODUCTSBYCATEGORIES, userID);
            item.categories = DataTableHelper.ConvertDataTable<ProductMasterCategories>(ds.Tables[0]);
            item.products = DataTableHelper.ConvertDataTable<MemberProduct>(ds.Tables[1]);
            //item.productdetails = DataTableHelper.ConvertDataTable<ProductMaster>(ds.Tables[2]);
            return item;
        }
    }
}
