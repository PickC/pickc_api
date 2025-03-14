using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.DataAccess.Contract
{
    public interface IRolesRepository
    {

        public Roles Save(Roles item);
        public bool Delete(short roleID, short userID);

        public Roles Get(short roleID);

        public List<Roles> ListAll();

        public Int64 GetRolesCount();

        public List<Roles> ListbyPageView(int pageNo, int rows);

        public List<RolesAccessType> GetAccessType(string LookupCategory);
    }

}
