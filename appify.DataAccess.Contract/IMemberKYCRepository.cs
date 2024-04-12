using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.DataAccess.Contract
{
    public interface IMemberKYCRepository
    {
        public MemberKYC Save(MemberKYC item);
        public bool Delete(long memberID);

        public MemberKYC Get(long memberID);

        public List<MemberKYC> ListAll();
    }
}
