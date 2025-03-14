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
        private IRoleRightsRepository rightsRepository;


        public RolesBusiness(IRolesRepository repository, IRoleRightsRepository rightsRepository)
        {
            this.repository = repository;
            this.rightsRepository = rightsRepository;
        }
        public bool Delete(short roleID, short userID)
        {
            return repository.Delete(roleID, userID);
        }

        public Roles Get(short roleID)
        {
            Roles item = new Roles();   
            item= repository.Get(roleID);

            item.RoleRights = rightsRepository.ListAll(item.RoleID);


            return item;        
        }

        public long GetRolesCount()
        {
            return repository.GetRolesCount();
        }

        public List<Roles> ListAll()
        {
            return repository.ListAll();
        }

        public List<Roles> ListbyPageView(int pageNo, int rows)
        {
            return repository.ListbyPageView(pageNo, rows);
        }

        public Roles Save(Roles item)
        {
            var result= repository.Save(item);

            foreach (var roleRight in item.RoleRights)
            {
                roleRight.RoleID = result.RoleID;

                rightsRepository.Save(roleRight);
            }

            return result;

        }
        public List<RolesAccessType> GetAccessType(string LookupCategory)
        {
            return repository.GetAccessType(LookupCategory);
        }
    }
}
