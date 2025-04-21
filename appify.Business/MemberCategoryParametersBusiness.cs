using appify.models;
using appify.Business.Contract;
using appify.DataAccess.Contract;
using appify.models;
using System;


namespace appify.Business
{
	public partial class MemberCategoryParametersBusiness : IMemberCategoryParametersBusiness 
	{
		IMemberCategoryParametersRepository repository;
		public MemberCategoryParametersBusiness (IMemberCategoryParametersRepository repository){
			this.repository = repository;
		}
		public MemberCategoryParameters GetMemberCategoryParameters(long id)
		{
			return repository.GetMemberCategoryParameters(id);
		}

		public List<MemberCategoryParameters> ListMemberCategoryParameters(long ProductID)
		{
			return repository.ListMemberCategoryParameters(ProductID);
		}
        public List<MemberCategoryParametersLite> ListMemberCategoryParametersLite(long ProductID)
        {
            return repository.ListMemberCategoryParametersLite(ProductID);
        }
        public bool SaveMemberCategoryParameters(MemberCategoryParameters item)
		{
			return repository.SaveMemberCategoryParameters(item);
		}

		public bool DeleteMemberCategoryParameters(Int64 ID)
		{
			return repository.DeleteMemberCategoryParameters(ID);
		}

	}
}
