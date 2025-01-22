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
    public class RolesBusiness : IRolesBusiness
    {
        private IRolesRepository repository;


        public RolesBusiness(IRolesRepository repository)
        {
            this.repository = repository;
        }
        public bool Delete(string roleCode, short userID)
        {
            return repository.Delete(roleCode, userID);
        }

        public Roles Get(string roleCode)
        {
            return repository.Get(roleCode);
        }

        public long GetRolesCount()
        {
            return repository.GetRolesCount();
        }

        public List<Roles> ListAll()
        {
            return repository.ListAll();
        }

        public List<OrderDiscount> ListbyPageView(int pageNo, int rows)
        {
            return repository.ListbyPageView(pageNo, rows);
        }

        public Roles Save(Roles item)
        {
            return repository.Save(item);
        }
    }
}
