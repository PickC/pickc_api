using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.DataAccess.Contract
{
    public interface IFacebookRepository
    {
        public List<MetaProduct> ProductListMeta(long VendorID, short SourceID);
    }
}
