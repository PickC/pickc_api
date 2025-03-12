using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using appify.DataAccess.Contract;

namespace appify.Business.Contract
{
    

    public interface IRoleRightsBusiness
    {

        public bool Save(RoleRights item);
        public bool Delete(short roleID, short securableID);

        public RoleRights Get(short roleID, short securableID);

        public List<RoleRights> ListAll(short securableID);



    }
}
