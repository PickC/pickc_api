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
    public partial class CategoryParameterBusiness : ICategoryParameterBusiness
    {
        private ICategoryParameterRepository repository;

        public CategoryParameterBusiness(ICategoryParameterRepository repository)
        {
            this.repository = repository;
        }

        public bool Save(CategoryParameter item) { 
            return repository.Save(item);
        }
        public bool Delete(long parameterID, long categoryID) { 
            return repository.Delete(parameterID, categoryID);
        }

        public CategoryParameter Get(long parameterID, long categoryID) { 
            return repository.Get(parameterID, categoryID);
        }

        public List<CategoryParameterLite> ListAll(long categoryID) { 
            return repository.ListAll(categoryID);
        }



    }

    public partial class ParameterTypeBusiness:IParameterTypeBusiness
    {
        private IParameterTypeRepository repository;

        public ParameterTypeBusiness(IParameterTypeRepository repository)
        {
            this.repository = repository;
        }

        public bool Save(ParameterType item)
        {
            return repository.Save(item);
        }
        public bool Delete(long parameterID, string parameterValue)
        {
            return repository.Delete(parameterID,parameterValue);
        }

        public ParameterType Get(long parameterID, string parameterValue)
        {
            return repository.Get(parameterID, parameterValue);
        }

        public List<ParameterType> ListAll(long parameterID)
        {
            return repository.ListAll(parameterID);
        }



    }


}
