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
    public partial class MemberKYCBusiness : IMemberKYCBusiness
    {
        private IMemberKYCRepository repository;

        public MemberKYCBusiness(IMemberKYCRepository repository)
        {
            this.repository = repository;
        }

        public bool Delete(long memberID)
        {
            return repository.Delete(memberID);
        }

        public MemberKYC Get(long memberID)
        {
            return repository.Get(memberID );
        }

        public List<MemberKYC> ListAll()
        {
            return repository.ListAll();
        }

        public MemberKYC Save(MemberKYC item)
        {
            return repository.Save(item);
        }
    }
}
