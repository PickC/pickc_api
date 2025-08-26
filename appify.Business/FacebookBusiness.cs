using appify.Business.Contract;
using appify.DataAccess.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.Business
{
    public partial class FacebookBusiness : IFacebookBusiness
    {
        private IFacebookRepository repository;
        public FacebookBusiness(IFacebookRepository repository)
        {
            this.repository = repository;
        }   
    }
}
