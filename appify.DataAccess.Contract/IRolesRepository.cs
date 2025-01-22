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
        public bool Delete(string roleCode, short userID);

        public Roles Get(string roleCode);

        public List<Roles> ListAll();

        public Int64 GetRolesCount();

        public List<Roles> ListbyPageView(int pageNo, int rows);


    }

}
