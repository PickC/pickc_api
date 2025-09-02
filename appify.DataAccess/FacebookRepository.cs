using appify.DataAccess.Contract;
using appify.models;
using appify.utility;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.DataAccess
{
    public partial class FacebookRepository : IFacebookRepository
    {
        private IConfiguration configuration;
        private string appify_connectionstring;
        private string PRODUCTSBYVENDORMETA = "[Operation].[usp_VendorProductsForMetaGet]";

        public FacebookRepository(IConfiguration config)
        {
            this.configuration = config;
            this.appify_connectionstring = config["ConnectionStrings:appify.connectionstring"].ToString();
        }
        public List<MetaProduct> ProductListMeta(long VendorID, short SourceID)
        {
            List<MetaProduct> products = new List<MetaProduct>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, PRODUCTSBYVENDORMETA, VendorID, SourceID);
                products = DataTableHelper.ConvertDataTable<MetaProduct>(ds.Tables[0]);
            }
            return products;
        }
    }
}
