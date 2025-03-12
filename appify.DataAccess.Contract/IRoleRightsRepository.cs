using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.DataAccess.Contract
{
    
    public interface IRoleRightsRepository
    {

        public bool Save(RoleRights item);
        public bool Delete(short roleID, short securableID);

        public RoleRights Get(short roleID,short securableID);

        public List<RoleRights> ListAll(short securableID);
          
         
    }

}
