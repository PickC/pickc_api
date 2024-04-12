using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.DataAccess.Contract
{
    public interface IMemberContactRepository
    {
        public MemberContact Save(MemberContact item);

        public bool BulkSave(List<MemberContact> items);

        public bool Delete(long memberID,string mobileNo);

        public MemberContact Get(long memberID,string mobileNo);

        public List<MemberContact> List(long memberID);
    }
}
