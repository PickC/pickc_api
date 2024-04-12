using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.models
{
    public partial class Notifications
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Notifications() { }

        public string ToEmail { get; set; }
        public string ToEmailCC {  get; set; }
        public string ToEmailBCC {  get; set; }
        public string EmailSubject {  get; set; }
        public string EmailTemplateURL {  get; set; }
        public string EmailTemplae_ReplaceName {  get; set; }
    }
}
