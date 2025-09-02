using appify.Business.Contract;
using appify.DataAccess.Contract;
using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.Business
{
    public partial class FacebookBusiness : IFacebookBusiness
    {
        private IFacebookRepository repository;
        public FacebookBusiness(IFacebookRepository repository)
        {
            this.repository = repository;
        }
        public List<MetaProduct> ProductListMeta(long VendorID, short SourceID)
        {
            return repository.ProductListMeta(VendorID, SourceID);
        }
    }
}
