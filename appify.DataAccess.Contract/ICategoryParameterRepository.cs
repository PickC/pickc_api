using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.DataAccess.Contract
{
    public interface ICategoryParameterRepository
    {
        public bool Save(CategoryParameter item);
        public bool Delete(long parameterID, long categoryID);

        public CategoryParameter Get(long parameterID, long categoryID);

        public List<CategoryParameter> ListAll(long parameterID);

    }

    public interface IParameterTypeRepository
    {
        public bool Save(ParameterType item);
        public bool Delete(long parameterID, string parameterValue);

        public ParameterType Get(long parameterID, string parameterValue);

        public List<ParameterType> ListAll(long parameterID);

    }


}
