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
    public partial class MemberReturnPolicyBusiness : IMemberReturnPolicyBusiness
    {

        IMemberReturnPolicyRepository repository;

        public MemberReturnPolicyBusiness(IMemberReturnPolicyRepository repository)
        {
            this.repository = repository;
        }

        public MemberReturnPolicy GetItem(long memberID)
        {
            return repository.GetItem(memberID);    
        }

        public List<MemberReturnPolicy> GetList()
        {
            return repository.GetList();
        }

        public bool Remove(long memberID)
        {
            return repository.Remove(memberID);
        }

        public bool Save(MemberReturnPolicy item)
        {
            return repository.Save(item);
        }
    }
}
