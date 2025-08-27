using appify.DataAccess.Contract;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.DataAccess
{
    public partial class FacebookRepository : IFacebookRepository
    {
        private IConfiguration configuration;
        private string appify_connectionstring;

        public FacebookRepository(IConfiguration config)
        {
            this.configuration = config;
            this.appify_connectionstring = config["ConnectionStrings:appify.connectionstring"].ToString();
        }
    }
}
