using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.models
{
    public partial class CommonServices
    {

    }

    public partial class Merchant
    {
        public string RazorPayAccountID {  get; set; }
        public string EmailID { get; set; }
        public string Phone {  get; set; }
        public string LegalBusinessName {  get; set; }
        public string BusinessType { get; set; }
        //public string ProfileCategoryName {  get; set; }
        //public string ProfileSubCategoryName { get; set; }
        public string GST {  get; set; }
        public string PAN { get; set; }
        public string IFSCCODE { get; set; }
        public string BankAccountNo {  get; set; }
        public string BeneficiaryName {  get; set; }
    }
}
