using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.DataAccess.Contract
{
    public interface IMemberReturnPolicyRepository
    {
        public List<MemberReturnPolicy> GetList();

        public MemberReturnPolicy GetItem(long memberID);

        public bool Save(MemberReturnPolicy item);

        public bool Remove(long memberID);


    }
}
