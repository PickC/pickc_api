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
    public partial class MemberContactBusiness : IMemberContactBusiness
    {
        private IMemberContactRepository repository;

        public MemberContactBusiness(IMemberContactRepository repository)
        {
            this.repository = repository;
        }

        public bool Delete(long memberID, string mobileNo)
        {
            return repository.Delete(memberID,mobileNo);
        }

        public MemberContact Get(long memberID,string mobileNo)
        {
            return repository.Get(memberID,mobileNo);
        }

        public List<MemberContact> List(long memberID)
        {
            return repository.List(memberID);
        }

        public MemberContact Save(MemberContact item)
        {
            return repository.Save(item);
        }
        public bool BulkSave(List<MemberContact> items)
        {
            return repository.BulkSave(items);
        }
    }
}
