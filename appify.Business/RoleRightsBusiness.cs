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

    public class RoleRightsBusiness : IRoleRightsBusiness
    {
        private IRoleRightsRepository repository;


        public RoleRightsBusiness(IRoleRightsRepository repository)
        {
            this.repository = repository;
        }
        public bool Delete(short roleID, short securableID)
        {
            return repository.Delete(roleID,securableID);
        }

        public RoleRights Get(short roleID, short securableID)
        {
            return repository.Get(roleID, securableID);
        }


        public List<RoleRights> ListAll(short roleID)
        {
            return repository.ListAll(roleID);
        }
         

        public bool Save(RoleRights item)
        {
            return repository.Save(item);
        }

    }
}
